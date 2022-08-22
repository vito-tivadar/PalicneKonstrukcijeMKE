using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Models;
using System;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class FileOpenCommand : CommandBase
{
    private TrussMainModel _trussModel;

    public FileOpenCommand(TrussMainModel trussMainModel)
    {
        _trussModel = trussMainModel;
    }


    public override void Execute(object parameter)
    {
        _trussModel.Open();
    }
}
