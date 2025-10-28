using System;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Represents a calculated planetary state at a specific moment in time.
    /// </summary>
    public class PlanetData
    {
        public DateTime DateTimeUtc { get; set; }
        public int PlanetId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Distance { get; set; }
        public double SpeedInLongitude { get; set; }
        public int ZodiakId { get; set; }
        public int NakshatraId { get; set; }
        public int PadaId { get; set; }
        public int NavamsaZodiakId { get; set; }  // 🔹 добавлено
        public bool IsRetrograde { get; set; }
        public double SiderealLongitude { get; set; }
        public double Ayanamsa { get; set; }

        public override string ToString()
        {
            return $"{DateTimeUtc:yyyy-MM-dd HH:mm:ss} | L={Longitude:F4}° | Z={ZodiakId} | N={NakshatraId} | P={PadaId} | " +
                   $"Nav={NavamsaZodiakId} | Speed={SpeedInLongitude:F5} | Retro={(IsRetrograde ? "R" : "D")}";
        }
    }
}
