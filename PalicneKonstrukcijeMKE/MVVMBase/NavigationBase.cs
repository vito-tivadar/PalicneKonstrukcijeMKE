using System;

namespace PalicneKonstrukcijeMKE.MVVMBase;

public class NavigationBase
{
    private ViewModelBase _currentViewModel;

    public ViewModelBase CurrentViewModel
    {
        get { return _currentViewModel; }
        set
        {
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action CurrentViewModelChanged;

    public virtual void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }

}
