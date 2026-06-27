using Microsoft.Maui.Graphics;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI.YearlyTransits;

public sealed class YearlyTransitsLabelsDrawable : IDrawable
{
    public YearlyTransitsLayout? Layout { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layout = Layout;
        if (layout == null)
            return;

        canvas.FillColor = Color.FromArgb("#FAFAFA");
        canvas.FillRectangle(dirtyRect);

        canvas.Font = GFont.Default;
        canvas.FontColor = Colors.Black;

        var topBandHeight = (float)layout.TopBandHeight;
        var labelWidth = (float)layout.LabelWidth;

        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(0.5f, 0.5f, labelWidth - 1, topBandHeight - 1);

        canvas.FontSize = 11;
        canvas.DrawString(
            layout.TopBandLabel,
            4,
            0,
            labelWidth - 8,
            topBandHeight,
            HorizontalAlignment.Center,
            VerticalAlignment.Center,
            TextFlow.ClipBounds);

        canvas.FontSize = 13;

        for (int i = 0; i < layout.Planets.Count; i++)
        {
            var y = topBandHeight + (float)(i * layout.PlanetGroupHeight);
            var h = (float)layout.PlanetContentHeight;

            canvas.StrokeColor = Color.FromArgb("#D0D0D0");
            canvas.StrokeSize = 1;
            canvas.DrawRectangle(0, y, labelWidth, h);

            canvas.DrawString(
                layout.Planets[i].Name,
                4,
                y,
                labelWidth - 8,
                h,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);
        }
    }
}
