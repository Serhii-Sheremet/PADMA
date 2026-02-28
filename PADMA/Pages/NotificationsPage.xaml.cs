using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
    public partial class NotificationsPage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public NotificationsPage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Notifications", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose notifications mode:", lang);

            OffLabel.Text = Localization.GetLocalizedText("Do not send", lang);
            min5Label.Text = Localization.GetLocalizedText("5 minutes before", lang);
            min15Label.Text = Localization.GetLocalizedText("15 minutes before", lang);
            min30Label.Text = Localization.GetLocalizedText("30 minutes before", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            _originalSettingCode = db.GetActiveSettingCode("NOTEREMINDER") ?? "OFF";
            _currentSettingCode = _originalSettingCode;

            OffRadioButton.IsChecked = _originalSettingCode == "OFF";
            min5RadioButton.IsChecked = _originalSettingCode == "MIN5";
            min15RadioButton.IsChecked = _originalSettingCode == "MIN15";
            min30RadioButton.IsChecked = _originalSettingCode == "MIN30";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == OffRadioButton)
                _currentSettingCode = "OFF";
            else if (sender == min5RadioButton)
                _currentSettingCode = "MIN5";
            else if (sender == min15RadioButton)
                _currentSettingCode = "MIN15";
            else if (sender == min30RadioButton)
                _currentSettingCode = "MIN30";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode == _originalSettingCode)
                return;

            var lang = DataCache.Instance.CurrentLanguageCode;

            bool save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply new notification settings?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (save)
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                db.SetAppSettingActive("NOTEREMINDER", _currentSettingCode);
                DataCache.Instance.Refresh(db);
                MessagingCenter.Send<object>(this, "SettingsChanged");
            }
        }
    }
}
