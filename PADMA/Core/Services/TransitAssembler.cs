using System;
using System.Collections.Generic;
using PADMA.Core.Models;
using PADMA.Core.Enums;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Combines data from Swiss calculations, cached entities, and app settings into unified daily timelines.
    /// </summary>
    public class TransitAssembler : ITransitAssembler
    {
        public List<DayTimeline> Build(DateTime fromUtc, DateTime toUtc)
        {
            var result = new List<DayTimeline>();
            var days = (toUtc.Date - fromUtc.Date).Days + 1;

            for (int i = 0; i < days; i++)
            {
                var date = fromUtc.Date.AddDays(i);
                var timeline = new DayTimeline(date);

                // TODO: merge all calculated transits (Swiss, cached, etc.)
                // Example placeholder:
                timeline.Items.Add(new CalendarSlice(
                    date.AddHours(6),
                    date.AddHours(18),
                    ETransitKind.Sunrise,
                    "Example Sunrise–Sunset range",
                    "color_sun"
                ));

                result.Add(timeline);
            }

            return result;
        }
    }
}
