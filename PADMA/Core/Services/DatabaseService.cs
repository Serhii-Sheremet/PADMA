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
    }
}
