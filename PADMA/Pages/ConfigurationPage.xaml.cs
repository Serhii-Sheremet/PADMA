using Microsoft.Maui.Controls;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();

            // Подписка на сообщение от дочерних страниц (например, FirstDayOfWeekPage)
            MessagingCenter.Subscribe<FirstDayOfWeekPage>(this, "SettingsChanged", async (sender) =>
            {
                await DisplayAlert("Configuration Updated",
                    "Settings have been successfully applied.",
                    "OK");
            });
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//main");
        }

        private async void OnFirstDayOfWeekClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("FirstDayOfWeekPage");
        }

        private async void OnLanguageClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LanguagePage");
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

        private async void OnMuhurtaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MuhurtaPage");
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
