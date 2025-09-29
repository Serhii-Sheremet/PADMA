using Microsoft.Maui.Controls;
using System;
using PADMA.UI.Models;
using PADMA.Core.Services;

namespace PADMA.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly CalendarViewModel viewModel;
        private readonly DatabaseService _db;

        public MainPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db;

            viewModel = new CalendarViewModel();
            BindingContext = viewModel;

            UpdateTitle();
            AddToolbarButtons();

            // Событие изменения настроек
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", _ =>
            {
                viewModel.RefreshCalendar();
                UpdateTitle();
            });

            // Тест: загрузка языков
            var langs = _db.GetLanguages();
            foreach (var lang in langs)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] Language: {lang.LanguageCode}, Culture: {lang.CultureCode}");
            }

            // Обработка выбора дня
            CalendarCollection.SelectionChanged += OnDaySelected;
        }

        private async void OnDaySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is DayItem selectedDay)
            {
                await Shell.Current.GoToAsync($"day?date={selectedDay.Date:O}");
            }
        }

        private void UpdateTitle()
        {
            Title = new DateTime(viewModel.Year, viewModel.Month, 1).ToString("MMMM yyyy");
        }

        private void AddToolbarButtons()
        {
            ToolbarItems.Clear();

            ToolbarItems.Add(new ToolbarItem("<", null, () =>
            {
                viewModel.MoveMonth(-1);
                UpdateTitle();
            }));

            ToolbarItems.Add(new ToolbarItem(">", null, () =>
            {
                viewModel.MoveMonth(1);
                UpdateTitle();
            }));
        }
    }
}
