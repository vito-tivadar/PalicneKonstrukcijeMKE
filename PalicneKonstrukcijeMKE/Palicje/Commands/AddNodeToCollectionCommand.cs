using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using System;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class AddNodeToCollectionCommand : CommandBase
{
    private NodeService _nodeService { get; set; }

    public AddNodeToCollectionCommand(NodeService nodeService)
    {
        _nodeService = nodeService;
    }

    public override void Execute(object parameter)
    {
        Point3D coordinates = _nodeService.TemporaryNodeModel.Coordinates;
        if (!_nodeService.AddTemporaryNodeToCollection())
        {
            App.MessageBlock.SetError($"Členek s koordinatami: {coordinates} že obstaja.");
        }
    }
}
