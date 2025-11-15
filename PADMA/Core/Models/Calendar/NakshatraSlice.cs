using PADMA.Core.Services;
using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public class NakshatraSlice : CalendarSlice
    {
        public int NakshatraId { get; set; }
        public int ColorId { get; set; }
        public ENakshatra NakshatraCode { get; set; }

        public NakshatraSlice()
        {
            Kind = ETransitKind.Nakshatra;
        }

        internal static int GetNakshatraColorId(int nakshatraId)
        {
            return DataCache.Instance.NakshatraList
                .FirstOrDefault(n => n.Id == nakshatraId)?
                .ColorId ?? 0;
        }
        
    }
}