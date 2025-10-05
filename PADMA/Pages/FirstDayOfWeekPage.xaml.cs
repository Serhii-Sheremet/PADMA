using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Extensions.DependencyInjection;
using PADMA.Core.Services;
using PADMA.Core.Models;

namespace PADMA.Pages
{
    public partial class FirstDayOfWeekPage : ContentPage
    {
        private readonly DatabaseService _db;

        // ID активной настройки на входе
        private int _originalSettingId;

        // ID выбранной (потенциально новой) настройки
        private int _pendingSettingId;

        public FirstDayOfWeekPage()
        {
            InitializeComponent();

            _db = ServiceLocator.Services.GetRequiredService<DatabaseService>();
            LoadCurrent();
        }

        private void LoadCurrent()
        {
            // Берём активную настройку из группы WEEK
            var list = _db.GetAppSettingsList();
            var active = list.FirstOrDefault(x => x.GroupCode == "WEEK" && x.Active == 1);

            _originalSettingId = active?.Id ?? 0;
            _pendingSettingId = _originalSettingId;

            // Код настройки: WEEKMONDAY / WEEKSUNDAY
            var code = active?.Code ?? "WEEKMONDAY";

            MondayRadio.IsChecked = code == "WEEKMONDAY";
            SundayRadio.IsChecked = code == "WEEKSUNDAY";
        }

        // ВАЖНО: правильная сигнатура для RadioButton.CheckedChanged
        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return; // реагируем только когда кнопка стала Checked = true

            if (sender is RadioButton rb)
            {
                var code = rb.Value?.ToString();
                if (string.IsNullOrEmpty(code)) return;

                var list = _db.GetAppSettingsList();
                var found = list.FirstOrDefault(x => x.GroupCode == "WEEK" && x.Code == code);
                if (found != null)
                    _pendingSettingId = found.Id;
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Если ничего не поменяли — просто уходим назад
            if (_pendingSettingId == _originalSettingId)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            var save = await DisplayAlert("Apply changes?",
                                          "Save and apply the new first day of week?",
                                          "Yes", "No");
            if (save)
            {
                // Сбрасываем группу и активируем выбранный вариант
                _db.DeactivateGroup("WEEK");
                _db.ActivateSetting(_pendingSettingId);

                // Сообщаем наверх (ConfigurationPage/MainPage и т.п.)
                MessagingCenter.Send(this, "SettingsChanged");
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}
