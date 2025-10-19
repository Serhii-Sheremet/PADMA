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


        #region Reference Data (Colors, Planets)

        public IReadOnlyList<ColorDef> GetColors() =>
            _connection.Query<ColorDef>("SELECT ID as Id, CODE as Code, ARGBVALUE as ArgbValue FROM COLOR");

        public IReadOnlyList<ColorDesc> GetColorDescs() =>
            _connection.Query<ColorDesc>("SELECT ID as Id, COLORID as ColorId, NAME as Name, LANGUAGECODE as LanguageCode FROM COLOR_DESC");

        public IReadOnlyList<PlanetDef> GetPlanets() =>
            _connection.Query<PlanetDef>("SELECT ID as Id, PLANETCODE as PlanetCode FROM PLANET");

        public IReadOnlyList<PlanetDesc> GetPlanetDescs() =>
            _connection.Query<PlanetDesc>("SELECT ID as Id, PLANETID as PlanetId, NAME as Name, LANGUAGECODE as LanguageCode FROM PLANET_DESC");

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
        /// Добавляет новую локацию в таблицу LOCATION, если её ещё нет.
        /// Возвращает ID добавленной или уже существующей записи.
        /// </summary>
        public int AddLocationAndReturnId(AppLocation loc)
        {
            try
            {
                var existing = FindLocationByLocality(loc.Locality);
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


        /// <summary>
        /// Ищет локации в таблице LOCATION по частичному совпадению имени (Locality).
        /// Возвращает список найденных записей.
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
                WHERE LOWER(LOCALITY) LIKE '%' || LOWER(?) || '%'
                ORDER BY LOCALITY ASC";
                return _connection.Query<AppLocation>(sql, query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] SearchLocationByName error: {ex.Message}");
                return new List<AppLocation>();
            }
        }

        /// <summary>
        /// Проверяет, существует ли локация с данным Locality.
        /// Возвращает найденную запись или null.
        /// </summary>
        public AppLocation? FindLocationByLocality(string locality)
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
                WHERE LOWER(LOCALITY) = LOWER(?)
                LIMIT 1";
                return _connection.FindWithQuery<AppLocation>(sql, locality);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] FindLocationByLocality error: {ex.Message}");
                return null;
            }
        }


        #endregion


    }
}
