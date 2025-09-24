namespace PADMA
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрируем маршруты для Shell навигации
            Routing.RegisterRoute("MainPageRoute", typeof(MainPage));
            Routing.RegisterRoute("ConfigurationPageRoute", typeof(ConfigurationPage));
        }
    }


}
