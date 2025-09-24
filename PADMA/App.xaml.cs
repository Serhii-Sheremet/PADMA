using Microsoft.Maui.Controls;

namespace PADMA
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}