using System;
using System.Text;
using System.Threading.Tasks;
using PADMA.Core.Native;
using PADMA.Core.Utilities;
using PADMA.Core.Enums;
using GeoTimeZone;
using NodaTime;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Provides calculation services using Swiss Ephemeris native API.
    /// </summary>
    public static class SwissService
    {
        private static bool _initialized;

        /// <summary>
        /// Initializes the Swiss Ephemeris path depending on the platform.
        /// </summary>
        public static async Task InitializeEphemerisPathAsync()
        {
            if (_initialized) return;

            #if WINDOWS
                        string path = Path.Combine(AppContext.BaseDirectory, "Resources", "Raw", "ephe");
                        if (!Directory.Exists(path))
                            throw new DirectoryNotFoundException($"Ephemeris folder not found: {path}");
                        SwissEphemerisNative.swe_set_ephe_path(path);
            #elif ANDROID
                try
                {
                    string targetDir = Path.Combine(FileSystem.AppDataDirectory, "ephe");
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);
            
                    // Already extracted (file count > 100) — skip
                    if (Directory.Exists(targetDir) && Directory.GetFiles(targetDir, "*.se*").Length > 100)
                    {
                        System.Diagnostics.Debug.WriteLine($"[EPHE] Already extracted: {targetDir}");
                    }
                    else
                    {
                        string zipPath = Path.Combine(FileSystem.AppDataDirectory, "ephe.zip");
                        // copy from the app package to local storage
                        await using (var stream = await FileSystem.OpenAppPackageFileAsync("ephe.zip"))
                        await using (var file = File.Create(zipPath))
                        {
                            await stream.CopyToAsync(file);
                        }

                        // extract (overwrite existing files)
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, targetDir, overwriteFiles: true);

                        // delete the temp zip so it doesn't linger and hold a file lock
                        try { File.Delete(zipPath); } catch { /* ignore */ }
                    }

                    SwissEphemerisNative.swe_set_ephe_path(targetDir);
                }
                catch (Exception ex)
                {
                    // log + rethrow as a more descriptive exception
                    System.Diagnostics.Debug.WriteLine("[EPHE][ERROR] " + ex);
                    throw new InvalidOperationException("Failed to initialize ephemeris on Android. " +
                        "Make sure Resources/Raw/ephe.zip exists and has Build Action = MauiAsset.", ex);
                }
            #else
                string defaultPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Raw", "ephe");
                SwissEphemerisNative.swe_set_ephe_path(defaultPath);
            #endif

            // set default sidereal mode (Lahiri ayanamsha)
            SwissEphemerisNative.swe_set_sid_mode(SweConst.SE_SIDM_LAHIRI, 0, 0);

            _initialized = true;
        }

        /// <summary>
        /// Gets planetary position (longitude, latitude, distance, speed) for a given UTC date and planet ID.
        /// Handles Rahu (Mean/True) and Ketu (computed as opposite point).
        /// </summary>
        /// <param name="utcDate">UTC datetime</param>
        /// <param name="planetId">Internal PADMA PlanetId</param>
        /// <returns>Array: [longitude, latitude, distance, speedLong]</returns>
        public static double[] GetPlanetPosition(DateTime utcDate, int planetId, EAppSetting nodeType)
        {
            if (!SwissUtility.IsSupportedPlanet(planetId))
                throw new ArgumentException($"Unsupported planetId {planetId}. Ketu is handled separately.", nameof(planetId));

            // ensure UTC
            var utc = utcDate.Kind == DateTimeKind.Utc ? utcDate : utcDate.ToUniversalTime();

            // JD(UT) is computed via swe_utc_to_jd
            var dret = new double[2];
            var serr = new StringBuilder(256);
            int conv = SwissEphemerisNative.swe_utc_to_jd(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second,
                                                          SweConst.SE_GREG_CAL, dret, serr);
            if (conv < 0) throw new InvalidOperationException($"swe_utc_to_jd error: {serr}");
            double jd = dret[1]; // UT

            int swePlanetConst = SwissUtility.GetPlanetSWEConstByPlanetId(planetId, nodeType);
            if (swePlanetConst < 0)
                throw new ArgumentException($"Invalid planetId {planetId} mapping.", nameof(planetId));

            double[] xx = new double[6];
            int flags = SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SPEED | SweConst.SEFLG_SIDEREAL;

            // Critical:
            // SEFLG_SIDEREAL only says "calculate sidereal".
            // The exact sidereal mode must be explicitly set before calculation.
            SwissEphemerisNative.swe_set_sid_mode(SweConst.SE_SIDM_LAHIRI, 0, 0);
            int result = SwissEphemerisNative.swe_calc_ut(jd, swePlanetConst, flags, xx, serr);
            if (result < 0)
                throw new InvalidOperationException($"Swiss Ephemeris error: {serr}");

            double lon = xx[0];
            double lat = xx[1];
            double dist = xx[2];
            double speed = xx[3];

            if (planetId is 9 or 11)
                lon = SwissUtility.AdjustForKetu(lon);

            lon = NormalizeDegrees(lon);
            return new[] { lon, lat, dist, speed };
        }


        /// <summary>
        /// Normalizes an angle to the [0, 360) range.
        /// </summary>
        public static double NormalizeDegrees(double value)
        {
            value %= 360.0;
            return value < 0 ? value + 360.0 : value;
        }

        /// <summary>
        /// Sets sidereal mode (default: Lahiri).
        /// </summary>
        public static void SetSiderealMode(int sidMode = SweConst.SE_SIDM_LAHIRI)
        {
            SwissEphemerisNative.swe_set_sid_mode(sidMode, 0, 0);
        }

        public static void SetTopo(double geolon, double geolan, double geoalt)
        {
            SwissEphemerisNative.swe_set_topo(geolon, geolan, geoalt);
        }


        /// <summary>
        /// Gets Ayanamsa (Lahiri) for the given UTC date.
        /// </summary>
        public static double GetAyanamsa(DateTime utcDate)
        {
            SwissEphemerisNative.swe_set_sid_mode(SweConst.SE_SIDM_LAHIRI, 0, 0);

            double jd = SwissEphemerisNative.swe_julday(
                utcDate.Year,
                utcDate.Month,
                utcDate.Day,
                utcDate.Hour + utcDate.Minute / 60.0 + utcDate.Second / 3600.0,
                SweConst.SE_GREG_CAL);

            return SwissEphemerisNative.swe_get_ayanamsa_ut(jd);
        }

        public static double ToJulianDay(DateTime utc)
        {
            // utc must be DateTimeKind.Utc
            var dret = new double[2];
            var sb = new StringBuilder(256);
            SwissEphemerisNative.swe_utc_to_jd(
                utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second,
                /*gregflag*/ 1, dret, sb);
            return dret[1]; // UT
        }

        public static DateTime FromJulianDay(double jd_ut)
        {
            SwissEphemerisNative.swe_revjul(
                jd_ut,
                /*gregflag*/ 1,
                out int y,
                out int m,
                out int d,
                out double hour);

            // Convert the complete fractional day into a rounded number of seconds.
            // AddSeconds correctly carries 60 seconds into the next minute,
            // 60 minutes into the next hour, and 24 hours into the next day.
            var totalSeconds = (long)Math.Round(
                hour * 3600.0,
                MidpointRounding.AwayFromZero);

            return new DateTime(y, m, d, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(totalSeconds);
        }

        /// <summary>
        /// Calculates Ascendant for given date, coordinates, and house system.
        /// </summary>
        /// <param name="dateTimeUtc">UTC time of calculation</param>
        /// <param name="latitude">Latitude (degrees, North +)</param>
        /// <param name="longitude">Longitude (degrees, East +)</param>
        /// <param name="altitude">Altitude in meters</param>
        /// <param name="hsys">House system (e.g. 'O' = Placidus, 'E' = Equal, etc.)</param>
        /// <returns>Ascendant longitude in degrees</returns>
        public static double CalculateAscendantForDate(
            DateTime dateTimeUtc,
            double latitude,
            double longitude,
            double altitude,
            char hsys = 'O')
        {
            // Convert to Julian day
            double jut = dateTimeUtc.Hour + dateTimeUtc.Minute / 60.0 + dateTimeUtc.Second / 3600.0;
            double tjd_ut = SwissEphemerisNative.swe_julday(
                dateTimeUtc.Year, dateTimeUtc.Month, dateTimeUtc.Day, jut, SweConst.SE_GREG_CAL);

            // Apply sidereal Lahiri mode and topocentric coordinates
            SwissEphemerisNative.swe_set_sid_mode(SweConst.SE_SIDM_LAHIRI, 0, 0);
            SwissEphemerisNative.swe_set_topo(longitude, latitude, altitude);

            // Prepare result arrays
            double[] cusps = new double[13];  // house cusps 1..12
            double[] ascmc = new double[10];  // contains Ascendant, MC, ARMC, etc.

            // Call Swiss Ephemeris
            SwissEphemerisNative.swe_houses_ex(
                tjd_ut,
                SweConst.SEFLG_SIDEREAL,
                latitude,
                longitude,
                hsys,
                cusps,
                ascmc);

            // ascmc[0] is the Ascendant (in degrees)
            return ascmc[0];
        }

        /// <summary>
        /// Calculates the sunrise UTC time for the given date and coordinates,
        /// using the active configuration from AppSettings (Tip / Center).
        /// </summary>
        public static DateTime? CalculateSunriseForDateAndLocation(DateTime dateUtc, double latitude, double longitude, double altitude = 0)
        {
            try
            {
                int ipl = SweConst.SE_SUN;
                int iflag = SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SIDEREAL;

                // --- Build rsmi bitmask for sunrise ---
                int rsmi = SweConst.SE_CALC_RISE; // base: sunrise

                // --- Get current sunrise calculation setting from AppSettings ---
                EAppSetting sunriseSetting = DataCache.Instance.GetActiveSunriseSetting();
                // TIP: default is disc edge (disc center only if explicitly selected)
                if (sunriseSetting == EAppSetting.SUNRISECENTER)
                {
                    rsmi |= SweConst.SE_BIT_DISC_CENTER; // center of disc
                }

                // --- Convert to Julian day ---
                double jut = dateUtc.Hour + dateUtc.Minute / 60.0 + dateUtc.Second / 3600.0;
                double tjd_ut = SwissEphemerisNative.swe_julday(
                    dateUtc.Year, dateUtc.Month, dateUtc.Day, jut, SweConst.SE_GREG_CAL);

                double[] geopos = { longitude, latitude, altitude };
                double[] tret = new double[1];
                StringBuilder serr = new StringBuilder(256);

                // --- Configure Swiss Ephemeris ---
                SwissEphemerisNative.swe_set_sid_mode(SweConst.SE_SIDM_LAHIRI, 0, 0);
                SwissEphemerisNative.swe_set_topo(longitude, latitude, altitude);

                // --- Perform sunrise calculation ---
                int ret = SwissEphemerisNative.swe_rise_trans(
                    tjd_ut,
                    ipl,
                    string.Empty,
                    iflag,
                    rsmi,
                    geopos,
                    0,      // pressure
                    0,      // temperature
                    tret,
                    serr);

                if (ret < 0 || tret[0] <= 0)
                {
                    Console.WriteLine($"[WARN] Sunrise not found for {dateUtc:yyyy-MM-dd} | {serr}");
                    return null;
                }

                // --- Convert Julian Day to DateTime (UTC) ---
                double jd = tret[0];
                double dayFraction = jd - Math.Floor(jd);
                DateTime sunriseUtc = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(jd - 1721425.5);
                return sunriseUtc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Sunrise calculation failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calculates the sunset UTC time for the given date and coordinates,
        /// using the active configuration from AppSettings (Tip / Center).
        /// </summary>
        public static DateTime? CalculateSunsetForDateAndLocation(DateTime dateUtc, double latitude, double longitude, double altitude = 0)
        {
            try
            {
                int ipl = SweConst.SE_SUN;
                int iflag = SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SIDEREAL;
                
                // --- Build rsmi bitmask for sunset ---
                int rsmi = SweConst.SE_CALC_SET; // base: sunset
                
                // --- Get current sunset calculation setting from AppSettings ---
                EAppSetting setting = DataCache.Instance.GetActiveSunriseSetting();
                if (setting == EAppSetting.SUNRISECENTER)
                {
                    rsmi |= SweConst.SE_BIT_DISC_CENTER; // center of disc
                }

                // --- Convert to Julian day ---
                double jut = dateUtc.Hour + dateUtc.Minute / 60.0 + dateUtc.Second / 3600.0;
                double tjd_ut = SwissEphemerisNative.swe_julday(
                    dateUtc.Year, dateUtc.Month, dateUtc.Day, jut, SweConst.SE_GREG_CAL);

                double[] geopos = { longitude, latitude, altitude };
                double[] tret = new double[1];
                StringBuilder serr = new StringBuilder(256);

                // --- Configure Swiss Ephemeris ---
                SwissEphemerisNative.swe_set_sid_mode(SweConst.SE_SIDM_LAHIRI, 0, 0);
                SwissEphemerisNative.swe_set_topo(longitude, latitude, altitude);

                // --- Perform sunset calculation ---
                int ret = SwissEphemerisNative.swe_rise_trans(
                    tjd_ut,
                    ipl,
                    string.Empty,
                    iflag,
                    rsmi,
                    geopos,
                    0,      // atpress
                    0,      // attemp
                    tret,
                    serr);

                if (ret < 0 || tret[0] <= 0)
                {
                    Console.WriteLine($"[WARN] Sunset not found for {dateUtc:yyyy-MM-dd} | {serr}");
                    return null;
                }

                // --- Convert Julian Day to DateTime (UTC) ---
                double jd = tret[0];
                DateTime sunsetUtc = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(jd - 1721425.5);

                return sunsetUtc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Sunset calculation failed: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Releases memory and closes Swiss Ephemeris session.
        /// </summary>
        public static void Close()
        {
            SwissEphemerisNative.swe_close();
        }



    }
}
