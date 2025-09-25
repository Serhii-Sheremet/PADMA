using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PADMA.Converters
{
    public class TodayBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isToday = (bool)value;
            return isToday ? 3 : 0; // толщина рамки
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
