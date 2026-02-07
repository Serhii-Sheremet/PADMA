using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PADMA.Core.TransitBuilder
{
    public static class Ghati60TransitBuilder
    {
        public static List<Ghati60Slice> BuildForLocalDay(
            DateTime dayLocal,          // local date 00:00
            TimeZoneInfo tzInfo,
            double latitude,
            double longitude,
            double altitude = 0)
        {
            var localDate = DateTime.SpecifyKind(dayLocal.Date, DateTimeKind.Unspecified);

            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, tzInfo);
            var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(1), tzInfo);

            var ss = SunriseTransitBuilder.Build(dayStartUtc, tzInfo, latitude, longitude, altitude);
            if (ss == null)
                return new List<Ghati60Slice>();

            var setting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "MUHURTAGHATI" && s.Active == 1);

            var code = setting?.SettingCode ?? "MUHURTAGHATIEQUAL";

            var gList = DataCache.Instance.Ghati60List
                .OrderBy(g => g.Id) // важно: 1..60
                .ToList();

            if (gList.Count < 60)
                return new List<Ghati60Slice>();

            DateTime? sunsetPreviousUtc = null;
            if (code == "MUHURTAGHATIDAYNIGHT")
            {
                var prevLocalDate = DateTime.SpecifyKind(localDate.AddDays(-1), DateTimeKind.Unspecified);
                var prevStartUtc = TimeZoneInfo.ConvertTimeToUtc(prevLocalDate, tzInfo);
                sunsetPreviousUtc = SwissService.CalculateSunsetForDateAndLocation(prevStartUtc, latitude, longitude, altitude);
            }

            return code switch
            {
                "MUHURTAGHATIDAYNIGHT" => Calc30Plus30(gList, ss, sunsetPreviousUtc, dayStartUtc, dayEndUtc),
                "MUHURTAGHATIEQUAL" => CalcEqual60(gList, ss, dayStartUtc, dayEndUtc),
                "MUHURTAGHATIFROM6" => CalcFrom6(gList, localDate, tzInfo, dayStartUtc, dayEndUtc),
                _ => CalcEqual60(gList, ss, dayStartUtc, dayEndUtc),
            };
        }

        // DAYNIGHT: night(30) + day(30) + night(30) with night using items 31..60
        private static List<Ghati60Slice> Calc30Plus30(
            List<Core.Models.Ghati60> gList,
            SunriseSlice ss,
            DateTime? sunsetPreviousUtc,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            if (sunsetPreviousUtc == null)
                return new List<Ghati60Slice>();

            var sunsetPrev = sunsetPreviousUtc.Value;
            var sunrise = ss.SunriseUtc;
            var sunset = ss.SunsetUtc;
            var sunriseNext = ss.NextSunriseUtc;

            var nightBefore = sunrise - sunsetPrev;
            var dayLen = sunset - sunrise;
            var nightAfter = sunriseNext - sunset;

            var stepNightBefore = TimeSpan.FromTicks(nightBefore.Ticks / 30);
            var stepDay = TimeSpan.FromTicks(dayLen.Ticks / 30);
            var stepNightAfter = TimeSpan.FromTicks(nightAfter.Ticks / 30);

            var all = new List<Ghati60Slice>(90);

            // prev night: use gList[30..59]
            var start = sunsetPrev;
            for (int i = 0; i < 30; i++)
            {
                var end = start + stepNightBefore;
                var g = gList[i + 30];
                all.Add(new Ghati60Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Ghati60Id = g.Id,
                    ColorId = g.ColorId,
                    IsDayLightGhati = false
                });
                start = end;
            }

            // current day: use gList[0..29]
            start = sunrise;
            for (int i = 0; i < 30; i++)
            {
                var end = start + stepDay;
                var g = gList[i];
                all.Add(new Ghati60Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Ghati60Id = g.Id,
                    ColorId = g.ColorId,
                    IsDayLightGhati = true
                });
                start = end;
            }

            // current night: use gList[30..59]
            start = sunset;
            for (int i = 0; i < 30; i++)
            {
                var end = start + stepNightAfter;
                var g = gList[i + 30];
                all.Add(new Ghati60Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Ghati60Id = g.Id,
                    ColorId = g.ColorId,
                    IsDayLightGhati = false
                });
                start = end;
            }

            return all
                .OrderBy(s => s.StartUtc)
                .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                .ToList();
        }

        // EQUAL: sunrise->next sunrise / 60 with buffer prev+current = 120
        private static List<Ghati60Slice> CalcEqual60(
            List<Core.Models.Ghati60> gList,
            SunriseSlice ss,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            var prevLen = ss.SunriseUtc - ss.PreviousSunriseUtc;
            var curLen = ss.NextSunriseUtc - ss.SunriseUtc;

            var prevStep = TimeSpan.FromTicks(prevLen.Ticks / 60);
            var curStep = TimeSpan.FromTicks(curLen.Ticks / 60);

            var all = new List<Ghati60Slice>(120);

            // prev 60
            var start = ss.PreviousSunriseUtc;
            for (int i = 0; i < 60; i++)
            {
                var end = start + prevStep;
                var g = gList[i];
                all.Add(new Ghati60Slice { StartUtc = start, EndUtc = end, Ghati60Id = g.Id, ColorId = g.ColorId });
                start = end;
            }

            // current 60
            start = ss.SunriseUtc;
            for (int i = 0; i < 60; i++)
            {
                var end = start + curStep;
                var g = gList[i];
                all.Add(new Ghati60Slice { StartUtc = start, EndUtc = end, Ghati60Id = g.Id, ColorId = g.ColorId });
                start = end;
            }

            return all
                .OrderBy(s => s.StartUtc)
                .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                .ToList();
        }

        // FROM6: 06:00 local -> 06:00 local / 60 with buffer prev+current
        private static List<Ghati60Slice> CalcFrom6(
            List<Core.Models.Ghati60> gList,
            DateTime localDateUnspecified,
            TimeZoneInfo tzInfo,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            DateTime MakeLocal0600(DateTime d) =>
                DateTime.SpecifyKind(new DateTime(d.Year, d.Month, d.Day, 6, 0, 0), DateTimeKind.Unspecified);

            var baseDate = localDateUnspecified.Date;
            var prevDate = baseDate.AddDays(-1);

            var prevStartLocal = MakeLocal0600(prevDate);
            var curStartLocal = MakeLocal0600(baseDate);

            var step = TimeSpan.FromTicks(TimeSpan.FromHours(24).Ticks / 60); // 24 minutes

            var all = new List<Ghati60Slice>(120);

            // prev 60
            var startLocal = prevStartLocal;
            for (int i = 0; i < 60; i++)
            {
                var endLocal = startLocal + step;
                var g = gList[i];

                all.Add(new Ghati60Slice
                {
                    StartUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, tzInfo),
                    EndUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, tzInfo),
                    Ghati60Id = g.Id,
                    ColorId = g.ColorId
                });

                startLocal = endLocal;
            }

            // cur 60
            startLocal = curStartLocal;
            for (int i = 0; i < 60; i++)
            {
                var endLocal = startLocal + step;
                var g = gList[i];

                all.Add(new Ghati60Slice
                {
                    StartUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, tzInfo),
                    EndUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, tzInfo),
                    Ghati60Id = g.Id,
                    ColorId = g.ColorId
                });

                startLocal = endLocal;
            }

            return all
                .OrderBy(s => s.StartUtc)
                .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                .ToList();
        }
    }
}
