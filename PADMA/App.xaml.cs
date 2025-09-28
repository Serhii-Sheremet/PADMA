namespace PADMA;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();

        // Теперь MainPage создаётся через DI
        MainPage = new NavigationPage(mainPage);
    }
}
