using System;
using System.Collections.Generic;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

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

            var data = new PlanetData
            {
                DateTimeUtc = utcDate,
                Longitude = lon,
                Latitude = lat,
                Distance = dist,
                SpeedInLongitude = speedLon,
                ZodiakId = SwissUtility.GetZodiakIdFromDegree(lon),
                NakshatraId = SwissUtility.GetNakshatraIdFromDegree(lon),
                PadaId = SwissUtility.GetPadaIdFromDegree(lon),
                IsRetrograde = speedLon < 0
            };

            return data;
        }

        private static int DateTimeToEpoch(DateTime date)
            => (int)(date - Epoch).TotalSeconds;

        private static DateTime EpochToDateTime(int epoch)
            => Epoch.AddSeconds(epoch);
    }
}
