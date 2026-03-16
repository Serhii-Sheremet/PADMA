using CommunityToolkit.Maui.Extensions;
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

    private CancellationTokenSource? _searchCts;
    private DateTime _lastNominatimCallUtc = DateTime.MinValue;
    private const int MinQueryLen = 3;
    private const int DebounceMs = 350;

    private AppLocation? _selected;
    public string Mode { get; set; } = ""; // "birth" | "living"

    public ICommand ItemTappedCommand { get; }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    private string _busyText = "Please wait…";
    public string BusyText
    {
        get => _busyText;
        set
        {
            if (_busyText == value) return;
            _busyText = value;
            OnPropertyChanged(nameof(BusyText));
        }
    }

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
        lbCountry.Text = Localization.GetLocalizedText("Country", lang);
        entryCountry.Placeholder = Localization.GetLocalizedText("Select country", lang);
        lbLocation.Text = Localization.GetLocalizedText("Location", lang);
        entrySearch.Placeholder = Localization.GetLocalizedText("Enter settlement name", lang);
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

    private string? _countryCode; // "pl", "ua", ...

    private async void OnCountryClicked(object sender, EventArgs e)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        var popup = new CountrySelectPopup(lang);
        await RunBusyAsync(Localization.GetLocalizedText(BusyText, lang), async () =>
        {
            await this.ShowPopupAsync(popup);
        });
        var item = popup.SelectedCountry;
        if (item != null)
        {
            _countryCode = item.Code;
            entryCountry.Text = item.Display;
        }
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var query = (e.NewTextValue ?? "").Trim();

        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        _selected = null;
        btnSelect.IsEnabled = false;

        if (query.Length < MinQueryLen)
        {
            _results.Clear();
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
            return;
        }

        try
        {
            // debounce
            await Task.Delay(DebounceMs, token);
            if (token.IsCancellationRequested) return;

            loadingIndicator.IsRunning = true;
            loadingIndicator.IsVisible = true;

            _results.Clear();

            // 1) Сначала БД (НО: важно, чтобы поиск был contains + case-insensitive)
            var local = _database.SearchLocationByName(query);
            if (!string.IsNullOrWhiteSpace(_countryCode))
            {
                local = local
                    .Where(l => string.Equals(l.CountryCode, _countryCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (token.IsCancellationRequested) return;

            if (local.Count > 0)
            {
                foreach (var loc in local)
                    _results.Add(loc);

                return; // только БД
            }

            bool hasCyrillic = query.Any(ch => ch >= '\u0400' && ch <= '\u04FF');

            // для кириллицы поднимаем порог Nominatim, чтобы меньше шума
            int minNominatimLen = hasCyrillic ? 4 : 3;

            if (query.Length < minNominatimLen)
            {
                // БД ничего не нашла, но в Nominatim пока рано — просто показываем пусто
                return;
            }

            // 2) Если БД пусто — Nominatim (не чаще 1 раза в секунду)
            var nowUtc = DateTime.UtcNow;
            var sinceLast = nowUtc - _lastNominatimCallUtc;
            if (sinceLast.TotalMilliseconds < 1000)
                await Task.Delay(1000 - (int)sinceLast.TotalMilliseconds, token);

            if (token.IsCancellationRequested) return;

            _lastNominatimCallUtc = DateTime.UtcNow;

            var lang = DataCache.Instance.CurrentLanguageCode;
            var found = await _nominatim.SearchAsync(query, lang, _countryCode);
            if (token.IsCancellationRequested) return;

            var unique = found
                .GroupBy(x => (x.DisplayName?.Trim().ToLowerInvariant() ?? ""))
                .Select(g => g.First())
                .ToList();

            foreach (var loc in unique)
                _results.Add(loc);
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
        finally
        {
            loadingIndicator.IsRunning = false;
            loadingIndicator.IsVisible = false;
        }
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

        var lang = DataCache.Instance.CurrentLanguageCode; //_database.GetActiveLanguageCode();

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
            var found = await _nominatim.SearchAsync(query, lang, _countryCode);
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

        // НИЧЕГО НЕ СОХРАНЯЕМ В БАЗУ ЗДЕСЬ

        // Просто отправляем выбранное AppLocation наверх
        MessagingCenter.Send(this, "LocationSelected", (Mode, _selected));
        await Shell.Current.GoToAsync("..", true);
    }

    private async Task RunBusyAsync(string text, Func<Task> action)
    {
        BusyText = text;
        IsBusy = true;

        await Task.Yield(); // дать UI шанс отрисовать overlay

        try { await action(); }
        finally { IsBusy = false; }
    }
}
