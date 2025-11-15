using PADMA.Core.Services;
using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public class TithiSlice : CalendarSlice
    {
        public int TithiId { get; set; }
        public int ColorId { get; set; }

        public TithiSlice()
        {
            Kind = ETransitKind.Tithi;
        }

        internal static int GetTithiColorId(int tithiId)
        {
            return DataCache.Instance.TithiList
                .FirstOrDefault(t => t.Id == tithiId)?
                .ColorId ?? 0;
        }
    }
}