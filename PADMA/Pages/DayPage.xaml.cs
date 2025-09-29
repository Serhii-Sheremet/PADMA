using Microsoft.Maui.Controls;

namespace PADMA.Pages
{
    [QueryProperty(nameof(DateQuery), "date")]
    public partial class DayPage : ContentPage
    {
        public DayPage()
        {
            InitializeComponent();
        }

        // yyyy-MM-dd from query
        private string _dateQuery;
        public string DateQuery
        {
            get => _dateQuery;
            set
            {
                _dateQuery = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (DateTime.TryParse(value, out var dt))
                    {
                        Title = dt.ToString("D");
                        TitleLabel.Text = dt.ToString("D");
                    }
                    else
                    {
                        Title = value;
                        TitleLabel.Text = value;
                    }
                }
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//calendar");
        }
    }
}
