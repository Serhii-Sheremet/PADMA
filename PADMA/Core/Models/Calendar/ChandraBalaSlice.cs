using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public class ChandraBalaSlice : CalendarSlice
    {
        public int HouseNumber { get; set; }       // 1..12
        public EZodiac ZodiacCode { get; set; }    // Current Moon zodiac
        public int ColorId { get; set; }           // RED/GREEN

        public ChandraBalaSlice()
        {
            Kind = ETransitKind.ChandraBala;
        }
    }
}
