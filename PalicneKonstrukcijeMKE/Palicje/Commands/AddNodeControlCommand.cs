using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

internal class AddNodeControlCommand : CommandBase
{
    private readonly NavigationBase _navigation;
    private readonly NodeService _nodeService;
    private readonly ElementService _elementService;

    public AddNodeControlCommand(NavigationBase navigation, NodeService nodeService, ElementService elementService)
    {
        _navigation = navigation;
        _nodeService = nodeService;
        _elementService = elementService;

        _nodeService.TemporaryNodeModelChanged += () => OnCanExecuteChanged();
        _elementService.TemporaryElementModelChanged += () => OnCanExecuteChanged();
        _navigation.CurrentViewModelChanged += () => OnCanExecuteChanged();
    }

    public override bool CanExecute(object parameter)
    {
        return _nodeService.TemporaryNodeModel == null
            && _elementService.TemporaryElementModel == null
            && _navigation.CurrentViewModel == null
            && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _nodeService.AddTemporaryNode();
        _navigation.CurrentViewModel = new NodeViewModel(null, _nodeService, _navigation, true);
    }
}
