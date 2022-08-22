using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Commands;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

/*
TODO:
- When you click on node it should turn change color so you know which one you're editing
- IMPORTANT: if two nodes are added to collection you can set them to same coordinates
*/

namespace PalicneKonstrukcijeMKE.Palicje.ViewModels
{
    public class NodeViewModel : ViewModelBase
    {
        #region Dependency property

        public static readonly DependencyProperty AddButtonVisibilityProperty = DependencyProperty.Register("ButtonsVisibility", typeof(bool), typeof(NodeViewModel), new PropertyMetadata(false));
        public bool AddButtonVisibility
        {
            get { return (bool)GetValue(AddButtonVisibilityProperty); }
            set { SetValue(AddButtonVisibilityProperty, value); }
        }

        #endregion // dependency property


        #region Properties

        public readonly NodeModel NodeModel;
        public readonly NodeVisual3D NodeVisual;
        public readonly NodeService NodeService;

        

        // Coordinate properties
        public double CoordinateX
        {
            get { return NodeModel.Coordinates.X; }
            set { Update<double>(nameof(CoordinateX), value); }
        }
        public double CoordinateY
        {
            get { return NodeModel.Coordinates.Y; }
            set { Update<double>(nameof(CoordinateY), value); }
        }
        public double CoordinateZ
        {
            get { return NodeModel.Coordinates.Z; }
            set { Update<double>(nameof(CoordinateZ), value); }
        }
        // Force properties
        public double ForceX
        {
            get { return NodeModel.Forces.X; }
            set { Update<double>(nameof(ForceX), value); }
        }
        public double ForceY
        {
            get { return NodeModel.Forces.Y; }
            set { Update<double>(nameof(ForceY), value); }
        }
        public double ForceZ
        {
            get { return NodeModel.Forces.Z; }
            set { Update<double>(nameof(ForceZ), value); }
        }
        // Support properties
        public bool SupportX
        {
            get { return NodeModel.Supports.X; }
            set { Update<bool>(nameof(SupportX), value); }
        }
        public bool SupportY
        {
            get { return NodeModel.Supports.Y; }
            set { Update<bool>(nameof(SupportY), value); }
        }
        public bool SupportZ
        {
            get { return NodeModel.Supports.Z; }
            set { Update<bool>(nameof(SupportZ), value); }
        }

        public string CoordinatesString => $"{NodeModel.Coordinates}\n{NodeModel.Forces}\n{NodeModel.Supports.ToString()}";

        #endregion // Properties

        #region Commands

        public ICommand AddNodeToCollection { get; }
        public ICommand RemoveNode { get; }


        #endregion // Commands


        public NodeViewModel(NodeVisual3D nodeVisual, NodeService nodeService, NavigationBase editControlNavigation, bool addButtonVisibility = false)
        {
            if (addButtonVisibility) NodeModel = nodeService.TemporaryNodeModel;
            else nodeService.GetNodeModel(nodeVisual.Coordinates, out NodeModel);

            NodeVisual = nodeVisual == null ? nodeService.TemporaryNodeVisual : nodeVisual;
            AddButtonVisibility = addButtonVisibility;

            AddNodeToCollection = new AddNodeToCollectionCommand(nodeService);
            RemoveNode = new RemoveNodeCommand(nodeService, this, editControlNavigation);
            NodeService = nodeService;

            this.ViewModelWidth = new GridLength(230, GridUnitType.Pixel);
        }


        private bool Update<T>(string NameOfProperty, T newValue)
        {
            string property = NameOfProperty.Substring(0, NameOfProperty.Length - 1) + "s";
            string coordinate = NameOfProperty.Substring(NameOfProperty.Length - 1);
            var valueObject = NodeModel.GetType().GetProperty(property).GetValue(NodeModel);

            if (valueObject.GetType() == typeof(Point3D))
            {
                Point3D currentCoordinates = (Point3D)valueObject;
                double X = (coordinate == nameof(X)) ? Convert.ToDouble(newValue) : currentCoordinates.X;
                double Y = (coordinate == nameof(Y)) ? Convert.ToDouble(newValue) : currentCoordinates.Y;
                double Z = (coordinate == nameof(Z)) ? Convert.ToDouble(newValue) : currentCoordinates.Z;
                Point3D newCoordinates = new Point3D(X, Y, Z);

                if (!NodeModel.IsTemporary)
                {
                    if (NodeService.NodeModelExists(newCoordinates))
                    {
                        App.MessageBlock.SetError($"Členek s koordinatami {newCoordinates}  že obstaja!");
                        return false;
                    }
                }

                NodeModel.Coordinates = newCoordinates;
                NodeVisual.Coordinates = newCoordinates;
                if(!NodeModel.IsTemporary) NodeService.UpdateConnectedElements(currentCoordinates, newCoordinates);

            }
            if (valueObject.GetType() == typeof(Vector3D))
            {
                Vector3D currentForces = (Vector3D)valueObject;
                double X = (coordinate == nameof(X)) ? Convert.ToDouble(newValue) : currentForces.X;
                double Y = (coordinate == nameof(Y)) ? Convert.ToDouble(newValue) : currentForces.Y;
                double Z = (coordinate == nameof(Z)) ? Convert.ToDouble(newValue) : currentForces.Z;
                Vector3D newForces = new Vector3D(X, Y, Z);
                NodeModel.Forces = newForces;
                NodeVisual.Forces = newForces;
            }
            if (valueObject.GetType() == typeof(BooleanVector))
            {
                BooleanVector currentSupports = (BooleanVector)valueObject;
                bool X = (coordinate == nameof(X)) ? Convert.ToBoolean(newValue) : currentSupports.X;
                bool Y = (coordinate == nameof(Y)) ? Convert.ToBoolean(newValue) : currentSupports.Y;
                bool Z = (coordinate == nameof(Z)) ? Convert.ToBoolean(newValue) : currentSupports.Z;
                BooleanVector newSupports = new BooleanVector(X, Y, Z);
                NodeModel.Supports = newSupports;
                NodeVisual.Supports = newSupports;
            }

            OnPropertyChanged(NameOfProperty);
            OnPropertyChanged(nameof(CoordinatesString));
            OnPropertyChanged(nameof(NodeVisual));
            return true;
        }
    }

}
