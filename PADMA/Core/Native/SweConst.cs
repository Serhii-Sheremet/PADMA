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

    /*
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
    */

    /* defines for eclipse computations */

    public const int SE_ECL_CENTRAL = 1;
    public const int SE_ECL_NONCENTRAL = 2;
    public const int SE_ECL_TOTAL = 4;
    public const int SE_ECL_ANNULAR = 8;
    public const int SE_ECL_PARTIAL = 16;
    public const int SE_ECL_ANNULAR_TOTAL = 32;
    public const int SE_ECL_PENUMBRAL = 64;
    public const int SE_ECL_ALLTYPES_SOLAR = (SE_ECL_CENTRAL | SE_ECL_NONCENTRAL | SE_ECL_TOTAL | SE_ECL_ANNULAR | SE_ECL_PARTIAL | SE_ECL_ANNULAR_TOTAL);
    public const int SE_ECL_ALLTYPES_LUNAR = (SE_ECL_TOTAL | SE_ECL_PARTIAL | SE_ECL_PENUMBRAL);
    public const int SE_ECL_VISIBLE = 128;
    public const int SE_ECL_MAX_VISIBLE = 256;
    public const int SE_ECL_1ST_VISIBLE = 512;              /* begin of partial eclipse */
    public const int SE_ECL_PARTBEG_VISIBLE = 512;          /* begin of partial eclipse */
    public const int SE_ECL_2ND_VISIBLE = 1024;             /* begin of total eclipse */
    public const int SE_ECL_TOTBEG_VISIBLE = 1024;          /* begin of total eclipse */
    public const int SE_ECL_3RD_VISIBLE = 2048;             /* end of total eclipse */
    public const int SE_ECL_TOTEND_VISIBLE = 2048;          /* end of total eclipse */
    public const int SE_ECL_4TH_VISIBLE = 4096;             /* end of partial eclipse */
    public const int SE_ECL_PARTEND_VISIBLE = 4096;         /* end of partial eclipse */
    public const int SE_ECL_PENUMBBEG_VISIBLE = 8192;       /* begin of penumbral eclipse */
    public const int SE_ECL_PENUMBEND_VISIBLE = 16384;      /* end of penumbral eclipse */
    public const int SE_ECL_OCC_BEG_DAYLIGHT = 8192;        /* occultation begins during the day */
    public const int SE_ECL_OCC_END_DAYLIGHT = 16384;       /* occultation ends during the day */
    public const int SE_ECL_ONE_TRY = (32 * 1024);
    /* check if the next conjunction of the moon with
     * a planet is an occultation; don't search further */






}
