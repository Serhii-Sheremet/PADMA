using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    public sealed class HoraSlice : CalendarSlice
    {
        public EHoraPlanet PlanetCode { get; set; }
        public EColor ColorCode { get; set; }
        public bool IsDayLightHora { get; set; }

        public HoraSlice()
        {
            Kind = ETransitKind.Hora;
        }
    }
}
