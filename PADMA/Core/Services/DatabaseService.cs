using SQLite;
using PADMA.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

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
                System.Diagnostics.Debug.WriteLine($"[PADMA] GetProfiles error: {ex.Message}");
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
        /// Searches for locations by name (LOCALITY, REGION, STATE, COUNTRY).
        /// </summary>
        public List<Location> SearchLocations(string text)
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
                                     FROM LOCATION
                                     WHERE LOCALITY LIKE ? OR REGION LIKE ? OR STATE LIKE ? OR COUNTRY LIKE ?
                                     ORDER BY LOCALITY COLLATE NOCASE";

                var pattern = $"%{text}%";
                return _connection.Query<Location>(sql, pattern, pattern, pattern, pattern);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] SearchLocations error: {ex.Message}");
                return new List<Location>();
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

        /// <summary>
        /// Adds a new location (used after Nominatim search).
        /// </summary>
        public void AddLocation(AppLocation location)
        {
            try
            {
                const string sql = @"INSERT INTO LOCATION 
                                     (LOCALITY, LATITUDE, LONGITUDE, REGION, STATE, COUNTRY, COUNTRYCODE, LANGUAGECODE)
                                     VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                _connection.Execute(sql,
                    location.Locality,
                    location.Latitude,
                    location.Longitude,
                    location.Region,
                    location.State,
                    location.Country,
                    location.CountryCode,
                    location.LanguageCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] AddLocation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Поиск локаций в таблице LOCATION по частичному совпадению имени.
        /// </summary>
        public List<AppLocation> SearchLocationByName(string query)
        {
            try
            {
                const string sql = @"
            SELECT 
                ID as Id,
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
                LOWER(LOCALITY) LIKE '%' || LOWER(?) || '%'
                OR LOWER(REGION) LIKE '%' || LOWER(?) || '%'
                OR LOWER(COUNTRY) LIKE '%' || LOWER(?) || '%'
            ORDER BY LOCALITY ASC";

                return _connection.Query<AppLocation>(sql, query, query, query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] SearchLocationByName error: {ex.Message}");
                return new List<AppLocation>();
            }
        }

        /// <summary>
        /// Ищет локацию в БД по Locality, Region и Country (без учёта регистра).
        /// Если совпадение найдено — возвращает существующую запись.
        /// </summary>
        public AppLocation? FindLocationByLocality(string locality, string? region = null, string? country = null)
        {
            try
            {
                const string sql = @"
            SELECT 
                ID as Id,
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
                LOWER(LOCALITY) = LOWER(?)
                AND (LOWER(REGION) = LOWER(?) OR (REGION IS NULL AND ? IS NULL))
                AND (LOWER(COUNTRY) = LOWER(?) OR (COUNTRY IS NULL AND ? IS NULL))
            LIMIT 1";

                return _connection.FindWithQuery<AppLocation>(sql, locality, region, region, country, country);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] FindLocationByLocality error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Добавляет новую локацию в LOCATION, если такой ещё нет.
        /// Возвращает ID добавленной или уже существующей записи.
        /// </summary>
        public int AddLocationAndReturnId(AppLocation loc)
        {
            try
            {
                var existing = FindLocationByLocality(loc.Locality, loc.Region, loc.Country);
                if (existing != null)
                    return existing.Id;

                const string sql = @"
            INSERT INTO LOCATION
                (LOCALITY, REGION, STATE, COUNTRY, COUNTRYCODE, LANGUAGECODE, LATITUDE, LONGITUDE)
            VALUES
                (?, ?, ?, ?, ?, ?, ?, ?)";

                _connection.Execute(sql,
                    loc.Locality,
                    loc.Region,
                    loc.State,
                    loc.Country,
                    loc.CountryCode,
                    loc.LanguageCode,
                    loc.Latitude,
                    loc.Longitude);

                return _connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] AddLocationAndReturnId error: {ex.Message}");
                return 0;
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






        #endregion




    }
}
