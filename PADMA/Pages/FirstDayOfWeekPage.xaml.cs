using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
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

            // если нет активного — выставляем по умолчанию (понедельник)
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

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            if (_currentSettingCode != _originalSettingCode)
            {
                bool save = await DisplayAlert("Save changes?", "Apply new setting for first day of week?", "Yes", "No");
                if (save)
                {
                    _db.SetFirstDayOfWeek(_currentSettingCode);

                    // уведомляем главную страницу
                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync(".."); // просто закрываем страницу
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
