using Microsoft.Maui.Controls;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace PADMA.Pages
{
    public partial class FirstDayOfWeekPage : ConfigBasePage
    {
        private readonly DatabaseService _db;
        private string _originalSettingCode;
        private string _currentSettingCode;
        private bool _isClosingByButton = false;

        public FirstDayOfWeekPage()
        {
            InitializeComponent();
            _db = ServiceLocator.Services.GetService<DatabaseService>();

            Title = Localization.GetLocalizedText("First day of week", DataCache.CurrentLanguageCode);
            HeaderLabel.Text = Title;

            ApplyLocalizedDayLabels();
            LoadCurrentSetting();
        }

        private void ApplyLocalizedDayLabels()
        {
            var culture = new CultureInfo(DataCache.CurrentLanguageCode);
            MondayLabel.Text = culture.DateTimeFormat.GetDayName(DayOfWeek.Monday);
            SundayLabel.Text = culture.DateTimeFormat.GetDayName(DayOfWeek.Sunday);
        }

        private void LoadCurrentSetting()
        {
            var settings = _db.GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "WEEK" && x.Active == 1);
            _originalSettingCode = active?.SettingCode ?? "WEEKMONDAY";
            _currentSettingCode = _originalSettingCode;

            MondayRadioButton.IsChecked = _currentSettingCode == "WEEKMONDAY";
            SundayRadioButton.IsChecked = _currentSettingCode == "WEEKSUNDAY";
        }

        private void OnRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            _currentSettingCode = sender == MondayRadioButton ? "WEEKMONDAY" : "WEEKSUNDAY";
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
                if (await TrySaveChangesAsync("Save changes?", "Apply new setting for first day of week?"))
                {
                    _db.SetFirstDayOfWeek(_currentSettingCode);
                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            base.OnNavigatingFrom(args);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            _isClosingByButton = true;

            if (_currentSettingCode != _originalSettingCode)
            {
                if (await TrySaveChangesAsync("Save changes?", "Apply new setting for first day of week?"))
                {
                    SaveSetting();
                    var cached = _db.GetAppSettingsList().Where(x => x.GroupCode == "WEEK").ToList();
                    foreach (var s in cached)
                        s.Active = s.SettingCode == _currentSettingCode ? 1 : 0;

                    MessagingCenter.Send(this, "SettingsChanged");
                }
            }

            await Shell.Current.GoToAsync("..");
        }

        private void SaveSetting()
        {
            var settings = _db.GetAppSettingsList();
            var weekSettings = settings.Where(x => x.GroupCode == "WEEK").ToList();

            foreach (var s in weekSettings)
                s.Active = 0;

            var selected = weekSettings.FirstOrDefault(x => x.SettingCode == _currentSettingCode);
            if (selected != null)
                selected.Active = 1;

            _db.UpdateAppSettings(weekSettings);
        }
    }
}
