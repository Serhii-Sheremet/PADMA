using System.Collections.Generic;

namespace PADMA.UI
{
    /// <summary>
    /// Extra computed data for DayOverviewPage (beyond what CalendarViewModel precomputes).
    /// Stores structural data (segments/ids), not localized strings.
    /// </summary>
    public class DayOverviewData
    {
        public DayKey Key { get; }

        /// <summary>
        /// 5 Muhurta bars (or any overview-level segments).
        /// Keep as a flat list for now; we can split into 5 lists later if needed.
        /// </summary>
        public IList<PanchangaSegment> MuhurtaSegments { get; } = new List<PanchangaSegment>();

        /// <summary>
        /// Day-level yogas or other quick indicators (structure only).
        /// Later we can replace string with enum/id model if needed.
        /// </summary>
        public IList<string> Yogas { get; } = new List<string>();

        public DayOverviewData(DayKey key)
        {
            Key = key;
        }
    }
}
