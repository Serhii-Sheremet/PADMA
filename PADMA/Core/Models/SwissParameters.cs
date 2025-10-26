using System;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Represents all required parameters for a Swiss Ephemeris calculation.
    /// </summary>
    public class SwissParameters
    {
        /// <summary>
        /// Internal PADMA planet identifier (from EPlanet enum and PLANET table).
        /// </summary>
        public int PlanetId { get; set; }

        /// <summary>
        /// Internal planet code (e.g., "SUN", "MOON", "MARS").
        /// </summary>
        public string PlanetCode { get; set; } = string.Empty;

        /// <summary>
        /// Geographic longitude of observation point (East positive).
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Geographic latitude of observation point (North positive).
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Altitude above sea level in meters.
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// UTC date and time used as the reference for calculation.
        /// </summary>
        public DateTime UtcDateTime { get; set; }

        /// <summary>
        /// Optional — Local timezone offset in hours (for later UI conversions).
        /// </summary>
        public double TimeZoneOffsetHours { get; set; }

        public SwissParameters() { }

        public SwissParameters(
            int planetId,
            string planetCode,
            double longitude,
            double latitude,
            double altitude,
            DateTime utcDateTime)
        {
            PlanetId = planetId;
            PlanetCode = planetCode;
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
            UtcDateTime = utcDateTime;
        }
    }
}
