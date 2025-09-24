using Microsoft.Maui.Controls;

namespace PADMA
{
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();

            // Устанавливаем текущий выбор
            if (AppSettings.FirstDayOfWeek == FirstDayOfWeek.Monday)
                MondayRadio.IsChecked = true;
            else
                SundayRadio.IsChecked = true;
        }

        private void OnFirstDayCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                var rb = sender as RadioButton;
                if (rb == MondayRadio)
                    AppSettings.FirstDayOfWeek = FirstDayOfWeek.Monday;
                else if (rb == SundayRadio)
                    AppSettings.FirstDayOfWeek = FirstDayOfWeek.Sunday;
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Сообщаем всем, кто подписан, что настройки изменились
            MessagingCenter.Send(this, "SettingsChanged");

            // Абсолютный переход к MainPage, очищаем стек
            await Shell.Current.GoToAsync("///MainPageRoute");
        }
    }
}