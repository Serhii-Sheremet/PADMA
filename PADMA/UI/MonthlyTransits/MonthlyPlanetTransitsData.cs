using PADMA.Core.Enums;
using PADMA.Core.Models.Calendar;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyPlanetTransitsData
{
    public int Year { get; init; }
    public int Month { get; init; }

    public DateTime MonthStartLocal { get; init; }
    public DateTime MonthEndLocal { get; init; }

    public IReadOnlyList<MonthlyPlanetGroup> PlanetGroups { get; init; } = [];
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