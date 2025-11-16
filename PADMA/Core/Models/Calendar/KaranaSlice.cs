using PADMA.Core.Enums;
using PADMA.Core.Services;

namespace PADMA.Core.Models.Calendar
{
    public class KaranaSlice : CalendarSlice
    {
        public int KaranaId { get; set; }
        public int ColorId { get; set; }

        public KaranaSlice()
        {
            Kind = ETransitKind.Karana;
        }

        internal static int GetKaranaColorId(int karanaId)
        {
            return DataCache.Instance.KaranaList
                .FirstOrDefault(k => k.Id == karanaId)?
                .ColorId ?? 0;
        }
    }
}
