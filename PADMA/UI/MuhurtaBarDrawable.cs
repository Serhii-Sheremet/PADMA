using System;
using Microsoft.Maui.Graphics;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI;

public sealed class MuhurtaBarDrawable : IDrawable
{
    public DateTime DayDate { get; set; }              // local date (00:00)
    public MuhurtaOverviewStripe? Stripe { get; set; } // твоя модель

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var st = Stripe;
        if (st == null)
            return;

        float width = dirtyRect.Width;
        float height = dirtyRect.Height;

        // Полная рамка (контур дня)
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 1;
        canvas.DrawRectangle(0, 0, width, height);

        // Если не образуется — только подпись слева
        var font = GFont.Default;
        var fontSize = Math.Max(10f, height * 0.55f);

        canvas.Font = font;
        canvas.FontSize = fontSize;
        canvas.FontColor = Colors.Black;

        DrawLeftText(canvas, st.Title, x: 2, y: 0, w: width, h: height, font, fontSize);

        if (!st.IsFormed || !st.StartLocal.HasValue || !st.EndLocal.HasValue)
            return;

        var dayStart = DayDate.Date;
        var dayEnd = dayStart.AddDays(1);
        var totalMinutes = (dayEnd - dayStart).TotalMinutes;

        // Clamp в пределах суток
        var segStart = st.StartLocal.Value;
        var segEnd = st.EndLocal.Value;

        if (segEnd <= dayStart || segStart >= dayEnd)
            return;

        if (segStart < dayStart) segStart = dayStart;
        if (segEnd > dayEnd) segEnd = dayEnd;

        double startMinutes = (segStart - dayStart).TotalMinutes;
        double endMinutes   = (segEnd   - dayStart).TotalMinutes;

        float x1 = (float)(width * (startMinutes / totalMinutes));
        float x2 = (float)(width * (endMinutes / totalMinutes));
        float wSeg = Math.Max(1f, x2 - x1);

        // Цветной сегмент
        canvas.FillColor = st.SegmentColor;
        canvas.FillRectangle(x1, 0, wSeg, height);

        // Вертикальные линии
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(x1 - 0.5f, 0, 1, height);
        canvas.FillRectangle(x2 - 0.5f, 0, 1, height);

        // --- Время ---
        // end-time: справа от end линии (как обычно)
        DrawLeftText(canvas, st.EndText, x: x2 + 3, y: 0, w: width - (x2 + 3), h: height, font, fontSize);

        // start-time: слева от start линии, но так, чтобы текст целиком влез
        // измеряем ширину "HH:mm"
        var sz = canvas.GetStringSize(st.StartText, font, fontSize);
        float startTextW = sz.Width;
        float startX = x1 - 3 - startTextW;

        // Не даём уйти левее 0 и не заехать на Title (минимум: после Title+gap)
        var titleSize = canvas.GetStringSize(st.Title, font, fontSize);
        float minX = 2 + titleSize.Width + 6; // Title + gap
        if (startX < minX) startX = minX;

        DrawLeftText(canvas, st.StartText, x: startX, y: 0, w: startTextW + 2, h: height, font, fontSize);
    }

    private static void DrawLeftText(ICanvas canvas, string text, float x, float y, float w, float h, GFont font, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(text) || w <= 1 || h <= 1)
            return;

        var s = text.Replace("\r", "").Replace("\n", " ").Trim();
        if (s.Length == 0)
            return;

        canvas.SaveState();
        canvas.ClipRectangle(x, y, w, h);

        // Вертикальное центрирование одной строки
        var oneLineH = canvas.GetStringSize("Ay", font, fontSize).Height;
        var baselineY = y + (h - oneLineH) / 2f;

        // Рисуем строго в одну строку: посимвольно, пока влазит
        float cx = x;
        for (int i = 0; i < s.Length; i++)
        {
            var ch = s[i].ToString();
            var sz = canvas.GetStringSize(ch, font, fontSize);

            // небольшой safety-pad (как в Panchanga)
            if (cx + sz.Width > x + w - 1f)
                break;

            canvas.DrawString(
                ch,
                cx, baselineY,
                sz.Width, oneLineH,
                HorizontalAlignment.Left,
                VerticalAlignment.Top,
                TextFlow.ClipBounds);

            cx += sz.Width;
        }

        canvas.RestoreState();
    }

}
