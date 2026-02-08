using System;
using PADMA.Core.Enums;
using PADMA.Core.Models.Calendar;

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

        public int? EclipseId { get; set; }
        public DateTime? EclipseDate { get; set; } // локальная дата дня (00:00) в TZ профиля
        public string? EclipseIcon { get; set; }
        public bool HasEclipse => !string.IsNullOrEmpty(EclipseIcon);
        public bool HasUserEvents { get; set; }

        public string PlanetMarkersText { get; set; } = string.Empty;

        public IList<PanchangaSegment> NakshatraSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> TaraBalaSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> TithiSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> KaranaSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> NityaYogaSegments { get; set; } = new List<PanchangaSegment>();
        public IList<PanchangaSegment> ChandraBalaSegments { get; set; } = new List<PanchangaSegment>();

        public Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>? TransitPack { get; set; }
    }
}
