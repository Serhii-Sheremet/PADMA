using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        private bool _hasConfigChanges = false;

        public ConfigurationPage()
        {
            InitializeComponent();

            // Универсальная подписка на событие "SettingsChanged" от всех дочерних страниц
            MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ =>
            {
                _hasConfigChanges = true;

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
            var langCode = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Settings", langCode);
            btnLanguage.Text = Localization.GetLocalizedText("Language", langCode);
            btnFirstDayOfWeek.Text = Localization.GetLocalizedText("First day of week", langCode);
            btnTransits.Text = Localization.GetLocalizedText("Planetary transits", langCode);
            btnNodes.Text = Localization.GetLocalizedText("Nodes (Rahu and Ketu)", langCode);
            btnHora.Text = Localization.GetLocalizedText("Hora", langCode);
            btnMuhurta.Text = Localization.GetLocalizedText("30 Muhurtas (60 Ghatis)", langCode);
            btnMrityuBhaga.Text = Localization.GetLocalizedText("Mrityu Bhaga", langCode);
            btnSunrise.Text = Localization.GetLocalizedText("Sunrise calculation", langCode);
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
            // Проверяем, были ли изменения с дочерних страниц
            if (_hasConfigChanges)
            {
                // Шлём глобальное сообщение для MainPage о необходимости обновления календаря
                MessagingCenter.Send<object>(this, "ConfigurationHubClosedWithChanges");
            }

            await Shell.Current.GoToAsync("//main", true);
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
    }
}
