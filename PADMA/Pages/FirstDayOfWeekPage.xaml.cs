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

            // локализуем титул и радиокнопки
            LocalizePageTexts();

            _db = ServiceLocator.Services.GetService<DatabaseService>();
            LoadCurrentSetting();
        }

        private void LocalizePageTexts()
        {
            string lang = DataCache.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("First day of week", lang);
            PageTitle.Text = Localization.GetLocalizedText("Specify the first day of a week", lang);
            MondayRadioButton.Content = Localization.GetLocalizedText("Monday", lang);
            SundayRadioButton.Content = Localization.GetLocalizedText("Sunday", lang);
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
            if (_isClosingByButton)
            {
                base.OnNavigatingFrom(args);
                return;
            }

            await TrySaveChangesAsync();
            base.OnNavigatingFrom(args);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            _isClosingByButton = true;
            await TrySaveChangesAsync();
            await Shell.Current.GoToAsync("..");
        }

        private async Task TrySaveChangesAsync()
        {
            if (_currentSettingCode == _originalSettingCode)
                return;

            string lang = DataCache.CurrentLanguageCode;
            string title = Localization.GetLocalizedText("Save changes?", lang);
            string message = Localization.GetLocalizedText("Apply new setting for first day of week?", lang);
            string yesText = Localization.GetLocalizedText("Yes", lang);
            string noText = Localization.GetLocalizedText("No", lang);

            bool save = await DisplayAlert(title, message, yesText, noText);
            if (!save)
                return;

            // обновл€ем в базе
            SaveSetting();

            // обновл€ем кэш
            var cached = _db.GetAppSettingsList().Where(x => x.GroupCode == "WEEK").ToList();
            foreach (var s in cached)
                s.Active = s.SettingCode == _currentSettingCode ? 1 : 0;

            // уведомл€ем главную страницу
            MessagingCenter.Send(this, "SettingsChanged");
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
