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
    public const int SEFLG_SWIEPH = 2;
    public const int SEFLG_SPEED = 256;
    public const int SEFLG_TOPOCTR = 32768;
    public const int SEFLG_SIDEREAL = 65536;

    public const int SE_SIDM_LAHIRI = 1;

}
