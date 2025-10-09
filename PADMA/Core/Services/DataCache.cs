using PADMA.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Centralized in-memory cache for reference data and app settings.
    /// </summary>
    public sealed class DataCache
    {
        private static DataCache? _instance;
        public static DataCache Instance => _instance ??= new DataCache();

        private DataCache() { }

        // --- Core Cached Data ---
        public IReadOnlyList<Language> Languages { get; private set; } = Array.Empty<Language>();
        public IReadOnlyList<ColorDef> Colors { get; private set; } = Array.Empty<ColorDef>();
        public IReadOnlyList<PlanetDef> Planets { get; private set; } = Array.Empty<PlanetDef>();

        public IReadOnlyDictionary<int, string> ColorNameById { get; private set; } = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> PlanetNameById { get; private set; } = new Dictionary<int, string>();

        // --- App Settings and Texts ---
        public List<AppSettingList> AppSettingsList { get; private set; } = new();
        public List<AppText> AppTextsList { get; private set; } = new();

        // --- Current Language ---
        public string CurrentLanguageCode { get; private set; } = "en";

        /// <summary>
        /// Load all static and localized reference data from the database.
        /// </summary>
        public void LoadAll(DatabaseService db, string? preferredUiLang = null)
        {
            // Определяем язык интерфейса
            CurrentLanguageCode = preferredUiLang ?? db.GetActiveLanguageCode();

            // Языки
            Languages = db.GetLanguages();

            // Цвета
            Colors = db.GetColors();
            var colorDescs = db.GetColorDescs();
            ColorNameById = colorDescs
                .Where(d => string.Equals(d.LanguageCode, CurrentLanguageCode, StringComparison.OrdinalIgnoreCase))
                .GroupBy(d => d.ColorId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            // Планеты
            Planets = db.GetPlanets();
            var planetDescs = db.GetPlanetDescs();
            PlanetNameById = planetDescs
                .Where(d => string.Equals(d.LanguageCode, CurrentLanguageCode, StringComparison.OrdinalIgnoreCase))
                .GroupBy(d => d.PlanetId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            // Настройки приложения (APPSETTING)
            AppSettingsList = db.GetAppSettingsList();

            // Тексты интерфейса (APP_TEXTS)
            AppTextsList = db.GetAppTextsList(CurrentLanguageCode);
        }

        /// <summary>
        /// Refresh app settings and localized texts (used after configuration changes).
        /// </summary>
        public void Refresh(DatabaseService db)
        {
            AppSettingsList = db.GetAppSettingsList();
            CurrentLanguageCode = db.GetActiveLanguageCode();
            AppTextsList = db.GetAppTextsList(CurrentLanguageCode);
        }

        /// <summary>
        /// Get localized text safely from cache.
        /// </summary>
        public string GetText(string nativeText)
        {
            return AppTextsList.FirstOrDefault(x => x.NativeText == nativeText)?.ForeignText ?? nativeText;
        }
    }
}
