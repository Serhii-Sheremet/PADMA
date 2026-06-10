using PADMA.Core.Enums;
using PADMA.Core.Models.Calendar;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyMasaShunyaBand
{
    public IReadOnlyList<MonthlyTransitSegment> MasaSegments { get; init; } = [];
    public IReadOnlyList<MonthlyTransitOverlaySegment> ShunyaNakshatraOverlays { get; init; } = [];
    public IReadOnlyList<MonthlyTransitOverlaySegment> ShunyaTithiOverlays { get; init; } = [];

    public IReadOnlyList<MonthlyMasaShunyaDetailSegment> DetailSegments { get; init; } = [];
}

public sealed class MonthlyMasaShunyaDetailSegment
{
    public MonthlyMasaShunyaDetailKind Kind { get; init; }

    public DateTime StartLocal { get; init; }
    public DateTime EndLocal { get; init; }

    // Main line text, for example:
    // "Джиештха", "7.Пунарвасу", "4.Чатуртхи"
    public string NameText { get; init; } = string.Empty;

    // Optional. Used for Masa and Shunya Nakshatra.
    // For example: "Кету", "Юпитер".
    public string RulerText { get; init; } = string.Empty;

    // Optional. Used only for Masa.
    // For example: "19.Мула".
    public string FullMoonNakshatraText { get; init; } = string.Empty;

    // Optional. Used only for Masa.
    // For example: "Кету".
    public string FullMoonNakshatraRulerText { get; init; } = string.Empty;
}

public enum MonthlyMasaShunyaDetailKind
{
    Masa,
    ShunyaNakshatra,
    ShunyaTithi
}

public sealed class MonthlyPlanetTransitsData
{
    public int Year { get; init; }
    public int Month { get; init; }

    public DateTime MonthStartLocal { get; init; }
    public DateTime MonthEndLocal { get; init; }

    public MonthlyMasaShunyaBand? MasaShunya { get; init; }

    public IReadOnlyList<MonthlyEclipseDayMarker> EclipseDays { get; init; } = [];

    public IReadOnlyList<MonthlyPlanetGroup> PlanetGroups { get; init; } = [];

    public IReadOnlyDictionary<EPlanet, IReadOnlyList<PlanetSlice>> TransitPack { get; init; }
    = new Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>();
    
}

public sealed class MonthlyPlanetGroup
{
    public EPlanet Planet { get; init; }
    public string PlanetName { get; init; } = string.Empty;
    public IReadOnlyList<MonthlyTransitLane> Lanes { get; init; } = [];
}

public sealed class MonthlyTransitLane
{
    public MonthlyTransitLaneKind Kind { get; init; }
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<MonthlyTransitSegment> Segments { get; init; } = [];
    public IReadOnlyList<MonthlyTransitOverlaySegment> Overlays { get; init; } = [];
}

public sealed class MonthlyTransitOverlaySegment
{
    public DateTime StartLocal { get; init; }
    public DateTime EndLocal { get; init; }

    public Color Color { get; init; } = Colors.Transparent;
}

public sealed class MonthlyTransitSegment
{
    public DateTime StartLocal { get; init; }
    public DateTime EndLocal { get; init; }

    // Real, non-visual-clipped period boundaries.
    // Tooltip/details should prefer these values.
    public DateTime RealStartLocal { get; init; }
    public DateTime RealEndLocal { get; init; }

    public string Text { get; init; } = string.Empty;

    public Color? Color { get; init; }
    public Color? ColorTop { get; init; }
    public Color? ColorBottom { get; init; }
    public bool IsSplitColor { get; init; }

    public PlanetSlice? SourceSlice { get; init; }
}

public enum MonthlyTransitLaneKind
{
    Zodiac,
    Nakshatra,
    Pada,
    TaraBala
}

public sealed class MonthlyEclipseDayMarker
{
    public DateTime DayLocal { get; init; }
    public int EclipseId { get; init; }
    public DateTime EclipseDateUtc { get; init; }
}