namespace PADMA.Core.Enums
{
    /// <summary>
    /// Defines the type of astrological event or transit.
    /// Used for grouping, filtering and visual presentation in the calendar.
    /// </summary>
    public enum ETransitKind
    {
        Unknown = 0,

        // Core Swiss-based transits
        Planet = 1,
        Nakshatra = 2,
        Tithi = 3,
		NityaYoga = 4,
        MrityuBhaga = 5,
        Eclipse = 6,

        // Solar cycles
        Sunrise = 20,
        Sunset = 21,

        CustomUserTransit = 100
    }
}
