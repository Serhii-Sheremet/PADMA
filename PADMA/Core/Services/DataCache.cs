using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Services;
using System.Globalization;
using System.Resources;
using Syncfusion.Maui.Inputs;

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

        public IReadOnlyList<SpecialNavamsaDesc> SpecialNavamsaDescList { get; private set; } = new List<SpecialNavamsaDesc>();

        public IReadOnlyList<Yoga> YogaList { get; private set; } = new List<Yoga>();
        public IReadOnlyList<YogaDesc> YogaDescList { get; private set; } = new List<YogaDesc>();

        public IReadOnlyList<DVLineName> DVLineNameList { get; private set; } = new List<DVLineName>();
        public IReadOnlyList<DVLineNameDesc> DVLineNameDescList { get; private set; } = new List<DVLineNameDesc>();


        public IReadOnlyList<Profile> ProfileList { get; private set; } = new List<Profile>();
        public Profile? ActiveProfile { get; set; }
        public IProfileContextService ProfileContextService { get; } = new ProfileContextService();

        public IReadOnlyList<AppLocation> LocationList { get; private set; } = new List<AppLocation>();



        // --- App Settings and Texts ---
        public List<AppText> AppTextsList { get; private set; } = new();
        public List<AppSettingList> AppSettingsList { get; private set; } = new();
        public string CurrentLanguageCode { get; private set; } = "en";
        public DayOfWeek DayOfWeek { get; private set; } = DayOfWeek.Monday;

        // --- User Events Cache ---
        public UserEventsWindowCache UserEventsWindowCache { get; private set; }


        /// <summary>
        /// Load all static and localized reference data from the database.
        /// </summary>
        public void LoadAll(DatabaseService db, string? preferredUiLang = null)
        {
            // all supported languages
            LanguageList = db.GetLanguages();

            // Interface language
            CurrentLanguageCode = preferredUiLang ?? db.GetActiveLanguageCode();

            // Interface texts (APP_TEXTS)
            AppTextsList = db.GetAppTextsList(CurrentLanguageCode);

            // Appllication settings (APPSETTING)
            AppSettingsList = db.GetAppSettingsList();

            // First day of week setting
            DayOfWeek = db.GetFirstDayOfWeekFromDb();

            // Colors
            ColorList = db.GetColors();
            ColorDescList = db.GetColorDescs().ToList();

            // Planets
            PlanetList = db.GetPlanets();
            PlanetDescList = db.GetPlanetDescs().ToList();

            // Transits
            TransitList = db.GetTransits();
            TransitDescList = db.GetTransitDescs().ToList();

            // Zodiacs
            ZodiacList = db.GetZodiacs().ToList();
            ZodiacDescList = db.GetZodiacDescs().ToList();

            // Padas 
            PadaList = db.GetPadas().ToList();

            // Mrityu Bhaga (dead degrees)
            MrityuBhagaList = db.GetMrityuBhaga().ToList();

            // Nitya Yogas
            NityaYogaList = db.GetNityaYogas().ToList();
            NityaYogaDescList = db.GetNityaYogaDescs().ToList();

            // Eclipeses
            EclipseList = db.GetEclipses().ToList();
            EclipseDescList = db.GetEclipseDescs().ToList();

            // Nakshatras
            NakshatraList = db.GetNakshatras().ToList();
            NakshatraDescList = db.GetNakshatraDescs().ToList();

            // Tara Balas
            TaraBalaList = db.GetTaraBalas().ToList();
            TaraBalaDescList = db.GetTaraBalaDescs().ToList();

            // tithis
            TithiList = db.GetTithis().ToList();
            TithiDescList = db.GetTithiDescs().ToList();

            // Karanas
            KaranaList = db.GetKaranas().ToList();
            KaranaDescList = db.GetKaranaDescs().ToList();

            // Muhurtas
            MuhurtaList = db.GetMuhurtas().ToList();
            MuhurtaDescList = db.GetMuhurtaDescs().ToList();
            Muhurta30List = db.GetMuhurta30s().ToList();
            Muhurta30DescList = db.GetMuhurta30Descs().ToList();
            Ghati60List = db.GetGhati60s().ToList();
            Ghati60DescList = db.GetGhati60Descs().ToList();

            // Masas
            MasaList = db.GetMasas().ToList();
            MasaDescList = db.GetMasaDescs().ToList();

            // Descriptions of special navamsas
            SpecialNavamsaDescList = db.GetSpecialNavamsaDescs().ToList();

            // Yogas
            YogaList = db.GetYogas().ToList();
            YogaDescList = db.GetYogaDescs().ToList();

            // DV Line Names
            DVLineNameList = db.GetDVLineNames().ToList();
            DVLineNameDescList = db.GetDVLineNameDescs().ToList();

            // Profiles
            ProfileList = db.GetProfiles().ToList();
            ActiveProfile = GetActiveProfile();

            // Locations
            LocationList = db.GetLocations().ToList();

            // User events cache
            UserEventsWindowCache = new UserEventsWindowCache(db);

        }

        public void ApplyLocalization()
        {
            // получаем culture
            string cultureCode = GetCurrentCultureCode(CurrentLanguageCode);

            var ci = new CultureInfo(cultureCode);

            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
            CultureInfo.CurrentCulture = ci;

            // подключаем ресурсы Syncfusion ColorPicker
            // подключаем ресурсы (ВАЖНО: base name должен быть точный)
            //var rm = new ResourceManager("PADMA.Resources.SfColorPicker", typeof(App).Assembly);
            //SfColorPickerResources.ResourceManager = rm;
        }

        public string GetCurrentCultureCode(string langCode)
        {
            return Instance.LanguageList.FirstOrDefault(l => l.LanguageCode == langCode)?.CultureCode ?? "en-US";
        }

        /// <summary>
        /// Refresh app settings and localized texts (used after configuration changes).
        /// </summary>
        public void Refresh(DatabaseService db)
        {
            AppSettingsList = db.GetAppSettingsList();
            CurrentLanguageCode = db.GetActiveLanguageCode();
            AppTextsList = db.GetAppTextsList(CurrentLanguageCode);
            ColorList = db.GetColors();
        }

        public Color GetColor(EColor color)
        {
            var id = (int)color;
            var c = ColorList.FirstOrDefault(x => x.Id == id);
            if (c != null)
            {
                // We assume that c.ARGBVALUE is an int in the 0xAARRGGBB format
                return Color.FromUint(unchecked((uint)c.ArgbValue));
            }
            return Colors.Black; // fallback, if the color is not found
        }

        public void ReloadProfiles(DatabaseService db, bool setActiveToDefault = true)
        {
            ProfileList = db.GetProfiles().ToList();
            if (setActiveToDefault)
                ActiveProfile = GetActiveProfile();
        }

        public Profile? GetActiveProfile()
        {
            return ProfileList.FirstOrDefault(p => p.Checked) ?? null;
        }

        public IReadOnlyList<Profile> GetProfiles(DatabaseService db)
        {
            return db.GetProfiles().ToList();
        }

        public event Action? ProfileContextUpdated;
        public async Task RebuildProfileContextAsync()
        {
            await ProfileContextService.RebuildAsync();
            ProfileContextUpdated?.Invoke();
        }

        public AppLocation? GetLocationById(int id)
        {
            return LocationList.FirstOrDefault(l => l.Id == id);
        }

        public IReadOnlyList<AppLocation> ReloadLocations(DatabaseService db)
        {
            LocationList = db.GetLocations().ToList();
            return LocationList;
        }

        public EAppSetting GetActiveTransitSettings()
        {
            var transitSetting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "TRANSIT" && s.Active == 1);
            return (EAppSetting)(transitSetting?.Id ?? (int)EAppSetting.TRANZITMOON);
        }

        public EAppSetting GetActiveHoraSettings()
        {
            var horaSetting = Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "HORA" && s.Active == 1);
            return (EAppSetting)(horaSetting?.Id ?? (int)EAppSetting.HORAEQUAL);
        }

        public EAppSetting GetActiveMuhurtaGhatSettings()
        {
            var muhurtaghatSetting = Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "MUHURTAGHATI" && s.Active == 1);
            return (EAppSetting)(muhurtaghatSetting?.Id ?? (int)EAppSetting.MUHURTAGHATIEQUAL);
        }

        public EAppSetting GetActiveMrityuBhagaSettings()
        {
            var mrityubhagaSetting = Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "MRITYUBHAGA" && s.Active == 1);
            return (EAppSetting)(mrityubhagaSetting?.Id ?? (int)EAppSetting.MRITYUBHAGAERNST);
        }

        public EAppSetting GetActiveNodeSetting()
        {
            var nodeSetting = Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "NODE" && s.Active == 1);
            return (EAppSetting)(nodeSetting?.Id ?? (int)EAppSetting.NODEMEAN);
        }

        public EAppSetting GetActiveWeekStartSetting()
        {
            var weekStartSetting = Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "WEEK" && s.Active == 1);
            return (EAppSetting)(weekStartSetting?.Id ?? (int)EAppSetting.WEEKSUNDAY);
        }

        public EAppSetting GetActiveSunriseSetting()
        {
            var sunriseTipSetting = Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "SUNRISE" && s.Active == 1);
            return (EAppSetting)(sunriseTipSetting?.Id ?? (int)EAppSetting.SUNRISETIP);
        }



    }
}
