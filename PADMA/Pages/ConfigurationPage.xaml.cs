using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        public bool IsMondayFirst => Preferences.Get("FirstDayOfWeek", "Monday") == "Monday";
        public bool IsSundayFirst => Preferences.Get("FirstDayOfWeek", "Monday") == "Sunday";

        public ConfigurationPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void OnFirstDayOfWeekChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value) // только при выборе
            {
                var rb = (RadioButton)sender;
                Preferences.Set("FirstDayOfWeek", rb.Content.ToString());

                // уведомляем MainPage, что настройки изменились
                MessagingCenter.Send(this, "SettingsChanged");
            }
        }
    }
}
