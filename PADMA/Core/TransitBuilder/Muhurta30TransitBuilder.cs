using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PADMA.Core.TransitBuilder
{
    public static class Muhurta30TransitBuilder
    {
        public static List<Muhurta30Slice> BuildForLocalDay(
            DateTime dayLocal,          // expected local date 00:00
            TimeZoneInfo tzInfo,
            double latitude,
            double longitude,
            double altitude = 0)
        {
            // Force Unspecified for stable ConvertTimeToUtc behavior
            var localDate = DateTime.SpecifyKind(dayLocal.Date, DateTimeKind.Unspecified);

            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, tzInfo);
            var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(1), tzInfo);

            var ss = SunriseTransitBuilder.Build(dayStartUtc, tzInfo, latitude, longitude, altitude);
            if (ss == null)
                return new List<Muhurta30Slice>();

            // active setting
            var setting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "MUHURTAGHATI" && s.Active == 1);

            var code = setting?.SettingCode ?? "MUHURTAGHATIEQUAL";

            // data list (must be ordered by ID 1..30)
            var muList = DataCache.Instance.Muhurta30List
                .OrderBy(m => m.Id)
                .ToList();

            if (muList.Count < 30)
                return new List<Muhurta30Slice>();

            // sunsetPrevious needed for DAYNIGHT
            DateTime? sunsetPreviousUtc = null;
            if (code == "MUHURTAGHATIDAYNIGHT")
            {
                var prevLocalDate = DateTime.SpecifyKind(localDate.AddDays(-1), DateTimeKind.Unspecified);
                var prevStartUtc = TimeZoneInfo.ConvertTimeToUtc(prevLocalDate, tzInfo);
                sunsetPreviousUtc = SwissService.CalculateSunsetForDateAndLocation(prevStartUtc, latitude, longitude, altitude);
            }

            var slices = code switch
            {
                "MUHURTAGHATIDAYNIGHT" => Calc15Plus15(muList, ss, sunsetPreviousUtc, tzInfo, dayStartUtc, dayEndUtc),
                "MUHURTAGHATIEQUAL" => CalcEqual30(muList, ss, tzInfo, dayStartUtc, dayEndUtc),
                "MUHURTAGHATIFROM6" => CalcFrom6(muList, localDate, tzInfo, dayStartUtc, dayEndUtc),
                _ => CalcEqual30(muList, ss, tzInfo, dayStartUtc, dayEndUtc),
            };

            return slices;
        }

        // 15 + 15 (day + night)
        private static List<Muhurta30Slice> Calc15Plus15(
            List<Core.Models.Muhurta30> muList,
            SunriseSlice ss,
            DateTime? sunsetPreviousUtc,
            TimeZoneInfo tzInfo,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            if (sunsetPreviousUtc == null)
                return new List<Muhurta30Slice>();

            var sunrisePrev = ss.PreviousSunriseUtc;
            var sunsetPrev = sunsetPreviousUtc.Value;
            var sunrise = ss.SunriseUtc;
            var sunset = ss.SunsetUtc;
            var sunriseNext = ss.NextSunriseUtc;

            var nightBefore = sunrise - sunsetPrev;
            var dayLen = sunset - sunrise;
            var nightAfter = sunriseNext - sunset;

            var stepNightBefore = TimeSpan.FromTicks(nightBefore.Ticks / 15);
            var stepDay = TimeSpan.FromTicks(dayLen.Ticks / 15);
            var stepNightAfter = TimeSpan.FromTicks(nightAfter.Ticks / 15);

            // Build "with buffer": prev night(15) + current day(15) + current night(15)
            var all = new List<Muhurta30Slice>(45);

            // prev night (use 16..30 => index 15..29)
            var start = sunsetPrev;
            for (int i = 0; i < 15; i++)
            {
                var end = start + stepNightBefore;
                var mu = muList[i + 15];
                all.Add(new Muhurta30Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
                });
                start = end;
            }

            // current day (use 1..15 => index 0..14)
            start = sunrise;
            for (int i = 0; i < 15; i++)
            {
                var end = start + stepDay;
                var mu = muList[i];
                all.Add(new Muhurta30Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
                });
                start = end;
            }

            // current night (use 16..30)
            start = sunset;
            for (int i = 0; i < 15; i++)
            {
                var end = start + stepNightAfter;
                var mu = muList[i + 15];
                all.Add(new Muhurta30Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
                });
                start = end;
            }

            return all
                .OrderBy(s => s.StartUtc)
                .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                .ToList();
        }

        // sunrise -> next sunrise / 30 (buffer prev + current = 60 slices)
        private static List<Muhurta30Slice> CalcEqual30(
            List<Core.Models.Muhurta30> muList,
            SunriseSlice ss,
            TimeZoneInfo tzInfo,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            var prevLen = ss.SunriseUtc - ss.PreviousSunriseUtc;
            var curLen = ss.NextSunriseUtc - ss.SunriseUtc;

            var prevStep = TimeSpan.FromTicks(prevLen.Ticks / 30);
            var curStep = TimeSpan.FromTicks(curLen.Ticks / 30);

            var all = new List<Muhurta30Slice>(60);

            // prev 30
            var start = ss.PreviousSunriseUtc;
            for (int i = 0; i < 30; i++)
            {
                var end = start + prevStep;
                var mu = muList[i];
                all.Add(new Muhurta30Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
                });
                start = end;
            }

            // current 30
            start = ss.SunriseUtc;
            for (int i = 0; i < 30; i++)
            {
                var end = start + curStep;
                var mu = muList[i];
                all.Add(new Muhurta30Slice
                {
                    StartUtc = start,
                    EndUtc = end,
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
                });
                start = end;
            }

            return all
                .OrderBy(s => s.StartUtc)
                .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                .ToList();
        }

        // 06:00 local -> 06:00 local / 30 (buffer prev + current)
        private static List<Muhurta30Slice> CalcFrom6(
            List<Core.Models.Muhurta30> muList,
            DateTime localDateUnspecified, // local date 00:00, Kind=Unspecified
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

            // total interval = 24h => step = 24h/30
            var step = TimeSpan.FromTicks(TimeSpan.FromHours(24).Ticks / 30);

            var all = new List<Muhurta30Slice>(60);

            // prev 30
            var startLocal = prevStartLocal;
            for (int i = 0; i < 30; i++)
            {
                var endLocal = startLocal + step;
                var mu = muList[i];

                all.Add(new Muhurta30Slice
                {
                    StartUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, tzInfo),
                    EndUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, tzInfo),
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
                });

                startLocal = endLocal;
            }

            // cur 30
            startLocal = curStartLocal;
            for (int i = 0; i < 30; i++)
            {
                var endLocal = startLocal + step;
                var mu = muList[i];

                all.Add(new Muhurta30Slice
                {
                    StartUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, tzInfo),
                    EndUtc = TimeZoneInfo.ConvertTimeToUtc(endLocal, tzInfo),
                    Muhurta30Id = mu.Id,
                    ColorId = mu.ColorId
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
