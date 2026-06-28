using PADMA.Core.Enums;
using PADMA.Core.Models.Calendar;
using PADMA.UI.MonthlyTransits;

namespace PADMA.UI.YearlyTransits;

/// <summary>
/// Precomputed visual transit data for one local calendar year.
/// The visual segment types are intentionally reused from Monthly Transits.
/// They describe time intervals and colors and are not month-specific.
/// </summary>
public sealed class YearlyPlanetTransitsData
{
    public int Year { get; init; }

    public DateTime YearStartLocal { get; init; }
    public DateTime YearEndLocal { get; init; }

    public MonthlyMasaShunyaBand? MasaShunya { get; init; }

    public IReadOnlyList<MonthlyPlanetGroup> PlanetGroups { get; init; } = [];

    // Kept for possible future yearly-to-day or yearly-to-month extensions.
    public IReadOnlyDictionary<EPlanet, IReadOnlyList<PlanetSlice>> TransitPack { get; init; }
        = new Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>();
}
