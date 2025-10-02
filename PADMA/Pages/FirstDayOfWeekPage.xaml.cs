using Microsoft.Maui.Controls;
using System;

namespace PADMA.Pages
{
    public partial class FirstDayOfWeekPage : ContentPage
    {
        private string _originalValue;
        private string _currentValue;

        public FirstDayOfWeekPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Загружаем значение (пока жёстко Monday)
            _originalValue = "Monday";
            _currentValue = _originalValue;

            // Устанавливаем RadioButton
            MondayRadioButton.IsChecked = _originalValue == "Monday";
            SundayRadioButton.IsChecked = _originalValue == "Sunday";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value) // только если выбрано
            {
                var rb = sender as RadioButton;
                _currentValue = rb?.Value?.ToString();
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            if (_currentValue != _originalValue)
            {
                bool save = await DisplayAlert("Save changes?",
                    $"Apply '{_currentValue}' as first day of week?",
                    "Yes", "No");

                if (save)
                {
                    // сохраняем выбор (позже - в DataCache/базу)
                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync(".."); // закрыть страницу
        }
    }
}
