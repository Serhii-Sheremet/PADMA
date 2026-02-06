using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PADMA.Core.TransitBuilder
{
    public static class HoraTransitBuilder
    {
        private sealed class HoraPlanetDef
        {
            public int Id { get; init; }
            public EHoraPlanet PlanetCode { get; init; }
            public EColor ColorCode { get; init; }
        }

        // Порядок 7 планет как в легаси
        private static readonly List<HoraPlanetDef> _hpList = new()
        {
            new HoraPlanetDef { Id = 1, PlanetCode = EHoraPlanet.SUN,     ColorCode = EColor.SUN },
            new HoraPlanetDef { Id = 2, PlanetCode = EHoraPlanet.VENUS,   ColorCode = EColor.VENUS },
            new HoraPlanetDef { Id = 3, PlanetCode = EHoraPlanet.MERCURY, ColorCode = EColor.MERCURY },
            new HoraPlanetDef { Id = 4, PlanetCode = EHoraPlanet.MOON,    ColorCode = EColor.MOON },
            new HoraPlanetDef { Id = 5, PlanetCode = EHoraPlanet.SATURN,  ColorCode = EColor.SATURN },
            new HoraPlanetDef { Id = 6, PlanetCode = EHoraPlanet.JUPITER, ColorCode = EColor.JUPITER },
            new HoraPlanetDef { Id = 7, PlanetCode = EHoraPlanet.MARS,    ColorCode = EColor.MARS },
        };

        public static List<HoraSlice> BuildForLocalDay(
            DateTime dayLocal, // 00:00 local
            TimeZoneInfo tzInfo,
            double latitude,
            double longitude,
            double altitude = 0)
        {
            var localDate = DateTime.SpecifyKind(dayLocal.Date, DateTimeKind.Unspecified);

            // Границы суток в UTC (для фильтрации)
            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, tzInfo);
            var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(1), tzInfo);

            // prev day for sunsetPrevious
            var prevLocalDate = DateTime.SpecifyKind(localDate.AddDays(-1), DateTimeKind.Unspecified);
            var prevStartUtc = TimeZoneInfo.ConvertTimeToUtc(prevLocalDate, tzInfo);
            var sunsetPreviousUtc = SwissService.CalculateSunsetForDateAndLocation(prevStartUtc, latitude, longitude, altitude);

            // sunriseSlice
            var sunriseSlice = SunriseTransitBuilder.Build(dayStartUtc, tzInfo, latitude, longitude, altitude);
            if (sunriseSlice == null)
                return new List<HoraSlice>();

            // Какой режим Хоры активен
            var setting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "HORA" && s.Active == 1);

            var code = setting?.SettingCode ?? "HORADAYNIGHT";

            return code switch
            {
                "HORADAYNIGHT" => Calc12Plus12(_hpList, sunriseSlice, sunsetPreviousUtc, dayLocal, tzInfo, dayStartUtc, dayEndUtc),
                "HORAEQUAL" => CalcEqual24(_hpList, sunriseSlice, dayLocal, tzInfo, dayStartUtc, dayEndUtc),
                "HORAFROM6" => CalcFrom6(_hpList, dayLocal, tzInfo, dayStartUtc, dayEndUtc),
                _ => CalcEqual24(_hpList, sunriseSlice, dayLocal, tzInfo, dayStartUtc, dayEndUtc),
            };
        }

        // --- 12+12: деление дня и ночи на 12 частей (в UTC) ---
        private static List<HoraSlice> Calc12Plus12(
            List<HoraPlanetDef> hpList,
            SunriseSlice ss,
            DateTime? sunsetPreviousUtc,
            DateTime dayLocal,
            TimeZoneInfo tzInfo,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            if (sunsetPreviousUtc == null)
                return new List<HoraSlice>();

            var sunrisePrev = ss.PreviousSunriseUtc;
            var sunsetPrev = sunsetPreviousUtc.Value;
            var sunrise = ss.SunriseUtc;
            var sunset = ss.SunsetUtc;
            var sunriseNext = ss.NextSunriseUtc;

            var dayLongBefore = sunsetPrev - sunrisePrev;
            var nightLongBefore = sunrise - sunsetPrev;
            var dayLong = sunset - sunrise;
            var nightLongAfter = sunriseNext - sunset;

            var dayBefore12 = TimeSpan.FromTicks(dayLongBefore.Ticks / 12);
            var nightBefore12 = TimeSpan.FromTicks(nightLongBefore.Ticks / 12);
            var day12 = TimeSpan.FromTicks(dayLong.Ticks / 12);
            var nightAfter12 = TimeSpan.FromTicks(nightLongAfter.Ticks / 12);

            var horaBaseLocalDate = TimeZoneInfo.ConvertTimeFromUtc(ss.SunriseUtc, tzInfo).Date;
            var before24 = GetHora24PlanetList(hpList, horaBaseLocalDate.AddDays(-1));
            var cur24 = GetHora24PlanetList(hpList, horaBaseLocalDate.Date);

            var all = new List<HoraSlice>(48);
            int idx = 0;

            // prev day daylight 12
            var start = sunrisePrev;
            for (int i = 0; i < 12; i++)
            {
                var end = start + dayBefore12;
                all.Add(new HoraSlice { StartUtc = start, EndUtc = end, PlanetCode = before24[idx].PlanetCode, ColorCode = before24[idx].ColorCode, IsDayLightHora = true });
                start = end;
                idx++;
            }

            // prev night 12
            start = sunsetPrev;
            for (int i = 0; i < 12; i++)
            {
                var end = start + nightBefore12;
                all.Add(new HoraSlice { StartUtc = start, EndUtc = end, PlanetCode = before24[idx].PlanetCode, ColorCode = before24[idx].ColorCode, IsDayLightHora = false });
                start = end;
                idx++;
            }

            // current daylight 12
            idx = 0;
            start = sunrise;
            for (int i = 0; i < 12; i++)
            {
                var end = start + day12;
                all.Add(new HoraSlice { StartUtc = start, EndUtc = end, PlanetCode = cur24[idx].PlanetCode, ColorCode = cur24[idx].ColorCode, IsDayLightHora = true });
                start = end;
                idx++;
            }

            // current night 12
            start = sunset;
            for (int i = 0; i < 12; i++)
            {
                var end = start + nightAfter12;
                all.Add(new HoraSlice { StartUtc = start, EndUtc = end, PlanetCode = cur24[idx].PlanetCode, ColorCode = cur24[idx].ColorCode, IsDayLightHora = false });
                start = end;
                idx++;
            }

            return all.Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc).ToList();
        }


        // --- Equal: sunrise -> next sunrise / 24 ---
        private static List<HoraSlice> CalcEqual24(
            List<HoraPlanetDef> hpList,
            SunriseSlice ss,
            DateTime dayLocal,
            TimeZoneInfo tzInfo,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {

            var horaBaseLocalDate = TimeZoneInfo.ConvertTimeFromUtc(ss.SunriseUtc, tzInfo).Date;
            var prev24 = GetHora24PlanetList(hpList, horaBaseLocalDate.AddDays(-1));
            var cur24 = GetHora24PlanetList(hpList, horaBaseLocalDate);

            var prevLen = ss.SunriseUtc - ss.PreviousSunriseUtc;
            var curLen = ss.NextSunriseUtc - ss.SunriseUtc;

            var prevStep = TimeSpan.FromTicks(prevLen.Ticks / 24);
            var curStep = TimeSpan.FromTicks(curLen.Ticks / 24);

            var all = new List<HoraSlice>(48);

            // prev day 24
            var start = ss.PreviousSunriseUtc;
            for (int i = 0; i < 24; i++)
            {
                var end = start + prevStep;
                all.Add(new HoraSlice
                {
                    StartUtc = start,
                    EndUtc = end,
                    PlanetCode = prev24[i].PlanetCode,
                    ColorCode = prev24[i].ColorCode,
                    IsDayLightHora = false
                });
                start = end;
            }

            // cur day 24
            start = ss.SunriseUtc;
            for (int i = 0; i < 24; i++)
            {
                var end = start + curStep;
                all.Add(new HoraSlice
                {
                    StartUtc = start,
                    EndUtc = end,
                    PlanetCode = cur24[i].PlanetCode,
                    ColorCode = cur24[i].ColorCode,
                    IsDayLightHora = false
                });
                start = end;
            }

            // Важно: фильтруем по UTC границам локального дня
            return all.Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc).ToList();
        }

        // --- From6: каждый час от 06:00 локально ---
        private static List<HoraSlice> CalcFrom6(
            List<HoraPlanetDef> hpList,
            DateTime dayLocal,        // ожидаем local day 00:00
            TimeZoneInfo tzInfo,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            var baseDate = dayLocal.Date; // именно локальная дата

            var prevBase = baseDate.AddDays(-1);

            // 24-планетные последовательности для двух суток
            var prev24 = GetHora24PlanetList(hpList, prevBase);
            var cur24 = GetHora24PlanetList(hpList, baseDate);

            // старт в 06:00 local (Unspecified чтобы ConvertTimeToUtc трактовал как time in tzInfo)
            DateTime MakeLocal0600(DateTime d) =>
                DateTime.SpecifyKind(new DateTime(d.Year, d.Month, d.Day, 6, 0, 0), DateTimeKind.Unspecified);

            var prevStartLocal = MakeLocal0600(prevBase);
            var curStartLocal = MakeLocal0600(baseDate);

            var all = new List<HoraSlice>(48);

            for (int i = 0; i < 24; i++)
            {
                var sLocal = prevStartLocal.AddHours(i);
                var eLocal = prevStartLocal.AddHours(i + 1);
                all.Add(new HoraSlice
                {
                    StartUtc = TimeZoneInfo.ConvertTimeToUtc(sLocal, tzInfo),
                    EndUtc = TimeZoneInfo.ConvertTimeToUtc(eLocal, tzInfo),
                    PlanetCode = prev24[i].PlanetCode,
                    ColorCode = prev24[i].ColorCode
                });
            }

            for (int i = 0; i < 24; i++)
            {
                var sLocal = curStartLocal.AddHours(i);
                var eLocal = curStartLocal.AddHours(i + 1);
                all.Add(new HoraSlice
                {
                    StartUtc = TimeZoneInfo.ConvertTimeToUtc(sLocal, tzInfo),
                    EndUtc = TimeZoneInfo.ConvertTimeToUtc(eLocal, tzInfo),
                    PlanetCode = cur24[i].PlanetCode,
                    ColorCode = cur24[i].ColorCode
                });
            }

            return all
                .OrderBy(s => s.StartUtc)
                .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                .ToList();
        }

        // ----- Планетный порядок на сутки -----
        private static List<HoraPlanetDef> GetHora24PlanetList(List<HoraPlanetDef> hpList, DateTime dateLocal)
        {
            var dayPlanet = GetMainHoraPlanetByDayOfWeek(dateLocal.DayOfWeek);
            var swapped = SwapList(hpList, dayPlanet);

            var res = new List<HoraPlanetDef>(24);
            int idx = 0;
            for (int i = 0; i < 24; i++)
            {
                if (idx == 7) idx = 0;
                res.Add(swapped[idx]);
                idx++;
            }
            return res;
        }

        private static EHoraPlanet GetMainHoraPlanetByDayOfWeek(DayOfWeek dow) =>
            dow switch
            {
                DayOfWeek.Monday => EHoraPlanet.MOON,
                DayOfWeek.Tuesday => EHoraPlanet.MARS,
                DayOfWeek.Wednesday => EHoraPlanet.MERCURY,
                DayOfWeek.Thursday => EHoraPlanet.JUPITER,
                DayOfWeek.Friday => EHoraPlanet.VENUS,
                DayOfWeek.Saturday => EHoraPlanet.SATURN,
                _ => EHoraPlanet.SUN
            };

        private static List<HoraPlanetDef> SwapList(List<HoraPlanetDef> hpList, EHoraPlanet startPlanet)
        {
            var id = hpList.FirstOrDefault(i => i.PlanetCode == startPlanet)?.Id ?? 1;
            var res = new List<HoraPlanetDef>();
            res.AddRange(hpList.Where(i => i.Id >= id));
            res.AddRange(hpList.Where(i => i.Id < id));
            return res;
        }
    }
}
