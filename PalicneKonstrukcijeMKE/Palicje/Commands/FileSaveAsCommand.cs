using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Models;
using System;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class FileSaveAsCommand : CommandBase
{
    private TrussMainModel _trussModel;

    public FileSaveAsCommand(TrussMainModel trussMainModel)
    {
        _trussModel = trussMainModel;
    }


    public override void Execute(object parameter)
    {
        _trussModel.Save(null, true);
    }
}
