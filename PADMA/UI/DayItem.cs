using System;

namespace PADMA.UI
{
    /// <summary>
    /// Модель одного дня в календаре.
    /// </summary>
    public class DayItem
    {
        public int DayNumber { get; set; }      // число дня (1–31)
        public DateTime Date { get; set; }      // полная дата
        public bool IsCurrentMonth { get; set; } // принадлежит ли текущему месяцу
        public bool IsToday { get; set; }        // сегодняшний ли день

        public IList<PanchangaSegment> NakshatraSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> TaraBalaSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> TithiSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> KaranaSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> NityaYogaSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> ChandraBalaSegments { get; set; } = new List<PanchangaSegment>();
    }
}
