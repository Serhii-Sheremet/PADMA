using Microsoft.Maui.Controls;

namespace PADMA.Pages
{
    [QueryProperty(nameof(Date), "Date")]
    public partial class DayPage : ContentPage
    {
        private DateTime _date;

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                Title = _date.ToString("dd MMMM yyyy"); // титул всегда выбранная дата
            }
        }

        public DayPage()
        {
            InitializeComponent();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // возвращаемся на календарь
            await Shell.Current.GoToAsync("//calendar");
        }
    }
}
