using PADMA.Core.Services;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;

namespace PADMA.Core.TransitBuilder
{
    public static class SunriseTransitBuilder
    {
        /// <summary>
        /// Строит SunriseSlice для конкретной даты.
        /// </summary>
        public static SunriseSlice Build(
            DateTime dateUtc,
            TimeZoneInfo tzInfo,
            double latitude,
            double longitude,
            double altitude = 0
        )
        {
            // dateUtc is a moment within the target local day. We must search sunrise starting
            // from local midnight, otherwise swe_rise_trans may return the next day's event.
            var local = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, tzInfo); // tzInfo должен быть доступен в Build
            var localDate = local.Date;

            var prevStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(-1), tzInfo);
            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, tzInfo);
            var nextStartUtc = TimeZoneInfo.ConvertTimeToUtc(localDate.AddDays(1), tzInfo);


            var prevSunrise  = SwissService.CalculateSunriseForDateAndLocation(prevStartUtc, latitude, longitude, altitude);
            var sunrise      = SwissService.CalculateSunriseForDateAndLocation(dayStartUtc, latitude, longitude, altitude);
            var sunset       = SwissService.CalculateSunsetForDateAndLocation(dayStartUtc, latitude, longitude, altitude);
            var nextSunrise  = SwissService.CalculateSunriseForDateAndLocation(nextStartUtc, latitude, longitude, altitude);

            // Если хотя бы один восход отсутствует — день считается некорректным
            if (prevSunrise == null || sunrise == null || nextSunrise == null)
                return null;

            return new SunriseSlice
            {
                PreviousSunriseUtc = prevSunrise.Value,
                SunriseUtc         = sunrise.Value,
                SunsetUtc          = sunset ?? sunrise.Value.AddHours(12),
                NextSunriseUtc     = nextSunrise.Value
            };
        }

        /// <summary>
        /// Строит диапазон SunriseSlice, начиная с указанной даты - startUtc.
        /// </summary>
        public static List<SunriseSlice> BuildRange(
            DateTime startUtc,
            DateTime endUtc,
            TimeZoneInfo tzInfo,
            double latitude,
            double longitude,
            double altitude = 0
        )
        {
            var list = new List<SunriseSlice>();

            // Двигаемся по полным датам UTC (Date-only шаг)
            var date = startUtc.Date;

            while (date <= endUtc.Date)
            {
                var slice = Build(date, tzInfo, latitude, longitude, altitude);
                list.Add(slice);

                date = date.AddDays(1);
            }

            return list;
        }
    }
}
