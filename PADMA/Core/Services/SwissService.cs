using System;
using System.Text;
using System.Threading.Tasks;
using PADMA.Core.Native;
using PADMA.Core.Utilities;

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
            
                    // если уже распаковано (типичных файлов > 100), не трогаем
                    if (Directory.Exists(targetDir) && Directory.GetFiles(targetDir, "*.se*").Length > 100)
                    {
                        System.Diagnostics.Debug.WriteLine($"[EPHE] Already extracted: {targetDir}");
                    }
                    else
                    {
                        string zipPath = Path.Combine(FileSystem.AppDataDirectory, "ephe.zip");
                        // копируем из ресурсов в локальный файл
                        await using (var stream = await FileSystem.OpenAppPackageFileAsync("ephe.zip"))
                        await using (var file = File.Create(zipPath))
                        {
                            await stream.CopyToAsync(file);
                        }
            
                        // распаковываем (перезапись разрешена)
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, targetDir, overwriteFiles: true);
            
                        // удал€ем временный zip, чтобы не висел в lock
                        try { File.Delete(zipPath); } catch { /* ignore */ }
                    }
            
                    SwissEphemerisNative.swe_set_ephe_path(targetDir);
                }
                catch (Exception ex)
                {
                    // лог + пробрасываем с контекстом
                    System.Diagnostics.Debug.WriteLine("[EPHE][ERROR] " + ex);
                    throw new InvalidOperationException("Failed to initialize ephemeris on Android. " +
                        "Make sure Resources/Raw/ephe.zip exists and has Build Action = MauiAsset.", ex);
                }
            #else
                string defaultPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Raw", "ephe");
                SwissEphemerisNative.swe_set_ephe_path(defaultPath);
            #endif

            // сидерика Ћахири по умолчанию
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
        public static double[] GetPlanetPosition(DateTime utcDate, int planetId)
        {
            if (!SwissUtility.IsSupportedPlanet(planetId))
                throw new ArgumentException($"Unsupported planetId {planetId}. Ketu is handled separately.", nameof(planetId));

            // ∆Єстко к UTC
            var utc = utcDate.Kind == DateTimeKind.Utc ? utcDate : utcDate.ToUniversalTime();

            // JD(UT) безопасно через swe_utc_to_jd
            var dret = new double[2];
            var serr = new StringBuilder(256);
            int conv = SwissEphemerisNative.swe_utc_to_jd(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second,
                                                          SweConst.SE_GREG_CAL, dret, serr);
            if (conv < 0) throw new InvalidOperationException($"swe_utc_to_jd error: {serr}");
            double jd = dret[1]; // UT

            int swePlanetConst = SwissUtility.GetPlanetSWEConstByPlanetId(planetId);
            if (swePlanetConst < 0)
                throw new ArgumentException($"Invalid planetId {planetId} mapping.", nameof(planetId));

            double[] xx = new double[6];
            int flags = SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SPEED | SweConst.SEFLG_SIDEREAL;

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

        /// <summary>
        /// Gets Ayanamsa (Lahiri) for the given UTC date.
        /// </summary>
        public static double GetAyanamsa(DateTime utcDate)
        {
            double jd = SwissEphemerisNative.swe_julday(
                utcDate.Year,
                utcDate.Month,
                utcDate.Day,
                utcDate.Hour + utcDate.Minute / 60.0 + utcDate.Second / 3600.0,
                SweConst.SE_GREG_CAL);

            return SwissEphemerisNative.swe_get_ayanamsa_ut(jd);
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
