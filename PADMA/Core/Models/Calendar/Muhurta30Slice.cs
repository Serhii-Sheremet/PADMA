using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public sealed class Muhurta30Slice : CalendarSlice
    {
        public int Muhurta30Id { get; set; }
        public int ColorId { get; set; }

        public Muhurta30Slice()
        {
            Kind = ETransitKind.Muhurta30;
        }
    }
}
