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

        // Portrait: как было — влезают 6 рядов
        if (width <= height)
            return height / 6.0;

        // Landscape: делаем клетки "нормальными" по высоте от ширины (чтобы появился скролл)
        // Можно чуть уменьшить коэффициент, если будет тесно:
        return (width / 7.0) * 0.98;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
