using System;

namespace PADMA.UI
{
    /// <summary>
    /// Cache key for day computations. Language and timezone are derived from ProfileId / DataCache.
    /// </summary>
    public readonly record struct DayKey(int ProfileId, DateOnly Date)
    {
        public static DayKey From(int profileId, DateTime date)
            => new(profileId, DateOnly.FromDateTime(date));
    }
}
