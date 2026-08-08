using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System;
using System.Linq;

namespace PADMA.Pages
{
    public partial class LanguagePage : ConfigBasePage
    {
        private readonly DatabaseService _db;
        private string _originalLanguageCode;
        private string _currentLanguageCode;

        public LanguagePage()
        {
            InitializeComponent();

            _db = ServiceLocator.Services.GetService<DatabaseService>();

            LoadCurrentLanguage();
            ApplyLocalizedLabels();
        }

        private void LoadCurrentLanguage()
        {
            var settings = _db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "LANGUAGE" && x.Active == 1);

            // fall back to ENGLISH if no active language setting is found
            _originalLanguageCode = active?.SettingCode ?? "ENGLISH";
            _currentLanguageCode = _originalLanguageCode;

            // reflect the current selection in the radio buttons
            EnglishRadioButton.IsChecked = _currentLanguageCode == "ENGLISH";
            UkrainianRadioButton.IsChecked = _currentLanguageCode == "UKRAINIAN";
            PolishRadioButton.IsChecked = _currentLanguageCode == "POLISH";
            RussianRadioButton.IsChecked = _currentLanguageCode == "RUSSIAN";
        }

        private void ApplyLocalizedLabels()
        {
            string langCode = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Language", langCode);
            InstructionLabel.Text = Localization.GetLocalizedText("Choose application language:", langCode);
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
                return;

            if (sender == EnglishRadioButton)
                _currentLanguageCode = "ENGLISH";
            else if (sender == UkrainianRadioButton)
                _currentLanguageCode = "UKRAINIAN";
            else if (sender == PolishRadioButton)
                _currentLanguageCode = "POLISH";
            else if (sender == RussianRadioButton)
                _currentLanguageCode = "RUSSIAN";
        }

        private void OnRowTapped(object sender, TappedEventArgs e)
        {
            string code = e.Parameter?.ToString();
            if (string.IsNullOrEmpty(code))
                return;

            switch (code)
            {
                case "ENGLISH": EnglishRadioButton.IsChecked = true; break;
                case "UKRAINIAN": UkrainianRadioButton.IsChecked = true; break;
                case "POLISH": PolishRadioButton.IsChecked = true; break;
                case "RUSSIAN": RussianRadioButton.IsChecked = true; break;
            }
        }

        private void SetCurrentLanguageCode(string code)
        {
            // If DataCache has a method to set the language, use it.
            // Otherwise, you may need to reload or refresh the cache.
            // Example using Refresh (if it reloads with the new language):
            DataCache.Instance.Refresh(_db);
            // Or, if you have a method like LoadAll that accepts a language:
            // DataCache.Instance.LoadAll(_db, code);
        }

        protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
        {
            // if closing via the close button, the setting was already saved there (avoid double-save)
            if (IsClosingByButton)
            {
                base.OnNavigatingFrom(args);
                return;
            }

            if (_currentLanguageCode != _originalLanguageCode)
            {
                if (await TrySaveChangesAsync("Save changes?", "Apply new language setting?"))
                {
                    // mark the selected language as the active app setting
                    _db.SetAppSettingActive("LANGUAGE", _currentLanguageCode);
                    SetCurrentLanguageCode(_currentLanguageCode);
                    MessagingCenter.Send<object>(this, "SettingsChanged");
                }
            }


            base.OnNavigatingFrom(args);
        }

        protected override async void OnCloseClicked(object sender, EventArgs e)
        {
            IsClosingByButton = true;

            if (_currentLanguageCode != _originalLanguageCode)
            {
                if (await TrySaveChangesAsync("Save changes?", "Apply new language setting?"))
                {
                    // mark the selected language as the active app setting
                    _db.SetAppSettingActive("LANGUAGE", _currentLanguageCode);
                    SetCurrentLanguageCode(_currentLanguageCode);
                    MessagingCenter.Send<object>(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync("..");
        }

    }
}
