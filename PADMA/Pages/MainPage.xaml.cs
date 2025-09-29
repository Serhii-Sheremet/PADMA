using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using System;

namespace PADMA.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly CalendarViewModel viewModel;
        private readonly DatabaseService _db;

        public MainPage()
        {
            InitializeComponent();

            // Resolve services
            _db = ServiceLocator.Services.GetRequiredService<DatabaseService>();

            viewModel = new CalendarViewModel();
            BindingContext = viewModel;

            UpdateTitle();
            AddToolbarButtons();

            // Settings changed
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", _ =>
            {
                viewModel.RefreshCalendar();
                UpdateTitle();
            });

            // Example debug: languages (visible in VS Output / Logcat)
            try
            {
                var langs = _db.GetLanguages();
                foreach (var lang in langs)
                    System.Diagnostics.Debug.WriteLine($"[PADMA] Language: {lang.LanguageCode}, Culture: {lang.CultureCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DB check failed: {ex}");
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

        private async void OnDaySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.Count > 0 && e.CurrentSelection[0] is DayItem day)
            {
                // pass date via query (yyyy-MM-dd)
                var todayLike = new DateTime(viewModel.Year, viewModel.Month, day.DayNumber);
                var dateStr = todayLike.ToString("yyyy-MM-dd");

                await Shell.Current.GoToAsync($"day?date={Uri.EscapeDataString(dateStr)}");
            }
        }
    }
}
