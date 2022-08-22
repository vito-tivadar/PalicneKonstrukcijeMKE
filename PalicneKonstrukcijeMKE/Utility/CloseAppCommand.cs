using PalicneKonstrukcijeMKE.MVVMBase;
using System;

namespace PalicneKonstrukcijeMKE.Utility
{
    public class CloseAppCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            App.Current.Shutdown();
        }
    }
}
