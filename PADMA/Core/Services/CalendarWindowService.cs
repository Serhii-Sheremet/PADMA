namespace PADMA.Core.Services
{
    public static class CalendarWindowService
    {
        /// <summary>
        /// Returns the 42-day visible window for a given Year/Month,
        /// starting from the calendar's first visible cell (with week offset),
        /// and also returns a buffered range [visibleStart-2d, visibleEnd+2d] for computing transits safely across time zones.
        /// </summary>
        /// <param name="year">Year of the month to display</param>
        /// <param name="month">Month to display</param>
        /// <param name="firstDayOfWeek">0 = Monday, 6 = Sunday, etc. Align with your AppSettings.</param>
        /// <param name="tz">Time zone of the user (affects when transits start/end locally)</param>
        public static (DateTimeOffset visibleStart, DateTimeOffset visibleEnd,
                       DateTimeOffset bufferStart, DateTimeOffset bufferEnd,
                       IReadOnlyList<DateOnly> visibleDays)
            BuildWindow(int year, int month, DayOfWeek firstDayOfWeek, TimeZoneInfo tz)
        {
            var firstOfMonthLocal = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);
            var firstOfMonthLocalOffset = new DateTimeOffset(firstOfMonthLocal, tz.GetUtcOffset(firstOfMonthLocal));

            // shift back to the first cell (week-aligned)
            int delta = ((int)firstOfMonthLocalOffset.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
            var visibleStart = firstOfMonthLocalOffset.AddDays(-delta);
            var visibleEnd   = visibleStart.AddDays(42 - 1).Date.AddDays(1).AddTicks(-1); // inclusive end within last day

            // buffer for cross-timezone safety
            var bufferStart = visibleStart.AddDays(-2);
            var bufferEnd   = visibleEnd.AddDays(+2);

            // Build 42 days as DateOnly for UI binding
            var list = new List<DateOnly>(42);
            for (int i = 0; i < 42; i++)
                list.Add(DateOnly.FromDateTime(visibleStart.AddDays(i).Date));

            return (visibleStart, visibleEnd, bufferStart, bufferEnd, list);
        }
    }
}
