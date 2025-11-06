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

        /// <summary>
        /// Returns UTC offset (in hours) for the given date/time and coordinates using NodaTime tzdb.
        /// </summary>
        public static double GetUtcOffsetHours(DateTime dateTimeUtc, double latitude, double longitude)
        {
            string tzIana = GetIanaTimeZoneId(latitude, longitude);
            var zone = DateTimeZoneProviders.Tzdb[tzIana];
            var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc));
            var offset = zone.GetZoneInterval(instant).StandardOffset;
            return offset.ToTimeSpan().TotalHours;
        }
    }
}
