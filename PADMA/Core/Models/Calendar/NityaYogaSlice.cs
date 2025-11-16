using PADMA.Core.Enums;
using PADMA.Core.Services;

namespace PADMA.Core.Models.Calendar
{
    public class NityaYogaSlice : CalendarSlice
    {
        public int NityaYogaId { get; set; }
        public int ColorId { get; set; }

        public NityaYogaSlice()
        {
            Kind = ETransitKind.NityaYoga;
        }

        internal static int GetYogaColorId(int yogaId)
        {
            return DataCache.Instance.NityaYogaList
                .FirstOrDefault(y => y.Id == yogaId)?
                .ColorId ?? 0;
        }
    }
}
