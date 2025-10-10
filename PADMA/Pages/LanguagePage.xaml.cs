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

            // если нет активного языка — по умолчанию ENGLISH
            _originalLanguageCode = active?.SettingCode ?? "ENGLISH";
            _currentLanguageCode = _originalLanguageCode;

            // выставляем активный радиобаттон
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
            // если выходим через крестик — пропускаем (обработка будет там)
            if (IsClosingByButton)
            {
                base.OnNavigatingFrom(args);
                return;
            }

            if (_currentLanguageCode != _originalLanguageCode)
            {
                if (await TrySaveChangesAsync("Save changes?", "Apply new language setting?"))
                {
                    // сохраняем выбранный язык и обновляем кеш
                    _db.SetLanguage(_currentLanguageCode);
                    SetCurrentLanguageCode(_currentLanguageCode);
                    //DataCache.Instance.CurrentLanguageCode = _currentLanguageCode;
                    MessagingCenter.Send(this, "SettingsChanged");
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
                    // сохраняем выбранный язык и обновляем кеш
                    _db.SetLanguage(_currentLanguageCode);
                    SetCurrentLanguageCode(_currentLanguageCode);
                    //DataCache.Instance.CurrentLanguageCode = _currentLanguageCode;
                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync("..");
        }

        private void ApplyLanguageChange()
        {
            var settings = _db.GetAppSettingsList();
            var languageSettings = settings.Where(x => x.GroupCode == "LANGUAGE").ToList();

            // сбрасываем все
            foreach (var s in languageSettings)
                s.Active = 0;

            // активируем выбранный язык
            var selected = languageSettings.FirstOrDefault(x => x.SettingCode == _currentLanguageCode);
            if (selected != null)
                selected.Active = 1;

            _db.UpdateAppSettings(languageSettings);

            // обновляем кэш
            var cached = _db.GetAppSettingsList().Where(x => x.GroupCode == "LANGUAGE").ToList();
            foreach (var s in cached)
                s.Active = s.SettingCode == _currentLanguageCode ? 1 : 0;

            // меняем глобальный язык в DataCache
            //DataCache.Instance.CurrentLanguageCode = _currentLanguageCode 
            SetCurrentLanguageCode(_currentLanguageCode);
            //switch
            //{
            //    "UKRAINIAN" => "uk",
            //    "POLISH" => "pl",
            //    "RUSSIAN" => "ru",
            //    _ => "en"
            //};

            // уведомляем систему
            MessagingCenter.Send(this, "SettingsChanged");
        }
    }
}
