using Microsoft.Maui.Graphics;
using System.Globalization;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI;

public sealed class MonthlyTransitsHeaderDrawable : IDrawable
{
    public MonthlyTransitsLayout? Layout { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layout = Layout;
        if (layout == null)
            return;

        canvas.FillColor = Color.FromArgb("#F7F7F7");
        canvas.FillRectangle(dirtyRect);

        var font = GFont.Default;
        canvas.Font = font;
        canvas.FontColor = Colors.Black;

        for (int day = 1; day <= layout.DaysInMonth; day++)
        {
            float x = (float)((day - 1) * layout.DayWidth);
            float w = (float)layout.DayWidth;

            var date = new DateTime(layout.Year, layout.Month, day);
            var dow = GetShortDayOfWeek(date, layout.Culture);

            canvas.FontSize = 10;
            canvas.DrawString(
                dow,
                x, 0,
                w, 20,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            canvas.FontSize = 13;
            canvas.DrawString(
                day.ToString(CultureInfo.InvariantCulture),
                x, 20,
                w, 24,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            canvas.StrokeColor = Color.FromArgb("#D0D0D0");
            canvas.StrokeSize = 1;
            canvas.DrawLine(x, 0, x, (float)layout.HeaderHeight);
        }

        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;
        canvas.DrawLine(0, (float)layout.HeaderHeight - 1, (float)layout.ContentWidth, (float)layout.HeaderHeight - 1);
    }

    private static string GetShortDayOfWeek(DateTime date, CultureInfo culture)
    {
        var text = culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek);

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        text = text.Replace(".", string.Empty);

        return text.Length <= 2 ? text : text[..2];
    }
}