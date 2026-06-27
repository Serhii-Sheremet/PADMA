namespace PADMA.UI.YearlyTransits;

public static class YearlyTransitsHitTestHelper
{
    public static int? HitTestMonth(YearlyTransitsLayout layout, double x, double y)
    {
        if (x < 0 || y < 0 || x >= layout.ContentWidth || y >= layout.ContentHeight)
            return null;

        var dayIndex = (int)(x / layout.DayWidth);
        if (dayIndex < 0 || dayIndex >= layout.DaysInYear)
            return null;

        return layout.YearStart.AddDays(dayIndex).Month;
    }
}
