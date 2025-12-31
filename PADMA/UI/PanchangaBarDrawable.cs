using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using GFont = Microsoft.Maui.Graphics.Font;

namespace PADMA.UI
{
    public class PanchangaBarDrawable : IDrawable
    {
        public DateTime DayDate { get; set; }
        public IList<PanchangaSegment>? Segments { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (Segments == null || Segments.Count == 0)
                return;

            var dayStart = DayDate.Date;
            var dayEnd = dayStart.AddDays(1);

            float width  = dirtyRect.Width;
            float height = dirtyRect.Height;

            double totalMinutes = (dayEnd - dayStart).TotalMinutes;

            foreach (var seg in Segments)
            {
                var segStart = seg.Start;
                var segEnd   = seg.End;

                if (segEnd <= dayStart || segStart >= dayEnd)
                    continue;

                if (segStart < dayStart) segStart = dayStart;
                if (segEnd > dayEnd)     segEnd   = dayEnd;

                var startMinutes = (segStart - dayStart).TotalMinutes;
                var endMinutes   = (segEnd   - dayStart).TotalMinutes;

                float x1 = (float)(width * (startMinutes / totalMinutes));
                float x2 = (float)(width * (endMinutes / totalMinutes));
                float w  = Math.Max(1f, x2 - x1);

                if (!seg.IsSplitColor)
                {
                    // цвет сегмента
                    canvas.FillColor = seg.Color ?? Colors.Transparent;
                    canvas.FillRectangle(x1, 0, w, height);
                }
                else 
                {
                    // верхняя половина
                    canvas.FillColor = seg.ColorTop ?? seg.Color ?? Colors.Transparent;
                    canvas.FillRectangle(x1, 0, w, height / 2);

                    // нижняя половина
                    canvas.FillColor = seg.ColorBottom ?? seg.Color ?? Colors.Transparent;
                    canvas.FillRectangle(x1, height / 2, w, height / 2);
                }

                // --- TEXT (no wrapping) ---
                var text = seg.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (height >= 18 && w >= 24)
                    {
                        var pad = 1f;
                        var drawW = Math.Max(1f, w - pad * 2);

                        var drawText = text
                            .Replace("\r", "")
                            .Replace("\n", " ")
                            .Trim();

                        var font = GFont.Default;
                        var fontSize = Math.Max(10f, height * 0.55f);

                        // центрируем по высоте одной строки (примерно)
                        var oneLineH = fontSize + 2f;
                        var y = (height - oneLineH) / 2f;

                        canvas.SaveState();
                        canvas.ClipRectangle(x1, 0, w, height);

                        canvas.Font = font;
                        canvas.FontSize = fontSize;
                        canvas.FontColor = Colors.Black;

                        // --- draw char-by-char to prevent any wrapping ---
                        float x = x1 + pad;
                        float xLimit = x1 + pad + drawW;

                        foreach (var ch in drawText)
                        {
                            var s = ch.ToString();

                            // измеряем ширину символа тем же шрифтом/размером
                            var sz = canvas.GetStringSize(s, font, fontSize);
                            var cw = sz.Width;

                            if (cw <= 0.01f) cw = fontSize * 0.3f; // запасной вариант

                            if (x + cw > xLimit)
                                break;

                            // рисуем символ в маленьком прямоугольнике
                            canvas.DrawString(
                                s,
                                x, y,
                                cw + 1f, oneLineH,
                                HorizontalAlignment.Left,
                                VerticalAlignment.Center,
                                TextFlow.ClipBounds);

                            x += cw;
                        }

                        canvas.RestoreState();
                    }
                }

                // вертикальная чёрная линия, если не в самом конце
                if (Math.Abs(x2 - width) > 0.5f)
                {
                    canvas.FillColor = Colors.Black;
                    canvas.FillRectangle(x2 - 0.5f, 0, 1, height);
                }
            }
            
            // --- ГОРИЗОНТАЛЬНЫЙ БОРДЕР по верхней и нижней границе полоски ---
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 1;

            // Верхняя граница
            canvas.DrawLine(0, 0, width, 0);

            // Нижняя граница
            canvas.DrawLine(0, height - 1, width, height - 1);
        }
    }
}
