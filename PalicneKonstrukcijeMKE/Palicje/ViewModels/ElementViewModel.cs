using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Commands;
using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Palicje.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PalicneKonstrukcijeMKE.Palicje.ViewModels;

public class ElementViewModel : ViewModelBase
{
    #region Dependency property

    public static readonly DependencyProperty AddButtonVisibilityProperty = DependencyProperty.Register("ButtonsVisibility", typeof(bool), typeof(ElementViewModel), new PropertyMetadata(false));
    public bool AddButtonVisibility
    {
        get { return (bool)GetValue(AddButtonVisibilityProperty); }
        set { SetValue(AddButtonVisibilityProperty, value); }
    }

    #endregion // dependency property

    public ElementService ElementService { get; }
    public ObservableCollection<NodeModel> NodeList => ElementService.NodeModels;

    public readonly PipeVisual3D ElementVisual;

    public readonly ElementModel ElementModel;
    public INodeModel FirstNode
    {
        get { return ElementModel.FirstNode; }
        set { UpdateNodeModel(nameof(FirstNode), FirstNode, value); }
    }
    public INodeModel SecondNode
    {
        get { return ElementModel.SecondNode; }
        set { UpdateNodeModel(nameof(SecondNode), SecondNode, value); }
    }
    public double SectionArea
    {
        get { return ElementModel.SectionArea; }
        set { ElementModel.SectionArea = value; }
    }
    public double YoungsModulus
    {
        get { return ElementModel.YoungsModulus; }
        set { ElementModel.YoungsModulus = value; }
    }


    public ICommand AddElement { get; }
    public ICommand RemoveElement { get; }


    public ElementViewModel(PipeVisual3D elementVisual, ElementService elementService, NavigationBase editControlNavigation, bool addButtonVisibility = false)
    {
        if (addButtonVisibility) ElementModel = elementService.TemporaryElementModel;
        else elementService.GetElementModel(elementVisual.Point1, elementVisual.Point2, out ElementModel);
        ElementService = elementService;

        ElementVisual = elementVisual == null ? elementService.TemporaryElementVisual : elementVisual;
        AddButtonVisibility = addButtonVisibility;

        AddElement = new AddElementToCollectionCommand(elementService);
        RemoveElement = new RemoveElementCommand(elementService, this, editControlNavigation);

        this.ViewModelWidth = new GridLength(230, GridUnitType.Pixel);

        ElementModel.FirstNode.IsSelected = true;
        ElementModel.SecondNode.IsSelected = true;
        
    }

    public void UpdateNodeModel(string nameOfProperty, INodeModel currentNodeModel, INodeModel newNodeModel)
    {
        if (nameOfProperty == nameof(FirstNode))
        {

            if (ElementService.ElementModelExists(newNodeModel.Coordinates, SecondNode.Coordinates))
            {
                ElementVisual.Visible = false;
                App.MessageBlock.SetError($"Element, ki povezuje členka {newNodeModel.Name} in {SecondNode.Name} že obstaja.");
                return;
            }
            ElementVisual.Visible = true;
            ElementModel.FirstNode = newNodeModel;
            ElementVisual.Point1 = newNodeModel.Coordinates;
        }
        else if (nameOfProperty == nameof(SecondNode))
        {
            if (ElementService.ElementModelExists(FirstNode.Coordinates, newNodeModel.Coordinates))
            {
                ElementVisual.Visible = false;
                App.MessageBlock.SetError($"Element, ki povezuje členka {FirstNode.Name} in {newNodeModel.Name} že obstaja.");
                return;
            }
            ElementVisual.Visible = true;
            ElementModel.SecondNode = newNodeModel;
            ElementVisual.Point2 = newNodeModel.Coordinates;
        }
        currentNodeModel.IsSelected = false;

        newNodeModel.IsSelected = true;

        OnPropertyChanged(nameOfProperty);
        OnPropertyChanged(nameof(NodeList));
        OnPropertyChanged(nameof(ElementVisual));
    }
}
