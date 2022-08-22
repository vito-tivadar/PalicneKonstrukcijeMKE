using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class RemoveElementCommand : CommandBase
{
    private ElementService _elementService;
    private ElementViewModel _elementViewModel;
    private NavigationBase _editControlNavigation;

    public RemoveElementCommand(ElementService elementService, ElementViewModel elementViewModel, NavigationBase editControlNavigation)
    {
        _elementService = elementService;
        _elementViewModel = elementViewModel;
        _editControlNavigation = editControlNavigation;
    }

    public override void Execute(object parameter)
    {
        Point3D coordinates1 = _elementViewModel.ElementModel.FirstNode.Coordinates;
        Point3D coordinates2 = _elementViewModel.ElementModel.SecondNode.Coordinates;

        if (_elementService.TemporaryElementVisual != null) _elementService.RemoveTemporaryElement();
        else if(_elementService.ElementModelExists(coordinates1, coordinates2))  _elementService.RemoveElement(coordinates1, coordinates2);
        else App.MessageBlock.SetError($"Element s členkoma: {coordinates1} in {coordinates2} ne obstaja.");

        _editControlNavigation.CurrentViewModel = null;
    }
}
