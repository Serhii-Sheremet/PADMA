using Microsoft.Maui.Controls;
using System;

namespace PADMA.Pages
{
    [QueryProperty(nameof(SelectedDate), "SelectedDate")]
    public partial class DayPage : ContentPage
    {
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                Title = _selectedDate.ToString("dd MMMM yyyy");
            }
        }

        public DayPage()
        {
            InitializeComponent();
            Title = "Day"; // на случай, если значение ещё не пришло
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Переход на корневой маршрут календаря
            await Shell.Current.GoToAsync("//calendar");
        }



    }
}
