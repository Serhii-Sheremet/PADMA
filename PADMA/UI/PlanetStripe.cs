using PADMA.Core.Enums;

namespace PADMA.UI
{
    public class PlanetStripe
    {
        public EPlanet Planet { get; set; }

        // Сегменты внутри дня (по времени)
        public IList<PanchangaSegment> Segments { get; set; } = new List<PanchangaSegment>();
    }
}
