using System;
using System.Collections.Generic;
using System.Linq;
using PADMA.Core.Enums;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Represents a full day (UTC-based) containing all active calendar slices (transits).
    /// </summary>
    public class DayTimeline
    {
        /// <summary>UTC date start of the day (00:00:00).</summary>
        public DateTime DateUtc { get; set; }

        /// <summary>List of all transit slices active during this day.</summary>
        public List<CalendarSlice> Items { get; set; } = new();

        public DayTimeline() { }

        public DayTimeline(DateTime dateUtc)
        {
            DateUtc = dateUtc.Date;
        }

        /// <summary>
        /// Returns all transits of a given type within this day.
        /// </summary>
        public IEnumerable<CalendarSlice> GetByKind(ETransitKind kind)
        {
            return Items.Where(i => i.Kind == kind);
        }

        public override string ToString()
        {
            return $"{DateUtc:yyyy-MM-dd} | {Items.Count} slices";
        }
    }
}
