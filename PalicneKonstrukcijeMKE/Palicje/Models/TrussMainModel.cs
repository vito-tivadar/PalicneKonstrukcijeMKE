using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.Utility;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using PalicneKonstrukcijeMKE.Palicje.Interfaces;

namespace PalicneKonstrukcijeMKE.Palicje.Models;

public class TrussMainModel
{
    private string _filePath { get; set; }
    public ObservableCollection<NodeModel> NodeModels { get; private set; }
    public ObservableCollection<ElementModel> ElementModels { get; private set; }

    private ObservableCollection<ModelVisual3D> _viewportItems;
    public TrussMainModel(ObservableCollection<ModelVisual3D> viewportItems)
    {
        NodeModels = new ObservableCollection<NodeModel>();
        ElementModels = new ObservableCollection<ElementModel>();
        _viewportItems = viewportItems;
    }

    public void New()
    {
        NodeModels.Clear();
        ElementModels.Clear();
        _viewportItems.Clear();
        _viewportItems.Add(new DefaultLights());
        _filePath = string.Empty;
    }

    public void Open(string? filePath = null)
    {
        string returnedFilePath = "";
        Tuple<ObservableCollection<NodeModel>, ObservableCollection<ElementModel>>? data;
        data = DataStorage.Open<Tuple<ObservableCollection<NodeModel>, ObservableCollection<ElementModel>>>(out returnedFilePath, filePath, new INodeModelJsonConverter());
        if (data != null)
        {
            New();

            foreach (INodeModel node in data.Item1)
            {
                NodeModels.Add((node as NodeModel)!);
                _viewportItems.Add(new NodeVisual3D() { Coordinates = node.Coordinates, Material = Materials.Gray, Supports = node.Supports, Forces = node.Forces });
            }

            foreach (ElementModel element in data.Item2)
            {
                IEnumerable<NodeModel> SelectedNodes = NodeModels.Where(node => node.Coordinates == element.FirstNode.Coordinates || node.Coordinates == element.SecondNode.Coordinates);
                if (SelectedNodes.Count() == 2)
                {
                    ElementModels.Add(new ElementModel(SelectedNodes.ElementAt(0), SelectedNodes.ElementAt(1), element.SectionArea, element.YoungsModulus));
                    _viewportItems.Add(new PipeVisual3D() { Point1 = element.FirstNode.Coordinates, Point2 = element.SecondNode.Coordinates, Material = Materials.LightGray, Diameter = 0.5 });
                }
            }
            _filePath = returnedFilePath;
            App.MessageBlock.SetText($"Konstrukcija odprta: {returnedFilePath}");
        }
        else App.MessageBlock.SetError($"Datoteke ni mogoče odpreti!");
    }

    public void Save(string? filePath = null, bool saveAs = false)
    {
        if (saveAs == false && _filePath != String.Empty) filePath = _filePath;
        Tuple<ObservableCollection<NodeModel>, ObservableCollection<ElementModel>> data = new Tuple<ObservableCollection<NodeModel>, ObservableCollection<ElementModel>>(NodeModels, ElementModels);
        string returnedFilePath = DataStorage.Save<Tuple<ObservableCollection<NodeModel>, ObservableCollection<ElementModel>>>(data, filePath);
        if (returnedFilePath != "")
        {
            App.MessageBlock.SetText($"Konstrukcija shranjena: {returnedFilePath}");

        }
        else App.MessageBlock.SetError("Konstrukcija ni bila shranjena!");
    }
}
