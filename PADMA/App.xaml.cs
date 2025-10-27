using System.Threading.Tasks;
using PADMA.Core.Services;

namespace PADMA;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        _ = SwissService.InitializeEphemerisPathAsync(); // запустится в фоне
        MainPage = new AppShell();
    }
}
