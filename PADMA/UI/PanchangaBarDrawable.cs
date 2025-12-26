using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

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

                // цвет сегмента
                canvas.FillColor = seg.Color;
                canvas.FillRectangle(x1, 0, w, height);

                // --- TEXT (no wrapping) ---
                var text = seg.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (height >= 18 && w >= 24)
                    {
                        var pad = 1f;
                        var drawW = Math.Max(1f, w - pad * 2);

                        // Подбираем размер шрифта (как у тебя было)
                        canvas.FontColor = Colors.Black;
                        var fontSize = Math.Max(10f, height * 0.55f);

                        // Задаём шрифт (обычный системный)
                        var font = Microsoft.Maui.Graphics.Font.Default; 
                        
                        // Измеряем текст
                        var textSize = canvas.GetStringSize(text, font, fontSize);

                        // Если текст не помещается — НЕ рисуем
                        if (textSize.Width <= drawW)
                        {
                            canvas.SaveState();

                            canvas.Font = font;
                            canvas.FontSize = fontSize;
                            canvas.FontColor = Colors.Black;

                            canvas.ClipRectangle(x1, 0, w, height);

                            canvas.DrawString(
                                text,
                                x1 + pad, 0,
                                drawW, height,
                                HorizontalAlignment.Left,
                                VerticalAlignment.Center);

                            canvas.RestoreState();
                        }
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
