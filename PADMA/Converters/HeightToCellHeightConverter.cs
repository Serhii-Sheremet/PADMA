using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PADMA.Converters
{
    public class HeightToCellHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double totalHeight && totalHeight > 0)
            {
                // Делим доступную высоту на 6 строк
                return totalHeight / 6.0;
            }
            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
