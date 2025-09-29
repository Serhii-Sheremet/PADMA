using Microsoft.Maui.Controls;
using System;

namespace PADMA.Pages
{
    public partial class DayPage : ContentPage
    {
        public DayPage(DateTime date)
        {
            InitializeComponent();
            Title = date.ToString("dd MMMM yyyy");
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
