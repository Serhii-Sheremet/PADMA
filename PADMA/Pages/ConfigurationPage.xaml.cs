using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using Plugin.LocalNotification;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        private bool _hasConfigChanges = false;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private async Task RunBusyAsync(string text, Func<Task> action)
        {
            BusyText = text;
            IsBusy = true;

            await Task.Yield(); // give the UI time to show the overlay

            try { await action(); }
            finally { IsBusy = false; }
        }

        public ConfigurationPage()
        {
            InitializeComponent();
            BindingContext = this;   // so IsBusy/BusyText bindings resolve against code-behind

            // (re-)subscribe to the "SettingsChanged" event each time the page is constructed
            MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ =>
            {
                _hasConfigChanges = true;

                ApplyLocalization();
                await ShowSettingsUpdatedMessage();
            });

            MessagingCenter.Subscribe<object>(this, "NotificationSettingsChanged", async _ =>
            {
                ApplyLocalization();
                await ShowSettingsUpdatedMessage();
            });

            MessagingCenter.Subscribe<object>(this, "RetentionPolicyChanged", async _ =>
            {
                ApplyLocalization();
                await ShowSettingsUpdatedMessage();
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Shell.Current.FlyoutIsPresented = false;
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Settings", lang);
            btnLanguage.Text = Localization.GetLocalizedText("Language", lang);
            btnFirstDayOfWeek.Text = Localization.GetLocalizedText("First day of week", lang);
            btnTransits.Text = Localization.GetLocalizedText("Planetary transits", lang);
            btnNodes.Text = Localization.GetLocalizedText("Nodes (Rahu and Ketu)", lang);
            btnHora.Text = Localization.GetLocalizedText("Hora", lang);
            btnMuhurta.Text = Localization.GetLocalizedText("30 Muhurtas (60 Ghatis)", lang);
            btnMrityuBhaga.Text = Localization.GetLocalizedText("Mrityu Bhaga", lang);
            btnSunrise.Text = Localization.GetLocalizedText("Sunrise calculation", lang);
            btnColorSettings.Text = Localization.GetLocalizedText("Color settings", lang);
            btnNotifications.Text = Localization.GetLocalizedText("Notifications", lang);
            btnRetention.Text = Localization.GetLocalizedText("Retention policy", lang);
        }

        private async Task ShowSettingsUpdatedMessage()
        {
            var langCode = DataCache.Instance.CurrentLanguageCode;

            await DisplayAlert(
                Localization.GetLocalizedText("Configuration Updated", langCode),
                Localization.GetLocalizedText("Settings have been successfully applied.", langCode),
                Localization.GetLocalizedText("OK", langCode)
            );
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            if (IsBusy) return;
            await CloseAsync();
        }

        private async Task CloseAsync()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            await RunBusyAsync(Localization.GetLocalizedText("Applying settings…", lang), async () =>
            {
                // notify only if something actually changed in the configuration hub
                if (_hasConfigChanges)
                {
                    // so MainPage knows it needs to recalculate/refresh with the new settings
                    MessagingCenter.Send<object>(this, "ConfigurationHubClosedWithChanges");
                }

                await Shell.Current.GoToAsync("//main", true);

                // give navigation a moment to complete, so MainPage can show its own overlay
                await Task.Yield();
            });
        }

        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                if (IsBusy) return;
                await CloseAsync();
            });
            return true; 
        }

        private async void OnLanguageClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LanguagePage");
        }

        private async void OnFirstDayOfWeekClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("FirstDayOfWeekPage");
        }

        private async void OnTransitsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TransitsPage");
        }

        private async void OnNodesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("NodesPage");
        }

        private async void OnHoraClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("HoraPage");
        }

        private async void OnMuhurtasClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MuhurtasPage");
        }

        private async void OnMrityuBhagaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MrityuBhagaPage");
        }

        private async void OnSunriseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SunrisePage");
        }

        private async void OnColorSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ColorSettingsPage");
        }

        private async void OnNotificationsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("NotificationsPage");
        }

        private async void OnRetentionClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RetentionPage");
        }

    }
}
