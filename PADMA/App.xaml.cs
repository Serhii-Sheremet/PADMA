namespace PADMA;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();

        // MainPage создаётся через DI
        MainPage = new NavigationPage(mainPage);
    }
}
