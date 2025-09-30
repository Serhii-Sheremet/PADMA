using Microsoft.Maui.Controls;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        private bool _weekStartsOnMonday;
        private bool _hasChanges;

        public ConfigurationPage()
        {
            InitializeComponent();

            // Загружаем сохранённое значение
            _weekStartsOnMonday = Preferences.Get("WeekStartsOnMonday", true);
            MondayRadio.IsChecked = _weekStartsOnMonday;
            SundayRadio.IsChecked = !_weekStartsOnMonday;

            _hasChanges = false;
        }

        private void OnOptionChanged(object sender, CheckedChangedEventArgs e)
        {
            bool newValue = MondayRadio.IsChecked;
            _hasChanges = newValue != _weekStartsOnMonday;
        }

        private void OnApplyClicked(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            if (_hasChanges)
            {
                bool save = await DisplayAlert("Save changes?", "Apply new settings?", "Yes", "No");
                if (save)
                {
                    ApplyChanges();
                }
            }

            // Закрываем страницу корректно
            await Shell.Current.Navigation.PopAsync();
        }

        private void ApplyChanges()
        {
            _weekStartsOnMonday = MondayRadio.IsChecked;
            Preferences.Set("WeekStartsOnMonday", _weekStartsOnMonday);

            MessagingCenter.Send(this, "SettingsChanged");

            _hasChanges = false;
        }
    }
}
