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
        ZodiakSign = 1,
        Nakshatra = 2,
		TaraBala = 3,
		ChandraBalla = 4,
		Tithi = 5,
        Karana = 6,
		NityaYoga = 7,
        MrityuBhaga = 8,
        Eclipse = 9,

        // Solar cycles
        Sunrise = 10,
        Sunset = 11,

        CustomUserTransit = 100
    }
}
