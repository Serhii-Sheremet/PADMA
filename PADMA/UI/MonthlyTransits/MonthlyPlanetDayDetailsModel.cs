using PADMA.Core.Enums;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyPlanetDayDetailsModel
{
    public EPlanet Planet { get; init; }
    public DateTime SelectedDayLocal { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;

    public IReadOnlyList<MonthlyPlanetDayDetailsBlock> Blocks { get; init; } = [];
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