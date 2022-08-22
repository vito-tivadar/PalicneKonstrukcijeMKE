using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Models;
using System;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class FileNewCommand : CommandBase
{
    private TrussMainModel _trussModel;

    public FileNewCommand(TrussMainModel trussModel)
    {
        _trussModel = trussModel;
    }

    public FileNewCommand()
    {

    }

    public override void Execute(object parameter)
    {
        _trussModel.New();
    }
}
