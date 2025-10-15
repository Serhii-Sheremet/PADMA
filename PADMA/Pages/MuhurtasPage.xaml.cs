using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
    public partial class MuhurtasPage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public MuhurtasPage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("30 Muhurtas (60 Ghatis)", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose how to calculate Muhurtas (Ghatis):", lang);

            DayNightLabel.Text = Localization.GetLocalizedText("From sunrise to sunset (1/15) + sunset to sunrise (1/15)", lang);
            EqualLabel.Text = Localization.GetLocalizedText("From sunrise to sunrise (1/30)", lang);
            From6Label.Text = Localization.GetLocalizedText("From 6:00 AM (Muhurta = 48 min)", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            var settings = db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "MUHURTAGHATI" && x.Active == 1);
            _originalSettingCode = active?.SettingCode ?? "HORAEQUAL";
            _currentSettingCode = _originalSettingCode;

            DayNightRadioButton.IsChecked = _currentSettingCode == "MUHURTAGHATIDAYNIGHT";
            EqualRadioButton.IsChecked = _currentSettingCode == "MUHURTAGHATIEQUAL";
            From6RadioButton.IsChecked = _currentSettingCode == "MUHURTAGHATIFROM6";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == DayNightRadioButton)
                _currentSettingCode = "MUHURTAGHATIDAYNIGHT";
            else if (sender == EqualRadioButton)
                _currentSettingCode = "MUHURTAGHATIEQUAL";
            else if (sender == From6RadioButton)
                _currentSettingCode = "MUHURTAGHATIFROM6";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode == _originalSettingCode)
                return;

            var lang = DataCache.Instance.CurrentLanguageCode;

            bool save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply new settings for Muhurtas (Ghatis) calculation?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (save)
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                db.SetAppSettingActive("MUHURTAGHATI", _currentSettingCode);
                DataCache.Instance.Refresh(db);

                MessagingCenter.Send<object>(this, "SettingsChanged");
            }
        }
    }
}
