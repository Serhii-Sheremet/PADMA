using CloudKit;
using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PADMA.Pages;

[QueryProperty(nameof(Mode), nameof(Mode))]
public partial class LocationSearchPage : ContentPage
{
    private readonly DatabaseService _database;
    public string Mode { get; set; } = "";
    private ObservableCollection<Location> _results = new();

    public LocationSearchPage()
    {
        InitializeComponent();
        _database = db;
        listResults.ItemsSource = _results;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        string query = entrySearch.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(query)) return;

        _results.Clear();

        // Заглушка — позже заменим вызовом Nominatim API
        var locations = await _database.SearchLocationsAsync(query);
        foreach (var loc in locations)
            _results.Add(loc);
    }

    private async void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not AppLocation selected)
            return;

        bool confirm = await DisplayAlert("Use this location?",
                                          $"Add '{selected.Locality}' to profile?",
                                          "Yes", "Cancel");
        if (confirm)
        {
            // возвращаем выбранную локацию (через DataCache)
            DataCache.Instance.SelectedLocation = selected;
            await Shell.Current.GoToAsync("..", true);
        }

        ((CollectionView)sender).SelectedItem = null;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }
}
