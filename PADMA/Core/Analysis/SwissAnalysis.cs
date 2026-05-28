using Microsoft.Maui.Controls;
using NodaTime;
using NodaTime.Extensions;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Native;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        private static readonly Dictionary<string, (DateTime StartUtc, DateTime EndUtc)> _zodiacBoundaryCache = new();
        private static readonly object _zodiacBoundaryCacheLock = new();

        public static (DateTime StartUtc, DateTime EndUtc) GetZodiacBoundariesCached(
            int planetId,
            int zodiacId,
            DateTime anchorUtc,
            EAppSetting nodeType)
        {
            // Ketu -> Rahu для границ времени
            if (planetId == (int)EPlanet.KETU)
            {
                planetId = (int)EPlanet.RAHU;
                zodiacId = zodiacId >= 1 && zodiacId <= 12 ? ((zodiacId + 5) % 12) + 1 : zodiacId;
            }

            var anchorKey = anchorUtc.Date.ToString("yyyyMMdd");
            if (anchorUtc.Kind != DateTimeKind.Utc)
                anchorUtc = DateTime.SpecifyKind(anchorUtc, DateTimeKind.Utc);
            var key = $"{planetId}:{zodiacId}:{(int)nodeType}:{anchorKey}";

            lock (_zodiacBoundaryCacheLock)
            {
                if (_zodiacBoundaryCache.TryGetValue(key, out var cached))
                    return cached;
            }

            var start = FindPreviousZodiacChangeUtc(planetId, anchorUtc, nodeType);
            var end = FindNextZodiacChangeUtc(planetId, anchorUtc, nodeType);

            lock (_zodiacBoundaryCacheLock)
            {
                _zodiacBoundaryCache[key] = (start, end);
            }

            return (start, end);
        }

        public static void ClearZodiacBoundaryCache()
        {
            lock (_zodiacBoundaryCacheLock)
                _zodiacBoundaryCache.Clear();
        }

        private static void EnsureAnchorAt(List<PlanetData> list, int planetId, DateTime utc, EAppSetting nodeType)
        {
            if (list.Count == 0 || list.All(x => x.DateTimeUtc != utc))
                list.Add(CalculatePlanetData(planetId, utc, nodeType));
        }

        public static List<PlanetData> CalculatePlanetDataList_London(
            int planetId,
            DateTime startUtc,
            DateTime endUtc,
            EAppSetting nodeType,
            bool includeOuterBoundaries = false)
        {
            if (!includeOuterBoundaries)
                return CalculatePlanetDataList_Core(planetId, startUtc, endUtc, nodeType);

            // 1) состояние в точках start/end (это твой CalculatePlanetData)
            var stateAtStart = CalculatePlanetData(planetId, startUtc, nodeType);
            var stateAtEnd = CalculatePlanetData(planetId, endUtc, nodeType);

            // 2) найдём realStartUtc = начало периода, который содержит startUtc
            var realStartUtc = FindPreviousBoundaryUtc_CoreDriven(planetId, startUtc, nodeType, stateAtStart);

            // 3) найдём realEndUtc = конец периода, который содержит endUtc
            var realEndUtc = FindNextBoundaryUtc_CoreDriven(planetId, endUtc, nodeType, stateAtEnd);

            // 4) один core-проход по расширенному диапазону
            var list = CalculatePlanetDataList_Core(planetId, realStartUtc, realEndUtc, nodeType);

            // 5) гарантируем наличие точек ровно на realStartUtc и realEndUtc
            // (иногда core может не вернуть их “ровно”, из-за шагов/уточнения)
            EnsureAnchorAt(list, planetId, realStartUtc, nodeType);
            EnsureAnchorAt(list, planetId, realEndUtc, nodeType);

            // 6) сортировка + уникальность по DateTimeUtc
            list = list
                .OrderBy(x => x.DateTimeUtc)
                .GroupBy(x => x.DateTimeUtc)
                .Select(g => g.Last())
                .ToList();

            return list;
        }

        private static DateTime FindPreviousBoundaryUtc_CoreDriven(
            int planetId,
            DateTime utcT,
            EAppSetting nodeType,
            PlanetData stateAtT)
        {
            var stepDays = 2;
            const int maxDays = 730; // safety (2 года)

            while (stepDays <= maxDays)
            {
                var from = utcT.AddDays(-stepDays);
                var to = utcT;

                var list = CalculatePlanetDataList_Core(planetId, from, to, nodeType);
                EnsureAnchorAt(list, planetId, utcT, nodeType); // чтобы utcT точно был

                // идём с конца назад, ищем последнее “другое”
                for (int i = list.Count - 2; i >= 0; i--)
                {
                    if (HasStateChanged(list[i], stateAtT))
                        return list[i + 1].DateTimeUtc;
                }

                stepDays *= 2;
            }

            return utcT.AddDays(-maxDays);
        }

        private static DateTime FindNextBoundaryUtc_CoreDriven(
            int planetId,
            DateTime utcT,
            EAppSetting nodeType,
            PlanetData stateAtT)
        {
            var stepDays = 2;
            const int maxDays = 730;

            while (stepDays <= maxDays)
            {
                var from = utcT;
                var to = utcT.AddDays(stepDays);

                var list = CalculatePlanetDataList_Core(planetId, from, to, nodeType);
                EnsureAnchorAt(list, planetId, utcT, nodeType);

                for (int i = 1; i < list.Count; i++)
                {
                    if (HasStateChanged(list[i], stateAtT))
                        return list[i].DateTimeUtc;
                }

                stepDays *= 2;
            }

            return utcT.AddDays(maxDays);
        }

        public static List<PlanetData> CalculatePlanetDataList_Core(int planetId, DateTime startUtc, DateTime endUtc, EAppSetting nodeType)
        {
            var results = new List<PlanetData>();

            int startEpoch = DateTimeToEpoch(startUtc);
            int endEpoch = DateTimeToEpoch(endUtc);
            int currentEpoch = startEpoch;

            int stepSeconds = 3600; // 1 hour step
            var prevData = CalculatePlanetData(planetId, EpochToDateTime(currentEpoch), nodeType);
            results.Add(prevData);

            while (currentEpoch < endEpoch)
            {
                currentEpoch += stepSeconds;
                var newData = CalculatePlanetData(planetId, EpochToDateTime(currentEpoch), nodeType);

                if (HasStateChanged(prevData, newData))
                {
                    int preciseEpoch = FindTransitionEpoch(planetId, prevData, newData, currentEpoch - stepSeconds, currentEpoch, nodeType);
                    var preciseData = CalculatePlanetData(planetId, EpochToDateTime(preciseEpoch), nodeType);

                    results.Add(preciseData);
                    prevData = preciseData;
                    currentEpoch = preciseEpoch;
                }
            }

            return results;
        }

        private static bool HasStateChanged(PlanetData a, PlanetData b)
        {
            return a.ZodiacId != b.ZodiacId
                || a.NakshatraId != b.NakshatraId
                || a.PadaId != b.PadaId
                || a.IsRetrograde != b.IsRetrograde;
        }

        public static DateTime FindPreviousZodiacChangeUtc(
            int planetId,
            DateTime utcT,
            EAppSetting nodeType)
        {
            var stateAtT = CalculatePlanetData(planetId, utcT, nodeType);

            var step = TimeSpan.FromDays(2);
            var maxLookback = TimeSpan.FromDays(365); // защитный лимит (можно 730)

            while (step <= maxLookback)
            {
                var from = utcT - step;
                var to = utcT;

                var list = CalculatePlanetDataList_London(planetId, from, to, nodeType);

                // гарантируем, что конец соответствует utcT
                var endData = CalculatePlanetData(planetId, utcT, nodeType);
                if (list.Count == 0 || list[^1].DateTimeUtc != utcT)
                {
                    // если последний элемент не в utcT, добавим endData как опорную точку
                    // (если он равен последнему по состоянию - всё равно не мешает)
                    list.Add(endData);
                }

                // ищем границу: последнее "не такое", после которого начинается stateAtT
                for (int i = list.Count - 2; i >= 0; i--)
                {
                    if (list[i].ZodiacId != stateAtT.ZodiacId)
                    {
                        return list[i + 1].DateTimeUtc; // начало текущего состояния
                    }
                }

                // Если в диапазоне всё ещё одно и то же состояние — расширяемся
                step = TimeSpan.FromTicks(step.Ticks * 2);
            }

            // если не нашли — возвращаем "как есть" (в крайнем случае)
            return utcT - maxLookback;
        }

        public static DateTime FindNextZodiacChangeUtc(
            int planetId,
            DateTime utcT,
            EAppSetting nodeType)
        {
            var stateAtT = CalculatePlanetData(planetId, utcT, nodeType);

            var step = TimeSpan.FromDays(2);
            var maxLookforward = TimeSpan.FromDays(365);

            while (step <= maxLookforward)
            {
                var from = utcT;
                var to = utcT + step;

                var list = CalculatePlanetDataList_London(planetId, from, to, nodeType);

                // гарантируем старт
                if (list.Count == 0 || list[0].DateTimeUtc != utcT)
                {
                    list.Insert(0, stateAtT);
                }

                // ищем первое "не такое" после utcT
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].ZodiacId != stateAtT.ZodiacId)
                    {
                        return list[i].DateTimeUtc; // конец текущего состояния
                    }
                }

                step = TimeSpan.FromTicks(step.Ticks * 2);
            }

            return utcT + maxLookforward;
        }

        private static int FindTransitionEpoch(int planetId, PlanetData fromState, PlanetData toState, int startEpoch, int endEpoch, EAppSetting nodeType)
        {
            if (endEpoch - startEpoch <= 1)
                return endEpoch;

            int midEpoch = startEpoch + (endEpoch - startEpoch) / 2;
            var midData = CalculatePlanetData(planetId, EpochToDateTime(midEpoch), nodeType);

            if (HasStateChanged(fromState, midData))
                return FindTransitionEpoch(planetId, fromState, midData, startEpoch, midEpoch, nodeType);
            else
                return FindTransitionEpoch(planetId, midData, toState, midEpoch, endEpoch, nodeType);
        }

        private static PlanetData CalculatePlanetData(int planetId, DateTime utcDate, EAppSetting nodeType)
        {
            var position = SwissService.GetPlanetPosition(utcDate, planetId, nodeType);
            double lon = position[0];
            double lat = position[1];
            double dist = position[2];

            // speed calculation (difference in longitude per minute)
            var positionLater = SwissService.GetPlanetPosition(utcDate.AddMinutes(1), planetId, nodeType);
            double speedLon = positionLater[0] - lon;
            if (speedLon > 180) speedLon -= 360;
            if (speedLon < -180) speedLon += 360;

            // derive IDs
            int zodiacId = SwissUtility.GetZodiacIdFromDegree(lon);
            int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(lon);
            int padaId = SwissUtility.GetPadaIdFromDegree(lon);

            // calculate Navamsa from cached PADA list
            int padaNumber = SwissUtility.GetPadaNumberByPadaId(padaId);
            int navamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber);

            var data = new PlanetData
            {
                PlanetId = planetId,
                DateTimeUtc = utcDate,
                Longitude = lon,
                Latitude = lat,
                Distance = dist,
                SpeedInLongitude = speedLon,
                ZodiacId = zodiacId,
                NakshatraId = nakshatraId,
                PadaId = padaId,
                NavamsaZodiacId = navamsaZodiacId,
                IsRetrograde = speedLon < 0
            };

            return data;
        }

        private static int DateTimeToEpoch(DateTime date)
            => (int)(date - Epoch).TotalSeconds;

        private static DateTime EpochToDateTime(int epoch)
            => Epoch.AddSeconds(epoch);

        public static List<PlanetData> CalculatePlanetPositionsForDate(DateTime date, double latitude, double longitude, EAppSetting nodeType)
        {
            List<PlanetData> pdList = new List<PlanetData>();

            PlanetData moonData = CalculatePlanetData((int)EPlanet.MOON, date, nodeType);
            pdList.Add(moonData);
            PlanetData sunData = CalculatePlanetData((int)EPlanet.SUN, date, nodeType);
            pdList.Add(sunData);
            PlanetData mercuryData = CalculatePlanetData((int)EPlanet.MERCURY, date, nodeType);
            pdList.Add(mercuryData);
            PlanetData venusData = CalculatePlanetData((int)EPlanet.VENUS, date, nodeType);
            pdList.Add(venusData);
            PlanetData marsData = CalculatePlanetData((int)EPlanet.MARS, date, nodeType);
            pdList.Add(marsData);
            PlanetData jupiterData = CalculatePlanetData((int)EPlanet.JUPITER, date, nodeType);
            pdList.Add(jupiterData);
            PlanetData saturnData = CalculatePlanetData((int)EPlanet.SATURN, date, nodeType);
            pdList.Add(saturnData);
            PlanetData rahuData = CalculatePlanetData((int)EPlanet.RAHU, date, nodeType);
            pdList.Add(rahuData);
            PlanetData ketuData = CalculateKetuData(rahuData);
            pdList.Add(ketuData);

            return pdList;
        }

        public static PlanetData CalculateKetuData(PlanetData rahuData)
        {
            double kLongitude = SwissUtility.AdjustForKetu(rahuData.Longitude);
            int planetId = 0;
            if (rahuData.PlanetId == 8)
            {
                planetId = 9;
            }
            if (rahuData.PlanetId == 10)
            {
                planetId = 11;
            }
            int zodiakId = SwissUtility.GetZodiacIdFromDegree(kLongitude);
            int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(kLongitude);
            int padaId = SwissUtility.GetPadaIdFromDegree(kLongitude);

            // calculate Navamsa from cached PADA list
            int padaNumber = SwissUtility.GetPadaNumberByPadaId(padaId);
            int navamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber);

            var pdData = new PlanetData
            {
                PlanetId = planetId,
                DateTimeUtc = rahuData.DateTimeUtc,
                Longitude = kLongitude,
                Latitude = rahuData.Latitude,
                Distance = rahuData.Distance,
                SpeedInLongitude = rahuData.SpeedInLongitude,
                ZodiacId = zodiakId,
                NakshatraId = nakshatraId,
                PadaId = padaId,
                NavamsaZodiacId = navamsaZodiacId,
                IsRetrograde = rahuData.SpeedInLongitude < 0
            };
            return pdData;
        }

        /// <summary>
        /// Calculates all Tithi change times within a UTC range.
        /// Sidereal (Lahiri), geocentric, using London coordinates (GMT+0).
        /// </summary>
        public static List<TithiData> CalculateTithiDataList_London(DateTime fromUtc, DateTime toUtc, EAppSetting nodeType)
        {
            if (toUtc <= fromUtc)
                return new List<TithiData>();

            // Сидерический режим — Лахири
            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            var results = new List<TithiData>();

            double msDiff0 = GetMoonSunDiff(fromUtc, nodeType);
            int currentTithi = GetCurrentTithi(msDiff0);

            results.Add(new TithiData
            {
                DateTimeUtc = fromUtc,
                MoonSunDifference = msDiff0,
                TithiId = currentTithi
            });

            DateTime cursor = fromUtc;
            while (cursor < toUtc)
            {
                // шаг 1 час — грубый поиск смены титхи
                var (changed, rough) = ScanUntilTithiChange(cursor, toUtc, currentTithi, TimeSpan.FromHours(1), nodeType);
                if (!changed)
                    break;

                // уточнение бинарным поиском до секунды
                DateTime exact = RefineChangeTimeToSecond(rough - TimeSpan.FromHours(1), rough, currentTithi, nodeType);

                double diffAtExact = GetMoonSunDiff(exact, nodeType);
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
            DateTime start, DateTime stop, int currentTithi, TimeSpan step, EAppSetting nodeType)
        {
            var t = start;
            while (t <= stop)
            {
                var diff = GetMoonSunDiff(t, nodeType);
                var tithi = GetCurrentTithi(diff);
                if (tithi != currentTithi)
                    return (true, t);
                t = t.Add(step);
            }
            return (false, stop);
        }

        private static DateTime RefineChangeTimeToSecond(DateTime lo, DateTime hi, int oldTithi, EAppSetting nodeType)
        {
            while ((hi - lo).TotalSeconds > 1)
            {
                var mid = lo.AddSeconds((hi - lo).TotalSeconds / 2.0);
                var diff = GetMoonSunDiff(mid, nodeType);
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
        private static double GetMoonSunDiff(DateTime utc, EAppSetting nodeType)
        {
            // Важно: GetPlanetPosition ожидает внутренний PlanetId (не SWE-константы!)
            var sun = SwissService.GetPlanetPosition(utc, (int)EPlanet.SUN, nodeType); // Sun
            var moon = SwissService.GetPlanetPosition(utc, (int)EPlanet.MOON, nodeType); // Moon

            double diff = moon[0] - sun[0];
            diff %= 360.0;
            if (diff < 0) diff += 360.0;
            return diff;
        }

        /// <summary>
        /// 1..30 — each 12° of Moon-Sun angular separation.
        /// </summary>
        private static int GetCurrentTithi(double msDiff)
        {
            const double tithiPart = 360.0 / 30.0; // 12°
            double d = msDiff / tithiPart;
            int i = (int)d;
            return (d > i) ? i + 1 : i;
        }

        public static List<NityaYogaData> CalculateNityaYogaDataList_London(DateTime fromUtc, DateTime toUtc, EAppSetting nodeType)
        {
            if (toUtc <= fromUtc)
                return new List<NityaYogaData>();

            // включаем сидерический режим Lahiri
            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            var results = new List<NityaYogaData>();

            // начальные данные (на секунду раньше диапазона, чтобы корректно определить текущую йогу)
            double[] sun0 = SwissService.GetPlanetPosition(fromUtc.AddSeconds(-1), (int)EPlanet.SUN, nodeType);
            double[] moon0 = SwissService.GetPlanetPosition(fromUtc.AddSeconds(-1), (int)EPlanet.MOON, nodeType);

            double yogaLon = GetYogaLongitude(sun0[0], moon0[0]);
            int currentNakshatraId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);
            int currentYogaId = DataCache.Instance.NityaYogaList.FirstOrDefault(y => y.NakshatraId == currentNakshatraId)?.Id ?? 0;

            results.Add(new NityaYogaData
            {
                DateTimeUtc = fromUtc,
                Longitude = yogaLon,
                NityaYogaId = currentYogaId
            });

            DateTime cursor = fromUtc;
            while (cursor < toUtc)
            {
                // грубый поиск смены йоги шагом 1 час
                var (changed, rough) = ScanUntilYogaChange(cursor, toUtc, currentYogaId, TimeSpan.FromHours(1), nodeType);
                if (!changed)
                    break;

                // уточнение момента смены йоги до секунды
                DateTime exact = RefineYogaChangeToSecond(rough - TimeSpan.FromHours(1), rough, currentYogaId, nodeType);

                // вычисляем положение Солнца и Луны в момент изменения
                var sun = SwissService.GetPlanetPosition(exact, (int)EPlanet.SUN, nodeType);
                var moon = SwissService.GetPlanetPosition(exact, (int)EPlanet.MOON, nodeType);
                yogaLon = GetYogaLongitude(sun[0], moon[0]);
                currentNakshatraId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);
                currentYogaId = DataCache.Instance.NityaYogaList.FirstOrDefault(y => y.NakshatraId == currentNakshatraId)?.Id ?? 0;

                results.Add(new NityaYogaData
                {
                    DateTimeUtc = exact,
                    Longitude = yogaLon,
                    NityaYogaId = currentYogaId
                });

                cursor = exact.AddSeconds(1); // продолжаем после найденного перехода
            }

            return results;
        }

        // --- внутренние помощники ---

        private static (bool changed, DateTime when) ScanUntilYogaChange(DateTime start, DateTime stop, int currentYogaId, TimeSpan step, EAppSetting nodeType)
        {
            var t = start;
            while (t <= stop)
            {
                var sun = SwissService.GetPlanetPosition(t, (int)EPlanet.SUN, nodeType);
                var moon = SwissService.GetPlanetPosition(t, (int)EPlanet.MOON, nodeType);
                double yogaLon = GetYogaLongitude(sun[0], moon[0]);
                int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);
                int yogaId = DataCache.Instance.NityaYogaList.FirstOrDefault(y => y.NakshatraId == nakshatraId)?.Id ?? 0;
                if (yogaId != currentYogaId)
                    return (true, t);
                t = t.Add(step);
            }
            return (false, stop);
        }

        private static DateTime RefineYogaChangeToSecond(DateTime lo, DateTime hi, int oldYogaId, EAppSetting nodeType)
        {
            while ((hi - lo).TotalSeconds > 1)
            {
                var mid = lo.AddSeconds((hi - lo).TotalSeconds / 2.0);
                var sun = SwissService.GetPlanetPosition(mid, (int)EPlanet.SUN, nodeType);
                var moon = SwissService.GetPlanetPosition(mid, (int)EPlanet.MOON, nodeType);
                double yogaLon = GetYogaLongitude(sun[0], moon[0]);
                int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);
                int yogaId = DataCache.Instance.NityaYogaList.FirstOrDefault(y => y.NakshatraId == nakshatraId)?.Id ?? 0;

                if (yogaId != oldYogaId)
                    hi = mid;
                else
                    lo = mid;
            }
            return hi;
        }

        /// <summary>
        /// Вычисляет долготу для Нитья Йоги: (SunLon + MoonLon + 7 * 360/27) % 360
        /// с нормализацией через SwissService.NormalizeDegrees()
        /// </summary>
        private static double GetYogaLongitude(double sunLon, double moonLon)
        {
            const double NAK_PART = 360.0 / 27.0; // 13°20′
            double raw = sunLon + moonLon + 7 * NAK_PART;
            return SwissService.NormalizeDegrees(raw);
        }

        private static (bool inside, int zodId, double lon, bool retro, MrityuBhaga? mb, double fromDeg, double toDeg)
        GetMrityuBhagaState(
            int planetId,
            DateTime utc,
            EAppSetting nodeType,
            IReadOnlyList<MrityuBhaga> mbList,
            EAppSetting mbSettingMode,
            double tol)
        {
            var pos = GetMrityuBhagaPlanetPosition(planetId, utc, nodeType);
            double lon = pos.lon;
            bool retro = pos.retro;
            int zodId = SwissUtility.GetZodiacIdFromDegree(lon);

            var mb = mbList.FirstOrDefault(x => x.PlanetId == planetId && x.ZodiacId == zodId);
            if (mb == null)
                return (false, zodId, lon, retro, null, 0, 0);

            double fromDeg = mb.Degree, toDeg = mb.Degree;

            switch (mbSettingMode)
            {
                case EAppSetting.MRITYUBHAGANEQUAL: fromDeg = mb.Degree - tol; toDeg = mb.Degree + tol; break;
                case EAppSetting.MRITYUBHAGANLESS: fromDeg = mb.Degree - tol; toDeg = mb.Degree; break;
                case EAppSetting.MRITYUBHAGANMORE: fromDeg = mb.Degree; toDeg = mb.Degree + tol; break;
                case EAppSetting.MRITYUBHAGAERNST: fromDeg = mb.Degree - tol; toDeg = mb.Degree + tol; break;
            }

            fromDeg = SwissService.NormalizeDegrees(fromDeg);
            toDeg = SwissService.NormalizeDegrees(toDeg);

            bool inside = IsWithinDegrees(lon, fromDeg, toDeg);
            return (inside, zodId, lon, retro, mb, fromDeg, toDeg);
        }

        private static (double lon, bool retro) GetMrityuBhagaPlanetPosition(
            int planetId,
            DateTime utc,
            EAppSetting nodeType)
        {
            if (planetId == (int)EPlanet.KETU)
            {
                var rahu = SwissService.GetPlanetPosition(
                    utc,
                    (int)EPlanet.RAHU,
                    nodeType);

                var ketuLon = SwissService.NormalizeDegrees(rahu[0] + 180.0);

                // Ketu is derived from Rahu, so use Rahu speed direction as the node motion marker.
                // For Mrityu Bhaga zone detection the longitude is the important value.
                bool retro = rahu[3] < 0;

                return (ketuLon, retro);
            }

            var planet = SwissService.GetPlanetPosition(utc, planetId, nodeType);
            var lon = SwissService.NormalizeDegrees(planet[0]);
            var isRetro = planet[3] < 0;

            return (lon, isRetro);
        }

        private static DateTime RefineBoundaryBinary(
            int planetId,
            DateTime t0,
            DateTime t1,
            bool targetInsideAtT1,
            EAppSetting nodeType,
            IReadOnlyList<MrityuBhaga> mbList,
            EAppSetting mbSettingMode,
            double tol,
            int maxIter = 30,
            TimeSpan? minResolution = null)
        {
            var minRes = minResolution ?? TimeSpan.FromSeconds(10);

            DateTime lo = t0;
            DateTime hi = t1;

            for (int i = 0; i < maxIter && (hi - lo) > minRes; i++)
            {
                var mid = lo + TimeSpan.FromTicks((hi - lo).Ticks / 2);
                var st = GetMrityuBhagaState(planetId, mid, nodeType, mbList, mbSettingMode, tol);

                if (st.inside == targetInsideAtT1)
                    hi = mid;
                else
                    lo = mid;
            }

            return hi;
        }

        private static DateTime ExpandBackwardToExit(
            int planetId,
            DateTime anchorUtc,
            EAppSetting nodeType,
            IReadOnlyList<MrityuBhaga> mbList,
            EAppSetting mbSettingMode,
            double tol,
            TimeSpan maxLookback)
        {
            var step = TimeSpan.FromMinutes(15);
            var cur = anchorUtc;

            // предполагаем: на anchorUtc inside==true
            while (anchorUtc - cur < maxLookback)
            {
                var prev = cur - step;
                var stPrev = GetMrityuBhagaState(planetId, prev, nodeType, mbList, mbSettingMode, tol);

                if (!stPrev.inside)
                {
                    // граница между prev (outside) и cur (inside)
                    return RefineBoundaryBinary(planetId, prev, cur, targetInsideAtT1: true,
                        nodeType, mbList, mbSettingMode, tol);
                }

                cur = prev;
                step = step < TimeSpan.FromHours(6) ? TimeSpan.FromTicks(step.Ticks * 2) : step; // ускоряемся
            }

            return anchorUtc - maxLookback; // fallback
        }

        private static DateTime ExpandForwardToExit(
            int planetId,
            DateTime anchorUtc,
            EAppSetting nodeType,
            IReadOnlyList<MrityuBhaga> mbList,
            EAppSetting mbSettingMode,
            double tol,
            TimeSpan maxLookforward)
        {
            var step = TimeSpan.FromMinutes(15);
            var cur = anchorUtc;

            // предполагаем: на anchorUtc inside==true
            while (cur - anchorUtc < maxLookforward)
            {
                var next = cur + step;
                var stNext = GetMrityuBhagaState(planetId, next, nodeType, mbList, mbSettingMode, tol);

                if (!stNext.inside)
                {
                    // граница между cur (inside) и next (outside)
                    return RefineBoundaryBinary(planetId, cur, next, targetInsideAtT1: false,
                        nodeType, mbList, mbSettingMode, tol);
                }

                cur = next;
                step = step < TimeSpan.FromHours(6) ? TimeSpan.FromTicks(step.Ticks * 2) : step;
            }

            return anchorUtc + maxLookforward; // fallback
        }

        public static List<MrityuBhagaData> CalculateMrityuBhagaDataList_London(
            int planetId, DateTime fromUtc, DateTime toUtc, EAppSetting nodeType)
        {
            var results = new List<MrityuBhagaData>();
            if (toUtc <= fromUtc) return results;

            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            var mbList = DataCache.Instance.MrityuBhagaList;
            var mbSettingMode = DataCache.Instance.GetActiveMrityuBhagaSettings();

            double tol = mbSettingMode switch
            {
                EAppSetting.MRITYUBHAGANEQUAL => 0.5,
                EAppSetting.MRITYUBHAGANLESS => 1.0,
                EAppSetting.MRITYUBHAGANMORE => 1.0,
                EAppSetting.MRITYUBHAGAERNST => 1.0,
                _ => 0.5
            };

            // --- NEW: boundary expansion limits ---
            var maxLookback = TimeSpan.FromDays(90);
            var maxLookforward = TimeSpan.FromDays(90);

            DateTime cur = fromUtc;
            DateTime prevCur = fromUtc;

            bool inZone = false;
            MrityuBhagaData? current = null;

            double lastLon = 0;
            bool lastRetro = false;
            bool hasLast = false;

            // --- NEW: check if we start inside, expand backward ---
            var stStart = GetMrityuBhagaState(planetId, fromUtc, nodeType, mbList, mbSettingMode, tol);
            if (stStart.mb != null && stStart.inside)
            {
                var realStart = ExpandBackwardToExit(
                    planetId, fromUtc, nodeType, mbList, mbSettingMode, tol, maxLookback);

                inZone = true;
                current = new MrityuBhagaData
                {
                    PlanetId = planetId,
                    ZodiacId = stStart.zodId,
                    Degree = stStart.mb.Degree,
                    MrityuBhagaSetting = mbSettingMode,
                    LongitudeFrom = stStart.fromDeg,
                    LongitudeTo = stStart.toDeg,
                    DateFromUtc = realStart
                };
            }

            while (cur <= toUtc)
            {
                var st = GetMrityuBhagaState(planetId, cur, nodeType, mbList, mbSettingMode, tol);
                if (st.mb == null)
                {
                    prevCur = cur;
                    cur = cur.AddMinutes(15);
                    continue;
                }

                bool inside = st.inside;
                int zodId = st.zodId;
                bool retro = st.retro;

                // --- zodiac/retro change ---
                // если состояние сменилось по знаку или ретроградности, закрываем предыдущий интервал
                // с уточнением границы между prevCur и cur
                if (hasLast)
                {
                    bool changedContext = SwissUtility.GetZodiacIdFromDegree(lastLon) != zodId || retro != lastRetro;
                    if (changedContext)
                    {
                        if (inZone && current != null)
                        {
                            DateTime exactEnd = cur;

                            // если на предыдущей точке были inside, а на текущей уже outside/другой контекст,
                            // уточняем границу бинарно
                            var stPrev = GetMrityuBhagaState(planetId, prevCur, nodeType, mbList, mbSettingMode, tol);
                            if (stPrev.mb != null && stPrev.inside && prevCur < cur)
                            {
                                exactEnd = RefineBoundaryBinary(
                                    planetId,
                                    prevCur,
                                    cur,
                                    targetInsideAtT1: false,
                                    nodeType,
                                    mbList,
                                    mbSettingMode,
                                    tol);
                            }

                            current.DateToUtc = exactEnd;
                            results.Add(current);
                        }

                        inZone = false;
                        current = null;
                    }
                }

                // --- entry ---
                if (inside && !inZone)
                {
                    DateTime exactStart = cur;

                    // если стартовали не с fromUtc-inside case, а вошли внутрь в цикле,
                    // уточняем границу между prevCur (outside) и cur (inside)
                    if (prevCur < cur)
                    {
                        var stPrev = GetMrityuBhagaState(planetId, prevCur, nodeType, mbList, mbSettingMode, tol);
                        if (stPrev.mb != null && !stPrev.inside)
                        {
                            exactStart = RefineBoundaryBinary(
                                planetId,
                                prevCur,
                                cur,
                                targetInsideAtT1: true,
                                nodeType,
                                mbList,
                                mbSettingMode,
                                tol);
                        }
                    }

                    inZone = true;
                    current = new MrityuBhagaData
                    {
                        PlanetId = planetId,
                        ZodiacId = zodId,
                        Degree = st.mb.Degree,
                        MrityuBhagaSetting = mbSettingMode,
                        LongitudeFrom = st.fromDeg,
                        LongitudeTo = st.toDeg,
                        DateFromUtc = exactStart
                    };
                }

                // --- exit ---
                if (!inside && inZone && current != null)
                {
                    DateTime exactEnd = cur;

                    if (prevCur < cur)
                    {
                        var stPrev = GetMrityuBhagaState(planetId, prevCur, nodeType, mbList, mbSettingMode, tol);
                        if (stPrev.mb != null && stPrev.inside)
                        {
                            exactEnd = RefineBoundaryBinary(
                                planetId,
                                prevCur,
                                cur,
                                targetInsideAtT1: false,
                                nodeType,
                                mbList,
                                mbSettingMode,
                                tol);
                        }
                    }

                    current.DateToUtc = exactEnd;
                    results.Add(current);
                    inZone = false;
                    current = null;
                }

                lastLon = st.lon;
                lastRetro = retro;
                hasLast = true;

                prevCur = cur;
                cur = inZone ? cur.AddMinutes(1) : cur.AddMinutes(15);
            }

            // --- NEW: close open interval by expanding forward (real end) ---
            if (inZone && current != null)
            {
                var realEnd = ExpandForwardToExit(
                    planetId, toUtc, nodeType, mbList, mbSettingMode, tol, maxLookforward);

                current.DateToUtc = realEnd;
                results.Add(current);
            }

            return results;
        }

        /*
        public static List<MrityuBhagaData> CalculateMrityuBhagaDataList_London(int planetId, DateTime fromUtc, DateTime toUtc, EAppSetting nodeType)
        {
            var results = new List<MrityuBhagaData>();
            if (toUtc <= fromUtc) return results;

            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            // отримуємо таблицю градусів
            var mbList = DataCache.Instance.MrityuBhagaList;

            // активна конфігурація
            var mbSettingMode = DataCache.Instance.GetActiveMrityuBhagaSettings();
            double tol = mbSettingMode switch
            {
                EAppSetting.MRITYUBHAGANEQUAL => 0.5,
                EAppSetting.MRITYUBHAGANLESS => 1.0,
                EAppSetting.MRITYUBHAGANMORE => 1.0,
                EAppSetting.MRITYUBHAGAERNST => 1.0,
                _ => 0.5
            };
            

            DateTime cur = fromUtc;
            bool inZone = false;
            MrityuBhagaData? current = null;

            double lastLon = 0;
            bool lastRetro = false;

            while (cur <= toUtc)
            {
                // позиція планети
                var planet = SwissService.GetPlanetPosition(cur, planetId, nodeType);
                double lon = SwissService.NormalizeDegrees(planet[0]);
                bool retro = planet[3] < 0;
                int zodId = SwissUtility.GetZodiacIdFromDegree(lon);

                // шукаємо градус для цієї планети в цьому знаку
                var mb = mbList.FirstOrDefault(x => x.PlanetId == planetId && x.ZodiacId == zodId);
                if (mb == null) { cur = cur.AddHours(6); continue; }

                // формуємо діапазон згідно з налаштуванням
                double fromDeg = mb.Degree, toDeg = mb.Degree;
                switch (mbSettingMode)
                {
                    case EAppSetting.MRITYUBHAGANEQUAL: fromDeg = mb.Degree - tol; toDeg = mb.Degree + tol; break;
                    case EAppSetting.MRITYUBHAGANLESS: fromDeg = mb.Degree - tol; toDeg = mb.Degree; break;
                    case EAppSetting.MRITYUBHAGANMORE: fromDeg = mb.Degree; toDeg = mb.Degree + tol; break;
                    case EAppSetting.MRITYUBHAGAERNST: fromDeg = mb.Degree - tol; toDeg = mb.Degree + tol; break;
                }

                fromDeg = SwissService.NormalizeDegrees(fromDeg);
                toDeg = SwissService.NormalizeDegrees(toDeg);

                // перевірка потрапляння у зону (враховує 0° переходи)
                bool inside = IsWithinDegrees(lon, fromDeg, toDeg);

                // --- вхід ---
                if (inside && !inZone)
                {
                    inZone = true;
                    current = new MrityuBhagaData
                    {
                        PlanetId = (int)planetId,
                        ZodiacId = zodId,
                        Degree = mb.Degree,
                        MrityuBhagaSetting = mbSettingMode,
                        LongitudeFrom = fromDeg,
                        LongitudeTo = toDeg,
                        DateFromUtc = cur
                    };
                }

                // --- вихід ---
                if (!inside && inZone && current != null)
                {
                    current.DateToUtc = cur;
                    results.Add(current);
                    inZone = false;
                    current = null;
                }

                // --- зміна знака або ретроградності ---
                if (SwissUtility.GetZodiacIdFromDegree(lastLon) != zodId || retro != lastRetro)
                {
                    if (inZone && current != null)
                    {
                        current.DateToUtc = cur;
                        results.Add(current);
                    }
                    inZone = false;
                    current = null;
                }

                lastLon = lon;
                lastRetro = retro;

                // адаптивний крок
                cur = inZone ? cur.AddMinutes(1) : cur.AddHours(1);
            }

            // закриваємо відкритий інтервал
            if (inZone && current != null)
            {
                current.DateToUtc = toUtc;
                results.Add(current);
            }

            return results;
        }
        */

        /// <summary>
        /// Перевіряє, чи знаходиться поточна довгота у діапазоні з урахуванням переходу через 0°.
        /// </summary>
        private static bool IsWithinDegrees(double lon, double from, double to)
        {
            if (from <= to)
                return lon >= from && lon <= to;
            else // через 0°
                return lon >= from || lon <= to;
        }

        // --- Фильтры типов ---
        // CENTRAL=1, NONCENTRAL=2, TOTAL=4, ANNULAR=8, PARTIAL=16, ANNULAR_TOTAL(HYBRID)=32, PENUMBRAL=64
        static bool IsLunarAllowed(int rc, bool includePenumbral)
        {
            bool totalOrPartial = (rc & (SweConst.SE_ECL_TOTAL | SweConst.SE_ECL_PARTIAL)) != 0;
            if (totalOrPartial) return true;
            return includePenumbral && ((rc & SweConst.SE_ECL_PENUMBRAL) != 0);
        }
        
        // --- Слияние близких лунных событий: оставляем более позднее ---
        // -- пока так - в 2027 нужно было более позднее (после солнечного затмения). Под вопросом - может нужны все --
        // -- но есть варианты когда лунное на пару недель раньше солнечного - хотя с этим пока по таким рассчетам проблем не было --
        static List<EclipseData> MergeCloseLunarByMagnitude(List<EclipseData> lunar, int windowDays = 32)
        {
            if (lunar == null || lunar.Count <= 1)
                return lunar ?? new List<EclipseData>();

            // Сортируем по дате
            var ordered = lunar.OrderBy(e => e.Date).ToList();
            var kept = new List<EclipseData>();

            foreach (var e in ordered)
            {
                // проверяем, есть ли в списке событие в пределах ±windowDays
                var conflictIndex = kept.FindIndex(k =>
                    k.EclipseId == (int)EEclipse.MOONECLIPSE &&
                    Math.Abs((k.Date - e.Date).TotalDays) < windowDays);

                if (conflictIndex < 0)
                {
                    kept.Add(e);
                }
                else
                {
                    // если нашли конфликт — просто заменяем на более позднее событие
                    if (e.Date > kept[conflictIndex].Date)
                    {
                        kept[conflictIndex] = e;
                    }
                }
            }

            return kept;
        }

        // === ОСНОВНАЯ ФУНКЦИЯ для рассчета пар затмений (лунное + солнечное) ===
        public static List<EclipseData> CalculateEclipses_London(DateTime fromUtc, DateTime toUtc)
        {
            var result = new List<EclipseData>();
            if (toUtc <= fromUtc) return result;

            var sb = new StringBuilder(256);
            var tret = new double[10];

            // --- ЛУННЫЕ (включаем penumbral, чтобы в "пустые" годы были всё равно 2 события) ---
            var lunarRaw = new List<EclipseData>();
            double jdLun = SwissService.ToJulianDay(fromUtc);
            while (true)
            {
                Array.Clear(tret, 0, tret.Length);
                sb.Clear();

                int rc = SwissEphemerisNative.swe_lun_eclipse_when(
                    jdLun,
                    SweConst.SEFLG_SWIEPH,
                    // просим "все" типы, фильтруем ниже
                    SweConst.SE_ECL_TOTAL | SweConst.SE_ECL_PARTIAL | SweConst.SE_ECL_PENUMBRAL,
                    tret,
                    0, // forward
                    sb);

                if (rc <= 0 || tret[0] <= 0) break;

                var dt = SwissService.FromJulianDay(tret[0]);
                if (dt > toUtc) break;

                // ВКЛЮЧАЕМ penumbral (чтобы гарантировать 2 события в годы без partial/total)
                if (IsLunarAllowed(rc, includePenumbral: true))
                    lunarRaw.Add(new EclipseData { Date = dt, EclipseId = (int)EEclipse.MOONECLIPSE });

                // Шаг вперёд на день, чтобы не поймать повтор
                jdLun = SwissService.ToJulianDay(dt.AddDays(1));
            }

            // Сжимаем близкие лунные события (оставляем нужное в окне ~32 дней)
            // Наверное нужны все - в 2027 - 3 лунных и 2 солнечных, в 2029 - 3 солнечных и 2 лунных
            //var lunarMerged = MergeCloseLunarByMagnitude(lunarRaw);

            // --- СОЛНЕЧНЫЕ (исходная рабочая версия) ---
            double jdSol1 = SwissService.ToJulianDay(fromUtc);
            while (true)
            {
                Array.Clear(tret, 0, tret.Length);
                sb.Clear();

                int rc = SwissEphemerisNative.swe_sol_eclipse_when_glob(
                    jdSol1,
                    SweConst.SEFLG_SWIEPH,
                    SweConst.SE_ECL_ALLTYPES_SOLAR,
                    tret,
                    0,
                    sb);

                if (rc < 0 || tret[0] <= 0)
                    break;

                var dt = SwissService.FromJulianDay(tret[0]);
                if (dt > toUtc)
                    break;

                if ((rc & SweConst.SE_ECL_ALLTYPES_SOLAR) != 0)
                    result.Add(new EclipseData
                    {
                        EclipseId = (int)EEclipse.SUNECLIPSE,
                        Date = dt
                    });

                // шаг вперёд ~ 32 дня, чтобы не поймать повтор
                jdSol1 = SwissService.ToJulianDay(dt.AddDays(32));
            }

            //result.AddRange(lunarMerged); // -- пока включаем все --
            result.AddRange(lunarRaw);

            // --- Объединяем, фильтруем по диапазону, убираем редкие дубли по дню ---
            result = result
                .Where(e => e.Date >= fromUtc && e.Date <= toUtc)
                .OrderBy(e => e.Date)
                .GroupBy(e => new { e.EclipseId, Day = e.Date.Date })
                .Select(g => g.First())
                .ToList();

            return result;
        }

        public static List<LagnaData> CalculateLagnaDataList(
            DateTime startUtc,
            DateTime endUtc,
            double latitude,
            double longitude,
            double altitude,
            char hsys = 'O')
        {
            var results = new List<LagnaData>();

            int startEpoch = DateTimeToEpoch(startUtc);
            int endEpoch = DateTimeToEpoch(endUtc);
            int currentEpoch = startEpoch;

            int stepSeconds = 120; // лагна меняет паду часто, 2 минуты безопасно
            var prevData = CalculateLagnaData(EpochToDateTime(currentEpoch), latitude, longitude, altitude, hsys);
            results.Add(prevData);

            while (currentEpoch < endEpoch)
            {
                currentEpoch += stepSeconds;
                var newData = CalculateLagnaData(EpochToDateTime(currentEpoch), latitude, longitude, altitude, hsys);

                if (HasLagnaStateChanged(prevData, newData))
                {
                    int preciseEpoch = FindLagnaTransitionEpoch(
                        prevData, newData,
                        currentEpoch - stepSeconds, currentEpoch,
                        latitude, longitude, altitude, hsys);

                    var preciseData = CalculateLagnaData(EpochToDateTime(preciseEpoch), latitude, longitude, altitude, hsys);

                    results.Add(preciseData);
                    prevData = preciseData;
                    currentEpoch = preciseEpoch;
                }
            }

            return results;
        }

        private static bool HasLagnaStateChanged(LagnaData a, LagnaData b)
        {
            return a.ZodiacId != b.ZodiacId
                || a.NakshatraId != b.NakshatraId
                || a.PadaId != b.PadaId;
        }

        private static int FindLagnaTransitionEpoch(
            LagnaData fromState,
            LagnaData toState,
            int startEpoch,
            int endEpoch,
            double latitude,
            double longitude,
            double altitude,
            char hsys)
        {
            if (endEpoch - startEpoch <= 1)
                return endEpoch;

            int midEpoch = startEpoch + (endEpoch - startEpoch) / 2;
            var midData = CalculateLagnaData(EpochToDateTime(midEpoch), latitude, longitude, altitude, hsys);

            if (HasLagnaStateChanged(fromState, midData))
                return FindLagnaTransitionEpoch(fromState, midData, startEpoch, midEpoch, latitude, longitude, altitude, hsys);
            else
                return FindLagnaTransitionEpoch(midData, toState, midEpoch, endEpoch, latitude, longitude, altitude, hsys);
        }

        private static LagnaData CalculateLagnaData(
            DateTime utcDate,
            double latitude,
            double longitude,
            double altitude,
            char hsys)
        {
            double lon = SwissService.CalculateAscendantForDate(utcDate, latitude, longitude, altitude, hsys);

            int zodiacId = SwissUtility.GetZodiacIdFromDegree(lon);
            int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(lon);
            int padaId = SwissUtility.GetPadaIdFromDegree(lon);

            int padaNumber = SwissUtility.GetPadaNumberByPadaId(padaId);
            int navamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber);

            return new LagnaData
            {
                DateTimeUtc = utcDate,
                Longitude = lon,
                ZodiacId = zodiacId,
                NakshatraId = nakshatraId,
                PadaId = padaId,
                NavamsaZodiacId = navamsaZodiacId
            };
        }




    }
}
