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
        Tithi = 2,
		NityaYoga = 3,
        MrityuBhaga = 4,
        Eclipse = 5,

        // Solar cycles
        Sunrise = 20,
        Sunset = 21,

        CustomUserTransit = 100
    }
}
