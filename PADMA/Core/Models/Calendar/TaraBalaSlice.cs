using PADMA.Core.Enums;
using PADMA.Core.Services;

namespace PADMA.Core.Models.Calendar
{
    public class TaraBalaSlice : CalendarSlice
    {
        public int TaraBalaId { get; set; }        // 1..9
        public int TaraBalaPercent { get; set; }   // 100, 50, 25
        public int ColorId { get; set; }           // From TARABALA table

        public TaraBalaSlice()
        {
            Kind = ETransitKind.TaraBala;
        }

        public static int GetTaraBalaColorId(int taraBalaId, int percent)
        {
            int colorId = DataCache.Instance.TaraBalaList
                .FirstOrDefault(t => t.Id == taraBalaId)?
                .ColorId ?? 0;

            if (percent == 25 && (EColor)colorId == EColor.RED)
                colorId = (int)EColor.PINK;
            
            return colorId;
        }
    }
}
