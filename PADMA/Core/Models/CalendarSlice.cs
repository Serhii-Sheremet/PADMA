using System;
using PADMA.Core.Enums;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Represents a single time slice of any astrological transit or event.
    /// It defines when the event starts and ends, what kind it is, and any display attributes.
    /// </summary>
    public class CalendarSlice
    {
        /// <summary>UTC start time of the event.</summary>
        public DateTime StartUtc { get; set; }

        /// <summary>UTC end time of the event.</summary>
        public DateTime EndUtc { get; set; }

        /// <summary>Type of event, used for filtering and display.</summary>
        public ETransitKind Kind { get; set; }

        /// <summary>Localized display title for UI (e.g. "Tithi 5 - Panchami").</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Optional identifier or color key for UI styling.</summary>
        public string ColorKey { get; set; } = string.Empty;

        /// <summary>Optional link to underlying data (e.g. database entity key).</summary>
        public string PayloadKey { get; set; } = string.Empty;

        /// <summary>Indicates if this slice covers the whole day (e.g. daily yoga or tithi).</summary>
        public bool IsFullDay { get; set; }

        /// <summary>
        /// Duration of the slice (UTC-based).
        /// </summary>
        public TimeSpan Duration => EndUtc - StartUtc;

        public CalendarSlice() { }

        public CalendarSlice(DateTime startUtc, DateTime endUtc, ETransitKind kind, string title, string colorKey, string payloadKey = "")
        {
            StartUtc = startUtc;
            EndUtc = endUtc;
            Kind = kind;
            Title = title;
            ColorKey = colorKey;
            PayloadKey = payloadKey;
        }

        public override string ToString()
        {
            return $"{Kind}: {Title} [{StartUtc:yyyy-MM-dd HH:mm:ss} → {EndUtc:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
