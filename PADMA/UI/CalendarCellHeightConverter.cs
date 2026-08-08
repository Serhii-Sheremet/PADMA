using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PADMA.UI;

public sealed class CalendarCellHeightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length < 2) return 0d;

        var width = values[0] is double w ? w : 0d;
        var height = values[1] is double h ? h : 0d;

        if (width <= 0 || height <= 0) return 0d;

        // Portrait: page height split into 6 rows
        if (width <= height)
            return height / 6.0;

        // Landscape: keep each cell roughly "square" — width divided by 7 (days per week)
        // slightly reduced to leave room for spacing/margins:
        return (width / 7.0) * 0.98;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
