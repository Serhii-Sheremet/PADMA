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
        TaraBala = 3,
        Tithi = 4,
		NityaYoga = 5,
        MrityuBhaga = 6,
        Eclipse = 7,

        // Solar cycles
        Sunrise = 20,
        Sunset = 21,

        CustomUserTransit = 100
    }
}
