using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System.Linq;
using System;

namespace PADMA.Pages
{
    public partial class TransitsPage : ConfigBasePage
    {
        private const string GROUP = "TRANSIT";
        private const string CODE_MOON = "MOON";
        private const string CODE_ASC = "LAGNA";
        private const string CODE_BOTH = "MOONANDLAGNA";

        private string _originalSettingCode;
        private string _currentSettingCode;

        private readonly DatabaseService _db;

        public TransitsPage()
        {
            InitializeComponent();

            _db = ServiceLocator.Services.GetService<DatabaseService>();

            LoadCurrentSetting();
            ApplyLocalization();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            // Титул страницы
            Title = Localization.GetLocalizedText("Planetary transits", lang);

            // Заголовок/инструкция
            HeaderLabel.Text = Localization.GetLocalizedText("Choose how to display planetary transits:", lang);

            // Подписи к опциям
            LblFromMoon.Text = Localization.GetLocalizedText("From natal Moon", lang);
            LblFromAsc.Text = Localization.GetLocalizedText("From Ascendant (Lagna)", lang);
            LblFromBoth.Text = Localization.GetLocalizedText("From both natal Moon and Ascendant", lang);
        }

        private void LoadCurrentSetting()
        {
            var settings = _db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == GROUP && x.Active == 1);

            _originalSettingCode = active?.SettingCode ?? CODE_MOON;
            _currentSettingCode = _originalSettingCode;

            // Устанавливаем активную радиокнопку
            RbFromMoon.IsChecked = _currentSettingCode == CODE_MOON;
            RbFromAsc.IsChecked = _currentSettingCode == CODE_ASC;
            RbFromBoth.IsChecked = _currentSettingCode == CODE_BOTH;
        }

        private void OnOptionCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == RbFromMoon)
                _currentSettingCode = CODE_MOON;
            else if (sender == RbFromAsc)
                _currentSettingCode = CODE_ASC;
            else if (sender == RbFromBoth)
                _currentSettingCode = CODE_BOTH;
        }

        // Унифицированное поведение при выходе со страницы
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                // Проверяем, были ли реальные изменения
                if (_currentSettingCode == _originalSettingCode)
                    return;

                var lang = DataCache.Instance.CurrentLanguageCode;
                bool save = await DisplayAlert(
                    Localization.GetLocalizedText("Save changes?", lang),
                    Localization.GetLocalizedText("Apply new settings for planetary transits display?", lang),
                    Localization.GetLocalizedText("Yes", lang),
                    Localization.GetLocalizedText("No", lang)
                );

                if (!save)
                {
                    // Откатываем выбор до исходного состояния
                    LoadCurrentSetting();
                    return;
                }

                // Сохраняем изменения в БД
                _db.SetAppSettingActive("TRANSIT", _currentSettingCode);

                // Обновляем кэш
                DataCache.Instance.Refresh(_db);

                // Отправляем уведомление вверх (в ConfigurationPage и MainPage)
                MessagingCenter.Send<object>(this, "SettingsChanged");

                _originalSettingCode = _currentSettingCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] TransitsPage.OnDisappearing error: {ex.Message}");
            }
        }
    }
}
