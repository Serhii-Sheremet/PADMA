using PADMA.Core.Analysis;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
    public partial class NodesPage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public NodesPage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            Title = Localization.GetLocalizedText("Nodes (Rahu and Ketu)", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose nodes settings for calculations:", lang);
            MeanLabel.Text = Localization.GetLocalizedText("Mean nodes", lang);
            TrueLabel.Text = Localization.GetLocalizedText("True nodes", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            _originalSettingCode = db.GetActiveSettingCode("NODE") ?? "MEAN";
            _currentSettingCode = _originalSettingCode;

            MeanRadioButton.IsChecked = _currentSettingCode == "MEAN";
            TrueRadioButton.IsChecked = _currentSettingCode == "TRUE";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == MeanRadioButton)
                _currentSettingCode = "MEAN";
            else if (sender == TrueRadioButton)
                _currentSettingCode = "TRUE";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode == _originalSettingCode)
                return;

            var lang = DataCache.Instance.CurrentLanguageCode;

            bool save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply new settings for Rahu and Ketu calculations?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (save)
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                db.SetAppSettingActive("NODE", _currentSettingCode);

                DataCache.Instance.Refresh(db);
                SwissAnalysis.ClearZodiacBoundaryCache();
                MessagingCenter.Send<object>(this, "SettingsChanged");
            }
        }
    }
}
