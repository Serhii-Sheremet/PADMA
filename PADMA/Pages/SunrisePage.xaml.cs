using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
    public partial class SunrisePage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public SunrisePage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            Title = Localization.GetLocalizedText("Sunrise calculation", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose how to calculate the sunrise:", lang);
            TipLabel.Text = Localization.GetLocalizedText("Visible upper edge of the Sun disk", lang);
            CenterLabel.Text = Localization.GetLocalizedText("Visible center of the Sun disk", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            _originalSettingCode = db.GetActiveSettingCode("SUNRISE") ?? "TIP";
            _currentSettingCode = _originalSettingCode;

            TipRadioButton.IsChecked = _originalSettingCode == "TIP";
            CenterRadioButton.IsChecked = _originalSettingCode == "CENTER";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!(sender as RadioButton)?.IsChecked ?? false)
                return;

            if (sender == TipRadioButton) _currentSettingCode = "TIP";
            else if (sender == CenterRadioButton) _currentSettingCode = "CENTER";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode != _originalSettingCode)
            {
                var lang = DataCache.Instance.CurrentLanguageCode;
                bool save = await DisplayAlert(
                    Localization.GetLocalizedText("Save changes?", lang),
                    Localization.GetLocalizedText("Apply new settings for sunrise calculation?", lang),
                    Localization.GetLocalizedText("Yes", lang),
                    Localization.GetLocalizedText("No", lang)
                );

                if (save)
                {
                    var db = ServiceLocator.Services.GetService<DatabaseService>();
                    db.SetAppSettingActive("SUNRISE", _currentSettingCode);
                    DataCache.Instance.Refresh(db);
                    MessagingCenter.Send<object>(this, "SettingsChanged");
                }
            }
        }
    }
}
