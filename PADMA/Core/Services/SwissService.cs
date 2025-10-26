using System;
using System.Text;
using PADMA.Core.Native;
using PADMA.Core.Utilities;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Provides calculation services using Swiss Ephemeris native API.
    /// </summary>
    public static class SwissService
    {
        /// <summary>
        /// Gets planetary position (longitude, latitude, distance) for a given UTC date and planet ID.
        /// Handles Rahu (Mean/True) and Ketu (computed as opposite point).
        /// </summary>
        /// <param name="utcDate">UTC datetime</param>
        /// <param name="planetId">Internal PADMA PlanetId</param>
        /// <returns>Array: [longitude, latitude, distance]</returns>
        public static double[] GetPlanetPosition(DateTime utcDate, int planetId)
        {
            // Validate planet support
            if (!SwissUtility.IsSupportedPlanet(planetId))
                throw new ArgumentException($"Unsupported planetId {planetId}. Ketu is handled separately.", nameof(planetId));

            // Convert to Julian Day
            double jd = SwissEphemerisNative.swe_julday(
                utcDate.Year,
                utcDate.Month,
                utcDate.Day,
                utcDate.Hour + utcDate.Minute / 60.0 + utcDate.Second / 3600.0,
                SweConst.SE_GREG_CAL);

            // Get Swiss Ephemeris constant
            int swePlanetConst = SwissUtility.GetPlanetSWEConstByPlanetId(planetId);
            if (swePlanetConst < 0)
                throw new ArgumentException($"Invalid planetId {planetId} mapping.", nameof(planetId));

            // Prepare buffers
            double[] xx = new double[6];
            StringBuilder serr = new(256);

            // Perform calculation
            int result = SwissEphemerisNative.swe_calc_ut(
                jd,
                swePlanetConst,
                SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SPEED,
                xx,
                serr);

            if (result < 0)
                throw new InvalidOperationException($"Swiss Ephemeris error: {serr}");

            double lon = xx[0];
            double lat = xx[1];
            double dist = xx[2];

            // Handle Ketu as opposite Rahu
            if (planetId is 9 or 11)
            {
                lon = SwissUtility.AdjustForKetu(lon);
            }

            return new[] { lon, lat, dist };
        }

        /// <summary>
        /// Sets sidereal mode (default: Lahiri).
        /// </summary>
        public static void SetSiderealMode(int sidMode = 1)
        {
            SwissEphemerisNative.swe_set_sid_mode(sidMode, 0, 0);
        }

        /// <summary>
        /// Gets Ayanamsa for the given UTC date.
        /// </summary>
        public static double GetAyanamsa(DateTime utcDate)
        {
            double jd = SwissEphemerisNative.swe_julday(
                utcDate.Year,
                utcDate.Month,
                utcDate.Day,
                utcDate.Hour + utcDate.Minute / 60.0 + utcDate.Second / 3600.0,
                SweConst.SE_GREG_CAL);

            return SwissEphemerisNative.swe_get_ayanamsa_ut(jd);
        }

        /// <summary>
        /// Releases memory and closes Swiss Ephemeris session.
        /// </summary>
        public static void Close()
        {
            SwissEphemerisNative.swe_close();
        }
    }
}
