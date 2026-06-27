using PADMA.Core.Enums;
using System.Globalization;

namespace PADMA.UI.YearlyTransits;

public sealed class YearlyTransitsLayout
{
    public int Year { get; init; }
    public CultureInfo Culture { get; init; } = CultureInfo.CurrentUICulture;
    public IReadOnlyList<YearlyTransitsPlanetRow> Planets { get; init; } = [];

    public string TopBandLabel { get; init; } = string.Empty;

    // Keep a day as the internal horizontal unit. Month widths therefore remain proportional
    // to their real day counts without displaying individual day labels.
    public double DayWidth { get; init; } = 10;
    public double HeaderHeight { get; init; } = 44;
    public double LabelWidth { get; init; } = 96;

    public double TopBandHeight { get; init; } = 26;
    public double LaneHeight { get; init; } = 20;
    public double PlanetGap { get; init; } = 2;

    public int DaysInYear => DateTime.IsLeapYear(Year) ? 366 : 365;
    public DateTime YearStart => new(Year, 1, 1);
    public double ContentWidth => DaysInYear * DayWidth;

    public double PlanetContentHeight => 4 * LaneHeight;
    public double PlanetGroupHeight => PlanetContentHeight + PlanetGap;
    public double ContentHeight => TopBandHeight + Planets.Count * PlanetGroupHeight;

    public int? SelectedMonth { get; init; }

    public double GetMonthStartX(int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        var monthStart = new DateTime(Year, month, 1);
        return (monthStart - YearStart).TotalDays * DayWidth;
    }

    public double GetMonthWidth(int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month));

        return DateTime.DaysInMonth(Year, month) * DayWidth;
    }
}

public sealed class YearlyTransitsPlanetRow
{
    public EPlanet Planet { get; init; }
    public string Name { get; init; } = string.Empty;
}
