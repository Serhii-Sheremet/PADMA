using PADMA.Core.Enums;
using System.Globalization;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyTransitsLayout
{
    public int Year { get; init; }
    public int Month { get; init; }
    public CultureInfo Culture { get; init; } = CultureInfo.CurrentUICulture;
    public IReadOnlyList<MonthlyTransitsPlanetRow> Planets { get; init; } = [];

    public string TopBandLabel { get; init; } = "Masa/Shunya";

    public double DayWidth { get; init; } = 42;
    public double HeaderHeight { get; init; } = 44;
    public double LabelWidth { get; init; } = 96;

    public double TopBandHeight { get; init; } = 26;
    public double TopBandPaddingY { get; init; } = 2;

    public double LaneHeight { get; init; } = 20;
    public double PlanetGap { get; init; } = 2;

    public int DaysInMonth => DateTime.DaysInMonth(Year, Month);
    public double ContentWidth => DaysInMonth * DayWidth;

    public double PlanetContentHeight => 4 * LaneHeight;
    public double PlanetGroupHeight => PlanetContentHeight + PlanetGap;

    public double ContentHeight => TopBandHeight + Planets.Count * PlanetGroupHeight;

    public MonthlyPlanetTransitsData? Data { get; init; }

    public MonthlyPlanetDaySelection? Selection { get; init; }
}

public sealed class MonthlyTransitsPlanetRow
{
    public EPlanet Planet { get; init; }
    public string Name { get; init; } = string.Empty;
}