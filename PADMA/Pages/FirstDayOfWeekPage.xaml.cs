namespace PADMA.Pages
{
    public partial class FirstDayOfWeekPage : ContentPage
    {
        private string _initialValue;

        public FirstDayOfWeekPage()
        {
            InitializeComponent();
            _initialValue = Preferences.Get("FirstDayOfWeek", "Monday");
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            string currentValue = Preferences.Get("FirstDayOfWeek", "Monday");
            if (currentValue != _initialValue)
            {
                bool save = await DisplayAlert("Confirm", "Save changes?", "Yes", "No");
                if (save)
                {
                    MessagingCenter.Send(this, "SettingsChanged");
                    _initialValue = currentValue;
                }
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}
