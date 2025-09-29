using Microsoft.Maui.Controls;
using System;

namespace PADMA.Pages
{
    [QueryProperty(nameof(SelectedDate), "SelectedDate")]
    public partial class DayPage : ContentPage
    {
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value;
                Title = selectedDate.ToString("dd MMMM yyyy");
            }
        }

        public DayPage()
        {
            InitializeComponent();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(".."); // возвращаемся назад
        }
    }
}
