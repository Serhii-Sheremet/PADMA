using PADMA.UI;

namespace PADMA.UI.Services
{
    /// <summary>
    /// Context for swipe (42-day window) and other navigation.
    /// </summary>
    public sealed class DayWindowContext
    {
        public required IReadOnlyList<DayItem> Days { get; init; }
        public required int SelectedIndex { get; init; }
    }

    /// <summary>
    /// What DayPage needs without recomputation.
    /// </summary>
    public sealed class DayNavBundle
    {
        public required DayItem Day { get; init; }
        public DayOverviewData? Overview { get; init; }

        // For future: swipe day-to-day in the same 42-day window
        public DayWindowContext? Window { get; init; }
    }
}
