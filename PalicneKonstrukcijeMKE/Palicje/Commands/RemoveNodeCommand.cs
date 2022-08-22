using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class RemoveNodeCommand : CommandBase
{
    private NodeService _nodeService { get; set; }
    private NodeViewModel _nodeViewModel;
    private NavigationBase _editControlNavigation;

    public RemoveNodeCommand(NodeService nodeService, NodeViewModel nodeViewModel, NavigationBase editControlNavigation)
    {
        _nodeService = nodeService;
        _nodeViewModel = nodeViewModel;
        _editControlNavigation = editControlNavigation;
    }

    public override void Execute(object parameter)
    {
        Point3D coordinates = _nodeViewModel.NodeModel.Coordinates;

        if (_nodeService.TemporaryNodeVisual != null) _nodeService.RemoveTemporaryNode();
        else if (_nodeService.NodeModelExists(coordinates)) _nodeService.RemoveNode(coordinates);
        else App.MessageBlock.SetError($"Členek s koordinatami: {coordinates} ne obstaja.");

        _editControlNavigation.CurrentViewModel = null;
    }
}
