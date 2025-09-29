namespace PADMA.Core.Models
{
    public class DayItem
    {
        // Calendar cell number
        public int DayNumber { get; set; }

        // True if this cell belongs to the current month
        public bool IsCurrentMonth { get; set; }

        // Absolute date of this cell (needed for navigation)
        public DateTime Date { get; set; }

        // Convenience flag for highlighting "today"
        public bool IsToday { get; set; }
    }
}
