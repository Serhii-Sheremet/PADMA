using System;
using Microsoft.Maui.Graphics;

namespace PADMA.UI
{
    /// <summary>
    /// Один непрерывный кусочек полосы панчанги в пределах суток.
    /// </summary>
    public class PanchangaSegment
    {
        public DateTime Start { get; set; }     // реальное время начала транзита
        public DateTime End { get; set; }       // реальное время конца транзита
        public Color Color { get; set; }        // цвет этого транзита

        // текст для отрисовки прямо на сегменте
        public string? Text { get; set; }
    }
}
