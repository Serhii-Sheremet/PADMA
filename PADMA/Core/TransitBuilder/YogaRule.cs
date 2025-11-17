using System;
using PADMA.Core.Enums;

namespace PADMA.Core.TransitBuilder
{
    public class YogaRule
    {
        public DayOfWeek[] Vara { get; set; }
        public int[] TithiIds { get; set; }
        public int[] ForbiddenTithis { get; set; }
        public ENakshatra[] Nakshatra { get; set; }

        // Поведение
        public bool NeedsOverlap { get; set; } = true;
        public bool MultiResultAllowed { get; set; } = true;
    }
}
