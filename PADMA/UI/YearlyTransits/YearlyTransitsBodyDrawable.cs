using Microsoft.Maui.Graphics;
using PADMA.UI.MonthlyTransits;

namespace PADMA.UI.YearlyTransits;

public sealed class YearlyTransitsBodyDrawable : IDrawable
{
    private const float MinTextWidth = 42f;

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

        DrawLaneBackgrounds(canvas, layout, width, topBandHeight, laneHeight, planetGroupHeight);
        DrawSegments(canvas, layout);
        DrawMasaShunyaBand(canvas, layout);

        // Month and lane lines are intentionally above fills, so they remain readable.
        DrawGridLines(canvas, layout, width, height, topBandHeight, laneHeight, planetGroupHeight);
        DrawSelection(canvas, layout, height);
    }

    private static void DrawLaneBackgrounds(
        ICanvas canvas,
        YearlyTransitsLayout layout,
        float width,
        float topBandHeight,
        float laneHeight,
        float planetGroupHeight)
    {
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
    }

    private static void DrawGridLines(
        ICanvas canvas,
        YearlyTransitsLayout layout,
        float width,
        float height,
        float topBandHeight,
        float laneHeight,
        float planetGroupHeight)
    {
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
    }

    private static void DrawMasaShunyaBand(ICanvas canvas, YearlyTransitsLayout layout)
    {
        var band = layout.Data?.MasaShunya;
        if (band == null)
            return;

        const float padY = 2f;

        var y = 0f;
        var h = (float)layout.TopBandHeight;
        var innerY = y + padY;
        var innerH = h - padY * 2;

        foreach (var segment in band.MasaSegments)
        {
            var x = (float)GetX(layout, segment.StartLocal);
            var x2 = (float)GetX(layout, segment.EndLocal);
            var w = Math.Max(1f, x2 - x);

            canvas.FillColor = segment.Color ?? Colors.White;
            canvas.FillRectangle(x, innerY, w, innerH);
        }

        foreach (var overlay in band.ShunyaNakshatraOverlays)
        {
            var x = (float)GetX(layout, overlay.StartLocal);
            var x2 = (float)GetX(layout, overlay.EndLocal);
            var w = Math.Max(1f, x2 - x);

            canvas.FillColor = overlay.Color;
            canvas.FillRectangle(x, innerY, w, innerH / 2f);
        }

        foreach (var overlay in band.ShunyaTithiOverlays)
        {
            var x = (float)GetX(layout, overlay.StartLocal);
            var x2 = (float)GetX(layout, overlay.EndLocal);
            var w = Math.Max(1f, x2 - x);

            canvas.FillColor = overlay.Color;
            canvas.FillRectangle(x, innerY + innerH / 2f, w, innerH / 2f);
        }

        foreach (var segment in band.MasaSegments)
        {
            var x = (float)GetX(layout, segment.StartLocal);
            var x2 = (float)GetX(layout, segment.EndLocal);
            var w = Math.Max(1f, x2 - x);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 0.5f;
            canvas.DrawRectangle(x, innerY, w, innerH);

            DrawSegmentText(canvas, segment.Text, x, innerY, w, innerH);
        }
    }

    private static void DrawSegments(ICanvas canvas, YearlyTransitsLayout layout)
    {
        var data = layout.Data;
        if (data == null)
            return;

        foreach (var group in data.PlanetGroups)
        {
            var planetIndex = layout.Planets
                .Select((p, index) => new { p.Planet, Index = index })
                .FirstOrDefault(x => x.Planet == group.Planet)
                ?.Index ?? -1;

            if (planetIndex < 0)
                continue;

            for (int laneIndex = 0; laneIndex < group.Lanes.Count; laneIndex++)
            {
                var y = (float)(
                    layout.TopBandHeight +
                    planetIndex * layout.PlanetGroupHeight +
                    laneIndex * layout.LaneHeight);

                DrawLaneSegments(canvas, layout, group.Lanes[laneIndex], y);
            }
        }
    }

    private static void DrawLaneSegments(
        ICanvas canvas,
        YearlyTransitsLayout layout,
        MonthlyTransitLane lane,
        float y)
    {
        var h = (float)layout.LaneHeight;

        foreach (var segment in lane.Segments)
        {
            var x = (float)GetX(layout, segment.StartLocal);
            var x2 = (float)GetX(layout, segment.EndLocal);
            var w = Math.Max(1f, x2 - x);

            if (segment.IsSplitColor)
            {
                canvas.FillColor = segment.ColorTop ?? Colors.Transparent;
                canvas.FillRectangle(x, y, w, h / 2f);

                canvas.FillColor = segment.ColorBottom ?? Colors.Transparent;
                canvas.FillRectangle(x, y + h / 2f, w, h / 2f);
            }
            else
            {
                canvas.FillColor = segment.Color ?? Colors.Transparent;
                canvas.FillRectangle(x, y, w, h);
            }
        }

        foreach (var overlay in lane.Overlays)
        {
            var x = (float)GetX(layout, overlay.StartLocal);
            var x2 = (float)GetX(layout, overlay.EndLocal);
            var w = Math.Max(1f, x2 - x);

            canvas.FillColor = overlay.Color;
            canvas.FillRectangle(x, y, w, h);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 0.5f;
            canvas.DrawRectangle(x, y, w, h);
        }

        foreach (var segment in lane.Segments)
        {
            var x = (float)GetX(layout, segment.StartLocal);
            var x2 = (float)GetX(layout, segment.EndLocal);
            var w = Math.Max(1f, x2 - x);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 0.5f;
            canvas.DrawRectangle(x, y, w, h);

            DrawSegmentText(canvas, segment.Text, x, y, w, h);
        }
    }

    private static double GetX(YearlyTransitsLayout layout, DateTime localTime)
    {
        var totalDays = (localTime - layout.YearStart).TotalDays;
        totalDays = Math.Clamp(totalDays, 0, layout.DaysInYear);

        return totalDays * layout.DayWidth;
    }

    private static void DrawSegmentText(
        ICanvas canvas,
        string text,
        float x,
        float y,
        float w,
        float h)
    {
        if (string.IsNullOrWhiteSpace(text) || w < MinTextWidth || h < 10)
            return;

        var drawText = text
            .Replace("\r", string.Empty)
            .Replace("\n", " ")
            .Trim();

        if (drawText.Length == 0)
            return;

        var font = Microsoft.Maui.Graphics.Font.Default;
        var fontSize = Math.Max(9f, h * 0.5f);
        const float pad = 2f;

        canvas.SaveState();
        canvas.ClipRectangle(x, y, w, h);

        canvas.Font = font;
        canvas.FontSize = fontSize;
        canvas.FontColor = Colors.Black;

        canvas.DrawString(
            drawText,
            x + pad,
            y,
            Math.Max(1f, w - pad * 2),
            h,
            HorizontalAlignment.Left,
            VerticalAlignment.Center,
            TextFlow.ClipBounds);

        canvas.RestoreState();
    }

    private static void DrawSelection(
        ICanvas canvas,
        YearlyTransitsLayout layout,
        float height)
    {
        if (layout.SelectedMonth is not int selectedMonth)
            return;

        var x = (float)layout.GetMonthStartX(selectedMonth);
        var w = (float)layout.GetMonthWidth(selectedMonth);

        canvas.StrokeColor = Colors.Gold;
        canvas.StrokeSize = 2;
        canvas.DrawRectangle(x + 1, 1, w - 2, height - 2);
    }
}
