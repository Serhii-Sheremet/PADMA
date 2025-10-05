using PADMA.Core.Services;
using PADMA.Core.Models;

namespace PADMA.Pages
{
    public partial class FirstDayOfWeekPage : ContentPage
    {
        private readonly AppSettingsService _settingsService;
        private AppSettingList _activeSetting;
        private bool _hasChanges = false;

        public FirstDayOfWeekPage(AppSettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            LoadCurrentSetting();
        }

        private void LoadCurrentSetting()
        {
            _activeSetting = _settingsService.GetActiveSetting("WEEK");

            if (_activeSetting == null)
                return;

            if (_activeSetting.Code == "WEEKMONDAY")
                MondayRadioButton.IsChecked = true;
            else if (_activeSetting.Code == "WEEKSUNDAY")
                SundayRadioButton.IsChecked = true;
        }

        private void OnDaySelected(object sender, CheckedChangedEventArgs e)
        {
            _hasChanges = true;
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            if (_hasChanges)
            {
                bool save = await DisplayAlert("Confirm", "Apply changes?", "Yes", "No");
                if (save)
                {
                    var selectedCode = MondayRadioButton.IsChecked ? "WEEKMONDAY" : "WEEKSUNDAY";
                    var newSetting = _settingsService.LoadSettings().FirstOrDefault(s => s.Code == selectedCode);
                    if (newSetting != null)
                        _settingsService.SetActiveSetting(newSetting.Id);

                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync("//configuration");
        }
    }
}
