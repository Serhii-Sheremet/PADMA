using Microsoft.Maui.Controls;
using PADMA.Pages;

namespace PADMA;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрируем маршруты
        Routing.RegisterRoute("day", typeof(DayPage));
        Routing.RegisterRoute("config", typeof(ConfigurationPage));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("config");
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        // Закрыть приложение (поведение зависит от платформы)
        await Application.Current.MainPage.DisplayAlert("Exit", "Closing app...", "OK");
        Application.Current.Quit();
    }
}
