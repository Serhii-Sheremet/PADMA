using PADMA.Core.Services;
using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public class SunriseSlice : CalendarSlice
    {
        public DateTime PreviousSunriseUtc { get; set; }
        public DateTime SunriseUtc { get; set; }
        public DateTime SunsetUtc { get; set; }
        public DateTime NextSunriseUtc { get; set; }

        public SunriseSlice()
        {
            Kind = ETransitKind.Sunrise;
        }
    }

}
