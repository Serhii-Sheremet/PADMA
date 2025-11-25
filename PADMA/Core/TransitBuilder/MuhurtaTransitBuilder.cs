using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PADMA.Core.TransitBuilder
{
    public static class MuhurtaTransitBuilder
    {
        /// <summary>
        /// Строит мухурты для всех SunriseSlice в диапазоне.
        /// Возвращает плоский список всех мухурт.
        /// </summary>
        public static List<MuhurtaSlice> BuildRange(List<SunriseSlice> sunriseSlices)
        {
            var result = new List<MuhurtaSlice>();

            foreach (var s in sunriseSlices)
            {
                if (s == null) continue;

                result.AddRange(BuildForDay(s));
            }

            return result
                .OrderBy(m => m.StartUtc)
                .ToList();
        }

        /// <summary>
        /// Строит 4–5 мухурт для одного дня.
        /// </summary>
        private static List<MuhurtaSlice> BuildForDay(SunriseSlice s)
        {
            var list = new List<MuhurtaSlice>();

            var sunrise = s.SunriseUtc;
            var previousSunrise = s.PreviousSunriseUtc;
            var sunset = s.SunsetUtc;

            var dayLong = sunset - sunrise;
            if (dayLong.TotalMinutes <= 0)
                return list;

            // порядок как в старом проекте
            var abhijit = BuildAbhijit(sunrise, dayLong, sunrise.DayOfWeek);
            var rahu = BuildRahuKala(sunrise, dayLong, sunrise.DayOfWeek);
            var brahma = BuildBrahmaMuhurta(sunrise, previousSunrise);
            var gulika = BuildGulikaKala(sunrise, dayLong, sunrise.DayOfWeek);
            var yamaganda = BuildYamaganda(sunrise, dayLong, sunrise.DayOfWeek);

            if (abhijit != null) list.Add(abhijit);
            if (rahu != null) list.Add(rahu);
            if (brahma != null) list.Add(brahma);
            if (gulika != null) list.Add(gulika);
            if (yamaganda != null) list.Add(yamaganda);

            return list;
        }

        // --------------------------------------------------------------------
        // ABHIJIT (нет в среду)
        // --------------------------------------------------------------------

        private static MuhurtaSlice BuildAbhijit(DateTime sunrise, TimeSpan dayLong, DayOfWeek dow)
        {
            if (dow == DayOfWeek.Wednesday)
                return null;

            // 15 частей
            var part = TimeSpan.FromTicks(dayLong.Ticks / 15);

            // 7-я часть
            var start = sunrise + part * 7;
            var end = start + part;

            return new MuhurtaSlice
            {
                MuhurtaCode = EMuhurta.ABHIJIT,
                StartUtc = start,
                EndUtc = end
            };
        }

        // --------------------------------------------------------------------
        // RAHU KALA (8 частей)
        // --------------------------------------------------------------------

        private static readonly Dictionary<DayOfWeek, int> RahuPartIndex = new()
        {
            { DayOfWeek.Monday,    1 },
            { DayOfWeek.Tuesday,   6 },
            { DayOfWeek.Wednesday, 4 },
            { DayOfWeek.Thursday,  5 },
            { DayOfWeek.Friday,    3 },
            { DayOfWeek.Saturday,  2 },
            { DayOfWeek.Sunday,    7 }
        };

        private static MuhurtaSlice BuildRahuKala(DateTime sunrise, TimeSpan dayLong, DayOfWeek dow)
        {
            var part = TimeSpan.FromTicks(dayLong.Ticks / 8);
            int index = RahuPartIndex[dow];

            var start = sunrise + part * index;
            var end = start + part;

            return new MuhurtaSlice
            {
                MuhurtaCode = EMuhurta.RAHUKALA,
                StartUtc = start,
                EndUtc = end
            };
        }

        // --------------------------------------------------------------------
        // GULIKA KALA (8 частей)
        // --------------------------------------------------------------------

        private static readonly Dictionary<DayOfWeek, int> GulikaPartIndex = new()
        {
            { DayOfWeek.Monday,    5 },
            { DayOfWeek.Tuesday,   6 },
            { DayOfWeek.Wednesday, 4 },
            { DayOfWeek.Thursday,  3 },
            { DayOfWeek.Friday,    2 },
            { DayOfWeek.Saturday,  1 },
            { DayOfWeek.Sunday,    7 }
        };

        private static MuhurtaSlice BuildGulikaKala(DateTime sunrise, TimeSpan dayLong, DayOfWeek dow)
        {
            var part = TimeSpan.FromTicks(dayLong.Ticks / 8);
            int index = GulikaPartIndex[dow];

            var start = sunrise + part * index;
            var end = start + part;

            return new MuhurtaSlice
            {
                MuhurtaCode = EMuhurta.GULIKAKALA,
                StartUtc = start,
                EndUtc = end
            };
        }

        // --------------------------------------------------------------------
        // YAMAGANDA (8 частей)
        // --------------------------------------------------------------------

        private static readonly Dictionary<DayOfWeek, int> YamagandaPartIndex = new()
        {
            { DayOfWeek.Monday,    7 },
            { DayOfWeek.Tuesday,   5 },
            { DayOfWeek.Wednesday, 6 },
            { DayOfWeek.Thursday,  4 },
            { DayOfWeek.Friday,    2 },
            { DayOfWeek.Saturday,  3 },
            { DayOfWeek.Sunday,    1 }
        };

        private static MuhurtaSlice BuildYamaganda(DateTime sunrise, TimeSpan dayLong, DayOfWeek dow)
        {
            var part = TimeSpan.FromTicks(dayLong.Ticks / 8);
            int index = YamagandaPartIndex[dow];

            var start = sunrise + part * index;
            var end = start + part;

            return new MuhurtaSlice
            {
                MuhurtaCode = EMuhurta.YAMAGANDA,
                StartUtc = start,
                EndUtc = end
            };
        }

        // --------------------------------------------------------------------
        // BRAHMA MUHURTA (особый случай)
        // previous_sunrise → sunrise, делится на 30 частей
        // --------------------------------------------------------------------

        private static MuhurtaSlice BuildBrahmaMuhurta(DateTime sunrise, DateTime prevSunrise)
        {
            var longPrev = sunrise - prevSunrise;
            if (longPrev.TotalMinutes <= 0)
                return null;

            // 30 частей
            var part = TimeSpan.FromTicks(longPrev.Ticks / 30);

            // 28-я часть (как в старом коде)
            var start = prevSunrise + part * 28;
            var end = start + part;

            return new MuhurtaSlice
            {
                MuhurtaCode = EMuhurta.BRAHMA,
                StartUtc = start,
                EndUtc = end
            };
        }
    }
}
