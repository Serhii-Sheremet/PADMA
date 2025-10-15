using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
    public partial class MrityuBhagaPage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public MrityuBhagaPage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Mrityu Bhaga", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose how to calculate Mrityu Bhaga:", lang);

            OptionNEQUALLabel.Text = Localization.GetLocalizedText("From (N° - 30') to (N° + 30')", lang);
            OptionNLESSLabel.Text = Localization.GetLocalizedText("From (N - 1)° to N°", lang);
            OptionNMORELabel.Text = Localization.GetLocalizedText("From N° to (N + 1)°", lang);
            OptionNERNSTLabel.Text = Localization.GetLocalizedText("From (N - 1)° to (N + 1)°", lang);

            NoteLabel.Text = Localization.GetLocalizedText("Where 'N' is the Mrityu Bhaga", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            _originalSettingCode = db.GetActiveSettingCode("MRITYUBHAGA") ?? "NERNST";
            _currentSettingCode = _originalSettingCode;

            OptionNEQUAL.IsChecked = _originalSettingCode == "NEQUAL";
            OptionNLESS.IsChecked = _originalSettingCode == "NLESS";
            OptionNMORE.IsChecked = _originalSettingCode == "NMORE";
            OptionNERNST.IsChecked = _originalSettingCode == "NERNST";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!(sender as RadioButton)?.IsChecked ?? false)
                return;

            if (sender == OptionNEQUAL) _currentSettingCode = "NEQUAL";
            else if (sender == OptionNLESS) _currentSettingCode = "NLESS";
            else if (sender == OptionNMORE) _currentSettingCode = "NMORE";
            else if (sender == OptionNERNST) _currentSettingCode = "NERNST";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode != _originalSettingCode)
            {
                var lang = DataCache.Instance.CurrentLanguageCode;
                bool save = await DisplayAlert(
                    Localization.GetLocalizedText("Save changes?", lang),
                    Localization.GetLocalizedText("Apply new settings for Mrityu Bhaga calculation?", lang),
                    Localization.GetLocalizedText("Yes", lang),
                    Localization.GetLocalizedText("No", lang)
                );

                if (save)
                {
                    var db = ServiceLocator.Services.GetService<DatabaseService>();
                    db.SetAppSettingActive("MRITYUBHAGA", _currentSettingCode);
                    DataCache.Instance.Refresh(db);
                    MessagingCenter.Send<object>(this, "SettingsChanged");
                }
            }
        }
    }
}
