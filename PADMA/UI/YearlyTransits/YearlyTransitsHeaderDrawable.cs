using Microsoft.Maui.Graphics;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI.YearlyTransits;

public sealed class YearlyTransitsHeaderDrawable : IDrawable
{
    public YearlyTransitsLayout? Layout { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layout = Layout;
        if (layout == null)
            return;

        canvas.FillColor = Color.FromArgb("#F7F7F7");
        canvas.FillRectangle(dirtyRect);

        for (int month = 1; month <= 12; month++)
        {
            var x = (float)layout.GetMonthStartX(month);
            var width = (float)layout.GetMonthWidth(month);
            var text = GetMonthName(layout, month);

            canvas.Font = GFont.Default;
            canvas.FontSize = 13;
            canvas.FontColor = Colors.Black;
            canvas.DrawString(
                text,
                x,
                0,
                width,
                (float)layout.HeaderHeight,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            if (layout.SelectedMonth == month)
            {
                canvas.StrokeColor = Colors.Gold;
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(x + 2, 2, width - 4, (float)layout.HeaderHeight - 4);
            }

            canvas.StrokeColor = Color.FromArgb("#B8B8B8");
            canvas.StrokeSize = 1.5f;
            canvas.DrawLine(x, 0, x, (float)layout.HeaderHeight);
        }

        canvas.StrokeColor = Color.FromArgb("#B8B8B8");
        canvas.StrokeSize = 1.5f;
        canvas.DrawLine((float)layout.ContentWidth - 0.5f, 0, (float)layout.ContentWidth - 0.5f, (float)layout.HeaderHeight);

        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;
        canvas.DrawLine(0, (float)layout.HeaderHeight - 1, (float)layout.ContentWidth, (float)layout.HeaderHeight - 1);
    }

    private static string GetMonthName(YearlyTransitsLayout layout, int month)
    {
        var text = layout.Culture.DateTimeFormat.GetMonthName(month);
        if (string.IsNullOrWhiteSpace(text))
            return month.ToString();

        return char.ToUpper(text[0], layout.Culture) + text[1..];
    }
}
