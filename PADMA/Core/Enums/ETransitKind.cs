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
        Karana = 5,
        NityaYoga = 6,
        MrityuBhaga = 7,
        Eclipse = 8,

        // Solar cycles
        Sunrise = 20,
        Sunset = 21,

        CustomUserTransit = 100
    }
}
