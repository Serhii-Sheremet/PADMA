using System;
using Microsoft.Maui.Graphics;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI;

public sealed class YogaBarDrawable : IDrawable
{
    public DateTime DayDate { get; set; }            // local date (00:00)
    public YogaOverviewStripe? Stripe { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var st = Stripe;
        if (st == null) return;

        float width = dirtyRect.Width;
        float height = dirtyRect.Height;

        var barHeight = dirtyRect.Height;
        var fontSize = Math.Max(10f, barHeight * 0.55f);
        canvas.FontSize = fontSize;

        // рамка дня
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(0, 0, width, height);

        var dayStart = st.DayStartLocal.Date;
        var dayEnd = dayStart.AddDays(1);
        var totalMinutes = (dayEnd - dayStart).TotalMinutes;
        if (totalMinutes <= 0) return;

        // 1) рисуем сегменты (под текстом)
        foreach (var seg in st.Segments)
        {
            var segStart = seg.StartLocal;
            var segEnd = seg.EndLocal;

            if (segEnd <= dayStart || segStart >= dayEnd) continue;

            if (segStart < dayStart) segStart = dayStart;
            if (segEnd > dayEnd) segEnd = dayEnd;

            double startMinutes = (segStart - dayStart).TotalMinutes;
            double endMinutes = (segEnd - dayStart).TotalMinutes;

            float x1 = (float)(width * (startMinutes / totalMinutes));
            float x2 = (float)(width * (endMinutes / totalMinutes));
            float wSeg = Math.Max(1f, x2 - x1);

            canvas.FillColor = st.SegmentColor;
            canvas.FillRectangle(x1, 0, wSeg, height);

            // линии и время
            canvas.FillColor = Colors.Black;

            if (seg.ShowStartBoundary)
            {
                canvas.FillRectangle(x1 - 0.5f, 0, 1, height);
                DrawLeftText(canvas, seg.StartText, x1 + 3, 0, width - (x1 + 3), height);
            }

            if (seg.ShowEndBoundary)
            {
                canvas.FillRectangle(x2 - 0.5f, 0, 1, height);
                DrawLeftText(canvas, seg.EndText, x2 + 3, 0, width - (x2 + 3), height);
            }
        }

        // 2) Title поверх всего (чтобы не перекрывалось заливкой)
        DrawLeftText(canvas, st.Title, 2, 0, width, height);
    }

    private static void DrawLeftText(ICanvas canvas, string text, float x, float y, float w, float h)
    {
        if (string.IsNullOrWhiteSpace(text) || w <= 1 || h <= 1)
            return;

        var drawText = text
            .Replace("\r", "")
            .Replace("\n", " ")
            .Trim();

        // параметры строки (как в PanchangaBarDrawable)
        float height = h;
        var font = GFont.Default;
        var fontSize = Math.Max(10f, height * 0.55f);
        
        canvas.Font = font;
        canvas.FontSize = fontSize; 
        var oneLineH = fontSize + 2f;

        // центрирование по высоте
        var yy = y + (h - oneLineH) / 2f;

        var pad = 1f;
        float xx = x + pad;
        float xLimit = x + w - pad;

        canvas.SaveState();
        canvas.ClipRectangle(x, y, w, h);

        // canvas.FontSize уже установлен
        // FontColor оставь как есть (у тебя по умолчанию черный, либо можно принудительно)
        // canvas.FontColor = Colors.Black;

        foreach (var ch in drawText)
        {
            var s = ch.ToString();

            var sz = canvas.GetStringSize(s, font, fontSize);
            var cw = sz.Width;
            if (cw <= 0.01f) cw = fontSize * 0.3f;

            if (xx + cw > xLimit)
                break;

            canvas.DrawString(
                s,
                xx, yy,
                cw + 1f, oneLineH,
                HorizontalAlignment.Left,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);

            xx += cw;
        }

        canvas.RestoreState();
    }


}
