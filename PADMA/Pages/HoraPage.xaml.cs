using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System;
using System.Linq;

namespace PADMA.Pages
{
    public partial class HoraPage : ConfigBasePage
    {
        private string _originalSettingCode;
        private string _currentSettingCode;

        public HoraPage()
        {
            InitializeComponent();
            ApplyLocalization();
            LoadCurrentSetting();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            Title = Localization.GetLocalizedText("Hora", lang);
            HeaderLabel.Text = Localization.GetLocalizedText("Choose Hora calculation mode:", lang);

            DayNightLabel.Text = Localization.GetLocalizedText(
                "From Sunrise to Sunset (1/12) + From Sunset to Sunrise (1/12)", lang);
            EqualLabel.Text = Localization.GetLocalizedText(
                "From Sunrise to Sunrise (1/24)", lang);
            From6Label.Text = Localization.GetLocalizedText(
                "From 6:00 a.m. (Hora = 1 hour)", lang);
        }

        private void LoadCurrentSetting()
        {
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            var settings = db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "HORA" && x.Active == 1);

            _originalSettingCode = active?.SettingCode ?? "HORAEQUAL";
            _currentSettingCode = _originalSettingCode;

            DayNightRadioButton.IsChecked = _originalSettingCode == "HORADAYNIGHT";
            EqualRadioButton.IsChecked = _originalSettingCode == "HORAEQUAL";
            From6RadioButton.IsChecked = _originalSettingCode == "HORAFROM6";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            if (sender == DayNightRadioButton)
                _currentSettingCode = "HORADAYNIGHT";
            else if (sender == EqualRadioButton)
                _currentSettingCode = "HORAEQUAL";
            else if (sender == From6RadioButton)
                _currentSettingCode = "HORAFROM6";
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            if (_currentSettingCode == _originalSettingCode)
                return;

            var lang = DataCache.Instance.CurrentLanguageCode;

            bool save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply new settings for Hora calculation?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (save)
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                db.SetAppSettingActive("HORA", _currentSettingCode);
                DataCache.Instance.Refresh(db);
                MessagingCenter.Send<object>(this, "SettingsChanged");
            }
        }
    }
}
