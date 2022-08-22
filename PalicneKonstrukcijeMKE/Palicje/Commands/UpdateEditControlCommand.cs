using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System;
using System.Windows;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class UpdateEditControlCommand : CommandBase
{
    private readonly NavigationBase _navigation;
    private readonly NodeService _nodeService;
    private readonly ElementService _elementService;

    public UpdateEditControlCommand(NavigationBase navigation, NodeService nodeService, ElementService elementService)
    {
        _navigation = navigation;
        _nodeService = nodeService;
        _elementService = elementService;
    }

    public override void Execute(object parameter)
    {
        if(_nodeService.TemporaryNodeModel != null)
        {
            if (MessageBox.Show("Trenutni členek še ni dodam in bo ob nadaljevanju odstranjen.", "FEM Calculator", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Cancel) return;
            else _nodeService.RemoveTemporaryNode();
        }
        if(_elementService.TemporaryElementModel != null)
        {
            if (MessageBox.Show("Trenutni element še ni dodam in bo ob nadaljevanju odstranjen.", "FEM Calculator", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel) return;
            else _elementService.RemoveTemporaryElement();
        }
        var firstHitElement = parameter as Viewport3DHelper.HitResult;
        if (firstHitElement == null)
        {
            _navigation.CurrentViewModel = null;
            return;
        }

        Type visualType = firstHitElement.Visual.GetType();

        if (visualType == typeof(NodeVisual3D))
        {
            NodeVisual3D nodeVisual = firstHitElement.Visual as NodeVisual3D;
            if (nodeVisual.GetName() == "ResultVisualItem") return;
            _navigation.CurrentViewModel = new NodeViewModel(nodeVisual, _nodeService, _navigation, false);
        }
        if (visualType == typeof(PipeVisual3D))
        {
            PipeVisual3D elementVisual = firstHitElement.Visual as PipeVisual3D;
            if (elementVisual.GetName() == "ResultVisualItem") return;
            _navigation.CurrentViewModel = new ElementViewModel(elementVisual, _elementService, _navigation, false);
        }

    }
}
