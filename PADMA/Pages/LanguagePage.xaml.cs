using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Linq;

namespace PADMA.Pages
{
    public partial class LanguagePage : ContentPage
    {
        private readonly DatabaseService _db;
        private string _originalLangCode;
        private string _currentLangCode;
        private bool _isClosingByButton = false;

        public LanguagePage()
        {
            InitializeComponent();

            _db = ServiceLocator.Services.GetService<DatabaseService>();

            // Заголовки/лейблы через локализацию (фолбэк на английский)
            Title = Localization.GetLocalizedText("Language", DataCache.CurrentLanguageCode);
            TitleLabel.Text = Localization.GetLocalizedText("Select Application language", DataCache.CurrentLanguageCode);

            // (опционально) локализуем названия языков, если появятся ключи в APP_TEXTS
            EnglishLabel.Text = Localization.GetLocalizedText("English", DataCache.CurrentLanguageCode);
            UkrainianLabel.Text = Localization.GetLocalizedText("Ukrainian", DataCache.CurrentLanguageCode);
            PolishLabel.Text = Localization.GetLocalizedText("Polish", DataCache.CurrentLanguageCode);
            RussianLabel.Text = Localization.GetLocalizedText("Russian", DataCache.CurrentLanguageCode);

            LoadCurrentLanguage();
        }

        private void LoadCurrentLanguage()
        {
            var settings = _db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "LANGUAGE" && x.Active == 1);
            _originalLangCode = active?.SettingCode ?? "ENGLISH";
            _currentLangCode = _originalLangCode;

            EnglishRadioButton.IsChecked = _currentLangCode == "ENGLISH";
            UkrainianRadioButton.IsChecked = _currentLangCode == "UKRAINIAN";
            PolishRadioButton.IsChecked = _currentLangCode == "POLISH";
            RussianRadioButton.IsChecked = _currentLangCode == "RUSSIAN";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == EnglishRadioButton) _currentLangCode = "ENGLISH";
            else if (sender == UkrainianRadioButton) _currentLangCode = "UKRAINIAN";
            else if (sender == PolishRadioButton) _currentLangCode = "POLISH";
            else if (sender == RussianRadioButton) _currentLangCode = "RUSSIAN";
        }

        protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
        {
            if (_isClosingByButton)
            {
                base.OnNavigatingFrom(args);
                return;
            }

            if (_currentLangCode != _originalLangCode)
            {
                string title = Localization.GetLocalizedText("Save changes?", DataCache.CurrentLanguageCode);
                string message = Localization.GetLocalizedText("Apply new language setting?", DataCache.CurrentLanguageCode);
                string yesText = Localization.GetLocalizedText("Yes", DataCache.CurrentLanguageCode);
                string noText = Localization.GetLocalizedText("No", DataCache.CurrentLanguageCode);

                bool save = await DisplayAlert(title, message, yesText, noText);
                if (save) ApplyLanguageChange();
            }

            base.OnNavigatingFrom(args);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            _isClosingByButton = true;

            if (_currentLangCode != _originalLangCode)
            {
                string title = Localization.GetLocalizedText("Save changes?", DataCache.CurrentLanguageCode);
                string message = Localization.GetLocalizedText("Apply new language setting?", DataCache.CurrentLanguageCode);
                string yesText = Localization.GetLocalizedText("Yes", DataCache.CurrentLanguageCode);
                string noText = Localization.GetLocalizedText("No", DataCache.CurrentLanguageCode);

                bool save = await DisplayAlert(title, message, yesText, noText);
                if (save) ApplyLanguageChange();
            }

            await Shell.Current.GoToAsync("..");
        }

        private void ApplyLanguageChange()
        {
            var settings = _db.GetAppSettingsList();
            var langSettings = settings.Where(x => x.GroupCode == "LANGUAGE").ToList();

            foreach (var s in langSettings) s.Active = 0;
            var selected = langSettings.FirstOrDefault(x => x.SettingCode == _currentLangCode);
            if (selected != null) selected.Active = 1;

            _db.UpdateAppSettings(langSettings);

            // обновляем текущий язык приложения
            DataCache.CurrentLanguageCode = _currentLangCode;

            // триггерим обновление UI
            MessagingCenter.Send(this, "SettingsChanged");
        }
    }
}
