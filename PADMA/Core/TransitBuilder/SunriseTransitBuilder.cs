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
            double latitude,
            double longitude,
            double altitude = 0
        )
        {
            var prevSunrise  = SwissService.CalculateSunriseForDateAndLocation(dateUtc.AddDays(-1), latitude, longitude, altitude);
            var sunrise      = SwissService.CalculateSunriseForDateAndLocation(dateUtc, latitude, longitude, altitude);
            var sunset       = SwissService.CalculateSunsetForDateAndLocation(dateUtc, latitude, longitude, altitude);
            var nextSunrise  = SwissService.CalculateSunriseForDateAndLocation(dateUtc.AddDays(1), latitude, longitude, altitude);

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
                var slice = Build(date, latitude, longitude, altitude);
                list.Add(slice);

                date = date.AddDays(1);
            }

            return list;
        }
    }
}
