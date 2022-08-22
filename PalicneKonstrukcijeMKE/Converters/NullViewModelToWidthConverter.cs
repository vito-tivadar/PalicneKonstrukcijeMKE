using PalicneKonstrukcijeMKE.MVVMBase;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PalicneKonstrukcijeMKE.Converters;

public class NullViewModelToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return new GridLength(0, GridUnitType.Pixel);
        //GridLength maxValue = (value as GridLength)
        return (value as ViewModelBase).ViewModelWidth;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
