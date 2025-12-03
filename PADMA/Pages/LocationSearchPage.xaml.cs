using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PADMA.Pages;

[QueryProperty(nameof(Mode), nameof(Mode))]
public partial class LocationSearchPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly NominatimService _nominatim;
    private readonly ObservableCollection<AppLocation> _results = new();

    private AppLocation? _selected;
    public string Mode { get; set; } = ""; // "birth" | "living"

    public ICommand ItemTappedCommand { get; }

    public LocationSearchPage(DatabaseService database, NominatimService nominatim)
    {
        InitializeComponent();
        _database = database;
        _nominatim = nominatim;

        ItemTappedCommand = new Command<AppLocation>(OnItemTapped);
        BindingContext = this;

        listResults.ItemsSource = _results;
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        string lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Location search", lang);
        entrySearch.Placeholder = Localization.GetLocalizedText("Enter city name", lang);
        btnSelect.Text = Localization.GetLocalizedText("Select", lang);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Устанавливаем курсор в поле ввода
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(200); // небольшой лаг, чтобы страница успела отрисоваться
            entrySearch.CursorPosition = entrySearch.Text?.Length ?? 0;
            entrySearch.SelectionLength = 0;
            entrySearch.Unfocus(); // оставляем без клавиатуры
        });
    }


    private async void OnSearchClicked(object sender, EventArgs e)
    {
        // Спрятать клавиатуру, если пользователь нажал "Поиск"
        KeyboardHelper.HideKeyboard();

        _results.Clear();
        btnSelect.IsEnabled = false;
        _selected = null;

        var query = entrySearch.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(query))
            return;

        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;

        var lang = _database.GetActiveLanguageCode();

        // Проверка локальной базы
        var localResults = _database.SearchLocationByName(query);
        if (localResults.Count > 0)
        {
            foreach (var loc in localResults)
                _results.Add(loc);
        }
        else
        {
            // Если нет — запрос к Nominatim
            var found = await _nominatim.SearchAsync(query, lang);
            var unique = found
                .GroupBy(x => (x.DisplayName?.Trim().ToLowerInvariant() ?? ""))
                .Select(g => g.First())
                .ToList();

            foreach (var loc in unique)
                _results.Add(loc);
        }

        await Task.Delay(1000); // задержка по политике Nominatim

        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;

    }

    private void OnItemTapped(AppLocation tapped)
    {
        if (tapped == null) return;

        // Скрываем клавиатуру, если ещё открыта
        KeyboardHelper.HideKeyboard();

        // снять выделение со всех и выделить один
        foreach (var loc in _results)
            loc.IsSelected = (loc == tapped);

        _selected = tapped;
        btnSelect.IsEnabled = true;
    }


    private void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        // выключаем автоселект, используем Tap
        ((CollectionView)sender).SelectedItem = null;
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
}
