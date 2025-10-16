using Microsoft.Maui.Controls;
using PADMA.Pages;

namespace PADMA;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрируем маршруты
        Routing.RegisterRoute(nameof(ProfilesPage), typeof(PADMA.Pages.ProfilesPage));
        Routing.RegisterRoute(nameof(ProfileDetailPage), typeof(PADMA.Pages.ProfileDetailPage));

        Routing.RegisterRoute("day", typeof(DayPage));

        // Регистрируем маршруты для всех страниц конфигурации
        Routing.RegisterRoute(nameof(ConfigurationPage), typeof(PADMA.Pages.ConfigurationPage));
        Routing.RegisterRoute(nameof(LanguagePage), typeof(PADMA.Pages.LanguagePage));
        Routing.RegisterRoute(nameof(FirstDayOfWeekPage), typeof(PADMA.Pages.FirstDayOfWeekPage));
        Routing.RegisterRoute(nameof(TransitsPage), typeof(PADMA.Pages.TransitsPage));
        Routing.RegisterRoute(nameof(NodesPage), typeof(PADMA.Pages.NodesPage));
        Routing.RegisterRoute(nameof(HoraPage), typeof(PADMA.Pages.HoraPage));
        Routing.RegisterRoute(nameof(MuhurtasPage), typeof(PADMA.Pages.MuhurtasPage));
        Routing.RegisterRoute(nameof(MrityuBhagaPage), typeof(PADMA.Pages.MrityuBhagaPage));
        Routing.RegisterRoute(nameof(SunrisePage), typeof(PADMA.Pages.SunrisePage));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ConfigurationPage");
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        // Закрыть приложение (поведение зависит от платформы)
        await Application.Current.MainPage.DisplayAlert("Exit", "Closing app...", "OK");
        Application.Current.Quit();
    }
}
