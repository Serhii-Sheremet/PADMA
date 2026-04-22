using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Globalization;
using PADMA.Core.Utilities;

namespace PADMA.Pages;

public partial class CountrySelectPopup : Popup
{
    public record CountryItem(string Code, string EnglishName, string NativeName)
    {
        public string Display =>
            string.IsNullOrWhiteSpace(NativeName) || NativeName.Equals(EnglishName, StringComparison.OrdinalIgnoreCase)
                ? EnglishName
                : $"{EnglishName} ({NativeName})";
    }

    private readonly List<CountryItem> _all;
    private readonly ObservableCollection<CountryItem> _items = new();
    public CountryItem? SelectedCountry { get; private set; }

    public CountrySelectPopup(string lang)
    {
        InitializeComponent();

        // Можно локализовать заголовок через твой Localization
        lblTitle.Text = Localization.GetLocalizedText("Country", lang);
        sb.Placeholder = Localization.GetLocalizedText("Search...", lang);

        _all = BuildCountries();
        foreach (var c in _all) _items.Add(c);
        cv.ItemsSource = _items;
    }

    private static List<CountryItem> BuildCountries()
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        var byCode = new Dictionary<string, List<CultureInfo>>(StringComparer.OrdinalIgnoreCase);

        foreach (var culture in cultures)
        {
            try
            {
                var region = new RegionInfo(culture.Name);
                var code = (region.TwoLetterISORegionName ?? "").Trim().ToLowerInvariant();

                if (code.Length != 2 || !code.All(char.IsLetter))
                    continue;

                if (!byCode.TryGetValue(code, out var list))
                {
                    list = new List<CultureInfo>();
                    byCode[code] = list;
                }

                list.Add(culture);
            }
            catch
            {
                // ignore invalid cultures
            }
        }

        var result = new List<CountryItem>();

        foreach (var pair in byCode)
        {
            var code = pair.Key;
            var culturesForCountry = pair.Value;

            var preferredCulture = SelectPreferredCulture(code, culturesForCountry);
            if (preferredCulture == null)
                continue;

            try
            {
                var region = new RegionInfo(preferredCulture.Name);
                var englishName = (region.EnglishName ?? "").Trim();

                if (string.IsNullOrWhiteSpace(englishName) || englishName.StartsWith("["))
                    continue;

                var nativeName = GetPreferredNativeCountryName(code, englishName, preferredCulture);

                result.Add(new CountryItem(code, englishName, nativeName));
            }
            catch
            {
                // ignore
            }
        }

        return result
            .OrderBy(x => x.EnglishName)
            .ToList();
    }

    private static CultureInfo? SelectPreferredCulture(string code, List<CultureInfo> cultures)
    {
        if (cultures == null || cultures.Count == 0)
            return null;

        // Явные предпочтения для стран, где особенно важно стабильное отображение
        var preferredCultureByCountry = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ua"] = "uk-UA",
            ["pl"] = "pl-PL",
            ["de"] = "de-DE",
            ["fr"] = "fr-FR",
            ["es"] = "es-ES",
            ["it"] = "it-IT",
            ["pt"] = "pt-PT",
            ["br"] = "pt-BR",
            ["ru"] = "ru-RU",
            ["by"] = "be-BY",
            ["kz"] = "kk-KZ",
            ["uz"] = "uz-UZ",
            ["gb"] = "en-GB",
            ["us"] = "en-US",
            ["ca"] = "en-CA",
            ["au"] = "en-AU"
        };

        if (preferredCultureByCountry.TryGetValue(code, out var preferredName))
        {
            var exact = cultures.FirstOrDefault(c =>
                c.Name.Equals(preferredName, StringComparison.OrdinalIgnoreCase));

            if (exact != null)
                return exact;
        }

        // Если есть culture, где language == country code, это часто лучший выбор:
        // uk-UA, pl-PL, de-DE, fr-FR и т.д.
        var sameLanguageAndCountry = cultures.FirstOrDefault(c =>
        {
            var parts = c.Name.Split('-');
            return parts.Length == 2 &&
                   parts[0].Equals(code, StringComparison.OrdinalIgnoreCase) &&
                   parts[1].Equals(code, StringComparison.OrdinalIgnoreCase);
        });

        if (sameLanguageAndCountry != null)
            return sameLanguageAndCountry;

        // Иначе предпочитаем culture, у которой регион совпадает с нужной страной
        var exactRegion = cultures.FirstOrDefault(c =>
        {
            try
            {
                var r = new RegionInfo(c.Name);
                return r.TwoLetterISORegionName.Equals(code, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        });

        return exactRegion ?? cultures.FirstOrDefault();
    }

    private static string GetPreferredNativeCountryName(string code, string englishName, CultureInfo culture)
    {
        var manualNativeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ua"] = "Україна",
            ["pl"] = "Polska",
            ["de"] = "Deutschland",
            ["fr"] = "France",
            ["es"] = "España",
            ["it"] = "Italia",
            ["pt"] = "Portugal",
            ["br"] = "Brasil",
            ["ru"] = "Россия",
            ["by"] = "Беларусь",
            ["kz"] = "Қазақстан",
            ["uz"] = "Oʻzbekiston",
            ["gb"] = "United Kingdom",
            ["us"] = "United States"
        };

        if (manualNativeNames.TryGetValue(code, out var manual))
            return manual;

        try
        {
            var region = new RegionInfo(culture.Name);
            var native = (region.NativeName ?? "").Trim();

            if (IsAcceptableNativeName(native, englishName))
                return native;
        }
        catch
        {
            // ignore
        }

        return englishName;
    }

    private static bool IsAcceptableNativeName(string nativeName, string englishName)
    {
        if (string.IsNullOrWhiteSpace(nativeName))
            return false;

        if (nativeName.StartsWith("["))
            return false;

        if (nativeName.Length < 2)
            return false;

        // Если совпадает с английским названием - это тоже нормально,
        // но тогда Display просто покажет EnglishName без скобок.
        return true;
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        var q = (e.NewTextValue ?? "").Trim().ToLowerInvariant();
        _items.Clear();

        foreach (var c in _all)
        {
            if (q.Length == 0 ||
                c.EnglishName.ToLowerInvariant().Contains(q) ||
                c.NativeName.ToLowerInvariant().Contains(q) ||
                c.Code.Contains(q))
            {
                _items.Add(c);
            }
        }
    }

    private async void OnSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection?.FirstOrDefault() is CountryItem item)
        {
            SelectedCountry = item;
            await base.CloseAsync(); // без параметров
        }
    }


}