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
        public IReadOnlyList<AppColorDesc> ColorDescList { get; private set; } = new List<AppColorDesc>();
        
        public IReadOnlyList<Planet> PlanetList { get; private set; } = new List<Planet>();
        public IReadOnlyList<PlanetDesc> PlanetDescList { get; private set; } = new List<PlanetDesc>();
        
        public IReadOnlyList<Transit> TransitList { get; private set; } = new List<Transit>();
        public IReadOnlyList<TransitDesc> TransitDescList { get; private set; } = new List<TransitDesc>();
        
        public IReadOnlyList<Zodiac> ZodiacList { get; private set; } = new List<Zodiac>();
        public IReadOnlyList<ZodiacDesc> ZodiacDescList { get; private set; } = new List<ZodiacDesc>();
        
        public IReadOnlyList<Pada> PadaList { get; private set; } = new List<Pada>();
        public IReadOnlyList<MrityuBhaga> MrityuBhagaList { get; private set; } = new List<MrityuBhaga>();
        
        public IReadOnlyList<NityaYoga> NityaYogaList { get; private set; } = new List<NityaYoga>();
        public IReadOnlyList<NityaYogaDesc> NityaYogaDescList { get; private set; } = new List<NityaYogaDesc>();

        public IReadOnlyList<Eclipse> EclipseList { get; private set; } = new List<Eclipse>();
        public IReadOnlyList<EclipseDesc> EclipseDescList { get; private set; } = new List<EclipseDesc>();

        public IReadOnlyList<Nakshatra> NakshatraList { get; private set; } = new List<Nakshatra>();
        public IReadOnlyList<NakshatraDesc> NakshatraDescList { get; private set; } = new List<NakshatraDesc>();

        public IReadOnlyList<TaraBala> TaraBalaList { get; private set; } = new List<TaraBala>();
        public IReadOnlyList<TaraBalaDesc> TaraBalaDescList { get; private set; } = new List<TaraBalaDesc>();

        public IReadOnlyList<Tithi> TithiList { get; private set; } = new List<Tithi>();
        public IReadOnlyList<TithiDesc> TithiDescList { get; private set; } = new List<TithiDesc>();

        public IReadOnlyList<Karana> KaranaList { get; private set; } = new List<Karana>();
        public IReadOnlyList<KaranaDesc> KaranaDescList { get; private set; } = new List<KaranaDesc>();

        public IReadOnlyList<Muhurta> MuhurtaList { get; private set; } = new List<Muhurta>();
        public IReadOnlyList<MuhurtaDesc> MuhurtaDescList { get; private set; } = new List<MuhurtaDesc>();

        public IReadOnlyList<Muhurta30> Muhurta30List { get; private set; } = new List<Muhurta30>();
        public IReadOnlyList<Muhurta30Desc> Muhurta30DescList { get; private set; } = new List<Muhurta30Desc>();

        public IReadOnlyList<Ghati60> Ghati60List { get; private set; } = new List<Ghati60>();
        public IReadOnlyList<Ghati60Desc> Ghati60DescList { get; private set; } = new List<Ghati60Desc>();

        public IReadOnlyList<Masa> MasaList { get; private set; } = new List<Masa>();
        public IReadOnlyList<MasaDesc> MasaDescList { get; private set; } = new List<MasaDesc>();

        public IReadOnlyList<SpecialNavamsaDesc> SpecNavamsaDescList { get; private set; } = new List<SpecialNavamsaDesc>();
        
        public IReadOnlyList<Yoga> YogaList { get; private set; } = new List<Yoga>();
        public IReadOnlyList<YogaDesc> YogaDescList { get; private set; } = new List<YogaDesc>();




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
            ColorDescList = db.GetColorDescs().ToList();

            // Планеты
            PlanetList = db.GetPlanets();
            PlanetDescList = db.GetPlanetDescs().ToList();

            // Транзиты
            TransitList = db.GetTransits();
            TransitDescList = db.GetTransitDescs().ToList();

            // Зодиаки
            ZodiacList = db.GetZodiacs().ToList();
            ZodiacDescList = db.GetZodiacDescs().ToList();

            // Пады 
            PadaList = db.GetPadas().ToList();

            // Mrityu Bhaga (мёртвые градусы)
            MrityuBhagaList = db.GetMrityuBhaga().ToList();

            // Nitya Yogas
            NityaYogaList = db.GetNityaYogas().ToList();
            NityaYogaDescList = db.GetNityaYogaDescs().ToList();

            // Затмения
            EclipseList = db.GetEclipses().ToList();
            EclipseDescList = db.GetEclipseDescs().ToList();

            // Накшатры
            NakshatraList = db.GetNakshatras().ToList();
            NakshatraDescList = db.GetNakshatraDescs().ToList();

            // Тара Бала
            TaraBalaList = db.GetTaraBalas().ToList();
            TaraBalaDescList = db.GetTaraBalaDescs().ToList();

            // Титхи
            TithiList = db.GetTithis().ToList();
            TithiDescList = db.GetTithiDescs().ToList();

            // Караны
            KaranaList = db.GetKaranas().ToList();
            KaranaDescList = db.GetKaranaDescs().ToList();

            // Мухурты
            MuhurtaList = db.GetMuhurtas().ToList();
            MuhurtaDescList = db.GetMuhurtaDescs().ToList();
            Muhurta30List = db.GetMuhurta30s().ToList();
            Muhurta30DescList = db.GetMuhurta30Descs().ToList();
            Ghati60List = db.GetGhati60s().ToList();
            Ghati60DescList = db.GetGhati60Descs().ToList();

            // Массы
            MasaList = db.GetMasas().ToList();
            MasaDescList = db.GetMasaDescs().ToList();

            // Описания специальных навамш
            SpecNavamsaDescList = db.GetSpecialNavamsaDescs().ToList();
            
            // Йоги
            YogaList = db.GetYogas().ToList();
            YogaDescList = db.GetYogaDescs().ToList();





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
