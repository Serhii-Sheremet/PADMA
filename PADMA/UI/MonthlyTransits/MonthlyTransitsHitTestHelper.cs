namespace PADMA.UI.MonthlyTransits;

public static class MonthlyTransitsHitTestHelper
{
    public static MonthlyMasaShunyaDaySelection? HitTestMasaShunyaDay(
        MonthlyTransitsLayout layout,
        double x,
        double y)
    {
        if (x < 0 || y < 0 || y >= layout.TopBandHeight)
            return null;

        var dayIndex = (int)(x / layout.DayWidth);
        if (dayIndex < 0 || dayIndex >= layout.DaysInMonth)
            return null;

        var dayLocal = new DateTime(layout.Year, layout.Month, dayIndex + 1);

        return new MonthlyMasaShunyaDaySelection
        {
            DayIndex = dayIndex,
            DayLocal = dayLocal
        };
    }

    public static MonthlyPlanetDaySelection? HitTestPlanetDay(
        MonthlyTransitsLayout layout,
        double x,
        double y)
    {
        if (x < 0 || y < layout.TopBandHeight)
            return null;

        var dayIndex = (int)(x / layout.DayWidth);
        if (dayIndex < 0 || dayIndex >= layout.DaysInMonth)
            return null;

        var relativeY = y - layout.TopBandHeight;

        var planetIndex = (int)(relativeY / layout.PlanetGroupHeight);
        if (planetIndex < 0 || planetIndex >= layout.Planets.Count)
            return null;

        var yInsideGroup = relativeY - planetIndex * layout.PlanetGroupHeight;

        // Ignore the 2px gap between planet groups.
        if (yInsideGroup >= layout.PlanetContentHeight)
            return null;

        var planet = layout.Planets[planetIndex].Planet;
        var dayLocal = new DateTime(layout.Year, layout.Month, dayIndex + 1);

        return new MonthlyPlanetDaySelection
        {
            Planet = planet,
            PlanetIndex = planetIndex,
            DayIndex = dayIndex,
            DayLocal = dayLocal
        };
    }



}