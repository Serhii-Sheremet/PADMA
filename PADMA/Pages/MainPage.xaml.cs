using Microsoft.Maui.Controls;
using System;
using PADMA.Core.Services;
using PADMA.Core.Models;
using PADMA.UI.ViewModels;

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

            // Подписка на изменения настроек
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", (sender) =>
            {
                viewModel.RefreshCalendar();
                UpdateTitle();
            });

            // Вывод языков в Output (для проверки)
            var langs = _db.GetLanguages();
            foreach (var lang in langs)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] Language: {lang.LanguageCode}, Culture: {lang.CultureCode}");
            }

            // Обработчик выбора дня
            CalendarCollection.SelectionChanged += async (s, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is DayItem selectedDay)
                {
                    await Shell.Current.GoToAsync("day", new Dictionary<string, object>
                    {
                        { "SelectedDay", selectedDay }
                    });
                }
            };
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

        private async void OnDaySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is PADMA.Models.DayItem day)
            {
                await Shell.Current.GoToAsync($"day?date={day.Date:yyyy-MM-dd}");
            }
        }


    }
}
