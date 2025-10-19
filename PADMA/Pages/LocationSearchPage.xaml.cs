using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System.Collections.ObjectModel;

namespace PADMA.Pages;

[QueryProperty(nameof(Mode), nameof(Mode))]
public partial class LocationSearchPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly NominatimService _nominatim;
    private readonly ObservableCollection<AppLocation> _results = new();

    private AppLocation? _selected;
    public string Mode { get; set; } = ""; // "birth" | "living"

    public LocationSearchPage(DatabaseService database, NominatimService nominatim)
    {
        InitializeComponent();
        _database = database;
        _nominatim = nominatim;

        listResults.ItemsSource = _results;
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        string lang = _database.GetActiveLanguageCode();
        Title = Localization.GetLocalizedText("Location search", lang);
        entrySearch.Placeholder = Localization.GetLocalizedText("Enter city name", lang);
        btnSelect.Text = Localization.GetLocalizedText("Select", lang);
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        _results.Clear();
        btnSelect.IsEnabled = false;
        _selected = null;

        var query = entrySearch.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(query))
            return;

        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;

        var lang = _database.GetActiveLanguageCode();

        // Сначала локальная БД
        var localResults = _database.SearchLocationByName(query);
        if (localResults.Count > 0)
        {
            foreach (var loc in localResults)
                _results.Add(loc);

            // Автоматически выделим первый результат
            listResults.SelectedItem = _results.First();
            _selected = _results.First();
            btnSelect.IsEnabled = true;

            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
            return;
        }

        // Если локально нет — идём в Nominatim
        var found = await _nominatim.SearchAsync(query, lang);

        // Дедупликация (по Locality + Country), и сразу формат в модель уже есть
        var unique = found
            .GroupBy(x => (x.Locality?.Trim().ToLowerInvariant() ?? "", x.Country?.Trim().ToLowerInvariant() ?? ""))
            .Select(g => g.First())
            .ToList();

        foreach (var loc in unique)
            _results.Add(loc);

        // Если что-то нашли — авто-выделение первого
        if (_results.Count > 0)
        {
            listResults.SelectedItem = _results.First();
            _selected = _results.First();
            btnSelect.IsEnabled = true;
        }

        // Пауза по правилам Nominatim
        await Task.Delay(1000);

        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
    }

    private void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        _selected = e.CurrentSelection.FirstOrDefault() as AppLocation;
        btnSelect.IsEnabled = _selected != null;
    }

    private async void OnSelectClicked(object sender, EventArgs e)
    {
        if (_selected == null)
            return;

        var existing = _database.FindLocationByLocality(_selected.Locality);
        if (existing != null)
            _selected.Id = existing.Id;
        else
            _selected.Id = _database.AddLocationAndReturnId(_selected);

        MessagingCenter.Send(this, "LocationSelected", (Mode, _selected));
        await Shell.Current.GoToAsync("..", true);
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        // Если что-то выбрано — возвращаем, как и при Select
        if (_selected != null)
            MessagingCenter.Send(this, "LocationSelected", (Mode, _selected));

        await Shell.Current.GoToAsync("..", true);
    }
}
