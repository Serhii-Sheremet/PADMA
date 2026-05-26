using Microsoft.Maui.Graphics;

namespace PADMA.UI;

public sealed class MonthlyTransitsBodyDrawable : IDrawable
{
    public MonthlyTransitsLayout? Layout { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layout = Layout;
        if (layout == null)
            return;

        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        DrawGrid(canvas, layout);
    }

    private static void DrawGrid(ICanvas canvas, MonthlyTransitsLayout layout)
    {
        float width = (float)layout.ContentWidth;
        float height = (float)layout.ContentHeight;

        float topBandHeight = (float)layout.TopBandHeight;
        float laneHeight = (float)layout.LaneHeight;
        float planetContentHeight = (float)layout.PlanetContentHeight;
        float planetGroupHeight = (float)layout.PlanetGroupHeight;

        // Full background
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(0, 0, width, height);

        // Top band background: Masa/Shunya placeholder area
        canvas.FillColor = Color.FromArgb("#FFFFFF");
        canvas.FillRectangle(0, 0, width, topBandHeight);

        // Planet lane backgrounds
        for (int p = 0; p < layout.Planets.Count; p++)
        {
            float groupTop = topBandHeight + p * planetGroupHeight;

            for (int lane = 0; lane < 4; lane++)
            {
                float laneTop = groupTop + lane * laneHeight;

                canvas.FillColor = lane % 2 == 1
                    ? Color.FromArgb("#FAFAFA")
                    : Colors.White;

                canvas.FillRectangle(0, laneTop, width, laneHeight);
            }
        }

        // Vertical day lines across the whole body, including Masa/Shunya row.
        canvas.StrokeColor = Color.FromArgb("#E0E0E0");
        canvas.StrokeSize = 1;

        for (int day = 0; day <= layout.DaysInMonth; day++)
        {
            float x = (float)(day * layout.DayWidth);

            if (day == 0)
                x = 0.5f;
            else if (day == layout.DaysInMonth)
                x = width - 0.5f;

            canvas.DrawLine(x, 0.5f, x, height);
        }

        // Top band borders
        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;
        canvas.DrawLine(0, 0.5f, width, 0.5f);
        canvas.DrawLine(0, topBandHeight - 0.5f, width, topBandHeight - 0.5f);

        // Planet lane and group lines
        for (int p = 0; p < layout.Planets.Count; p++)
        {
            float groupTop = topBandHeight + p * planetGroupHeight;

            for (int lane = 0; lane <= 4; lane++)
            {
                float y = groupTop + lane * laneHeight;

                canvas.StrokeColor = lane == 0 || lane == 4
                    ? Color.FromArgb("#D0D0D0")
                    : Color.FromArgb("#E0E0E0");

                canvas.DrawLine(0, y, width, y);
            }

            // The 2px gap remains empty after the planet block.
            // Draw only the next group border; no extra lane-like row is drawn here.
        }
    }

}