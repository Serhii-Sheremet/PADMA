using PADMA.Core.Enums;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyPlanetDayDetailsModel
{
    public EPlanet Planet { get; init; }
    public DateTime SelectedDayLocal { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;

    // Main grouped details: one block per Pada period intersecting selected day.
    public IReadOnlyList<MonthlyPlanetDayPadaPeriodBlock> PadaPeriodBlocks { get; init; } = [];

    // Additional independent blocks, for example Vedha and Mrityu Bhaga.
    public IReadOnlyList<MonthlyPlanetDayDetailsBlock> ExtraBlocks { get; init; } = [];

}

public sealed class MonthlyPlanetDayPadaPeriodBlock
{
    public DateTime StartLocal { get; init; }
    public DateTime EndLocal { get; init; }

    public string Title { get; init; } = string.Empty;

    public IReadOnlyList<MonthlyPlanetDayDetailsLine> Lines { get; init; } = [];

    public string RangeText => $"{StartLocal:dd.MM.yyyy HH:mm:ss} – {EndLocal:dd.MM.yyyy HH:mm:ss}";
}

public sealed class MonthlyPlanetDayDetailsLine
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;

    public DateTime? StartLocal { get; init; }
    public DateTime? EndLocal { get; init; }

    public bool HasRange => StartLocal.HasValue && EndLocal.HasValue;

    public string RangeText => HasRange
        ? $"{StartLocal:dd.MM.yyyy HH:mm:ss} – {EndLocal:dd.MM.yyyy HH:mm:ss}"
        : string.Empty;
}

public sealed class MonthlyPlanetDayDetailsBlock
{
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<MonthlyPlanetDayDetailsRow> Rows { get; init; } = [];
}

public sealed class MonthlyPlanetDayDetailsRow
{
    public DateTime StartLocal { get; init; }
    public DateTime EndLocal { get; init; }

    public string Value { get; init; } = string.Empty;

    public string RangeText => $"{StartLocal:dd.MM.yyyy HH:mm:ss} – {EndLocal:dd.MM.yyyy HH:mm:ss}";
}

