using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Services;
using System;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class AddElementToCollectionCommand : CommandBase
{
    private ElementService _elementService { get; set; }

    public AddElementToCollectionCommand(ElementService elementService)
    {
        _elementService = elementService;
    }

    public override void Execute(object parameter)
    {
        Point3D c1 = _elementService.TemporaryElementModel.FirstNode.Coordinates;
        Point3D c2 = _elementService.TemporaryElementModel.SecondNode.Coordinates;

        _elementService.AddTemporaryElementToCollection();
    }
}
