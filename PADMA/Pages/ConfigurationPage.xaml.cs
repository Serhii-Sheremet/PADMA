namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnLanguageClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(LanguagePage));

        private async void OnFirstDayOfWeekClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(FirstDayOfWeekPage));

        private async void OnTransitsClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(TransitsPage));

        private async void OnNodesClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(NodesPage));

        private async void OnHoraClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(HoraPage));

        private async void OnMuhurtasClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(MuhurtasPage));

        private async void OnMrityuClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(MrityuPage));

        private async void OnSunriseClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync(nameof(SunrisePage));
    }
}
