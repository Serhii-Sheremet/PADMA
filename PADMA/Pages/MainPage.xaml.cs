using Microsoft.Maui.Controls;
using System;
using PADMA;
using PADMA.Services;

namespace PADMA
{
    public partial class MainPage : ContentPage
    {
        private readonly CalendarViewModel viewModel;
        private readonly DatabaseService _db;

        public MainPage()
        {
            InitializeComponent();

            // создаём сервис базы прямо здесь
            _db = new DatabaseService();

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

            // тестовый вывод языков из базы
            var langs = _db.GetLanguages();
            foreach (var lang in langs)
            {
                Console.WriteLine($"Language: {lang.LanguageCode}, Culture: {lang.CultureCode}");
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
