using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI;
using PADMA.UI.Services;
using PADMA.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PADMA.Pages
{
    public partial class DayOverviewPage : UI.Templates.ConfigBasePage, IQueryAttributable
    {
        private DayItem? _day;
        private string? _windowToken;
        private DayWindowContext? _window;
        private readonly IDayComputationService _dayService;
        private List<DayOverviewItemVm>? _items;
        private bool _fixingWrap;

        public DayItem? Day
        {
            get => _day;
            private set
            {
                _day = value;
                BindingContext = this;
                ApplyLocalizedLabels();
                OnPropertyChanged(nameof(Day));
            }
        }

        private DayOverviewData? _overviewData;
        public DayOverviewData? OverviewData
        {
            get => _overviewData;
            private set
            {
                _overviewData = value;
                OnPropertyChanged(nameof(OverviewData));
            }
        }

        public DayOverviewPage()
        {
            InitializeComponent();
            BindingContext = this;

            _dayService = ServiceLocator.Services.GetService<IDayComputationService>()
                ?? throw new InvalidOperationException("IDayComputationService is not registered");

        }

        private void BuildCarousel()
        {
            if (_window == null) return;

            _items = _window.Days.Select(d => new DayOverviewItemVm(d)).ToList();
            DayCarousel.ItemsSource = _items;

            var pos = Math.Max(0, Math.Min(_window.SelectedIndex, _items.Count - 1));
            DayCarousel.Position = pos;
        }

        private async Task EnsureOverviewLoadedAsync(DayOverviewItemVm item)
        {
            if (item.Overview != null) return;

            var profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (profile == null || ctx == null) return;

            var key = new DayKey(profile.Id, DateOnly.FromDateTime(item.Day.Date));
            item.Overview = await _dayService.GetOverviewAsync(key, item.Day);
        }

        private async Task LoadOverviewAsync(DayItem day)
        {
            var profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService.Current;

            if (profile == null || ctx == null)
                return;

            var key = new DayKey(profile.Id, DateOnly.FromDateTime(day.Date));
            OverviewData = await _dayService.GetOverviewAsync(key, day);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("WindowToken", out var wt) && wt is string wts)
            {
                _windowToken = wts;
                var store = ServiceLocator.Services.GetService<NavigationDataStore>();
                if (store != null && store.TryGet(wts, out DayWindowContext? window) && window != null)
                    _window = window;
            }

            // парсим query синхронно
            ReadQuery(query);

            // дальше Ч асинхронна€ инициализаци€
            _ = InitializeCarouselAsync();

            if (query.TryGetValue("Day", out var obj) && obj is DayItem day)
            {
                Day = day;
                _ = LoadOverviewAsync(day);
                return;
            }

            Day = null;
        }

        private void ReadQuery(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Day", out var obj) && obj is DayItem day)
                Day = day;

            if (query.TryGetValue("WindowToken", out var wt) && wt is string wts)
            {
                _windowToken = wts;

                var store = ServiceLocator.Services.GetService<NavigationDataStore>();
                if (store != null && store.TryGet(wts, out DayWindowContext? window) && window != null)
                    _window = window;
            }
        }

        private async Task InitializeCarouselAsync()
        {
            if (_window == null) return;

            BuildCarousel(); // создаЄт _items, ставит ItemsSource, Position

            if (_items == null || _items.Count == 0) return;

            var pos = Math.Clamp(_window.SelectedIndex, 0, _items.Count - 1);
            var item = _items[pos];

            Day = item.Day; // обновит title
            await EnsureOverviewLoadedAsync(item);
            _ = PreloadNeighborsAsync(pos);
        }

        private void ApplyLocalizedLabels()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var culture = new CultureInfo(lang);

            if (Day != null)
                Title = Day.Date.ToString("dd MMMM yyyy", culture);

            OpenDayDetailsButton.Text = Localization.GetLocalizedText("Open Day Details", lang);
        }

        private async void OnOpenDayDetailsClicked(object sender, EventArgs e)
        {
            if (Day == null) return;

            var store = ServiceLocator.Services.GetService<NavigationDataStore>();
            if (store == null)
                throw new InvalidOperationException("NavigationDataStore is not registered");

            var pos = DayCarousel.Position;
            var current = _items?[pos];

            var bundle = new DayNavBundle
            {
                Day = Day,
                Overview = current?.Overview,
                Window = _window
            };

            var token = store.Put(bundle);

            await Shell.Current.GoToAsync("day", true,
                new Dictionary<string, object> { { "token", token } });
        }

        private void OnCarouselPositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (_items == null || _items.Count == 0) return;
            if (_fixingWrap) return;

            int last = _items.Count - 1;

            // Detect wrap cases (loop)
            bool wrappedFromEndToStart = (e.PreviousPosition == last && e.CurrentPosition == 0);
            bool wrappedFromStartToEnd = (e.PreviousPosition == 0 && e.CurrentPosition == last);

            if (wrappedFromEndToStart || wrappedFromStartToEnd)
            {
                // IMPORTANT: do NOT set Position immediately here.
                _ = FixWrapAsync(wrappedFromEndToStart ? last : 0);
                return;
            }

            // Normal: load data async (no Position set here)
            _ = OnPositionSettledAsync(e.CurrentPosition);
        }

        private async Task FixWrapAsync(int targetPosition)
        {
            if (_fixingWrap) return;
            _fixingWrap = true;

            try
            {
                // Let the gesture/layout finish
                await Task.Delay(1);
                await Task.Yield();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DayCarousel.ScrollTo(targetPosition, animate: false);
                });
            }
            finally
            {
                _fixingWrap = false;
            }
        }

        private async Task OnPositionSettledAsync(int pos)
        {
            if (_items == null) return;
            if (pos < 0 || pos >= _items.Count) return;

            var item = _items[pos];
            Day = item.Day;
            await EnsureOverviewLoadedAsync(item);
            _ = PreloadNeighborsAsync(pos);

            if (_window != null)
                _window = new DayWindowContext { Days = _window.Days, SelectedIndex = pos };
        }

        private async Task PreloadNeighborsAsync(int idx)
        {
            if (_items == null) return;

            // 1 шаг в обе стороны Ч обычно достаточно
            if (idx - 1 >= 0) await EnsureOverviewLoadedAsync(_items[idx - 1]);
            if (idx + 1 < _items.Count) await EnsureOverviewLoadedAsync(_items[idx + 1]);
            
        }






    }
}
