using PADMA.Core.Enums;
using PADMA.Core.Services;

namespace PADMA.Core.Models.Calendar
{
    public class YogaSlice : CalendarSlice
    {
        public int YogaId { get; set; }
        public EYoga YogaCode { get; set; }
        public DayOfWeek Vara { get; set; }
        public int TithiId { get; set; }
        public ENakshatra NakshatraCode { get; set; }
        public int ColorId { get; set; }

        public YogaSlice()
        {
            Kind = ETransitKind.Yoga;
        }

        internal static int GetYogaColorId(int yogaId)
        {
            return DataCache.Instance.YogaList
                .FirstOrDefault(y => y.Id == yogaId)?
                .ColorId ?? 0;
        }
    }
}
