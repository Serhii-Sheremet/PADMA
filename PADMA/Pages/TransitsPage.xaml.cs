using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System.Linq;

namespace PADMA.Pages
{
    public partial class TransitsPage : ConfigBasePage
    {
        private const string GROUP = "TRANSIT";
        private const string CODE_MOON = "TRANSIT_MOON";
        private const string CODE_ASC = "TRANSIT_ASC";
        private const string CODE_BOTH = "TRANSIT_BOTH";

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

            // Титул страницы в тулбаре
            Title = Localization.GetLocalizedText("Planetary transits", lang);

            // Заголовок/инструкция
            HeaderLabel.Text = Localization.GetLocalizedText("Choose how to display planet transits:", lang);

            // Подписи опций
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

            // Устанавливаем выбранную радиокнопку
            RbFromMoon.IsChecked = _currentSettingCode == CODE_MOON;
            RbFromAsc.IsChecked = _currentSettingCode == CODE_ASC;
            RbFromBoth.IsChecked = _currentSettingCode == CODE_BOTH;
        }

        private async void OnOptionCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return; // реагируем только на включение

            if (sender == RbFromMoon) _currentSettingCode = CODE_MOON;
            else if (sender == RbFromAsc) _currentSettingCode = CODE_ASC;
            else if (sender == RbFromBoth) _currentSettingCode = CODE_BOTH;

            if (_currentSettingCode == _originalSettingCode)
                return;

            var lang = DataCache.Instance.CurrentLanguageCode;
            var ok = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply the selected option?", lang), // если этой строки нет — можно убрать второй параметр или добавить в APP_TEXTS
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (!ok)
            {
                // Откатываем визуально
                LoadCurrentSetting();
                return;
            }

            // Сохраняем выбор в БД
            _db.DeactivateGroup(GROUP);
            var settings = _db.GetAppSettingsList();
            var newSetting = settings.FirstOrDefault(x => x.GroupCode == GROUP && x.SettingCode == _currentSettingCode);
            if (newSetting != null)
                _db.ActivateSetting(newSetting.Id);

            // Обновляем кэш и уведомляем хаб/главную
            DataCache.Instance.Refresh(_db);
            MessagingCenter.Send<object>(this, "SettingsChanged");

            _originalSettingCode = _currentSettingCode;
        }
    }
}
