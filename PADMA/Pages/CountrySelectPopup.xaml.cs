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
        var list = new List<CountryItem>();
        var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var c in cultures)
        {
            try
            {
                var r = new RegionInfo(c.Name);

                var code = (r.TwoLetterISORegionName ?? "").Trim().ToLowerInvariant();
                if (code.Length != 2 || !code.All(char.IsLetter))
                    continue;

                if (!seen.Add(code))
                    continue;

                var en = (r.EnglishName ?? "").Trim();
                var native = (r.NativeName ?? "").Trim();

                if (en.Length == 0 || en.StartsWith("["))
                    continue;

                list.Add(new CountryItem(code, en, native));
            }
            catch
            {
                // ignore
            }
        }

        return list
            .OrderBy(x => x.EnglishName)
            .ToList();
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