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
        public int ZodiacId { get; set; }
        public int NakshatraId { get; set; }
        public int PadaId { get; set; }
        public int NavamsaZodiacId { get; set; }  
        public bool IsRetrograde { get; set; }
        public double SiderealLongitude { get; set; }
        public double Ayanamsa { get; set; }

        public override string ToString()
        {
            return $"{DateTimeUtc:yyyy-MM-dd HH:mm:ss} | L={Longitude:F4}° | Z={ZodiacId} | N={NakshatraId} | P={PadaId} | " +
                   $"Nav={NavamsaZodiacId} | Speed={SpeedInLongitude:F5} | Retro={(IsRetrograde ? "R" : "D")}";
        }
    }
}
