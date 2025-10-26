namespace PADMA.Core.Models
{
    /// <summary>
    /// Contains the calculated planetary position data and related details.
    /// </summary>
    public class SwissResult
    {
        /// <summary>
        /// Raw array returned by Swiss Ephemeris:
        /// [longitude, latitude, distance, speedLon, speedLat, speedDist].
        /// </summary>
        public double[] CalculationValues { get; set; } = new double[6];

        /// <summary>
        /// Seconds since the start of the UTC day for this calculation.
        /// </summary>
        public int UtcSecondsOfDay { get; set; }

        /// <summary>
        /// Zodiac sign number (1–12) derived from ecliptic longitude.
        /// </summary>
        public int Sign { get; set; }

        /// <summary>
        /// Indicates whether the planet is in retrograde motion.
        /// </summary>
        public bool IsRetrograde { get; set; }

        /// <summary>
        /// True if calculation failed or produced invalid data.
        /// </summary>
        public bool IsCalculationFailed { get; set; }

        /// <summary>
        /// Helper — formatted string for longitude in degrees°minutes′seconds″.
        /// </summary>
        public string FormattedLongitude
        {
            get
            {
                double lon = CalculationValues.Length > 0 ? CalculationValues[0] : 0.0;
                int degrees = (int)lon;
                double minutesFull = (lon - degrees) * 60;
                int minutes = (int)minutesFull;
                int seconds = (int)((minutesFull - minutes) * 60);
                return $"{degrees}° {minutes}′ {seconds}″";
            }
        }
    }
}
