using GeoTimeZone;
using TimeZoneConverter;
using NodaTime;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Provides timezone utilities for both historical and current calculations.
    /// Combines NodaTime (for accurate tzdb offsets) and TimeZoneConverter (for .NET compatibility).
    /// </summary>
    public static class TimeZoneService
    {
        /// <summary>
        /// Returns the IANA timezone ID (e.g. "Europe/Warsaw") for given geographic coordinates.
        /// </summary>
        public static string GetIanaTimeZoneId(double latitude, double longitude)
        {
            return TimeZoneLookup.GetTimeZone(latitude, longitude).Result; // из GeoTimeZone
        }

        /// <summary>
        /// Returns the .NET/Windows timezone ID for given coordinates.
        /// </summary>
        public static string GetDotNetTimeZoneId(double latitude, double longitude)
        {
            string tzIana = GetIanaTimeZoneId(latitude, longitude);
            return TZConvert.IanaToWindows(tzIana);
        }

        public static DateTime ConvertLocalToUtcHistorical(DateTime localDateTime, double latitude, double longitude)
        {
            string tzIana = GetIanaTimeZoneId(latitude, longitude);
            var zone = DateTimeZoneProviders.Tzdb[tzIana];

            var unspecifiedLocal = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
            var local = LocalDateTime.FromDateTime(unspecifiedLocal);

            var zoned = local.InZoneLeniently(zone);

            return zoned.ToInstant().ToDateTimeUtc();
        }

        public static double GetHistoricalUtcOffsetHoursForLocal(
            DateTime localDateTime,
            double latitude,
            double longitude)
        {
            string tzIana = GetIanaTimeZoneId(latitude, longitude);
            var zone = DateTimeZoneProviders.Tzdb[tzIana];

            var local = LocalDateTime.FromDateTime(
                DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified));

            var zoned = local.InZoneLeniently(zone);

            return zoned.Offset.ToTimeSpan().TotalHours;
        }

        public static string FormatUtcOffset(double offsetHours)
        {
            var sign = offsetHours >= 0 ? "+" : "-";
            var abs = Math.Abs(offsetHours);

            int hours = (int)Math.Floor(abs);
            int minutes = (int)Math.Round((abs - hours) * 60);

            return $"UTC{sign}{hours:00}:{minutes:00}";
        }

        public static DateTime ShiftDateByDaylightDelta(DateTime date, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
        {
            DateTime newDate = date;

            foreach (var rule in adjustmentRules)
            {
                if (date >= rule.DateStart && date <= rule.DateEnd)
                {
                    var start = GetAdjustmentDate(rule.DaylightTransitionStart, date.Year);
                    var end = GetAdjustmentDate(rule.DaylightTransitionEnd, date.Year);

                    // переход может идти через Новый год
                    bool inDstPeriod = start < end
                        ? date >= start && date < end
                        : date >= start || date < end;

                    if (inDstPeriod)
                        newDate = newDate.Add(rule.DaylightDelta);
                }
            }

            return newDate;
        }

        private static DateTime GetAdjustmentDate(TimeZoneInfo.TransitionTime transition, int year)
        {
            if (transition.IsFixedDateRule)
            {
                return new DateTime(year, transition.Month, transition.Day,
                    transition.TimeOfDay.Hour, transition.TimeOfDay.Minute, transition.TimeOfDay.Second);
            }
            else
            {
                // Рассчитываем n-е вхождение дня недели месяца (например, последнее воскресенье октября)
                DateTime firstDay = new DateTime(year, transition.Month, 1);
                int dayOfWeek = (int)firstDay.DayOfWeek;
                int delta = (int)transition.DayOfWeek - dayOfWeek;
                if (delta < 0) delta += 7;
                int day = 1 + delta + (transition.Week - 1) * 7;
                if (day > DateTime.DaysInMonth(year, transition.Month))
                    day -= 7;

                return new DateTime(year, transition.Month, day,
                    transition.TimeOfDay.Hour, transition.TimeOfDay.Minute, transition.TimeOfDay.Second);
            }
        }

        public static DateTime ConvertUtcToLocalSmart(DateTime utc, double lat, double lon)
        {
            string tzId = GetDotNetTimeZoneId(lat, lon);
            var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            // базовый сдвиг
            DateTime local = utc + tzInfo.BaseUtcOffset;

            // коррекция по DST
            local = ShiftDateByDaylightDelta(local, tzInfo.GetAdjustmentRules());

            return local;
        }




    }
}
