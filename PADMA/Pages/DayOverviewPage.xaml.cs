using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI;
using PADMA.UI.Services;
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
        private bool _isSwitchingDay;

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

            if (query.TryGetValue("Day", out var obj) && obj is DayItem day)
            {
                Day = day;
                _ = LoadOverviewAsync(day);
                return;
            }

            Day = null;
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

            var bundle = new DayNavBundle
            {
                Day = Day,
                Overview = OverviewData,
                Window = _window
            };

            var token = store.Put(bundle);

            await Shell.Current.GoToAsync("day", true,
                new Dictionary<string, object> { { "token", token } });
        }

        private async void OnSwipePrevDay(object sender, EventArgs e)
        {
            try { await TrySwitchDayAsync(-1); }
            finally { DaySwipeView.Close(); }
        }

        private async void OnSwipeNextDay(object sender, EventArgs e)
        {
            try { await TrySwitchDayAsync(+1); }
            finally { DaySwipeView.Close(); }
        }

        private async Task TrySwitchDayAsync(int delta)
        {
            if (_window == null) return;
            if (_isSwitchingDay) return;

            var newIndex = _window.SelectedIndex + delta;
            if (newIndex < 0 || newIndex >= _window.Days.Count) return;

            await SwitchToWindowIndexAsync(newIndex);
        }

        private async Task SwitchToWindowIndexAsync(int newIndex)
        {
            if (_window == null) return;

            try
            {
                _isSwitchingDay = true;

                // 1) обновл€ем контекст
                _window = new DayWindowContext
                {
                    Days = _window.Days,
                    SelectedIndex = newIndex
                };

                // (опционально) сохранить обновлЄнный window обратно в store,
                // чтобы DayPage тоже получил актуальный SelectedIndex
                var store = ServiceLocator.Services.GetService<NavigationDataStore>();
                if (store != null && !string.IsNullOrWhiteSpace(_windowToken))
                {
                    // ” нас store не умеет "update", поэтому:
                    // - либо оставить как есть (DayPage получит старый индекс Ч не критично сейчас)
                    // - либо заменить токен на новый и хранить его в поле
                    store.Remove(_windowToken);
                    _windowToken = store.Put(_window);
                }

                // 2) берЄм новый DayItem
                var newDay = _window.Days[newIndex];
                Day = newDay;

                // 3) грузим/берЄм из кэша overview (важно: без пересчЄта, сервис сам кэширует)
                // у теб€ уже есть метод LoadOverviewAsync(day) Ч используем его
                await LoadOverviewAsync(newDay);

                // 4) (опционально) обновить заголовок/состо€ние кнопок, если есть
                // UpdateHeaderText(); UpdateNavButtons();
            }
            finally
            {
                _isSwitchingDay = false;
            }
        }

        




    }
}
