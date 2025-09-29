using PADMA.Pages;

namespace PADMA;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрируем маршрут для страницы дня
        Routing.RegisterRoute(nameof(DayPage), typeof(DayPage));
    }
}
