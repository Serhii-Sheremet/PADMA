using Microsoft.Maui.Controls;

namespace PADMA;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Регистрируем route для страницы дня (если понадобится навигация по имени)
        Routing.RegisterRoute("day", typeof(Pages.DayPage));
    }
}

