using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using PalicneKonstrukcijeMKE.Palicje.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Services;

public class ElementService
{
    public ObservableCollection<ModelVisual3D> ViewportItems { get; }

    public readonly ObservableCollection<ElementModel> ElementModels;
    public readonly ObservableCollection<NodeModel> NodeModels;


    public event Action TemporaryElementModelChanged;
    private ElementModel _temporaryElementModel;
    public ElementModel TemporaryElementModel
    {
        get { return _temporaryElementModel; }
        set
        {
            _temporaryElementModel = value;
            OnTemporaryElementModelChanged();
        }
    }
    public PipeVisual3D TemporaryElementVisual;

    public ElementService(ObservableCollection<ElementModel> elementModels, ObservableCollection<NodeModel> nodeModels, ObservableCollection<ModelVisual3D> viewportItems)
    {
        ElementModels = elementModels;
        NodeModels = nodeModels;
        ViewportItems = viewportItems;
    }

    private void OnTemporaryElementModelChanged()
    {
        TemporaryElementModelChanged?.Invoke();
    }

    public void GetElementModel(Point3D coordinates1, Point3D coordinates2, out ElementModel elementModel, bool findTemporaryModel = false)
    {
        foreach (ElementModel _elementModel in ElementModels)
        {
            Point3D c1 = _elementModel.FirstNode.Coordinates;
            Point3D c2 = _elementModel.SecondNode.Coordinates;

            if ((c1 == coordinates1 && c2 == coordinates2) || (c1 == coordinates2 && c2 == coordinates1))
            {
                //if(findTemporaryModel && _elementModel.IsTemporary) 
                elementModel = _elementModel;
                return;
            }
        }
        elementModel = null;
    }

    public bool ElementModelExists(Point3D coordinates1, Point3D coordinates2)
    {
        foreach (ElementModel elementModel in ElementModels)
        {
            Point3D c1 = elementModel.FirstNode.Coordinates;
            Point3D c2 = elementModel.SecondNode.Coordinates;

            if ((c1 == coordinates1 && c2 == coordinates2) || (c1 == coordinates2 && c2 == coordinates1)) return true;
        }
        return false;
    }

    public void AddTemporaryElement()
    {
        NodeModel firstNode = NodeModels[0];
        NodeModel secondNode = NodeModels[1];

        //firstNode.IsSelected = true;
        //secondNode.IsSelected = true;

        TemporaryElementVisual = new PipeVisual3D() { Point1 = firstNode.Coordinates, Point2 = secondNode.Coordinates, Material = Materials.Green, Diameter = 0.5 };
        TemporaryElementVisual.SetName("TemporaryElement");
        ViewportItems.Add(TemporaryElementVisual);

        TemporaryElementModel = new ElementModel(firstNode, secondNode);
    }

    public void RemoveTemporaryElement()
    {
        ModelVisual3D itemToRemove = null;

        foreach (ModelVisual3D modelVisual in ViewportItems)
        {
            if (modelVisual.GetName() == "TemporaryElement")
            {
                itemToRemove = modelVisual;
                break;
            }
        }
        ViewportItems.Remove(itemToRemove);
        TemporaryElementModel.FirstNode.IsSelected = false;
        TemporaryElementModel.SecondNode.IsSelected = false;
        TemporaryElementModel = null;
    }

    public bool AddTemporaryElementToCollection()
    {
        Point3D c1 = TemporaryElementModel.FirstNode.Coordinates;
        Point3D c2 = TemporaryElementModel.SecondNode.Coordinates;

        if (ElementModelExists(c1, c2))
        {
            App.MessageBlock.SetError($"Element s koordinatami: {c1} {c2} že obstaja.");
            return false;
        }

        TemporaryElementModel.FirstNode.IsSelected = false;
        TemporaryElementModel.SecondNode.IsSelected = false;

        ElementModels.Add(TemporaryElementModel);
        TemporaryElementModel = null;

        TemporaryElementVisual.Material = Materials.LightGray;
        TemporaryElementVisual.SetName("");
        TemporaryElementVisual.Visible = true;
        TemporaryElementVisual = null;

        return true;
    }

    public void RemoveElement(Point3D coordinates1, Point3D coordinates2)
    {
        //remove visuals
        ModelVisual3D visualItemToRemove = null;
        foreach (ModelVisual3D modelVisual in ViewportItems)
        {
            if (modelVisual is PipeVisual3D)
            {
                PipeVisual3D elementVisual = (modelVisual as PipeVisual3D);
                Point3D c1 = elementVisual.Point1;
                Point3D c2 = elementVisual.Point2;

                if ((c1 == coordinates1 && c2 == coordinates2) || (c1 == coordinates2 && c2 == coordinates1))
                {
                    visualItemToRemove = modelVisual;
                    break;
                }
            }
        }
        if(visualItemToRemove != null) ViewportItems.Remove(visualItemToRemove);

        // remove element model
        ElementModel elementModelToRemove = null;
        foreach (ElementModel elementModel in ElementModels)
        {
            INodeModel firstNode = elementModel.FirstNode;
            INodeModel secondNode = elementModel.SecondNode;

            if ((firstNode.Coordinates == coordinates1 && secondNode.Coordinates == coordinates2) || (firstNode.Coordinates == coordinates2 && secondNode.Coordinates == coordinates1))
            {
                elementModelToRemove = elementModel;
                break;
            }
        }
        if(elementModelToRemove != null) ElementModels.Remove(elementModelToRemove);
    }
}
