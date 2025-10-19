using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PADMA.Converters
{
    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is DateTime dt ? dt.TimeOfDay : TimeSpan.Zero;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is TimeSpan t ? DateTime.Today.Add(t) : DateTime.Now;
    }
}
