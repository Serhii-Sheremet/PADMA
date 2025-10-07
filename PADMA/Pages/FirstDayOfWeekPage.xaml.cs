using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PADMA.Pages
{
    [QueryProperty(nameof(Date), "Date")]
    public partial class FirstDayOfWeekPage : ContentPage
    {
        private readonly DatabaseService _db;
        private string _originalSettingCode;
        private string _currentSettingCode;
        private bool _isClosingByButton = false;

        public FirstDayOfWeekPage()
        {
            InitializeComponent();

            _db = ServiceLocator.Services.GetService<DatabaseService>();
            LoadCurrentSetting();
        }

        private void LoadCurrentSetting()
        {
            var settings = _db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "WEEK" && x.Active == 1);

            // если нет активного Ч выставл€ем по умолчанию (понедельник)
            _originalSettingCode = active?.SettingCode ?? "WEEKMONDAY";
            _currentSettingCode = _originalSettingCode;

            MondayRadioButton.IsChecked = _currentSettingCode == "WEEKMONDAY";
            SundayRadioButton.IsChecked = _currentSettingCode == "WEEKSUNDAY";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
                return;

            if (sender == MondayRadioButton)
                _currentSettingCode = "WEEKMONDAY";
            else if (sender == SundayRadioButton)
                _currentSettingCode = "WEEKSUNDAY";
        }

        protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
        {
            // если уже выходим через крестик Ч не показываем второй раз диалог
            if (_isClosingByButton)
            {
                base.OnNavigatingFrom(args);
                return;
            }

            // если пользователь просто возвращаетс€ назад стрелкой
            if (_currentSettingCode != _originalSettingCode)
            {
                string titleText = Localization.GetLocalizedText("Save changes?", DataCache.CurrentLanguageCode);
                string messageText = Localization.GetLocalizedText("Apply new setting for first day of week?", DataCache.CurrentLanguageCode);
                string yesText = Localization.GetLocalizedText("Yes", DataCache.CurrentLanguageCode);
                string noText = Localization.GetLocalizedText("No", DataCache.CurrentLanguageCode);
                bool save = await DisplayAlert(
                    titleText,
                    messageText,
                    yesText,
                    noText);

                if (save)
                {
                    _db.SetFirstDayOfWeek(_currentSettingCode);
                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            base.OnNavigatingFrom(args);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            _isClosingByButton = true; // выставл€ем флаг

            if (_currentSettingCode != _originalSettingCode)
            {
                string titleText = Localization.GetLocalizedText("Save changes?", DataCache.CurrentLanguageCode);
                string messageText = Localization.GetLocalizedText("Apply new setting for first day of week?", DataCache.CurrentLanguageCode);
                string yesText = Localization.GetLocalizedText("Yes", DataCache.CurrentLanguageCode);
                string noText = Localization.GetLocalizedText("No", DataCache.CurrentLanguageCode);
                bool save = await DisplayAlert(
                    titleText,
                    messageText,
                    yesText,
                    noText);

                if (save)
                {
                    SaveSetting(); // сохран€ем в Ѕƒ

                    // обновл€ем кэш
                    var cached = _db.GetAppSettingsList().Where(x => x.GroupCode == "WEEK").ToList();
                    foreach (var s in cached)
                        s.Active = s.SettingCode == _currentSettingCode ? 1 : 0;

                    // уведомл€ем главную страницу дл€ обновлени€ календар€
                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync(".."); // возвращаемс€ на страницу конфигурации
        }

        private void SaveSetting()
        {
            var settings = _db.GetAppSettingsList();
            var weekSettings = settings.Where(x => x.GroupCode == "WEEK").ToList();

            // деактивируем все
            foreach (var s in weekSettings)
                s.Active = 0;

            // активируем выбранную настройку
            var selected = weekSettings.FirstOrDefault(x => x.SettingCode == _currentSettingCode);
            if (selected != null)
                selected.Active = 1;

            _db.UpdateAppSettings(weekSettings);
        }
    }
}
