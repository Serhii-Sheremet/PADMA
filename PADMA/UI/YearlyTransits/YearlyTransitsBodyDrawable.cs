using Microsoft.Maui.Graphics;

namespace PADMA.UI.YearlyTransits;

public sealed class YearlyTransitsBodyDrawable : IDrawable
{
    public YearlyTransitsLayout? Layout { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layout = Layout;
        if (layout == null)
            return;

        var width = (float)layout.ContentWidth;
        var height = (float)layout.ContentHeight;
        var topBandHeight = (float)layout.TopBandHeight;
        var laneHeight = (float)layout.LaneHeight;
        var planetGroupHeight = (float)layout.PlanetGroupHeight;

        canvas.FillColor = Colors.White;
        canvas.FillRectangle(0, 0, width, height);

        // The initial scaffold intentionally contains no transit segments yet.
        // Four sub-lanes per planet match the existing Monthly Planet Transits geometry.
        for (int planetIndex = 0; planetIndex < layout.Planets.Count; planetIndex++)
        {
            var groupTop = topBandHeight + planetIndex * planetGroupHeight;

            for (int lane = 0; lane < 4; lane++)
            {
                var laneTop = groupTop + lane * laneHeight;

                canvas.FillColor = lane % 2 == 1
                    ? Color.FromArgb("#FAFAFA")
                    : Colors.White;
                canvas.FillRectangle(0, laneTop, width, laneHeight);
            }
        }

        // Strong boundaries divide the year into twelve selectable month regions.
        for (int month = 1; month <= 12; month++)
        {
            var x = (float)layout.GetMonthStartX(month);

            canvas.StrokeColor = Color.FromArgb("#B8B8B8");
            canvas.StrokeSize = 1.5f;
            canvas.DrawLine(x, 0.5f, x, height);
        }

        canvas.StrokeColor = Color.FromArgb("#B8B8B8");
        canvas.StrokeSize = 1.5f;
        canvas.DrawLine(width - 0.5f, 0.5f, width - 0.5f, height);

        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;
        canvas.DrawLine(0, 0.5f, width, 0.5f);
        canvas.DrawLine(0, topBandHeight - 0.5f, width, topBandHeight - 0.5f);

        for (int planetIndex = 0; planetIndex < layout.Planets.Count; planetIndex++)
        {
            var groupTop = topBandHeight + planetIndex * planetGroupHeight;

            for (int lane = 0; lane <= 4; lane++)
            {
                var y = groupTop + lane * laneHeight;
                canvas.StrokeColor = lane == 0 || lane == 4
                    ? Color.FromArgb("#D0D0D0")
                    : Color.FromArgb("#E0E0E0");
                canvas.StrokeSize = 1;
                canvas.DrawLine(0, y, width, y);
            }
        }

        if (layout.SelectedMonth is int selectedMonth)
        {
            var x = (float)layout.GetMonthStartX(selectedMonth);
            var selectedWidth = (float)layout.GetMonthWidth(selectedMonth);

            canvas.StrokeColor = Colors.Gold;
            canvas.StrokeSize = 2;
            canvas.DrawRectangle(x + 1, 1, selectedWidth - 2, height - 2);
        }
    }
}
