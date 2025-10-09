using PADMA.Core.Models;
using PADMA.Core.Services;

namespace PADMA.Core.Services
{
    /// <summary>
    /// In-memory cache for reference data. Loaded once on app startup.
    /// </summary>
    public sealed class DataCache
    {
        // Languages
        public IReadOnlyList<Language> Languages { get; private set; } = Array.Empty<Language>();

        // Colors
        public IReadOnlyList<ColorDef> Colors { get; private set; } = Array.Empty<ColorDef>();
        public IReadOnlyDictionary<int, ColorDef> ColorById { get; private set; } = new Dictionary<int, ColorDef>();
        public IReadOnlyDictionary<int, string> ColorNameById { get; private set; } = new Dictionary<int, string>();

        // Planets
        public IReadOnlyList<PlanetDef> Planets { get; private set; } = Array.Empty<PlanetDef>();
        public IReadOnlyDictionary<int, PlanetDef> PlanetById { get; private set; } = new Dictionary<int, PlanetDef>();
        public IReadOnlyDictionary<string, int> PlanetIdByCode { get; private set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyDictionary<int, string> PlanetNameById { get; private set; } = new Dictionary<int, string>();

        // Active language code for UI (e.g. "en", "ru", "pl")
        public string UiLanguageCode { get; private set; } = "en";

        private static DataCache? _instance;
        public static DataCache Instance => _instance ??= new DataCache();

        private DataCache() { }

        public List<AppText> AppTextsList { get; private set; }

        public void LoadAll(DatabaseService db, string preferredUiLang)
        {
            UiLanguageCode = preferredUiLang;

            // Languages
            var langs = db.GetLanguages();
            Languages = langs;

            // Colors
            var colors = db.GetColors();
            Colors = colors;
            ColorById = colors.ToDictionary(c => c.Id);
            var colorDescs = db.GetColorDescs();
            ColorNameById = colorDescs
                .Where(d => string.Equals(d.LanguageCode, UiLanguageCode, StringComparison.OrdinalIgnoreCase))
                .GroupBy(d => d.ColorId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            // Planets
            var planets = db.GetPlanets();
            Planets = planets;
            PlanetById = planets.ToDictionary(p => p.Id);
            PlanetIdByCode = planets
                .GroupBy(p => p.PlanetCode, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var planetDescs = db.GetPlanetDescs();
            PlanetNameById = planetDescs
                .Where(d => string.Equals(d.LanguageCode, UiLanguageCode, StringComparison.OrdinalIgnoreCase))
                .GroupBy(d => d.PlanetId)
                .ToDictionary(g => g.Key, g => g.First().Name);

            AppTextsList = db.GetAppTextsList(preferredUiLang);
            

        }
    }
}
