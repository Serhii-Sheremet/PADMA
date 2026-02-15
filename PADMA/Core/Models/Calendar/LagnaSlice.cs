using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public class LagnaSlice : CalendarSlice
    {
        public int ZodiacId { get; set; }
        public int NakshatraId { get; set; }
        public int PadaId { get; set; }
        public int NavamsaZodiacId { get; set; }

        public LagnaSlice()
        {
            Kind = ETransitKind.Lagna;
        }
    }
}
