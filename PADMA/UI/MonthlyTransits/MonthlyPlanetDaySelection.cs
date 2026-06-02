using PADMA.Core.Enums;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyPlanetDaySelection
{
    public EPlanet Planet { get; init; }
    public int PlanetIndex { get; init; }

    public DateTime DayLocal { get; init; }

    public int DayIndex { get; init; } // 0-based inside visible month
}