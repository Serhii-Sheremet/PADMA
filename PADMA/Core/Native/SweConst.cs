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

    /* for swe_rise_transit() */
    public const int SE_CALC_RISE = 1;
    public const int SE_CALC_SET = 2;
    public const int SE_CALC_MTRANSIT = 4;
    public const int SE_CALC_ITRANSIT = 8;
    public const int SE_BIT_DISC_CENTER = 256;         /* to be or'ed to SE_CALC_RISE/SET,
				                                            * if rise or set of disc center is 
				                                            * required*/
    public const int SE_BIT_DISC_BOTTOM = 8192;        /* to be or'ed to SE_CALC_RISE/SET,
                                                            * if rise or set of lower limb of
                                                            * disc is requried*/
    public const int SE_BIT_GEOCTR_NO_ECL_LAT = 128;   /* use geocentric rather than topocentric 
                                                              position of object and
                                                              ignore its ecliptic latitude */
    public const int SE_BIT_NO_REFRACTION = 512;       /* to be or'ed to SE_CALC_RISE/SET, 
				                                            * if refraction is to be ignored*/
    public const int SE_BIT_CIVIL_TWILIGHT = 1024;     /* to be or'ed to SE_CALC_RISE/SET */
    public const int SE_BIT_NAUTIC_TWILIGHT = 2048;    /* to be or'ed to SE_CALC_RISE/SET */
    public const int SE_BIT_ASTRO_TWILIGHT = 4096;     /* to be or'ed to SE_CALC_RISE/SET */
    public const int SE_BIT_FIXED_DISC_SIZE = 16384;   /* or'ed to SE_CALC_RISE/SET:
                                                            * neglect the effect of distance on
				                                            * disc size */
    public const int SE_BIT_FORCE_SLOW_METHOD = 32768; /* This is only a Astrodienst in-house
                                                            * test flag.It forces the usage
                                                            * of the old, slow calculation of
                                                            * risings and settings. */
    public const int SE_BIT_HINDU_RISING = (SE_BIT_DISC_CENTER | SE_BIT_NO_REFRACTION | SE_BIT_GEOCTR_NO_ECL_LAT);

    //  ---- my custom sunrise const ----
    public const int SE_SUNRISE_TIP = (SE_CALC_RISE | SE_BIT_DISC_BOTTOM | SE_BIT_GEOCTR_NO_ECL_LAT);
    public const int SE_SUNRISE_CENTER = (SE_CALC_RISE | SE_BIT_DISC_CENTER | SE_BIT_GEOCTR_NO_ECL_LAT);
    public const int SE_SUNSET_TIP = (SE_CALC_SET | SE_BIT_DISC_BOTTOM | SE_BIT_GEOCTR_NO_ECL_LAT);
    public const int SE_SUNSET_CENTER = (SE_CALC_SET | SE_BIT_DISC_CENTER | SE_BIT_GEOCTR_NO_ECL_LAT);
    //  ----------------------------------




}
