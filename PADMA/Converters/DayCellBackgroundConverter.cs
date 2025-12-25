using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using PADMA.UI;

namespace PADMA.Converters
{
    public class DayCellBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = IsToday (bool)
            // values[1] = Date (DateOnly)
            // values[2] = SelectedDay (DayItem)

            var isToday = values.Length > 0 && values[0] is bool b && b;
            DateOnly dateOnly = values.Length > 1 ? values[1] switch
            {
                DateOnly d => d,
                DateTime dt => DateOnly.FromDateTime(dt),
                _ => default
            } : default;

            DayItem? selected = values.Length > 2 ? values[2] as DayItem : null;

            if (selected != null && DateOnly.FromDateTime(selected.Date) == dateOnly)
                return new SolidColorBrush(Color.FromArgb("#FFFFE0B2")); // light orange

            return isToday
                ? new SolidColorBrush(Colors.LightBlue)
                : new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
