using PADMA.Core.Enums;

namespace PADMA.Core.Models.Calendar
{
    /// <summary>
    /// A transit slice for a single planet: interval where all key attributes
    /// (zodiac, nakshatra, pada, retrograde state...) remain constant.
    /// </summary>
    public class PlanetSlice : CalendarSlice
    {
        /// <summary>
        /// Planet ID (mapped to EPlanet and to DB tables).
        /// </summary>
        public int PlanetId { get; set; }

        /// <summary>
        /// For Rahu/Ketu we must know whether slice refers to MEAN or TRUE mode.
        /// </summary>
        public EAppSetting NodeType { get; set; } = EAppSetting.NODEMEAN;

        /// <summary>
        /// Zodiac sign ID (1–12), corresponds to EZodiac.
        /// </summary>
        public int ZodiacId { get; set; }

        /// <summary>
        /// Nakshatra ID (1–27), corresponds to ENakshatra.
        /// </summary>
        public int NakshatraId { get; set; }

        /// <summary>
        /// Pada 1–4.
        /// </summary>
        public int PadaId { get; set; }

        /// <summary>
        /// Whether planet is retrograde during this slice.
        /// </summary>
        public bool IsRetrograde { get; set; }

        /// <summary>
        /// Tara Bala group (1–27). Derived from nakshatra & natal Moon nakshatra.
        /// </summary>
        public int TaraBalaId { get; set; }

        /// <summary>
        /// Tara Bala percentage (0–100). Derived metric.
        /// </summary>
        public int TaraBalaPercent { get; set; }

        /// <summary>
        /// Zodiac sign ID for Navamsa (1–12), corresponds to EZodiac.
        /// </summary>
        public int NavamsaZodiacId { get; set; }

        /// <summary>
        /// House number (1–12) from natal Moon (Chandra Lagna).
        /// </summary>
        public int HouseFromMoon { get; set; }

        /// <summary>
        /// House number (1–12) from Lagna (Ascendant).
        /// </summary>
        public int HouseFromLagna { get; set; }

        /// <summary>
        /// Color code for this planet slice from Natal Moon.
        /// </summary>
        public EColor MoonColorCode { get; set; } = EColor.NOCOLOR;

        /// <summary>
        /// Color code for this planet slice from Lagna.
        /// </summary>
        public EColor LagnaColorCode { get; set; } = EColor.NOCOLOR;

        public PlanetSlice()
        {
            Kind = ETransitKind.Planet;
        }
    }
}
