using PADMA.Pages;

namespace PADMA;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Hidden (non-flyout) route for the Day page
        Routing.RegisterRoute("day", typeof(DayPage));
    }
}
