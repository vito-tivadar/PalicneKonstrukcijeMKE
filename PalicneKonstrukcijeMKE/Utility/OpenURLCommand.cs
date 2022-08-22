using PalicneKonstrukcijeMKE.MVVMBase;
using System;

namespace PalicneKonstrukcijeMKE.Utility
{
    public class OpenURLCommand : CommandBase
    {
        private readonly string _url;
        public OpenURLCommand(string url)
        {
            _url = url;
        }
        public override void Execute(object parameter)
        {

        }
    }
}
