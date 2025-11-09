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
            return a.ZodiacId != b.ZodiacId
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
            int zodiacId = SwissUtility.GetZodiakIdFromDegree(lon);
            int nakshatraId = SwissUtility.GetNakshatraIdFromDegree(lon);
            int padaId = SwissUtility.GetPadaIdFromDegree(lon);

            // calculate Navamsa from cached PADA list
            int padaNumber = SwissUtility.GetPadaNumberByPadaId(padaId);
            int navamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber);

            var data = new PlanetData
            {
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


        /// <summary>
        /// Calculates all Tithi change times within a UTC range.
        /// Sidereal (Lahiri), geocentric, using London coordinates (GMT+0).
        /// </summary>
        public static List<TithiData> CalculateTithiDataList_London(DateTime fromUtc, DateTime toUtc)
        {
            if (toUtc <= fromUtc)
                return new List<TithiData>();

            // Сидерический режим — Лахири
            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            var results = new List<TithiData>();

            double msDiff0 = GetMoonSunDiff(fromUtc);
            int currentTithi = GetCurrentTithi(msDiff0);

            DateTime cursor = fromUtc;

            while (cursor < toUtc)
            {
                // шаг 1 час — грубый поиск смены титхи
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
            // Важно: GetPlanetPosition ожидает внутренний PlanetId (не SWE-константы!)
            var sun = SwissService.GetPlanetPosition(utc, (int)EPlanet.SUN); // Sun
            var moon = SwissService.GetPlanetPosition(utc, (int)EPlanet.MOON); // Moon

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

        public static List<NityaYogaData> CalculateNityaYogaDataList_London(DateTime fromUtc, DateTime toUtc)
        {
            if (toUtc <= fromUtc)
                return new List<NityaYogaData>();

            // включаем сидерический режим Lahiri
            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            var results = new List<NityaYogaData>();

            // начальные данные (на секунду раньше диапазона, чтобы корректно определить текущую йогу)
            double[] sun0 = SwissService.GetPlanetPosition(fromUtc.AddSeconds(-1), (int)EPlanet.SUN);
            double[] moon0 = SwissService.GetPlanetPosition(fromUtc.AddSeconds(-1), (int)EPlanet.MOON);

            double yogaLon = GetYogaLongitude(sun0[0], moon0[0]);
            int currentYogaId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);

            DateTime cursor = fromUtc;
            while (cursor < toUtc)
            {
                // грубый поиск смены йоги шагом 1 час
                var (changed, rough) = ScanUntilYogaChange(cursor, toUtc, currentYogaId, TimeSpan.FromHours(1));
                if (!changed)
                    break;

                // уточнение момента смены йоги до секунды
                DateTime exact = RefineYogaChangeToSecond(rough - TimeSpan.FromHours(1), rough, currentYogaId);

                // вычисляем положение Солнца и Луны в момент изменения
                var sun = SwissService.GetPlanetPosition(exact, (int)EPlanet.SUN);
                var moon = SwissService.GetPlanetPosition(exact, (int)EPlanet.MOON);
                yogaLon = GetYogaLongitude(sun[0], moon[0]);
                currentYogaId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);

                results.Add(new NityaYogaData
                {
                    DateTimeUtc = exact,
                    Longitude = yogaLon,
                    YogaId = currentYogaId
                });

                cursor = exact.AddSeconds(1); // продолжаем после найденного перехода
            }

            return results;
        }

        // --- внутренние помощники ---

        private static (bool changed, DateTime when) ScanUntilYogaChange(
            DateTime start, DateTime stop, int currentYogaId, TimeSpan step)
        {
            var t = start;
            while (t <= stop)
            {
                var sun = SwissService.GetPlanetPosition(t, (int)EPlanet.SUN);
                var moon = SwissService.GetPlanetPosition(t, (int)EPlanet.MOON);
                double yogaLon = GetYogaLongitude(sun[0], moon[0]);
                int yogaId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);
                if (yogaId != currentYogaId)
                    return (true, t);
                t = t.Add(step);
            }
            return (false, stop);
        }

        private static DateTime RefineYogaChangeToSecond(
            DateTime lo, DateTime hi, int oldYogaId)
        {
            while ((hi - lo).TotalSeconds > 1)
            {
                var mid = lo.AddSeconds((hi - lo).TotalSeconds / 2.0);
                var sun = SwissService.GetPlanetPosition(mid, (int)EPlanet.SUN);
                var moon = SwissService.GetPlanetPosition(mid, (int)EPlanet.MOON);
                double yogaLon = GetYogaLongitude(sun[0], moon[0]);
                int yogaId = SwissUtility.GetNakshatraIdFromDegree(yogaLon);

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

        public static List<MrityuBhagaData> CalculateMrityuBhagaDataList_London(int planetId, DateTime fromUtc, DateTime toUtc)
        {
            var results = new List<MrityuBhagaData>();
            if (toUtc <= fromUtc) return results;

            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            // отримуємо таблицю градусів
            var mbList = DataCache.Instance.MrityuBhagaList;

            // активна конфігурація
            var setting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "MRITYUBHAGA" && s.Active == 1);

            var mode = setting?.SettingCode ?? "NEQUAL";
            double tol = mode switch
            {
                "NEQUAL" => 0.5,
                "NLESS" => 1.0,
                "NMORE" => 1.0,
                "NERNST" => 1.0,
                _ => 0.5
            };
            var appSettingEnum = (EAppSetting)(setting?.Id ?? (int)EAppSetting.MRITYUBHAGANEQUAL);

            DateTime cur = fromUtc;
            bool inZone = false;
            MrityuBhagaData? current = null;

            double lastLon = 0;
            bool lastRetro = false;

            while (cur <= toUtc)
            {
                // позиція планети
                var planet = SwissService.GetPlanetPosition(cur, planetId);
                double lon = SwissService.NormalizeDegrees(planet[0]);
                bool retro = planet[3] < 0;
                int zodId = SwissUtility.GetZodiakIdFromDegree(lon);

                // шукаємо градус для цієї планети в цьому знаку
                var mb = mbList.FirstOrDefault(x => x.PlanetId == planetId && x.ZodiakId == zodId);
                if (mb == null) { cur = cur.AddHours(6); continue; }

                // формуємо діапазон згідно з налаштуванням
                double fromDeg = mb.Degree, toDeg = mb.Degree;
                switch (mode)
                {
                    case "NEQUAL": fromDeg = mb.Degree - tol; toDeg = mb.Degree + tol; break;
                    case "NLESS": fromDeg = mb.Degree - tol; toDeg = mb.Degree; break;
                    case "NMORE": fromDeg = mb.Degree; toDeg = mb.Degree + tol; break;
                    case "NERNST": fromDeg = mb.Degree - tol; toDeg = mb.Degree + tol; break;
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
                        ZodiakId = zodId,
                        Degree = mb.Degree,
                        MrityuBhagaSetting = appSettingEnum,
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
                if (SwissUtility.GetZodiakIdFromDegree(lastLon) != zodId || retro != lastRetro)
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
        

        

    }
}
