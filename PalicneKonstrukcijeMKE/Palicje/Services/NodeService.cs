using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Utility;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Services;

public class NodeService
{
    public ObservableCollection<ModelVisual3D> ViewportItems { get; }

    public readonly ObservableCollection<NodeModel> NodeModels;
    public readonly ObservableCollection<ElementModel> ElementModels;


    public event Action TemporaryNodeModelChanged;
    private NodeModel _temporaryNodeModel;
    public NodeModel TemporaryNodeModel
    {
        get { return _temporaryNodeModel; }
        set
        {
            _temporaryNodeModel = value;
            OnTemporaryNodeModelChanged();
        }
    }
    public NodeVisual3D TemporaryNodeVisual;

    public NodeService(ObservableCollection<NodeModel> nodeModels, ObservableCollection<ElementModel> elementModels, ObservableCollection<ModelVisual3D> viewportItems)
    {
        NodeModels = nodeModels;
        ElementModels = elementModels;
        ViewportItems = viewportItems;
    }

    private void OnTemporaryNodeModelChanged()
    {
        TemporaryNodeModelChanged?.Invoke();
    }

    public void GetNodeModel(Point3D coordinates, out NodeModel nodeModel)
    {
        foreach (NodeModel _nodeModel in NodeModels)
        {
            if (_nodeModel.Coordinates == coordinates)
            {
                nodeModel = _nodeModel;
                return;
            }
        }
        nodeModel = null;
    }

    public bool NodeModelExists(Point3D coordinates)
    {
        foreach (NodeModel nodeModel in NodeModels)
        {
            if (nodeModel.Coordinates == coordinates) return true;
        }
        return false;
    }

    public void AddTemporaryNode()
    {
        Point3D coordinates = new Point3D(0, 0, 0);
        BooleanVector supports = new BooleanVector();
        Vector3D forces = new Vector3D(0, 0, 0);

        TemporaryNodeVisual = new NodeVisual3D() { Coordinates = coordinates, Forces = forces, Supports = supports, Material = Materials.Green };
        TemporaryNodeVisual.SetName("TemporaryNode");
        ViewportItems.Add(TemporaryNodeVisual);

        TemporaryNodeModel = new NodeModel(coordinates, forces, supports);
    }
    

    public void RemoveTemporaryNode()
    {
        ModelVisual3D itemToRemove = null;

        foreach (ModelVisual3D modelVisual in ViewportItems)
        {
            if(modelVisual.GetName() == "TemporaryNode")
            {
                itemToRemove = modelVisual;
                break;
            }
        }
        ViewportItems.Remove(itemToRemove);
        TemporaryNodeModel = null;
    }

    public bool AddTemporaryNodeToCollection()
    {
        if (NodeModelExists(TemporaryNodeModel.Coordinates)) return false;

        TemporaryNodeModel.IsTemporary = false;
        NodeModels.Add(TemporaryNodeModel);
        TemporaryNodeModel = null;

        TemporaryNodeVisual.Material = Materials.Gray;
        TemporaryNodeVisual.SetName("");
        TemporaryNodeVisual = null;

        return true;
    }

    public void RemoveNode(Point3D coordinates)
    {
        //remove visuals
        Collection<ModelVisual3D> itemsToRemove = new Collection<ModelVisual3D>();
        foreach (ModelVisual3D modelVisual in ViewportItems)
        {
            if(modelVisual is NodeVisual3D)
            {
                if ((modelVisual as NodeVisual3D).Coordinates == coordinates)
                {
                    itemsToRemove.Add(modelVisual);
                    continue;
                }
            }

            if(modelVisual is PipeVisual3D)
            {
                if((modelVisual as PipeVisual3D).Point1 == coordinates || (modelVisual as PipeVisual3D).Point2 == coordinates)
                {
                    itemsToRemove.Add(modelVisual);
                    continue;
                }
            }
        }
        foreach (ModelVisual3D modelVisual in itemsToRemove)
        {
            ViewportItems.Remove(modelVisual);
        }

        // remove node models
        NodeModel nodeModelToRemove = null;
        foreach (NodeModel nodeModel in NodeModels)
        {
            if(nodeModel.Coordinates == coordinates) nodeModelToRemove = nodeModel;
        }
        if (nodeModelToRemove != null) NodeModels.Remove(nodeModelToRemove);

        // remove connected element models
        ObservableCollection<ElementModel> connectedElements = new ObservableCollection<ElementModel>();
        foreach (ElementModel elementModel in ElementModels)
        {
            if (elementModel.FirstNode.Coordinates == coordinates || elementModel.SecondNode.Coordinates == coordinates)
                connectedElements.Add(elementModel);
        }
        foreach (ElementModel elementModel in connectedElements)
        {
            ElementModels.Remove(elementModel);
        }
    }

    public void UpdateConnectedElements(Point3D currentCoordinates, Point3D newCoordinates)
    {
        foreach (ModelVisual3D visualItem in ViewportItems)
        {
            if(visualItem is PipeVisual3D)
            {
                PipeVisual3D elementVisual = visualItem as PipeVisual3D;
                if (elementVisual.Point1 == currentCoordinates) elementVisual.Point1 = newCoordinates;
                if (elementVisual.Point2 == currentCoordinates) elementVisual.Point2 = newCoordinates;
            }
        }
        /*
        foreach (ElementModel elementModel in ElementModels)
        {
            if (elementModel.FirstNode.Coordinates == currentCoordinates) elementModel.FirstNode.Coordinates = newCoordinates;
            if (elementModel.SecondNode.Coordinates == currentCoordinates) elementModel.SecondNode.Coordinates = newCoordinates;
        }*/
    }
}
