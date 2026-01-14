using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PADMA.UI
{
    /// <summary>
    /// Extra computed data for DayOverviewPage (beyond what CalendarViewModel precomputes).
    /// Stores structural data (segments/ids), not localized strings.
    /// </summary>
    public class DayOverviewData
    {
        public string SunriseLabelText { get; set; } = string.Empty;
        public string SunsetLabelText { get; set; } = string.Empty;
        public string SunriseText { get; set; } = string.Empty;
        public string SunsetText { get; set; } = string.Empty;
        public DateTime? SunriseUtc { get; set; }
        public DateTime? SunsetUtc { get; set; }

        public bool HasEclipse { get; set; }
        public string EclipseIcon { get; set; } = string.Empty;
        public string EclipseText { get; set; } = string.Empty;
        public string EclipseTime { get; set; } = string.Empty; // "HH:mm"


        public DayKey Key { get; }

        public IList<PlanetStripe> PlanetStripes { get; } = new List<PlanetStripe>();

        public ObservableCollection<MuhurtaOverviewStripe> MuhurtaStripes { get; } = new();

        public ObservableCollection<YogaOverviewStripe> YogaStripes { get; } = new();

        public DayOverviewData(DayKey key)
        {
            Key = key;
        }
    }
}
