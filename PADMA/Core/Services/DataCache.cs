using PADMA.Core.Enums;
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
        public IReadOnlyList<Language> LanguageList { get; private set; } = new List<Language>();
        public IReadOnlyList<AppColor> ColorList { get; private set; } = new List<AppColor>();
        public IReadOnlyList<Planet> PlanetList { get; private set; } = new List<Planet>();
        public IReadOnlyList<Pada> PadaList { get; private set; } = new List<Pada>();
        public IReadOnlyList<MrityuBhaga> MrityuBhagaList { get; private set; } = new List<MrityuBhaga>();
        public IReadOnlyList<NityaYoga> NityaYogaList { get; private set; } = new List<NityaYoga>();
        public IReadOnlyList<Eclipse> EclipseList { get; private set; } = new List<Eclipse>();
        public IReadOnlyList<Nakshatra> NakshatraList { get; private set; } = new List<Nakshatra>();
        public IReadOnlyList<TaraBala> TaraBalaList { get; private set; } = new List<TaraBala>();
        public IReadOnlyList<Tithi> TithiList { get; private set; } = new List<Tithi>();
        public IReadOnlyList<Karana> KaranaList { get; private set; } = new List<Karana>();
        public IReadOnlyList<Muhurta> MuhurtaList { get; private set; } = new List<Muhurta>();
        public IReadOnlyList<Muhurta30> Muhurta30List { get; private set; } = new List<Muhurta30>();
        public IReadOnlyList<Ghati60> Ghati60List { get; private set; } = new List<Ghati60>();
        




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
            LanguageList = db.GetLanguages();

            // Настройки приложения (APPSETTING)
            AppSettingsList = db.GetAppSettingsList();

            // Тексты интерфейса (APP_TEXTS)
            AppTextsList = db.GetAppTextsList(CurrentLanguageCode);

            // Цвета
            ColorList = db.GetColors();

            // Планеты
            PlanetList = db.GetPlanets();

            // Пады 
            PadaList = db.GetPadas().ToList();

            // Mrityu Bhaga (мёртвые градусы)
            MrityuBhagaList = db.GetMrityuBhaga().ToList();

            // Nitya Yogas
            NityaYogaList = db.GetNityaYogas().ToList();
            
            // Затмения
            EclipseList = db.GetEclipses().ToList();

            // Накшатры
            NakshatraList = db.GetNakshatras().ToList();

            // Тара Бала
            TaraBalaList = db.GetTaraBalas().ToList();
            
            // Титхи
            TithiList = db.GetTithis().ToList();
            
            // Караны
            KaranaList = db.GetKaranas().ToList();
            
            // Мухурты
            MuhurtaList = db.GetMuhurtas().ToList();
            Muhurta30List = db.GetMuhurta30s().ToList();
            Ghati60List = db.GetGhati60s().ToList();





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

        
    }
}
