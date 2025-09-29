using Microsoft.Maui.Controls;
using System;

namespace PADMA.Pages
{
    [QueryProperty(nameof(Date), "date")]
    public partial class DayPage : ContentPage
    {
        private DateTime _date;

        public string Date
        {
            set
            {
                if (DateTime.TryParse(value, out var parsed))
                {
                    _date = parsed;
                    Title = _date.ToString("dd MMMM yyyy"); // титул = дата
                }
            }
        }

        public DayPage()
        {
            InitializeComponent();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//calendar");
        }
    }
}
