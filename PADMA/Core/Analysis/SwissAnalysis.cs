using PADMA.Core.Models;
using PADMA.Core.Native;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Core.Enums;
using System;
using System.Collections.Generic;

namespace PADMA.Core.Analysis
{
    /// <summary>
    /// Calculates all planetary state changes (sign, nakshatra, pada, retrograde)
    /// for a given planet within a specified UTC range.
    /// Uses London coordinates as the base reference (GMT+0).
    /// </summary>
    public static class SwissAnalysis
    {
        private const double LondonLongitude = -0.17;
        private const double LondonLatitude = 51.5;

        private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static List<PlanetData> CalculatePlanetDataList_London(int planetId, DateTime startUtc, DateTime endUtc)
        {
            var results = new List<PlanetData>();

            int startEpoch = DateTimeToEpoch(startUtc);
            int endEpoch = DateTimeToEpoch(endUtc);
            int currentEpoch = startEpoch;

            int stepSeconds = 3600; // 1 hour step
            var prevData = CalculatePlanetData(planetId, EpochToDateTime(currentEpoch));
            results.Add(prevData);

            while (currentEpoch < endEpoch)
            {
                currentEpoch += stepSeconds;
                var newData = CalculatePlanetData(planetId, EpochToDateTime(currentEpoch));

                if (HasStateChanged(prevData, newData))
                {
                    int preciseEpoch = FindTransitionEpoch(planetId, prevData, newData, currentEpoch - stepSeconds, currentEpoch);
                    var preciseData = CalculatePlanetData(planetId, EpochToDateTime(preciseEpoch));

                    results.Add(preciseData);
                    prevData = preciseData;
                    currentEpoch = preciseEpoch;
                }
            }

            return results;
        }

        private static bool HasStateChanged(PlanetData a, PlanetData b)
        {
            return a.ZodiakId != b.ZodiakId
                || a.NakshatraId != b.NakshatraId
                || a.PadaId != b.PadaId
                || a.IsRetrograde != b.IsRetrograde;
        }

        private static int FindTransitionEpoch(int planetId, PlanetData fromState, PlanetData toState, int startEpoch, int endEpoch)
        {
            if (endEpoch - startEpoch <= 1)
                return endEpoch;

            int midEpoch = startEpoch + (endEpoch - startEpoch) / 2;
            var midData = CalculatePlanetData(planetId, EpochToDateTime(midEpoch));

            if (HasStateChanged(fromState, midData))
                return FindTransitionEpoch(planetId, fromState, midData, startEpoch, midEpoch);
            else
                return FindTransitionEpoch(planetId, midData, toState, midEpoch, endEpoch);
        }

        private static PlanetData CalculatePlanetData(int planetId, DateTime utcDate)
        {
            var position = SwissService.GetPlanetPosition(utcDate, planetId);
            double lon = position[0];
            double lat = position[1];
            double dist = position[2];

            // speed calculation (difference in longitude per minute)
            var positionLater = SwissService.GetPlanetPosition(utcDate.AddMinutes(1), planetId);
            double speedLon = positionLater[0] - lon;
            if (speedLon > 180) speedLon -= 360;
            if (speedLon < -180) speedLon += 360;

            // derive IDs
            int zodiakId = SwissUtility.GetZodiakIdFromDegree(lon);
            int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(lon);
            int padaId = SwissUtility.GetPadaIdFromDegree(lon);

            // calculate Navamsa from cached PADA list
            int padaNumber = SwissUtility.GetPadaNumberByPadaId(padaId);
            int navamsaZodiakId = SwissUtility.GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber);

            var data = new PlanetData
            {
                DateTimeUtc = utcDate,
                Longitude = lon,
                Latitude = lat,
                Distance = dist,
                SpeedInLongitude = speedLon,
                ZodiakId = zodiakId,
                NakshatraId = nakshatraId,
                PadaId = padaId,
                NavamsaZodiakId = navamsaZodiakId,
                IsRetrograde = speedLon < 0
            };

            return data;
        }

        private static int DateTimeToEpoch(DateTime date)
            => (int)(date - Epoch).TotalSeconds;

        private static DateTime EpochToDateTime(int epoch)
            => Epoch.AddSeconds(epoch);


        /// <summary>
        /// Calculates all Tithi change times within a UTC range.
        /// Sidereal (Lahiri), geocentric, using London coordinates (GMT+0).
        /// </summary>
        public static List<TithiData> CalculateTithiDataList_London(DateTime fromUtc, DateTime toUtc)
        {
            if (toUtc <= fromUtc)
                return new List<TithiData>();

            // —идерический режим Ч Ћахири
            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            var results = new List<TithiData>();

            double msDiff0 = GetMoonSunDiff(fromUtc);
            int currentTithi = GetCurrentTithi(msDiff0);

            DateTime cursor = fromUtc;

            while (cursor < toUtc)
            {
                // шаг 1 час Ч грубый поиск смены титхи
                var (changed, rough) = ScanUntilTithiChange(cursor, toUtc, currentTithi, TimeSpan.FromHours(1));
                if (!changed)
                    break;

                // уточнение бинарным поиском до секунды
                DateTime exact = RefineChangeTimeToSecond(rough - TimeSpan.FromHours(1), rough, currentTithi);

                double diffAtExact = GetMoonSunDiff(exact);
                currentTithi = GetCurrentTithi(diffAtExact);

                results.Add(new TithiData
                {
                    DateTimeUtc = exact,
                    MoonSunDifference = diffAtExact,
                    TithiId = currentTithi
                });

                cursor = exact.AddSeconds(1);
            }

            return results;
        }

        // --- internal Tithi helpers ---

        private static (bool changed, DateTime when) ScanUntilTithiChange(
            DateTime start, DateTime stop, int currentTithi, TimeSpan step)
        {
            var t = start;
            while (t <= stop)
            {
                var diff = GetMoonSunDiff(t);
                var tithi = GetCurrentTithi(diff);
                if (tithi != currentTithi)
                    return (true, t);
                t = t.Add(step);
            }
            return (false, stop);
        }

        private static DateTime RefineChangeTimeToSecond(
            DateTime lo, DateTime hi, int oldTithi)
        {
            while ((hi - lo).TotalSeconds > 1)
            {
                var mid = lo.AddSeconds((hi - lo).TotalSeconds / 2.0);
                var diff = GetMoonSunDiff(mid);
                var tithi = GetCurrentTithi(diff);
                if (tithi != oldTithi)
                    hi = mid;
                else
                    lo = mid;
            }
            return hi;
        }

        /// <summary>
        /// Returns Moon-Sun longitude difference in sidereal Lahiri mode.
        /// Uses SwissService.GetPlanetPosition() (already sidereal).
        /// </summary>
        private static double GetMoonSunDiff(DateTime utc)
        {
            // ¬ажно: GetPlanetPosition ожидает внутренний PlanetId (не SWE-константы!)
            var sun = SwissService.GetPlanetPosition(utc, (int)EPlanet.SUN); // Sun
            var moon = SwissService.GetPlanetPosition(utc, (int)EPlanet.MOON); // Moon

            double diff = moon[0] - sun[0];
            diff %= 360.0;
            if (diff < 0) diff += 360.0;
            return diff;
        }

        /// <summary>
        /// 1..30 Ч each 12∞ of Moon-Sun angular separation.
        /// </summary>
        private static int GetCurrentTithi(double msDiff)
        {
            const double tithiPart = 360.0 / 30.0; // 12∞
            double d = msDiff / tithiPart;
            int i = (int)d;
            return (d > i) ? i + 1 : i;
        }


    }
}
