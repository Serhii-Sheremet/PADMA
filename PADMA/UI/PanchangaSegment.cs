using System;
using Microsoft.Maui.Graphics;
using PADMA.Core.Enums;

namespace PADMA.UI
{
    /// <summary>
    /// Один непрерывный кусочек полосы панчанги в пределах суток.
    /// </summary>
    public class PanchangaSegment
    {
        public ETransitKind TransitKind { get; set; }
        public int TransitId { get; set; }
        public DateTime TransitStart { get; set; }   // full transit start (local)
        public DateTime TransitEnd { get; set; }   // full transit end (local)
        public DateTime Start { get; set; }     // effective start within this day
        public DateTime End { get; set; }       // effective end within this day
        public Color? Color { get; set; }        // цвет этого транзита
        public Color? ColorTop { get; set; }   // optional
        public Color? ColorBottom { get; set; } // optional
        public bool IsSplitColor { get; set; }  // optional

        // текст для отрисовки прямо на сегменте
        public string? Text { get; set; }
    }
}
