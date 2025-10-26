using PADMA.Core.Native;

namespace PADMA.Core.Utilities
{
    /// <summary>
    /// Provides helper functions for Swiss Ephemeris integration,
    /// including mapping between internal planet IDs and SweConst values.
    /// </summary>
    public static class SwissUtility
    {
        /// <summary>
        /// Converts PADMA internal PlanetId (EPlanet / PLANET table) 
        /// to its Swiss Ephemeris constant (SweConst.SE_*).
        /// </summary>
        /// <param name="planetId">Internal PADMA PlanetId</param>
        /// <returns>Swiss Ephemeris planet constant or -1 if unsupported</returns>
        public static int GetPlanetSWEConstByPlanetId(int planetId)
        {
            return planetId switch
            {
                1 => SweConst.SE_SUN,
                2 => SweConst.SE_MOON,
                3 => SweConst.SE_MARS,
                4 => SweConst.SE_MERCURY,
                5 => SweConst.SE_JUPITER,
                6 => SweConst.SE_VENUS,
                7 => SweConst.SE_SATURN,
                8 => SweConst.SE_MEAN_NODE,   // Rahu (Mean)
                10 => SweConst.SE_TRUE_NODE,  // Rahu (True)
                // 9 and 11 correspond to Ketu (Mean / True) — computed separately
                _ => -1
            };
        }

        /// <summary>
        /// Calculates the opposite longitude position for Ketu based on Rahu.
        /// </summary>
        /// <param name="rahuLongitude">Longitude of Rahu in degrees (0–360)</param>
        /// <returns>Longitude of Ketu in degrees (0–360)</returns>
        public static double AdjustForKetu(double rahuLongitude)
        {
            double ketuLongitude = rahuLongitude + 180.0;
            if (ketuLongitude >= 360.0)
                ketuLongitude -= 360.0;
            return ketuLongitude;
        }

        /// <summary>
        /// Validates if the provided planetId is supported for Swiss Ephemeris computation.
        /// </summary>
        public static bool IsSupportedPlanet(int planetId)
        {
            // Exclude Ketu (Mean and True), handle separately
            return planetId is >= 1 and <= 8 or 10;
        }
    }
}
