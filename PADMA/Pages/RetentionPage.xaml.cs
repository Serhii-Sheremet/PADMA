using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
    public partial class RetentionPage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public RetentionPage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Retention policy", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose how long to keep user notes:", lang);

            labelOff.Text = Localization.GetLocalizedText("Keep forever", lang);
            label30days.Text = Localization.GetLocalizedText("Keep last 30 days", lang);
            label3months.Text = Localization.GetLocalizedText("Keep last 3 months", lang);
            label6months.Text = Localization.GetLocalizedText("Keep last 6 months", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            _originalSettingCode = db.GetActiveSettingCode("RETENTION") ?? "OFF";
            _currentSettingCode = _originalSettingCode;

            radioButtonOff.IsChecked = _originalSettingCode == "OFF";
            radioButton30days.IsChecked = _originalSettingCode == "DAY30";
            radioButton3months.IsChecked = _originalSettingCode == "MONTH3";
            radioButton6months.IsChecked = _originalSettingCode == "MONTH6";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == radioButtonOff)
                _currentSettingCode = "OFF";
            else if (sender == radioButton30days)
                _currentSettingCode = "DAY30";
            else if (sender == radioButton3months)
                _currentSettingCode = "MONTH3";
            else if (sender == radioButton6months)
                _currentSettingCode = "MONTH6";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode == _originalSettingCode)
                return;

            var lang = DataCache.Instance.CurrentLanguageCode;

            bool save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply new retention policy settings?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (save)
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                db.SetAppSettingActive("RETENTION", _currentSettingCode);
                DataCache.Instance.Refresh(db);

                var retention = ServiceLocator.Services.GetService<UserEventRetentionService>();
                if (retention != null)
                    await retention.ApplyAsync();

                MessagingCenter.Send<object>(this, "RetentionPolicyChanged");
            }
        }
    }
}
