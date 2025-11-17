using System;
using System.Collections.Generic;
using System.Linq;
using PADMA.Core.Enums;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Utilities;

namespace PADMA.Core.TransitBuilder
{
    public static class YogaTransitBuilder
    {
        public static List<YogaSlice> BuildYogaSlices(
            EYoga yogaCode,
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            DayOfWeek vara,
            List<TithiSlice> tithiSlices,
            List<NakshatraSlice> nakshatraSlices)
        {
            if (!YogaRules.Rules.TryGetValue(yogaCode, out var rule))
                return new List<YogaSlice>();

            // ---------------------------------------------------------
            // СЛОЖНЫЕ ЙОГИ: идут в отдельные методы
            // ---------------------------------------------------------
            if (yogaCode == EYoga.AMRITASIDDHA)
                return BuildAmritasiddha(periodStartUtc, periodEndUtc, vara, tithiSlices, nakshatraSlices);

            if (yogaCode == EYoga.SARVARTHA)
                return BuildSarvartha(periodStartUtc, periodEndUtc, vara, tithiSlices, nakshatraSlices);

            if (yogaCode == EYoga.SIDDHA)
            {
                // Basic Siddha (by tithi only)
                var basic = BuildSiddha(periodStartUtc, periodEndUtc, vara, tithiSlices);

                // Large Siddha (tithi + nakshatra)
                var large = BuildLargeSiddha(
                    periodStartUtc,
                    periodEndUtc,
                    vara,
                    nakshatraSlices,
                    tithiSlices);

                // Merge, sort
                return basic
                    .Concat(large)
                    .OrderBy(s => s.StartUtc)
                    .ToList();
            }

            // ---------------------------------------------------------
            // ПРОСТЫЕ ЙОГИ (Dwipushkar, Tripushkar)
            // ---------------------------------------------------------
            var result = new List<YogaSlice>();

            if (!rule.Vara.Contains(vara))
                return result;

            // Tithi фильтр
            var tithiFiltered = rule.TithiIds != null
                ? tithiSlices.Where(t => rule.TithiIds.Contains(t.TithiId)).ToList()
                : tithiSlices;

            // Nakshatra фильтр
            var nakFiltered = rule.Nakshatra != null
                ? nakshatraSlices.Where(n => rule.Nakshatra.Contains((ENakshatra)n.NakshatraId)).ToList()
                : nakshatraSlices;

            foreach (var t in tithiFiltered)
            {
                foreach (var n in nakFiltered)
                {
                    var start = t.StartUtc > n.StartUtc ? t.StartUtc : n.StartUtc;
                    var end = t.EndUtc < n.EndUtc ? t.EndUtc : n.EndUtc;

                    if (start < end)
                    {
                        result.Add(new YogaSlice
                        {
                            YogaId = (int)yogaCode,
                            YogaCode = yogaCode,
                            StartUtc = start,
                            EndUtc = end,
                            Vara = vara,
                            TithiId = t.TithiId,
                            NakshatraCode = (ENakshatra)n.NakshatraId,
                            ColorId = YogaSlice.GetYogaColorId((int)yogaCode)
                        });

                        if (!rule.MultiResultAllowed)
                            return result;
                    }
                }
            }

            return result;
        }

        // ======================================================================
        //                AMRITASIDDHA — СЛОЖНАЯ ЙОГА
        // ======================================================================

        private static List<YogaSlice> BuildAmritasiddha(
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            DayOfWeek vara,
            List<TithiSlice> tithiSlices,
            List<NakshatraSlice> nakshatraSlices)
        {
            var result = new List<YogaSlice>();

            // -----------------------------------------------------------------
            // 1. Списки условий по дням недели
            // -----------------------------------------------------------------

            var allowedNakshatra = vara switch
            {
                DayOfWeek.Monday => new[] { ENakshatra.MRIGASHIRA },
                DayOfWeek.Tuesday => new[] { ENakshatra.ASHWINI },
                DayOfWeek.Wednesday => new[] { ENakshatra.ANURADHA },
                DayOfWeek.Thursday => new[] { ENakshatra.PUSHYA },
                DayOfWeek.Friday => new[] { ENakshatra.REVATI },
                DayOfWeek.Saturday => new[] { ENakshatra.ROHINI },
                DayOfWeek.Sunday => new[] { ENakshatra.HASTA },
                _ => Array.Empty<ENakshatra>()
            };

            var forbiddenTithi = vara switch
            {
                DayOfWeek.Monday => new[] { 6, 21 },
                DayOfWeek.Tuesday => new[] { 7, 22 },
                DayOfWeek.Wednesday => new[] { 8, 23 },
                DayOfWeek.Thursday => new[] { 9, 24 },
                DayOfWeek.Friday => new[] { 10, 25 },
                DayOfWeek.Saturday => new[] { 11, 26 },
                DayOfWeek.Sunday => new[] { 5, 20 },
                _ => Array.Empty<int>()
            };

            // -----------------------------------------------------------------
            // 2. Фильтрация титхи
            // -----------------------------------------------------------------

            var tithiFiltered =
                tithiSlices
                .Where(t => !forbiddenTithi.Contains(t.TithiId))
                .ToList();

            // -----------------------------------------------------------------
            // 3. Фильтрация накшатр
            // -----------------------------------------------------------------

            var nakFiltered =
                nakshatraSlices
                .Where(n => allowedNakshatra.Contains((ENakshatra)n.NakshatraId))
                .ToList();

            // -----------------------------------------------------------------
            // 4. Пересечения
            // -----------------------------------------------------------------
            foreach (var t in tithiFiltered)
            {
                foreach (var n in nakFiltered)
                {
                    bool overlap = t.StartUtc < n.EndUtc && n.StartUtc < t.EndUtc;
                    if (!overlap)
                        continue;

                    var start = t.StartUtc > n.StartUtc ? t.StartUtc : n.StartUtc;
                    var end = t.EndUtc < n.EndUtc ? t.EndUtc : n.EndUtc;

                    result.Add(new YogaSlice
                    {
                        YogaId = (int)EYoga.AMRITASIDDHA,
                        YogaCode = EYoga.AMRITASIDDHA,
                        Vara = vara,
                        StartUtc = start,
                        EndUtc = end,
                        TithiId = t.TithiId,
                        NakshatraCode = (ENakshatra)n.NakshatraId,
                        ColorId = YogaSlice.GetYogaColorId((int)EYoga.AMRITASIDDHA)
                    });
                }
            }

            return result;
        }

        private static List<YogaSlice> BuildSarvartha(
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            DayOfWeek vara,
            List<TithiSlice> tithiSlices,
            List<NakshatraSlice> nakshatraSlices)
            {
                var result = new List<YogaSlice>();

                // -------------------------------
                // Списки накшатр по дням недели
                // -------------------------------

                ENakshatra[] allowedNakshatra = vara switch
                {
                    DayOfWeek.Monday => new[]
                    {
                        ENakshatra.SHRAVANA, ENakshatra.ROHINI, ENakshatra.PUSHYA,
                        ENakshatra.ANURADHA, ENakshatra.MRIGASHIRA
                    },

                    DayOfWeek.Tuesday => new[]
                    {
                        ENakshatra.UTTARABHADRAPADA, ENakshatra.KRITTIKA,
                        ENakshatra.ASHLESHA, ENakshatra.ASHWINI
                    },

                    DayOfWeek.Wednesday => new[]
                    {
                        ENakshatra.ROHINI, ENakshatra.HASTA, ENakshatra.KRITTIKA,
                        ENakshatra.MRIGASHIRA, ENakshatra.ANURADHA
                    },

                    DayOfWeek.Thursday => new[]
                    {
                        ENakshatra.REVATI, ENakshatra.ANURADHA, ENakshatra.ASHWINI,
                        ENakshatra.PUNARVASU, ENakshatra.PUSHYA
                    },

                    DayOfWeek.Friday => new[]
                    {
                        ENakshatra.ANURADHA, ENakshatra.ASHWINI, ENakshatra.PUNARVASU,
                        ENakshatra.SHRAVANA, ENakshatra.REVATI
                    },

                    DayOfWeek.Saturday => new[]
                    {
                        ENakshatra.SHRAVANA, ENakshatra.SWATI, ENakshatra.ROHINI
                    },

                    DayOfWeek.Sunday => new[]
                    {
                        ENakshatra.MULA, ENakshatra.UTTARAPHALGUNI, ENakshatra.UTTARAASHADHA,
                        ENakshatra.UTTARABHADRAPADA, ENakshatra.PUSHYA,
                        ENakshatra.ASHWINI, ENakshatra.HASTA
                    },
                    _ => Array.Empty<ENakshatra>()
                };

            // --------------------
            // Фильтрация накшатр
            // --------------------

            var nakFiltered =
                nakshatraSlices
                .Where(n => allowedNakshatra.Contains((ENakshatra)n.NakshatraId))
                .ToList();

            if (nakFiltered.Count == 0)
                return result;

            // ---------------------------------------------------------
            // Формирование йоги (на весь интервал накшатры)
            // ---------------------------------------------------------

            foreach (var n in nakFiltered)
            {
                // накшатр период ограничиваем внешним периодом:
                DateTime start = n.StartUtc > periodStartUtc ? n.StartUtc : periodStartUtc;
                DateTime end = n.EndUtc < periodEndUtc ? n.EndUtc : periodEndUtc;

                if (start >= end)
                    continue;

                result.Add(new YogaSlice
                {
                    YogaId = (int)EYoga.SARVARTHA,
                    YogaCode = EYoga.SARVARTHA,
                    Vara = vara,
                    StartUtc = start,
                    EndUtc = end,
                    NakshatraCode = (ENakshatra)n.NakshatraId,
                    TithiId = 0,                     
                    ColorId = YogaSlice.GetYogaColorId((int)EYoga.SARVARTHA)
                });
            }

            return result;
        }

        private static List<YogaSlice> BuildSiddha(
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            DayOfWeek vara,
            List<TithiSlice> tithiSlices)
        {
            var result = new List<YogaSlice>();

            // ---------------------------------------------------------
            // Таблицы титхи по дням недели
            // ---------------------------------------------------------

            int[] allowedTithis = vara switch
            {
                DayOfWeek.Tuesday => new[] { 3, 8, 13, 18, 23, 28 },
                DayOfWeek.Wednesday => new[] { 2, 7, 12, 17, 22, 27 },
                DayOfWeek.Thursday => new[] { 5, 10, 15, 20, 25, 30 },
                DayOfWeek.Friday => new[] { 1, 6, 11, 16, 21, 26 },
                DayOfWeek.Saturday => new[] { 4, 9, 14, 19, 24, 29 },

                // Monday, Sunday → нет Siddha
                _ => Array.Empty<int>()
            };

            if (allowedTithis.Length == 0)
                return result;

            // ---------------------------------------------------------
            // Фильтрация титхи
            // ---------------------------------------------------------

            var tithiFiltered =
                tithiSlices
                .Where(t => allowedTithis.Contains(t.TithiId))
                .ToList();

            if (tithiFiltered.Count == 0)
                return result;

            // ---------------------------------------------------------
            // Формируем Siddha по каждому титхи-интервалу
            // ---------------------------------------------------------

            foreach (var t in tithiFiltered)
            {
                DateTime start = t.StartUtc > periodStartUtc ? t.StartUtc : periodStartUtc;
                DateTime end = t.EndUtc < periodEndUtc ? t.EndUtc : periodEndUtc;

                if (start >= end)
                    continue;

                result.Add(new YogaSlice
                {
                    YogaId = (int)EYoga.SIDDHA,
                    YogaCode = EYoga.SIDDHA,

                    Vara = vara,

                    StartUtc = start,
                    EndUtc = end,

                    // Siddha зависит только от титхи:
                    TithiId = t.TithiId,
                    NakshatraCode = ENakshatra.NULL,

                    ColorId = YogaSlice.GetYogaColorId((int)EYoga.SIDDHA)
                });
            }

            return result;
        }

        private static List<YogaSlice> BuildLargeSiddha(
            DateTime periodStartUtc,
            DateTime periodEndUtc,
            DayOfWeek vara,
            List<NakshatraSlice> nakshatraSlices,
            List<TithiSlice> tithiSlices)
        {
            var result = new List<YogaSlice>();

            // правила large siddha на этот день
            if (!YogaRules.LargeSiddha.TryGetValue(vara, out var rule))
                return result;

            // 1. Фильтруем накшатры по списку AllowedNakshatras
            var nakFiltered = nakshatraSlices
                .Where(n => rule.AllowedNakshatras.Contains((ENakshatra)n.NakshatraId))
                .ToList();

            if (nakFiltered.Count == 0)
                return result;

            // 2. Особый случай — Tuesday: только по накшатрам, без тити
            if (rule.AllowedTithis == null || rule.AllowedTithis.Count == 0)
            {
                foreach (var n in nakFiltered)
                {
                    // ограничиваем интервал днём
                    var start = n.StartUtc > periodStartUtc ? n.StartUtc : periodStartUtc;
                    var end = n.EndUtc < periodEndUtc ? n.EndUtc : periodEndUtc;

                    if (start >= end)
                        continue;

                    result.Add(new YogaSlice
                    {
                        YogaId = (int)EYoga.SIDDHA,
                        YogaCode = EYoga.SIDDHA,
                        Vara = vara,
                        StartUtc = start,
                        EndUtc = end,
                        NakshatraCode = (ENakshatra)n.NakshatraId,
                        TithiId = 0,
                        ColorId = YogaSlice.GetYogaColorId((int)EYoga.SIDDHA)
                    });
                }

                return result;
            }

            // 3. Общий случай: нужны и накшатра, и титхи (пересечение интервалов)
            var tithiFiltered = tithiSlices
                .Where(t => rule.AllowedTithis.Contains(t.TithiId))
                .ToList();

            if (tithiFiltered.Count == 0)
                return result;

            foreach (var n in nakFiltered)
            {
                foreach (var t in tithiFiltered)
                {
                    // пересечение интервалов nakshatra & tithi внутри дня
                    var start = n.StartUtc > t.StartUtc ? n.StartUtc : t.StartUtc;
                    if (start < periodStartUtc) start = periodStartUtc;

                    var end = n.EndUtc < t.EndUtc ? n.EndUtc : t.EndUtc;
                    if (end > periodEndUtc) end = periodEndUtc;

                    if (start >= end)
                        continue;

                    result.Add(new YogaSlice
                    {
                        YogaId = (int)EYoga.SIDDHA,
                        YogaCode = EYoga.SIDDHA,
                        Vara = vara,
                        StartUtc = start,
                        EndUtc = end,
                        NakshatraCode = (ENakshatra)n.NakshatraId,
                        TithiId = t.TithiId,
                        ColorId = YogaSlice.GetYogaColorId((int)EYoga.SIDDHA)
                    });
                }
            }

            return result;
        }






    }
}
