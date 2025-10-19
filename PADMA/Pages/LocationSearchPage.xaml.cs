using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using System.Collections.ObjectModel;

namespace PADMA.Pages;

[QueryProperty(nameof(Mode), nameof(Mode))]
public partial class LocationSearchPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly NominatimService _nominatim;

    public string Mode { get; set; } = ""; // "birth" | "living"

    private readonly ObservableCollection<AppLocation> _results = new();

    // Получаем сервисы через DI (зарегистрированы в MauiProgram)
    public LocationSearchPage(DatabaseService database, NominatimService nominatim)
    {
        InitializeComponent();
        _database = database;
        _nominatim = nominatim;

        listResults.ItemsSource = _results;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        var query = entrySearch.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(query))
            return;

        _results.Clear();

        // используем выбранный язык из БД
        var lang = _database.GetActiveLanguageCode(); // "en"/"uk"/"pl"/"ru"
        var found = await _nominatim.SearchAsync(query, lang);

        foreach (var loc in found)
            _results.Add(loc);
    }

    private async void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not AppLocation selected)
            return;

        // Предлагаем добавить в локальную БД 
        bool save = await DisplayAlert(
            "Save location",
            $"Add '{selected.Locality}' to saved locations?",
            "Yes", "No");

        if (save)
        {
            var id = _database.AddLocationAndReturnId(selected);
            selected.Id = id; // важный момент — теперь локация имеет ID в БД
        }

        // Возвращаем выбранную локацию в ProfileDetailPage
        // (используем MessagingCenter, чтобы не плодить статик-кэши)
        MessagingCenter.Send(this, "LocationSelected", (Mode, selected));

        ((CollectionView)sender).SelectedItem = null;
        await Shell.Current.GoToAsync("..", true);
    }

    private async void OnCloseClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..", true);
}
