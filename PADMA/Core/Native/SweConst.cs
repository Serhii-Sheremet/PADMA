namespace PADMA.Core.Native;

internal static class SweConst
{
    public const int SE_GREG_CAL = 1;
    public const int SE_JUL_CAL = 0;

    // === Планеты ===
    public const int SE_SUN = 0;
    public const int SE_MOON = 1;
    public const int SE_MERCURY = 2;
    public const int SE_VENUS = 3;
    public const int SE_MARS = 4;
    public const int SE_JUPITER = 5;
    public const int SE_SATURN = 6;
    public const int SE_URANUS = 7;
    public const int SE_NEPTUNE = 8;
    public const int SE_PLUTO = 9;

    // === Лунные узлы ===
    public const int SE_MEAN_NODE = 10;  // Rahu (Mean)
    public const int SE_TRUE_NODE = 11;  // Rahu (True)

    // === Флаги расчётов ===
    public const int SEFLG_JPLEPH = 1;   // JPL ephemeris
    public const int SEFLG_SWIEPH = 2;
    public const int SEFLG_MOSEPH = 4;   // Moshier ephemeris
    public const int SEFLG_SPEED = 256;
    public const int SEFLG_TOPOCTR = 32768;
    public const int SEFLG_SIDEREAL = 65536;

    public const int SE_SIDM_LAHIRI = 1;

    // === Eclipse flags ===
    // Eclipses: type bits
    public const int SE_ECL_TOTAL = 0x0001; // 1
    public const int SE_ECL_ANNULAR = 0x0002; // 2
    public const int SE_ECL_PARTIAL = 0x0004; // 4
    public const int SE_ECL_ANNULAR_TOTAL = 0x0008; // 8 (hybrid)

    // Geometry (centrality)
    public const int SE_ECL_NONCENTRAL = 0x0010; // 16
    public const int SE_ECL_CENTRAL = 0x0020; // 32

    // Convenience masks
    public const int SE_ECL_ALLTYPES_SOLAR =
        SE_ECL_CENTRAL | SE_ECL_NONCENTRAL |
        SE_ECL_TOTAL | SE_ECL_ANNULAR | SE_ECL_PARTIAL | SE_ECL_ANNULAR_TOTAL;

    public const int SE_ECL_ALLTYPES_LUNAR =
        SE_ECL_TOTAL | SE_ECL_PARTIAL; // (полутеневые мы отбрасываем)

    // Для совместимости c кодом:
    public const int SE_ECL_HYBRID = SE_ECL_ANNULAR_TOTAL;









}
