using Microsoft.Maui.Graphics;

namespace PADMA.UI.MonthlyTransits;

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

        DrawSegments(canvas, layout);
        
        // Masa/Shunya is a top-level band and must be drawn above the background day grid.
        DrawMasaShunyaBand(canvas, layout);
    }

    private static void DrawMasaShunyaBand(ICanvas canvas, MonthlyTransitsLayout layout)
    {
        var band = layout.Data?.MasaShunya;
        if (band == null)
            return;

        float y = 0;
        float h = (float)layout.TopBandHeight;
        float padY = 2f;
        float innerY = y + padY;
        float innerH = h - padY * 2;

        // base Masa segments
        foreach (var segment in band.MasaSegments)
        {
            float x = (float)GetX(layout, segment.StartLocal);
            float x2 = (float)GetX(layout, segment.EndLocal);
            float w = Math.Max(1, x2 - x);

            canvas.FillColor = segment.Color ?? Colors.White;
            canvas.FillRectangle(x, innerY, w, innerH);

            DrawSegmentText(canvas, segment.Text, x, innerY, w, innerH);
        }

        // Shunya Nakshatra: top half
        foreach (var overlay in band.ShunyaNakshatraOverlays)
        {
            float x = (float)GetX(layout, overlay.StartLocal);
            float x2 = (float)GetX(layout, overlay.EndLocal);
            float w = Math.Max(1, x2 - x);

            canvas.FillColor = overlay.Color;
            canvas.FillRectangle(x, innerY, w, innerH / 2f);
        }

        // Shunya Tithi: bottom half
        foreach (var overlay in band.ShunyaTithiOverlays)
        {
            float x = (float)GetX(layout, overlay.StartLocal);
            float x2 = (float)GetX(layout, overlay.EndLocal);
            float w = Math.Max(1, x2 - x);

            canvas.FillColor = overlay.Color;
            canvas.FillRectangle(x, innerY + innerH / 2f, w, innerH / 2f);
        }

        // Masa segment borders: show where a new Masa starts/ends.
        foreach (var segment in band.MasaSegments)
        {
            float x = (float)GetX(layout, segment.StartLocal);
            float x2 = (float)GetX(layout, segment.EndLocal);
            float w = Math.Max(1, x2 - x);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 0.5f;
            canvas.DrawRectangle(x, innerY, w, innerH);
        }

        // text again on top, so overlays do not hide Masa label
        foreach (var segment in band.MasaSegments)
        {
            float x = (float)GetX(layout, segment.StartLocal);
            float x2 = (float)GetX(layout, segment.EndLocal);
            float w = Math.Max(1, x2 - x);

            DrawSegmentText(canvas, segment.Text, x, innerY, w, innerH);
        }

        canvas.StrokeColor = Color.FromArgb("#D0D0D0");
        canvas.StrokeSize = 1;
        canvas.DrawLine(0, 0.5f, (float)layout.ContentWidth, 0.5f);
        canvas.DrawLine(0, h - 0.5f, (float)layout.ContentWidth, h - 0.5f);
    }

    private static void DrawSegments(ICanvas canvas, MonthlyTransitsLayout layout)
    {
        var data = layout.Data;
        if (data == null)
            return;

        foreach (var group in data.PlanetGroups)
        {
            int planetIndex = layout.Planets
                .Select((p, i) => new { p.Planet, Index = i })
                .FirstOrDefault(x => x.Planet == group.Planet)
                ?.Index ?? -1;

            if (planetIndex < 0)
                continue;

            for (int laneIndex = 0; laneIndex < group.Lanes.Count; laneIndex++)
            {
                var lane = group.Lanes[laneIndex];

                float y = (float)(
                    layout.TopBandHeight +
                    planetIndex * layout.PlanetGroupHeight +
                    laneIndex * layout.LaneHeight);

                DrawLaneSegments(canvas, layout, lane, y);
            }
        }
    }

    private static void DrawLaneSegments(
        ICanvas canvas,
        MonthlyTransitsLayout layout,
        MonthlyTransitLane lane,
        float y)
    {
        // 1. Base segment fills
        foreach (var segment in lane.Segments)
        {
            float x = (float)GetX(layout, segment.StartLocal);
            float x2 = (float)GetX(layout, segment.EndLocal);
            float w = Math.Max(1, x2 - x);
            float h = (float)layout.LaneHeight;

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

        // 2. Overlay segments, for example Mrityu Bhaga
        DrawLaneOverlays(canvas, layout, lane, y);

        // 3. Segment borders
        foreach (var segment in lane.Segments)
        {
            float x = (float)GetX(layout, segment.StartLocal);
            float x2 = (float)GetX(layout, segment.EndLocal);
            float w = Math.Max(1, x2 - x);
            float h = (float)layout.LaneHeight;

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 0.5f;
            canvas.DrawRectangle(x, y, w, h);
        }

        // 4. Text must be the top layer
        foreach (var segment in lane.Segments)
        {
            float x = (float)GetX(layout, segment.StartLocal);
            float x2 = (float)GetX(layout, segment.EndLocal);
            float w = Math.Max(1, x2 - x);
            float h = (float)layout.LaneHeight;

            DrawSegmentText(canvas, segment.Text, x, y, w, h);
        }
    }

    private static void DrawLaneOverlays(
        ICanvas canvas,
        MonthlyTransitsLayout layout,
        MonthlyTransitLane lane,
        float y)
    {
        if (lane.Overlays.Count == 0)
            return;

        foreach (var overlay in lane.Overlays)
        {
            float x = (float)GetX(layout, overlay.StartLocal);
            float x2 = (float)GetX(layout, overlay.EndLocal);
            float w = Math.Max(1, x2 - x);
            float h = (float)layout.LaneHeight;

            canvas.FillColor = overlay.Color;
            canvas.FillRectangle(x, y, w, h);

            // Optional. If it looks too noisy, remove these 3 lines.
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 0.5f;
            canvas.DrawRectangle(x, y, w, h);
        }
    }

    private static double GetX(MonthlyTransitsLayout layout, DateTime localTime)
    {
        var monthStart = new DateTime(layout.Year, layout.Month, 1);
        var totalDays = (localTime - monthStart).TotalDays;

        if (totalDays < 0)
            totalDays = 0;

        if (totalDays > layout.DaysInMonth)
            totalDays = layout.DaysInMonth;

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
        if (string.IsNullOrWhiteSpace(text) || w < 12 || h < 10)
            return;

        var drawText = text
            .Replace("\r", "")
            .Replace("\n", " ")
            .Trim();

        if (drawText.Length == 0)
            return;

        var font = Microsoft.Maui.Graphics.Font.Default;
        var fontSize = Math.Max(9f, h * 0.50f);

        var pad = 2f;
        var drawW = Math.Max(1f, w - pad * 2);
        var oneLineH = fontSize + 2f;
        var textY = y + (h - oneLineH) / 2f;

        canvas.SaveState();
        canvas.ClipRectangle(x, y, w, h);

        canvas.Font = font;
        canvas.FontSize = fontSize;
        canvas.FontColor = Colors.Black;

        float cx = x + pad;
        float xLimit = x + pad + drawW;

        foreach (var ch in drawText)
        {
            var s = ch.ToString();
            var sz = canvas.GetStringSize(s, font, fontSize);
            var cw = sz.Width;

            if (cw <= 0.01f)
                cw = fontSize * 0.3f;

            if (cx + cw > xLimit)
                break;

            canvas.DrawString(
                s,
                cx,
                textY,
                cw + 1f,
                oneLineH,
                HorizontalAlignment.Left,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            cx += cw;
        }

        canvas.RestoreState();
    }

    

}