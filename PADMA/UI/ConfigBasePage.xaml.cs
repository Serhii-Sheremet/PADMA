using Microsoft.Maui.Controls;

namespace PADMA.UI.Templates
{
    public partial class ConfigBasePage : ContentPage
    {
        protected bool IsClosingByButton = false;

        public ConfigBasePage()
        {
            InitializeComponent();
        }

        protected virtual async void OnCloseClicked(object sender, EventArgs e)
        {
            IsClosingByButton = true;
            await Shell.Current.GoToAsync("..");
        }
    }
}
