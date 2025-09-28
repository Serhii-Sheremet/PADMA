using Microsoft.Extensions.DependencyInjection;
using PADMA.Core.Services;

namespace PADMA;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Resolve MainPage from DI after App resources are loaded
        var mainPage = ServiceLocator.Services.GetRequiredService<MainPage>();
        MainPage = new NavigationPage(mainPage);
    }
}
