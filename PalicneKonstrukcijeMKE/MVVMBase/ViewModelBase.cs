using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PalicneKonstrukcijeMKE.MVVMBase;

public class ViewModelBase : DependencyObject, INotifyPropertyChanged
{
    public GridLength ViewModelWidth;
    public GridLength ViewModelHeight;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Used to update UI when <paramref name="PropertyName"/> Changes
    /// </summary>
    /// <param name="PropertyName">Name of property that changed</param>
    protected void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    /// <summary>
    /// Used to unsubscribe from events, when you leave ViewModel and display new View
    /// </summary>
    public virtual void Dispose() { }
}
