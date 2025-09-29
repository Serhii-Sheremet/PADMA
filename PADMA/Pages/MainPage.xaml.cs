using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using PADMA.Core.Services;
using PADMA.Pages;

namespace PADMA
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
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", _ =>
            {
                viewModel.RefreshCalendar();
                UpdateTitle();
            });

            // Тестовый вывод языков
            var langs = _db.GetLanguages();
            foreach (var lang in langs)
            {
                Debug.WriteLine($"[PADMA] Language: {lang.LanguageCode}, Culture: {lang.CultureCode}");
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
                CalendarCollection.SelectedItem = null; // сброс выбора
                UpdateTitle();
            }));

            ToolbarItems.Add(new ToolbarItem(">", null, () =>
            {
                viewModel.MoveMonth(1);
                CalendarCollection.SelectedItem = null; // сброс выбора
                UpdateTitle();
            }));
        }

        // Обработчик выбора дня
        private async void OnDayTapped(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.CurrentSelection?.Count > 0 ? e.CurrentSelection[0] as DayItem : null;
            if (selected == null)
                return;

            // убираем выделение
            if (sender is CollectionView cv)
                cv.SelectedItem = null;

            // вычисляем дату
            var dateToShow = new DateTime(viewModel.Year, viewModel.Month, selected.DayNumber);

            // переход на страницу дня
            await Shell.Current.GoToAsync("//day", new Dictionary<string, object>
            {
                { "SelectedDate", dateToShow }
            });
        }
    }
}
