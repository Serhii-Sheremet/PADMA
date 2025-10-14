using System;

namespace PADMA.Core.Utilities
{
    /// <summary>
    /// Provides helper extension methods for DateTime operations —
    /// range comparisons and timezone adjustments.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns true if the date is between startDate and endDate (inclusive).
        /// </summary>
        public static bool Between(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        /// <summary>
        /// Returns true if the date is strictly between startDate and endDate (exclusive).
        /// </summary>
        public static bool StrictBetween(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date > startDate && date < endDate;
        }

        /// <summary>
        /// Shifts the date by a given UTC offset.
        /// </summary>
        public static DateTime ShiftByUtcOffset(this DateTime date, TimeSpan baseUtcOffset)
        {
            return date.Add(baseUtcOffset);
        }

        /// <summary>
        /// Shifts date based on daylight saving adjustment rules.
        /// Used when calculating sunrise/sunset or transit times.
        /// </summary>
        public static DateTime ShiftByDaylightDelta(this DateTime date, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
        {
            if (adjustmentRules == null || adjustmentRules.Length == 0)
                return date;

            foreach (var rule in adjustmentRules)
            {
                if (date >= rule.DateStart && date <= rule.DateEnd)
                {
                    return date.Add(rule.DaylightDelta);
                }
            }

            return date;
        }
    }
}
