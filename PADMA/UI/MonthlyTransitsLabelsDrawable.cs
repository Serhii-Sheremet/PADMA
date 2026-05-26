using Microsoft.Maui.Graphics;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI;

public sealed class MonthlyTransitsLabelsDrawable : IDrawable
{
    public MonthlyTransitsLayout? Layout { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layout = Layout;
        if (layout == null)
            return;

        canvas.FillColor = Color.FromArgb("#FAFAFA");
        canvas.FillRectangle(dirtyRect);

        canvas.Font = GFont.Default;
        canvas.FontColor = Colors.Black;

        // Top band: Masa/Shunya
        float topBandHeight = (float)layout.TopBandHeight;

        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;

        float labelWidth = (float)layout.LabelWidth;

        canvas.DrawLine(0.5f, 0.5f, labelWidth - 0.5f, 0.5f);
        canvas.DrawLine(0.5f, topBandHeight - 0.5f, labelWidth - 0.5f, topBandHeight - 0.5f);
        canvas.DrawLine(0.5f, 0.5f, 0.5f, topBandHeight - 0.5f);
        canvas.DrawLine(labelWidth - 0.5f, 0.5f, labelWidth - 0.5f, topBandHeight - 0.5f);

        canvas.FontSize = 11;
        canvas.DrawString(
            layout.TopBandLabel,
            4, 0,
            (float)layout.LabelWidth - 8, topBandHeight,
            HorizontalAlignment.Center,
            VerticalAlignment.Center,
            TextFlow.ClipBounds);

        // Planet labels
        canvas.FontSize = 13;

        for (int i = 0; i < layout.Planets.Count; i++)
        {
            float y = topBandHeight + (float)(i * layout.PlanetGroupHeight);
            float h = (float)layout.PlanetContentHeight;

            canvas.StrokeColor = Color.FromArgb("#D0D0D0");
            canvas.StrokeSize = 1;
            canvas.DrawRectangle(0, y, (float)layout.LabelWidth, h);

            canvas.DrawString(
                layout.Planets[i].Name,
                4, y,
                (float)layout.LabelWidth - 8, h,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);
        }
    }
}