using System;
using PADMA.Core.Native;

namespace PADMA.Core.Utilities
{
    /// <summary>
    /// Provides helper functions for Swiss Ephemeris integration,
    /// including planet mapping and astrological zone calculations.
    /// </summary>
    public static class SwissUtility
    {
        /// <summary>
        /// Converts PADMA internal PlanetId (EPlanet / PLANET table)
        /// to its Swiss Ephemeris constant (SweConst.SE_*).
        /// </summary>
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
                // 9 и 11 — Ketu (Mean / True) — вычисляются как противоположные точки
                _ => -1
            };
        }

        /// <summary>
        /// Calculates the opposite longitude (for Ketu based on Rahu).
        /// </summary>
        public static double AdjustForKetu(double rahuLongitude)
        {
            double ketuLongitude = rahuLongitude + 180.0;
            if (ketuLongitude >= 360.0)
                ketuLongitude -= 360.0;
            return ketuLongitude;
        }

        /// <summary>
        /// Validates if planetId is supported for Swiss Ephemeris computation.
        /// </summary>
        public static bool IsSupportedPlanet(int planetId)
        {
            // Exclude Ketu (Mean and True), handle separately
            return planetId is >= 1 and <= 8 or 10;
        }

        // ─────────────────────────────────────────────
        //  Astrological divisions by longitude
        // ─────────────────────────────────────────────

        /// <summary>
        /// Returns Zodiac sign ID (1–12) from longitude.
        /// </summary>
        public static int GetZodiakIdFromDegree(double longitude)
        {
            double znakPart = 360.0 / 12.0;
            double currentDZnak = longitude / znakPart;
            double intDZnak = Math.Floor(currentDZnak);

            int currentZnak = (currentDZnak > intDZnak)
                ? Convert.ToInt32(intDZnak) + 1
                : Convert.ToInt32(intDZnak);

            if (currentZnak < 1) currentZnak = 1;
            if (currentZnak > 12) currentZnak = 12;

            return currentZnak;
        }

        /// <summary>
        /// Returns Nakshatra ID (1–27) from longitude.
        /// </summary>
        public static int GetNakshatraIdFromDegree(double longitude)
        {
            double nakshatraPart = 360.0 / 27.0;
            double currentDNakshatra = longitude / nakshatraPart;
            double intDNakshatra = Math.Floor(currentDNakshatra);

            int currentNakshatra = (currentDNakshatra > intDNakshatra)
                ? Convert.ToInt32(intDNakshatra) + 1
                : Convert.ToInt32(intDNakshatra);

            if (currentNakshatra < 1) currentNakshatra = 1;
            if (currentNakshatra > 27) currentNakshatra = 27;

            return currentNakshatra;
        }

        /// <summary>
        /// Returns Pada ID (1–108) from longitude.
        /// </summary>
        public static int GetPadaIdFromDegree(double longitude)
        {
            double padaPart = 360.0 / 108.0;
            double currentDPada = longitude / padaPart;
            double intDPada = Math.Floor(currentDPada);

            int currentPada = (currentDPada > intDPada)
                ? Convert.ToInt32(intDPada) + 1
                : Convert.ToInt32(intDPada);

            if (currentPada < 1) currentPada = 1;
            if (currentPada > 108) currentPada = 108;

            return currentPada;
        }

        /// <summary>
        /// Returns Navamsa Zodiac ID (1–12) based on Nakshatra and Pada.
        /// Placeholder until DB cache is connected.
        /// </summary>
        public static int GetNavamsaZodiakId(int nakshatraId, int padaId)
        {
            // TODO: replace with CacheLoad._padaList lookup
            if (padaId <= 0) return 0;
            return ((padaId - 1) / 9) + 1; // Approximation: 108 padas / 12 signs = 9 per sign
        }
    }
}
