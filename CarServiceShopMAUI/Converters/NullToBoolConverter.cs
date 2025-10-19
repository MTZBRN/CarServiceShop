// Converters/NullToBoolConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CarServiceShopMAUI.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value != null;
            if (parameter != null && parameter.ToString().Equals("invert", StringComparison.OrdinalIgnoreCase))
                result = !result;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}