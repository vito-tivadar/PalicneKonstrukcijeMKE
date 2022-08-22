using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Models;
using System;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class FileSaveCommand : CommandBase
{
    private TrussMainModel _trussModel;

    public FileSaveCommand(TrussMainModel trussMainModel)
    {
        _trussModel = trussMainModel;
    }


    public override void Execute(object parameter)
    {
        _trussModel.Save();
    }
}
