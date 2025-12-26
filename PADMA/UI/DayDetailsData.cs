using System.Collections.Generic;

namespace PADMA.UI
{
    /// <summary>
    /// Heavy computed data for DayPage (timeline with 20+ bars, transits, eclipses, etc.).
    /// Stores structural data (segments/ids), not localized strings.
    /// </summary>
    public class DayDetailsData
    {
        public DayKey Key { get; }

        /// <summary>
        /// Vertical timeline segments (placeholder).
        /// Later we will define a richer model (tracks/rows, time scale, etc.).
        /// </summary>
        public IList<PanchangaSegment> TimelineSegments { get; } = new List<PanchangaSegment>();

        public DayDetailsData(DayKey key)
        {
            Key = key;
        }
    }
}
