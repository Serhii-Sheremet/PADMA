using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using PADMA.UI;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages
{
    public partial class DayOverviewPage : UI.Templates.ConfigBasePage, IQueryAttributable
    {
        private DayItem? _day;

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

        public DayOverviewPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Day", out var obj) && obj is DayItem day)
            {
                Day = day;
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
