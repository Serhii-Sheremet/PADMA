using Microsoft.Maui.Controls;

namespace PADMA.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        private bool _isMondayFirstInitial;
        private bool _hasChanges;

        public bool IsMondayFirst { get; set; }
        public bool IsSundayFirst => !IsMondayFirst;

        public ConfigurationPage()
        {
            InitializeComponent();

            // читаем текущее состояние из настроек
            IsMondayFirst = Preferences.Get("WeekStartsOnMonday", true);
            _isMondayFirstInitial = IsMondayFirst;

            BindingContext = this;
        }

        private async void OnApplyClicked(object sender, EventArgs e)
        {
            ApplyChanges();
            await DisplayAlert("Settings", "Changes applied.", "OK");
        }

        private void ApplyChanges()
        {
            Preferences.Set("WeekStartsOnMonday", IsMondayFirst);
            MessagingCenter.Send(this, "SettingsChanged");
            _isMondayFirstInitial = IsMondayFirst;
            _hasChanges = false;
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            if (_hasChanges)
            {
                bool apply = await DisplayAlert("Save changes?",
                    "Do you want to apply changes before exit?",
                    "Yes", "No");

                if (apply)
                    ApplyChanges();
            }

            await Shell.Current.GoToAsync("//calendar");
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsMondayFirst))
            {
                _hasChanges = (IsMondayFirst != _isMondayFirstInitial);
            }
        }
    }
}
