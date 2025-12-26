using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using PADMA.UI;
using PADMA.UI.Services;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages
{
    public partial class DayOverviewPage : UI.Templates.ConfigBasePage, IQueryAttributable
    {
        private DayItem? _day;
        private readonly IDayComputationService _dayService;

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
            if (query.TryGetValue("Day", out var obj) && obj is DayItem day)
            {
                Day = day;

                // не блокируем UI — просто запускаем подгрузку
                _ = LoadOverviewAsync(day);
                return;
            }

            // На всякий случай, чтобы не падало
            Day = null;
        }

        private void ApplyLocalizedLabels()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            if (Day != null)
                Title = Day.Date.ToString("dd MMMM yyyy");

            OpenDayDetailsButton.Text = Localization.GetLocalizedText("Open Day Details", lang);
        }

        private async void OnOpenDayDetailsClicked(object sender, EventArgs e)
        {
            if (Day == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "Date", Day.Date }
            };

            await Shell.Current.GoToAsync("day", true, parameters);
        }
    }
}
