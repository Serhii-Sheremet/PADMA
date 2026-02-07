using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public sealed class Ghati60Slice : CalendarSlice
    {
        public int Ghati60Id { get; set; }
        public int ColorId { get; set; }
        public bool IsDayLightGhati { get; set; }

        public Ghati60Slice()
        {
            Kind = ETransitKind.Ghati60;
        }
    }
}
