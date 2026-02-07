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
        ChandraBala = 7,
        MrityuBhaga = 8,
        Eclipse = 9,

        // Solar cycles
        Sunrise = 20,
        Yoga = 21,
        Muhurta = 22,
        Hora = 23,
        Muhurta30 = 24,
        Ghati60 = 25,

        CustomUserTransit = 100
    }
}
