using PADMA.Core.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Centralized database service for all app data (settings, localization, reference tables).
    /// Handles SQLite operations and ensures DB version consistency.
    /// </summary>
    public class DatabaseService
    {
        private readonly string _dbPath;
        private SQLiteConnection _connection;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");
            EnsureDatabaseExists();
            _connection = new SQLiteConnection(_dbPath);
            BackfillLocationNormColumnsIfNeeded();
        }

        #region Initialization

        /// <summary>
        /// Ensures the database file exists in AppDataDirectory.
        /// Copies from embedded resources if missing.
        /// </summary>
        private void EnsureDatabaseExists()
        {
            var assetDb = Path.Combine(AppContext.BaseDirectory, "Resources", "Raw", "PADMADB.db3");
            if (!File.Exists(_dbPath) && File.Exists(assetDb))
                File.Copy(assetDb, _dbPath);
        }

        /// <summary>
        /// Returns raw SQLite connection (for debug/advanced cases).
        /// </summary>
        public SQLiteConnection GetConnection() => _connection;

        #endregion


        #region Languages

        /// <summary>
        /// Returns all available languages from LANGUAGE table.
        /// </summary>
        public IReadOnlyList<Language> GetLanguages()
        {
            const string sql = @"SELECT ID as Id, LANGUAGECODE as LanguageCode, CULTURECODE as CultureCode FROM LANGUAGE";
            return _connection.Query<Language>(sql);
        }

        /// <summary>
        /// Reads currently active language (from APPSETTING group LANGUAGE).
        /// </summary>
        public Language GetCurrentLanguage()
        {
            var settings = GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "LANGUAGE" && x.Active == 1);
            var langCode = active?.SettingCode ?? "ENGLISH";

            var languages = GetLanguages();
            return languages.FirstOrDefault(l =>
                string.Equals(l.LanguageCode, langCode, StringComparison.OrdinalIgnoreCase))
                ?? new Language { Id = 1, LanguageCode = "ENGLISH", CultureCode = "en" };
        }

        /// <summary>
        /// Returns active UI language code (e.g. "en", "uk", "pl", "ru").
        /// </summary>
        public string GetActiveLanguageCode()
        {
            var settings = GetAppSettingsList();
            var activeLang = settings.FirstOrDefault(x => x.GroupCode == "LANGUAGE" && x.Active == 1);

            return activeLang?.SettingCode switch
            {
                "ENGLISH" => "en",
                "UKRAINIAN" => "uk",
                "POLISH" => "pl",
                "RUSSIAN" => "ru",
                _ => "en"
            };
        }
        
        #endregion


        #region App Settings (Configuration)

        /// <summary>
        /// Returns full APPSETTING list.
        /// </summary>
        public List<AppSettingList> GetAppSettingsList()
        {
            try
            {
                return _connection.Table<AppSettingList>().ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DB read error: {ex.Message}");
                return new List<AppSettingList>();
            }
        }

        public AppSettingList? GetActiveSetting(string groupCode)
        {
            var settings = GetAppSettingsList();
            return settings.FirstOrDefault(x =>
                string.Equals(x.GroupCode, groupCode, StringComparison.OrdinalIgnoreCase) && x.Active == 1);
        }

        public string? GetActiveSettingCode(string groupCode)
        {
            return GetActiveSetting(groupCode)?.SettingCode;
        }

        /// <summary>
        /// Updates multiple APPSETTING records (mass update).
        /// </summary>
        public void UpdateAppSettings(List<AppSettingList> settings)
        {
            try
            {
                foreach (var s in settings)
                    _connection.Update(s);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DB update error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets current "First day of week" from DB (Sunday/Monday).
        /// </summary>
        public DayOfWeek GetFirstDayOfWeekFromDb()
        {
            var settings = GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "WEEK" && x.Active == 1);

            var code = active?.SettingCode ?? "WEEKMONDAY";
            return code == "WEEKSUNDAY" ? DayOfWeek.Sunday : DayOfWeek.Monday;
        }
        
        /// <summary>
        /// Activates the specified setting within a given group.
        /// Automatically deactivates all other records in that group.
        /// </summary>
        public void SetAppSettingActive(string groupCode, string settingCode)
        {
            try
            {
                // Сначала деактивируем все настройки этой группы
                _connection.Execute("UPDATE APPSETTING SET ACTIVE = 0 WHERE GROUPCODE = ?", groupCode);

                // Активируем выбранную настройку
                _connection.Execute("UPDATE APPSETTING SET ACTIVE = 1 WHERE GROUPCODE = ? AND SETTINGCODE = ?", groupCode, settingCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DB SetAppSettingActive error: {ex.Message}");
            }
        }


        /// <summary>
        /// Deactivates all records in a given APPSETTING group.
        /// </summary>
        public void DeactivateGroup(string groupCode)
            => _connection.Execute("UPDATE APPSETTING SET ACTIVE = 0 WHERE GROUPCODE = ?", groupCode);

        /// <summary>
        /// Activates specific APPSETTING record by ID.
        /// </summary>
        public void ActivateSetting(int id)
            => _connection.Execute("UPDATE APPSETTING SET ACTIVE = 1 WHERE ID = ?", id);

        #endregion


        #region Localization

        /// <summary>
        /// Loads localized UI texts (APP_TEXTS) for a given language.
        /// </summary>
        public List<AppText> GetAppTextsList(string languageCode = null)
        {
            try
            {
                _connection.CreateTable<AppText>();

                if (string.IsNullOrEmpty(languageCode))
                    return _connection.Table<AppText>().ToList();

                return _connection.Table<AppText>().Where(x => x.LanguageCode == languageCode).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB] Error loading AppTexts: {ex.Message}");
                return new List<AppText>();
            }
        }

        #endregion


        #region Profiles & Locations

        /// <summary>
        /// Returns all profiles from PROFILE table.
        /// </summary>
        public List<Profile> GetProfiles()
        {
            try
            {
                const string sql = @"SELECT ID as Id, 
                                            PROFILENAME as ProfileName, 
                                            PERSONNAME as PersonName, 
                                            PERSONSURNAME as PersonSurname, 
                                            DATEOFBIRTH as DateOfBirth, 
                                            PLACEOFBIRTHID as PlaceOfBirthId, 
                                            PLACEOFLIVINGID as PlaceOfLivingId, 
                                            MESSAGE as Message, 
                                            CHECKED as Checked 
                                     FROM PROFILE 
                                     ORDER BY PROFILENAME COLLATE NOCASE";

                return _connection.Query<Profile>(sql);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] GetProfiles error: {ex.Message}");
                return new List<Profile>();
            }
        }

        /// <summary>
        /// Returns a single profile by ID.
        /// </summary>
        public Profile? GetProfileById(int id)
        {
            try
            {
                const string sql = @"SELECT ID as Id, 
                                            PROFILENAME as ProfileName, 
                                            PERSONNAME as PersonName, 
                                            PERSONSURNAME as PersonSurname, 
                                            DATEOFBIRTH as DateOfBirth, 
                                            PLACEOFBIRTHID as PlaceOfBirthId, 
                                            PLACEOFLIVINGID as PlaceOfLivingId, 
                                            MESSAGE as Message, 
                                            CHECKED as Checked 
                                     FROM PROFILE 
                                     WHERE ID = ?";

                return _connection.FindWithQuery<Profile>(sql, id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetProfileById error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Adds a new profile.
        /// </summary>
        public void AddProfile(Profile profile)
        {
            try
            {
                var sql = @"INSERT INTO PROFILE 
                            (PROFILENAME, PERSONNAME, PERSONSURNAME, DATEOFBIRTH, 
                             PLACEOFBIRTHID, PLACEOFLIVINGID, MESSAGE, CHECKED) 
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                var dateText = profile.DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                _connection.Execute(sql,
                    profile.ProfileName,
                    profile.PersonName,
                    profile.PersonSurname,
                    dateText,
                    profile.PlaceOfBirthId,
                    profile.PlaceOfLivingId,
                    profile.Message,
                    profile.Checked ? 1 : 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] AddProfile error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing profile.
        /// </summary>
        public void UpdateProfile(Profile profile)
        {
            try
            {
                var sql = @"UPDATE PROFILE SET 
                                PROFILENAME = ?, 
                                PERSONNAME = ?, 
                                PERSONSURNAME = ?, 
                                DATEOFBIRTH = ?, 
                                PLACEOFBIRTHID = ?, 
                                PLACEOFLIVINGID = ?, 
                                MESSAGE = ?, 
                                CHECKED = ?
                            WHERE ID = ?";

                var dateText = profile.DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                _connection.Execute(sql,
                    profile.ProfileName,
                    profile.PersonName,
                    profile.PersonSurname,
                    dateText,
                    profile.PlaceOfBirthId,
                    profile.PlaceOfLivingId,
                    profile.Message,
                    profile.Checked ? 1 : 0,
                    profile.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] UpdateProfile error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a profile by ID.
        /// </summary>
        public void DeleteProfile(int id)
        {
            try
            {
                _connection.Execute("DELETE FROM PROFILE WHERE ID = ?", id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DeleteProfile error: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets one profile as default (CHECKED = 1), others = 0.
        /// </summary>
        public void SetDefaultProfile(int id)
        {
            try
            {
                _connection.Execute("UPDATE PROFILE SET CHECKED = 0");
                _connection.Execute("UPDATE PROFILE SET CHECKED = 1 WHERE ID = ?", id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] SetDefaultProfile error: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns location by ID.
        /// </summary>
        public AppLocation? GetLocationById(int id)
        {
            try
            {
                const string sql = @"SELECT ID as Id, 
                                            LOCALITY as Locality, 
                                            LATITUDE as Latitude, 
                                            LONGITUDE as Longitude, 
                                            REGION as Region, 
                                            STATE as State, 
                                            COUNTRY as Country, 
                                            COUNTRYCODE as CountryCode, 
                                            LANGUAGECODE as LanguageCode
                                     FROM LOCATION WHERE ID = ?";

                return _connection.FindWithQuery<AppLocation>(sql, id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetLocationById error: {ex.Message}");
                return null;
            }
        }

        private int InsertLocationWithNorm(AppLocation location)
        {
            // важно: тут можно быть мягче к пустым полям
            var locality = location.Locality ?? "";
            var region = location.Region ?? "";
            var state = location.State ?? "";
            var country = location.Country ?? "";
            var countryCode = location.CountryCode ?? "";
            var lang = location.LanguageCode ?? "";
            var lat = location.Latitude ?? "0";
            var lon = location.Longitude ?? "0";

            _connection.Execute(@"
                    INSERT INTO LOCATION
                    (LOCALITY, REGION, STATE, COUNTRY, COUNTRYCODE, LANGUAGECODE, LATITUDE, LONGITUDE,
                     LOCALITY_NORM, REGION_NORM, STATE_NORM, COUNTRY_NORM)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                locality, region, state, country, countryCode, lang, lat, lon,
                Norm(locality), Norm(region), Norm(state), Norm(country));

            // sqlite-net: получить last_insert_rowid
            var id = _connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
            location.Id = id;
            return id;
        }

        /// <summary>
        /// Adds a new location (used after Nominatim search) and return its Id.
        /// </summary>
        public int AddLocation(AppLocation location)
        {
            try
            {
                return InsertLocationWithNorm(location);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] AddLocation error: {ex.Message}");
                return 0;
            }
        }

        public int GetOrCreateLocation(AppLocation location)
        {
            try
            {
                // 1. Если ID уже есть — значит это существующая запись
                if (location.Id > 0)
                    return location.Id;

                // 2. Ищем в базе аналогичную локацию
                var existing = FindExistingLocation(location);
                if (existing != null)
                {
                    location.Id = existing.Id;
                    return location.Id;
                }

                // 3. Ничего не нашли — создаём новую запись
                return InsertLocationWithNorm(location);  // sqlite-net заполнит location.Id
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetOrCreateLocation error: {ex.Message}");
                return 0;
            }
        }

        private AppLocation? FindExistingLocation(AppLocation loc)
        {
            if (string.IsNullOrWhiteSpace(loc.Locality))
                return null;

            var localityNorm = Norm(loc.Locality);
            var regionNorm = Norm(loc.Region);
            var stateNorm = Norm(loc.State);

            var countryNorm = Norm(loc.Country);
            var countryCode = (loc.CountryCode ?? "").Trim().ToUpperInvariant();

            const string sqlRegion = @"
                    SELECT * FROM LOCATION
                     WHERE LOCALITY_NORM = ?
                       AND (
                            (COUNTRYCODE IS NOT NULL AND COUNTRYCODE = ?)
                         OR (COUNTRY_NORM IS NOT NULL AND COUNTRY_NORM = ?)
                       )
                       AND (
                             (REGION_NORM IS NOT NULL AND REGION_NORM = ?)
                          OR (STATE_NORM  IS NOT NULL AND STATE_NORM  = ?)
                       )
                     LIMIT 1";

            var matchByRegion = _connection.Query<AppLocation>(
                sqlRegion, localityNorm, countryCode, countryNorm, regionNorm, stateNorm
            ).FirstOrDefault();

            if (matchByRegion != null)
                return matchByRegion;

            const string sqlSimple = @"
                    SELECT * FROM LOCATION
                     WHERE LOCALITY_NORM = ?
                       AND (
                            (COUNTRYCODE IS NOT NULL AND COUNTRYCODE = ?)
                         OR (COUNTRY_NORM IS NOT NULL AND COUNTRY_NORM = ?)
                       )
                     LIMIT 1";

            return _connection.Query<AppLocation>(sqlSimple, localityNorm, countryCode, countryNorm)
                              .FirstOrDefault();
        }

        private static string Norm(string? s)
        {
            s = (s ?? "").Trim();
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");
            return s.ToLowerInvariant();
        }

        public void BackfillLocationNormColumnsIfNeeded()
        {
            try
            {
                // Если таблица пустая — ничего не делаем
                var total = _connection.ExecuteScalar<int>("SELECT COUNT(1) FROM LOCATION");
                if (total == 0) return;

                // Сколько записей не имеют нормализованных полей
                var missing = _connection.ExecuteScalar<int>(@"
                                    SELECT COUNT(1)
                                    FROM LOCATION
                                    WHERE 
                                        LOCALITY_NORM IS NULL OR LOCALITY_NORM = ''
                                        OR REGION_NORM IS NULL OR REGION_NORM = ''
                                        OR STATE_NORM IS NULL OR STATE_NORM = ''
                                        OR COUNTRY_NORM IS NULL OR COUNTRY_NORM = ''");

                if (missing == 0) return;

                var rows = _connection.Query<(int Id, string Locality, string Region, string State, string Country)>(@"
                                SELECT 
                                    ID as Id,
                                    LOCALITY as Locality,
                                    IFNULL(REGION,'') as Region,
                                    IFNULL(STATE,'') as State,
                                    IFNULL(COUNTRY,'') as Country
                                FROM LOCATION");

                _connection.RunInTransaction(() =>
                {
                    foreach (var r in rows)
                    {
                        _connection.Execute(@"
                                UPDATE LOCATION
                                SET 
                                    LOCALITY_NORM = ?,
                                    REGION_NORM   = ?,
                                    STATE_NORM    = ?,
                                    COUNTRY_NORM  = ?
                                WHERE ID = ?",
                            Norm(r.Locality),
                            Norm(r.Region),
                            Norm(r.State),
                            Norm(r.Country),
                            r.Id);
                    }
                });

                System.Diagnostics.Debug.WriteLine($"[PADMA] BackfillLocationNormColumnsIfNeeded: updated {rows.Count} rows");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] BackfillLocationNormColumnsIfNeeded error: {ex.Message}");
            }
        }

        /// <summary>
        /// Поиск локаций в таблице LOCATION по частичному совпадению имени.
        /// </summary>
        public List<AppLocation> SearchLocationByName(string query)
        {
            try
            {
                query = (query ?? "").Trim();
                if (query.Length == 0)
                    return new List<AppLocation>();

                // нормализуем запрос
                var q = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ").Trim().ToLowerInvariant();

                const string sql = @"
                        SELECT 
                            MIN(ID) as Id,
                            LOCALITY as Locality,
                            REGION as Region,
                            STATE as State,
                            COUNTRY as Country,
                            COUNTRYCODE as CountryCode,
                            LANGUAGECODE as LanguageCode,
                            LATITUDE as Latitude,
                            LONGITUDE as Longitude
                        FROM LOCATION
                        WHERE 
                            LOCALITY_NORM LIKE '%' || ? || '%'
                            OR REGION_NORM LIKE '%' || ? || '%'
                            OR STATE_NORM LIKE '%' || ? || '%'
                            OR COUNTRY_NORM LIKE '%' || ? || '%'
                        GROUP BY 
                            LOWER(LOCALITY), LOWER(REGION), LOWER(STATE), LOWER(COUNTRY),
                            LATITUDE, LONGITUDE
                        ORDER BY LOCALITY ASC";

                return _connection.Query<AppLocation>(sql, q, q, q, q);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] SearchLocationByName error: {ex.Message}");
                return new List<AppLocation>();
            }
        }

        /// <summary>
        /// Возвращает полный список локаций из таблицы LOCATION.
        /// </summary>
        public List<AppLocation> GetLocations()
        {
            try
            {
                const string sql = @"SELECT ID as Id, 
                                            LOCALITY as Locality, 
                                            REGION as Region, 
                                            STATE as State, 
                                            COUNTRY as Country, 
                                            COUNTRYCODE as CountryCode, 
                                            LANGUAGECODE as LanguageCode, 
                                            LATITUDE as Latitude, 
                                            LONGITUDE as Longitude 
                                     FROM LOCATION 
                                     ORDER BY ID";

                return _connection.Query<AppLocation>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetLocations error: {ex.Message}");
                return new List<AppLocation>();
            }
        }

        #endregion


        #region Reference Data

        /// <summary>
        /// Return a list of Color Codes from COLOR table.
        /// </summary>
        public IReadOnlyList<AppColor> GetColors()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        CODE        AS Code,
                        ARGBVALUE   AS ArgbValue
                    FROM COLOR
                    ORDER BY ID";

                return _connection.Query<AppColor>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetColors error: {ex.Message}");
                return new List<AppColor>();
            }
        }

        /// <summary>
        /// Return a list of Color Descriptions from COLOR_DESC table.
        /// </summary>
        public IReadOnlyList<AppColorDesc> GetColorDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        COLORID         AS ColorId,
                        NAME            AS Name,
                        LANGUAGECODE    AS LanguageCode
                    FROM COLOR_DESC
                    ORDER BY ID";

                return _connection.Query<AppColorDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetColorDescs error: {ex.Message}");
                return new List<AppColorDesc>();
            }
        }

        /// <summary>
        /// Return a list of Planet Codes from PLANET table.
        /// </summary>
        public IReadOnlyList<Planet> GetPlanets()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        PLANETCODE  AS PlanetCode
                    FROM PLANET
                    ORDER BY ID";

                return _connection.Query<Planet>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetPlanets error: {ex.Message}");
                return new List<Planet>();
            }
        }

        /// <summary>
        /// Return a list of Planet Descriptions from PLANET_DESC table.
        /// </summary>
        public IReadOnlyList<PlanetDesc> GetPlanetDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        PLANETID        AS PlanetId,
                        NAME            AS Name,
                        LANGUAGECODE    AS LanguageCode
                    FROM PLANET_DESC
                    ORDER BY ID";

                return _connection.Query<PlanetDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetPlanetDescs error: {ex.Message}");
                return new List<PlanetDesc>();
            }
        }

        /// <summary>
        /// Return a list of Transits from TRANSIT table.
        /// </summary>
        public IReadOnlyList<Transit> GetTransits()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        PLANETID    AS PlanetId,
                        HOUSE       AS House,
                        COLORID     AS ColorId,
                        VEDHA       AS Vedha
                    FROM TRANSIT
                    ORDER BY ID";

                return _connection.Query<Transit>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetTransits error: {ex.Message}");
                return new List<Transit>();
            }
        }

        /// <summary>
        /// Return a list of Transit Descriptions from TRANSIT_DESC table.
        /// </summary>
        public IReadOnlyList<TransitDesc> GetTransitDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        TRANSITID       AS TransitId,
                        DESCRIPTION     AS Description,
                        LANGUAGECODE    AS LanguageCode
                    FROM TRANSIT_DESC
                    ORDER BY ID";

                return _connection.Query<TransitDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetTransitDescs error: {ex.Message}");
                return new List<TransitDesc>();
            }
        }

        /// <summary>
        /// Return a list of Zodiac Codes from ZODIAC table.
        /// </summary>
        public IReadOnlyList<Zodiac> GetZodiacs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        ZODIACCODE  AS ZodiacCode
                    FROM ZODIAC
                    ORDER BY ID";

                return _connection.Query<Zodiac>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetZodiacs error: {ex.Message}");
                return new List<Zodiac>();
            }
        }

        /// <summary>
        /// Return a list of Zodiac Descriptions from ZODIAC_DESC table.
        /// </summary>
        public IReadOnlyList<ZodiacDesc> GetZodiacDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        ZODIACID        AS ZodiacId,
                        NAME            AS Name,
                        LANGUAGECODE    AS LanguageCode
                    FROM ZODIAC_DESC
                    ORDER BY ID";

                return _connection.Query<ZodiacDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetZodiacDescs error: {ex.Message}");
                return new List<ZodiacDesc>();
            }
        }

        /// <summary>
        /// Returns all Pada records (108 entries total).
        /// </summary>
        public IReadOnlyList<Pada> GetPadas()
        {
            try
            {
                const string sql = @"SELECT 
                                ID              AS Id,
                                ZODIACID        AS ZodiacId,
                                NAKSHATRAID     AS NakshatraId,
                                PADANUMBER      AS PadaNumber,
                                DREKKANA        AS Drekkana,
                                SPECIALNAVAMSA  AS SpecialNavamsa,
                                NAVAMSA         AS Navamsa,
                                COLORID         AS ColorId
                             FROM PADA
                             ORDER BY ID";
                return _connection.Query<Pada>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetPadas error: {ex.Message}");
                return new List<Pada>();
            }
        }

        /// <summary>
        /// Возвращает список фиксированных "мёртвых" градусов (Mrityu Bhaga)
        /// для каждой планеты и знака зодиака.
        /// </summary>
        public IReadOnlyList<MrityuBhaga> GetMrityuBhaga()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID        AS Id,
                        PLANETID  AS PlanetId,
                        ZODIACID  AS ZodiacId,
                        DEGREE    AS Degree
                    FROM MRITYUBHAGA
                    ORDER BY ID";

                return _connection.Query<MrityuBhaga>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMrityuBhaga error: {ex.Message}");
                return new List<MrityuBhaga>();
            }
        }

        /// <summary>
        /// Returns a list of Nitya Yoga Codes from NITYAYOGA table..
        /// </summary>
        public IReadOnlyList<NityaYoga> GetNityaYogas()
        {
            try
            {
                const string sql = @"SELECT 
                                ID              AS Id,
                                NYCODE          AS Code,
                                COLORID         AS ColorId,
                                NAKSHATRAID     AS NakshatraId,
                                YOGIPLANETID    AS YogiPlanetId,
                                AVAYOGIPLANETID AS AvaYogiPlanetId
                             FROM NITYAYOGA
                             ORDER BY ID";
                return _connection.Query<NityaYoga>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetNityaYogas error: {ex.Message}");
                return new List<NityaYoga>();
            }
        }

        /// <summary>
        /// Return a list of Nitya Yoga Descriptions from NITYAYOGA_DESC table.
        /// </summary>
        public IReadOnlyList<NityaYogaDesc> GetNityaYogaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        NITYAYOGAID     AS NityaYogaId,
                        NAME            AS Name,
                        DEITY           AS Deity,
                        MEANING         AS Meaning,
                        DESCRIPTION     AS Description,
                        LANGUAGECODE    AS LanguageCode
                    FROM NITYAYOGA_DESC
                    ORDER BY ID";

                return _connection.Query<NityaYogaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetNityaYogaDescs error: {ex.Message}");
                return new List<NityaYogaDesc>();
            }
        }

        /// <summary>
        /// Return a list of Eclipse Codes from ECLIPSE table.
        /// </summary>
        public IReadOnlyList<Eclipse> GetEclipses()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        ECLIPSECODE AS EclipseCode
                    FROM ECLIPSE
                    ORDER BY ID";

                return _connection.Query<Eclipse>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetEclipses error: {ex.Message}");
                return new List<Eclipse>();
            }
        }

        /// <summary>
        /// Return a list of Eclipse Descriptions from ECLIPSE_DESC table.
        /// </summary>
        public IReadOnlyList<EclipseDesc> GetEclipseDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        ECLIPSEID       AS EclipseId,
                        NAME            AS Name,
                        LANGUAGECODE    AS LanguageCode
                    FROM ECLIPSE_DESC
                    ORDER BY ID";

                return _connection.Query<EclipseDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetEclipseDescs error: {ex.Message}");
                return new List<EclipseDesc>();
            }
        }


        /// <summary>
        /// Return a list of Nakshatras from NAKSHATRA table.
        /// </summary>
        public IReadOnlyList<Nakshatra> GetNakshatras()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        NAKSHATRACODE   AS NakshatraCode,
                        COLORID         AS ColorId
                    FROM NAKSHATRA
                    ORDER BY ID";

                return _connection.Query<Nakshatra>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetNakshatras error: {ex.Message}");
                return new List<Nakshatra>();
            }
        }

        /// <summary>
        /// Return a list of Nakshatra Descriptions from NAKSHATRA_DESC table.
        /// </summary>
        public IReadOnlyList<NakshatraDesc> GetNakshatraDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        NAKSHATRAID     AS NakshatraId,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        RULER           AS Ruler,
                        NATURE          AS Nature,
                        DESCRIPTION     AS Description,
                        GOODFOR         AS GoodFor,
                        BADFOR          AS BadFor,
                        LANGUAGECODE    AS LanguageCode
                    FROM NAKSHATRA_DESC
                    ORDER BY ID";

                return _connection.Query<NakshatraDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetNakshatraDescs error: {ex.Message}");
                return new List<NakshatraDesc>();
            }
        }

        /// <summary>
        /// Return a list of TaraBala from TARABALA table.
        /// </summary>
        public IReadOnlyList<TaraBala> GetTaraBalas()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID      AS Id,
                        COLORID AS ColorId
                    FROM TARABALA
                    ORDER BY ID";

                return _connection.Query<TaraBala>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetTaraBalas error: {ex.Message}");
                return new List<TaraBala>();
            }
        }

        /// <summary>
        /// Return a list of TaraBala Descriptions from TARABALA_DESC table.
        /// </summary>
        public IReadOnlyList<TaraBalaDesc> GetTaraBalaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        TARABALAID      AS TaraBalaId,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        DESCRIPTION     AS Description,
                        LANGUAGECODE    AS LanguageCode
                    FROM TARABALA_DESC
                    ORDER BY ID";

                return _connection.Query<TaraBalaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetTaraBalaDescs error: {ex.Message}");
                return new List<TaraBalaDesc>();
            }
        }

        /// <summary>
        /// Return a list of Tithi from TITHI table.
        /// </summary>
        public IReadOnlyList<Tithi> GetTithis()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        COLORID         AS ColorId
                    FROM TITHI
                    ORDER BY ID";

                return _connection.Query<Tithi>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetTithis error: {ex.Message}");
                return new List<Tithi>();
            }
        }

        /// <summary>
        /// Return a list of Tithi Descriptions from TITHI_DESC table.
        /// </summary>
        public IReadOnlyList<TithiDesc> GetTithiDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        TITHIID         AS TithiId,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        RULER           AS Ruler,
                        TYPE            AS Type,
                        GOODFOR         AS GoodFor,
                        BADFOR          AS BadFor,
                        LANGUAGECODE    AS LanguageCode
                    FROM TITHI_DESC
                    ORDER BY ID";

                return _connection.Query<TithiDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetTithiDescs error: {ex.Message}");
                return new List<TithiDesc>();
            }
        }

        /// <summary>
        /// Return a list of Karana from KARANA table.
        /// </summary>
        public IReadOnlyList<Karana> GetKaranas()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        TITHIID         AS TithiId,
                        POSITION        AS Position,
                        COLORID         AS ColorId
                    FROM KARANA
                    ORDER BY ID";

                return _connection.Query<Karana>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetKaranas error: {ex.Message}");
                return new List<Karana>();
            }
        }

        /// <summary>
        /// Return a list of Karana Descriptions from TITHI_DESC table.
        /// </summary>
        public IReadOnlyList<KaranaDesc> GetKaranaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        KARANAID        AS KaranaId,
                        NAME            AS Name,
                        RULER           AS Ruler,
                        GOODFOR         AS GoodFor,
                        BADFOR          AS BadFor,
                        LANGUAGECODE    AS LanguageCode
                    FROM KARANA_DESC
                    ORDER BY ID";

                return _connection.Query<KaranaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetKaranaDescs error: {ex.Message}");
                return new List<KaranaDesc>();
            }
        }

        /// <summary>
        /// Return a list of Muhurta from MUHURTA table.
        /// </summary>
        public IReadOnlyList<Muhurta> GetMuhurtas()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        MUHURTACODE AS MuhurtaCode,
                        COLORID     AS ColorId
                    FROM MUHURTA
                    ORDER BY ID";

                return _connection.Query<Muhurta>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMuhurtas error: {ex.Message}");
                return new List<Muhurta>();
            }
        }

        /// <summary>
        /// Return a list of Muhurta Descriptions from MUHURTA_DESC table.
        /// </summary>
        public IReadOnlyList<MuhurtaDesc> GetMuhurtaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        MUHURTAID       AS MuhurtaId,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        LANGUAGECODE    AS LanguageCode
                    FROM MUHURTA_DESC
                    ORDER BY ID";

                return _connection.Query<MuhurtaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMuhurtaDescs error: {ex.Message}");
                return new List<MuhurtaDesc>();
            }
        }

        /// <summary>
        /// Return a list of Muhurta30 from MUHURTA30 table.
        /// </summary>
        public IReadOnlyList<Muhurta30> GetMuhurta30s()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        MUHURTA30CODE   AS Muhurta30Code,
                        COLORID         AS ColorId
                    FROM MUHURTA30
                    ORDER BY ID";

                return _connection.Query<Muhurta30>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMuhurta30s error: {ex.Message}");
                return new List<Muhurta30>();
            }
        }

        /// <summary>
        /// Return a list of Muhurta30 Descriptions from MUHURTA_DESC table.
        /// </summary>
        public IReadOnlyList<Muhurta30Desc> GetMuhurta30Descs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        MUHURTA30ID     AS Muhurta30Id,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        DESCRIPTION     AS Description,
                        LANGUAGECODE    AS LanguageCode
                    FROM MUHURTA30_DESC
                    ORDER BY ID";

                return _connection.Query<Muhurta30Desc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMuhurta30Descs error: {ex.Message}");
                return new List<Muhurta30Desc>();
            }
        }

        /// <summary>
        /// Return a list of Ghati60 from GHATI60 table.
        /// </summary>
        public IReadOnlyList<Ghati60> GetGhati60s()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        GHATI60CODE AS Ghati60Code,
                        POSITION    AS Position,
                        COLORID     AS ColorId
                    FROM GHATI60
                    ORDER BY ID";

                return _connection.Query<Ghati60>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetGhati60s error: {ex.Message}");
                return new List<Ghati60>();
            }
        }

        /// <summary>
        /// Return a list of Ghati60 Descriptions from GHATI60_DESC table.
        /// </summary>
        public IReadOnlyList<Ghati60Desc> GetGhati60Descs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        GHATI60ID       AS Ghati60Id,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        DESCRIPTION     AS Description,
                        LANGUAGECODE    AS LanguageCode
                    FROM GHATI60_DESC
                    ORDER BY ID";

                return _connection.Query<Ghati60Desc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetGhati60Descs error: {ex.Message}");
                return new List<Ghati60Desc>();
            }
        }

        /// <summary>
        /// Return a list of Masa from MASA table.
        /// </summary>
        public IReadOnlyList<Masa> GetMasas()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        ZODIACID        AS ZodiacId,
                        SHUNYANAKSHATRA AS ShunyaNakshatra,
                        SHUNYATITHI     AS ShunyaTithi
                    FROM MASA
                    ORDER BY ID";

                var masas = _connection.Query<Masa>(sql);
                foreach (var masa in masas)
                {
                    masa.ShunyaNakshatraIdArray = MakeIdsArrayFromString(masa.ShunyaNakshatra);
                    masa.ShunyaTithiIdArray = MakeIdsArrayFromString(masa.ShunyaTithi);
                }
                return masas;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMasas error: {ex.Message}");
                return new List<Masa>();
            }
        }

        private int[] MakeIdsArrayFromString(string str)
        {
            var row = str.Split(new char[] { ',' });
            int[] array = new int[row.Length];
            for (int i = 0; i < row.Length; i++)
            {
                array[i] = Convert.ToInt32(row[i]);
            }
            return array;
        }

        /// <summary>
        /// Return a list of Masa Descriptions from MASA_DESC table.
        /// </summary>
        public IReadOnlyList<MasaDesc> GetMasaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        MASAID          AS MasaId,
                        NAME            AS Name,
                        LANGUAGECODE    AS LanguageCode
                    FROM MASA_DESC
                    ORDER BY ID";

                return _connection.Query<MasaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetMasaDescs error: {ex.Message}");
                return new List<MasaDesc>();
            }
        }

        /// <summary>
        /// Return a list of Special Navamsa from SPECIALNAVAMSA_DESC table.
        /// </summary>
        public IReadOnlyList<SpecialNavamsaDesc> GetSpecialNavamsaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID                  AS Id,
                        SPECIALNAVAMSAID    AS SpecialNavamsaId,
                        NAME                AS Name,
                        LANGUAGECODE        AS LanguageCode
                    FROM SPECIALNAVAMSA_DESC
                    ORDER BY ID";

                return _connection.Query<SpecialNavamsaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetSpecialNavamsaDescs error: {ex.Message}");
                return new List<SpecialNavamsaDesc>();
            }
        }

        /// <summary>
        /// Return a list of Yogi from YOGA table.
        /// </summary>
        public IReadOnlyList<Yoga> GetYogas()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID          AS Id,
                        YOGACODE    AS YogaCode,
                        COLORID     AS ColorId
                    FROM YOGA
                    ORDER BY ID";

                return _connection.Query<Yoga>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetYogas error: {ex.Message}");
                return new List<Yoga>();
            }
        }

        /// <summary>
        /// Return a list of Yogi Descriptions from YOGA_DESC table.
        /// </summary>
        public IReadOnlyList<YogaDesc> GetYogaDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        YOGAID          AS YogaId,
                        NAME            AS Name,
                        SHORTNAME       AS ShortName,
                        DESCRIPTION     AS Description,
                        LANGUAGECODE    AS LanguageCode
                    FROM YOGA_DESC
                    ORDER BY ID";

                return _connection.Query<YogaDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetYogaDescs error: {ex.Message}");
                return new List<YogaDesc>();
            }
        }

        /// <summary>
        /// Return a list of DVlineNAmes Codes from DVLINENAME table.
        /// </summary>
        public IReadOnlyList<DVLineName> GetDVLineNames()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID   AS Id,
                        CODE AS Code
                    FROM DVLINENAME
                    ORDER BY ID";

                return _connection.Query<DVLineName>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetDVLineNames error: {ex.Message}");
                return new List<DVLineName>();
            }
        }

        /// <summary>
        /// Return a list of DVLineName Descriptions from DVLINENAME_DESC table.
        /// </summary>
        public IReadOnlyList<DVLineNameDesc> GetDVLineNameDescs()
        {
            try
            {
                const string sql = @"
                    SELECT 
                        ID              AS Id,
                        DVLINENAMEID    AS DVLineNameId,
                        SHORTNAME       AS ShortName,
                        NAME            AS Name,
                        LANGUAGECODE    AS LanguageCode
                    FROM DVLINENAME_DESC
                    ORDER BY ID";

                return _connection.Query<DVLineNameDesc>(sql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetDVLineNameDescs error: {ex.Message}");
                return new List<DVLineNameDesc>();
            }
        }




        #endregion


        #region User Events (Appointments)

        /// <summary>
        /// Returns all user events for a given profile that intersect the specified local day.
        /// dayLocal is interpreted in profile local time.
        /// </summary>
        public List<UserEvent> GetUserEventsForDay(int profileId, DateTime dayLocal)
        {
            try
            {
                var dayStart = new DateTime(dayLocal.Year, dayLocal.Month, dayLocal.Day, 0, 0, 0);
                var dayEnd = dayStart.AddDays(1);

                var dayStartStr = UserEventDateHelper.ToDbString(dayStart);
                var dayEndStr = UserEventDateHelper.ToDbString(dayEnd);

                const string sql = @"
                        SELECT
                            ID        as Id,
                            PROFILEID as ProfileId,
                            DATESTART as DATESTART,
                            DATEEND   as DATEEND,
                            NAME      as Name,
                            MESSAGE   as Message,
                            ARGBVALUE as ArgbValue
                        FROM USER_EVENTS
                        WHERE PROFILEID = ?
                          AND DATESTART < ?
                          AND DATEEND   > ?
                        ORDER BY DATESTART;";

                var rows = _connection.Query<UserEventRow>(sql, profileId, dayEndStr, dayStartStr);

                var result = new List<UserEvent>(rows.Count);
                foreach (var r in rows)
                {
                    var ev = new UserEvent
                    {
                        Id = r.Id,
                        ProfileId = r.ProfileId,
                        StartLocal = UserEventDateHelper.TryParse(r.DATESTART),
                        EndLocal = UserEventDateHelper.TryParse(r.DATEEND),
                        Name = r.Name ?? string.Empty,
                        Message = r.Message ?? string.Empty,
                        ArgbValue = r.ArgbValue
                    };
                    result.Add(ev);
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] DB GetUserEventsForDay error: {ex}");
                return new List<UserEvent>();
            }
        }

        public List<UserEvent> GetUserEventsForRange(int profileId, DateTime windowStartLocal, DateTime windowEndExclusiveLocal)
        {
            try
            {
                var startStr = UserEventDateHelper.ToDbString(windowStartLocal);
                var endStr = UserEventDateHelper.ToDbString(windowEndExclusiveLocal);

                const string sql = @"
                    SELECT
                        ID        as Id,
                        PROFILEID as ProfileId,
                        DATESTART as DATESTART,
                        DATEEND   as DATEEND,
                        NAME      as Name,
                        MESSAGE   as Message,
                        ARGBVALUE as ArgbValue
                    FROM USER_EVENTS
                    WHERE PROFILEID = ?
                      AND DATESTART < ?
                      AND DATEEND   > ?
                    ORDER BY DATESTART;";

                var rows = _connection.Query<UserEventRow>(sql, profileId, endStr, startStr);

                var result = new List<UserEvent>(rows.Count);
                foreach (var r in rows)
                {
                    result.Add(new UserEvent
                    {
                        Id = r.Id,
                        ProfileId = r.ProfileId,
                        StartLocal = UserEventDateHelper.TryParse(r.DATESTART),
                        EndLocal = UserEventDateHelper.TryParse(r.DATEEND),
                        Name = r.Name ?? string.Empty,
                        Message = r.Message ?? string.Empty,
                        ArgbValue = r.ArgbValue
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] DB GetUserEventsForRange error: {ex}");
                return new List<UserEvent>();
            }
        }

        private sealed class UserEventRow
        {
            public int Id { get; set; }
            public int ProfileId { get; set; }
            public string DATESTART { get; set; } = "";
            public string DATEEND { get; set; } = "";
            public string Name { get; set; } = "";
            public string Message { get; set; } = "";
            public int ArgbValue { get; set; }
        }

        public int InsertUserEvent(UserEvent ev)
        {
            try
            {
                const string sql = @"
                        INSERT INTO USER_EVENTS (PROFILEID, DATESTART, DATEEND, NAME, MESSAGE, ARGBVALUE)
                        VALUES (?, ?, ?, ?, ?, ?);";

                var startStr = UserEventDateHelper.ToDbString(ev.StartLocal);
                var endStr = UserEventDateHelper.ToDbString(ev.EndLocal);

                _connection.Execute(sql,
                    ev.ProfileId,
                    startStr,
                    endStr,
                    ev.Name ?? string.Empty,
                    ev.Message ?? string.Empty,
                    ev.ArgbValue);

                // получить последний ID
                var newId = _connection.ExecuteScalar<int>("SELECT last_insert_rowid();");
                ev.Id = newId;
                return newId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] DB InsertUserEvent error: {ex}");
                return 0;
            }
        }

        public bool UpdateUserEvent(UserEvent ev)
        {
            try
            {
                const string sql = @"
                        UPDATE USER_EVENTS
                           SET DATESTART = ?,
                               DATEEND   = ?,
                               NAME      = ?,
                               MESSAGE   = ?,
                               ARGBVALUE = ?
                         WHERE ID = ?;";

                _connection.Execute(sql,
                    UserEventDateHelper.ToDbString(ev.StartLocal),
                    UserEventDateHelper.ToDbString(ev.EndLocal),
                    ev.Name ?? string.Empty,
                    ev.Message ?? string.Empty,
                    ev.ArgbValue,
                    ev.Id);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] DB UpdateUserEvent error: {ex}");
                return false;
            }
        }

        public bool DeleteUserEvent(int id)
        {
            try
            {
                _connection.Execute("DELETE FROM USER_EVENTS WHERE ID = ?", id);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] DB DeleteUserEvent error: {ex.Message}");
                return false;
            }
        }



        #endregion



    }
}
