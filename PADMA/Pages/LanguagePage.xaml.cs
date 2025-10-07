using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Linq;

namespace PADMA.Pages
{
    public partial class LanguagePage : ContentPage
    {
        private readonly DatabaseService _db;
        private string _originalSettingCode;
        private string _currentSettingCode;
        private bool _isClosingByButton = false;

        public LanguagePage()
        {
            InitializeComponent();

            _db = ServiceLocator.Services.GetService<DatabaseService>();
            LoadCurrentLanguage();
            LocalizePageTexts();
        }

        private void LoadCurrentLanguage()
        {
            var settings = _db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "LANGUAGE" && x.Active == 1);

            // если нет активного Ч выставл€ем по умолчанию (английский)
            _originalSettingCode = active?.SettingCode ?? "ENGLISH";
            _currentSettingCode = _originalSettingCode;

            EnglishRadioButton.IsChecked = _currentSettingCode == "ENGLISH";
            UkrainianRadioButton.IsChecked = _currentSettingCode == "UKRAINIAN";
            PolishRadioButton.IsChecked = _currentSettingCode == "POLISH";
            RussianRadioButton.IsChecked = _currentSettingCode == "RUSSIAN";
        }

        private void LocalizePageTexts()
        {
            string lang = DataCache.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Language", lang);
            PageTitle.Text = Localization.GetLocalizedText("Select application language:", lang);
            EnglishLabel.Text = Localization.GetLocalizedText("English", lang);
            UkrainianLabel.Text = Localization.GetLocalizedText("Ukrainian", lang);
            PolishLabel.Text = Localization.GetLocalizedText("Polish", lang);
            RussianLabel.Text = Localization.GetLocalizedText("Russian", lang);
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
                return;

            if (sender == EnglishRadioButton)
                _currentSettingCode = "ENGLISH";
            else if (sender == UkrainianRadioButton)
                _currentSettingCode = "UKRAINIAN";
            else if (sender == PolishRadioButton)
                _currentSettingCode = "POLISH";
            else if (sender == RussianRadioButton)
                _currentSettingCode = "RUSSIAN";
        }

        protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
        {
            if (_isClosingByButton)
            {
                base.OnNavigatingFrom(args);
                return;
            }

            if (_currentSettingCode != _originalSettingCode)
            {
                string titleText = Localization.GetLocalizedText("Save changes?", DataCache.CurrentLanguageCode);
                string messageText = Localization.GetLocalizedText("Apply new language setting?", DataCache.CurrentLanguageCode);
                string yesText = Localization.GetLocalizedText("Yes", DataCache.CurrentLanguageCode);
                string noText = Localization.GetLocalizedText("No", DataCache.CurrentLanguageCode);

                bool save = await DisplayAlert(titleText, messageText, yesText, noText);

                if (save)
                {
                    ApplyLanguageChange();
                }
            }

            base.OnNavigatingFrom(args);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            _isClosingByButton = true;

            if (_currentSettingCode != _originalSettingCode)
            {
                string titleText = Localization.GetLocalizedText("Save changes?", DataCache.CurrentLanguageCode);
                string messageText = Localization.GetLocalizedText("Apply new language setting?", DataCache.CurrentLanguageCode);
                string yesText = Localization.GetLocalizedText("Yes", DataCache.CurrentLanguageCode);
                string noText = Localization.GetLocalizedText("No", DataCache.CurrentLanguageCode);

                bool save = await DisplayAlert(titleText, messageText, yesText, noText);

                if (save)
                {
                    ApplyLanguageChange();
                }
            }

            await Shell.Current.GoToAsync("..");
        }

        private void ApplyLanguageChange()
        {
            var settings = _db.GetAppSettingsList();
            var langSettings = settings.Where(x => x.GroupCode == "LANGUAGE").ToList();

            foreach (var s in langSettings)
                s.Active = 0;

            var selected = langSettings.FirstOrDefault(x => x.SettingCode == _currentSettingCode);
            if (selected != null)
                selected.Active = 1;

            _db.UpdateAppSettings(langSettings);

            // обновл€ем кэш
            var cached = _db.GetAppSettingsList().Where(x => x.GroupCode == "LANGUAGE").ToList();
            foreach (var s in cached)
                s.Active = s.SettingCode == _currentSettingCode ? 1 : 0;

            // уведомл€ем приложение о смене €зыка
            MessagingCenter.Send(this, "SettingsChanged");
        }
    }
}
