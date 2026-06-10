using Microsoft.Maui.Graphics;
using System.Globalization;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI.MonthlyTransits;

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

        var todayDayIndex = GetTodayDayIndex(layout);

        var selectedDayIndex =
                layout.HeaderSelectedDay.HasValue &&
                layout.HeaderSelectedDay.Value.Year == layout.Year &&
                layout.HeaderSelectedDay.Value.Month == layout.Month
                    ? layout.HeaderSelectedDay.Value.Day - 1
                    : (int?)null;

        var eclipseDayIndexes = layout.EclipseDays
                .Select(x => x.DayLocal.Date)
                .Where(x => x.Year == layout.Year && x.Month == layout.Month)
                .Select(x => x.Day - 1)
                .ToHashSet();

        var font = GFont.Default;
        canvas.Font = font;
        canvas.FontColor = Colors.Black;

        for (int day = 1; day <= layout.DaysInMonth; day++)
        {
            float x = (float)((day - 1) * layout.DayWidth);
            float w = (float)layout.DayWidth;

            var date = new DateTime(layout.Year, layout.Month, day);
            var dow = GetShortDayOfWeek(date, layout.Culture);

            if (eclipseDayIndexes.Contains(day - 1))
            {
                canvas.FillColor = Color.FromArgb("#F7D1D1");
                canvas.FillRectangle(
                    x + 1,
                    1,
                    w - 2,
                    (float)layout.HeaderHeight - 2);
            }

            if (todayDayIndex.HasValue && todayDayIndex.Value == day - 1)
            {
                canvas.FillColor = Color.FromArgb("#CFEFF7");
                canvas.FillRectangle(
                    x + 1,
                    1,
                    w - 2,
                    (float)layout.HeaderHeight - 2);
            }

            canvas.FontSize = 10;
            canvas.FontColor = Colors.Black;
            canvas.DrawString(
                dow,
                x, 0,
                w, 20,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            canvas.FontSize = 13;
            canvas.FontColor = Colors.Black;
            canvas.DrawString(
                day.ToString(CultureInfo.InvariantCulture),
                x, 20,
                w, 24,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            if (selectedDayIndex.HasValue && selectedDayIndex.Value == day - 1)
            {
                canvas.StrokeColor = Colors.Gold;
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(
                    x + 2,
                    2,
                    w - 4,
                    (float)layout.HeaderHeight - 4);
            }

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

    private static int? GetTodayDayIndex(MonthlyTransitsLayout layout)
    {
        var today = DateTime.Today;

        if (today.Year != layout.Year || today.Month != layout.Month)
            return null;

        return today.Day - 1;
    }

}