using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PADMA.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isCurrent = value is bool b && b;
            return isCurrent ? Colors.Black : Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
