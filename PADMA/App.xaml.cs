namespace PADMA;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Используем Shell (с бургером)
        MainPage = new AppShell();
    }
}
