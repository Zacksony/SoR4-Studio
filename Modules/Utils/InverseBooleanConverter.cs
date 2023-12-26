using System;
using System.Windows.Data;

namespace SoR4_Studio.Modules.Utils;

[ValueConversion(typeof(bool?), typeof(bool))]
internal class InverseBooleanConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture) => targetType != typeof(bool?) && targetType != typeof(bool)
            ? throw new InvalidOperationException("The target must be a boolean")
            : (object)(value is not null && !(bool)value);

    public object? ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture) => targetType != typeof(bool?) && targetType != typeof(bool)
            ? throw new InvalidOperationException("The target must be a boolean")
            : (object)(value is not null && !(bool)value);

    #endregion
}
