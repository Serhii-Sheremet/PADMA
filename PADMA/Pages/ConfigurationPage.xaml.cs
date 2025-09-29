using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        // Simple binding props for initial state
        public bool IsMondayFirst => Preferences.Get("FirstDayOfWeek", "Monday") == "Monday";
        public bool IsSundayFirst => Preferences.Get("FirstDayOfWeek", "Monday") == "Sunday";

        public ConfigurationPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void OnFirstDayOfWeekChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            var rb = (RadioButton)sender;
            var selected = rb.Content?.ToString() == "Sunday" ? "Sunday" : "Monday";
            Preferences.Set("FirstDayOfWeek", selected);

            // Уведомим MainPage обновить календарь
            MessagingCenter.Send(this, "SettingsChanged");
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Закрыть страницу и вернуться к календарю
            // Если пришли из флаяута, PopAsync корректно вернёт назад
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch
            {
                // Fallback
                if (Navigation.NavigationStack.Count > 1)
                    await Navigation.PopAsync();
            }
        }
    }
}
