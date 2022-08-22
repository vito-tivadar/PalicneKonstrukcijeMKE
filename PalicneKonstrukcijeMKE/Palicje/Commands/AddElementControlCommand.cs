using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

internal class AddElementControlCommand : CommandBase
{
    private readonly NavigationBase _navigation;
    private readonly ElementService _elementService;
    private readonly NodeService _nodeService;

    public AddElementControlCommand(NavigationBase navigation, ElementService elementService, NodeService nodeService)
    {
        _navigation = navigation;
        _elementService = elementService;
        _nodeService = nodeService;

        _nodeService.NodeModels.CollectionChanged += (s, e) => OnCanExecuteChanged();
        _nodeService.TemporaryNodeModelChanged += () => OnCanExecuteChanged();
        _elementService.TemporaryElementModelChanged += () => OnCanExecuteChanged();
        _navigation.CurrentViewModelChanged += () => OnCanExecuteChanged();
    }


    public override bool CanExecute(object parameter)
    {
        return _nodeService.NodeModels.Count > 1
            && _nodeService.TemporaryNodeModel == null
            && _elementService.TemporaryElementModel == null
            && _navigation.CurrentViewModel == null
            && base.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
        _elementService.AddTemporaryElement();
        _navigation.CurrentViewModel = new ElementViewModel(null, _elementService, _navigation, true);
    }
}
