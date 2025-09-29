using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using PADMA.Core.Services;

namespace PADMA
{
    public partial class MainPage : ContentPage
    {
        private readonly CalendarViewModel viewModel;
        private readonly DatabaseService _db;

        public MainPage()
        {
            InitializeComponent();

            // Resolve DatabaseService from DI container
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

            // Example query (shows in VS Output / Logcat)
            var langs = _db.GetLanguages();
            foreach (var lang in langs)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] Language: {lang.LanguageCode}, Culture: {lang.CultureCode}");
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
            if (e.CurrentSelection.FirstOrDefault() is DayItem selectedDay)
            {
                var date = new DateTime(viewModel.Year, viewModel.Month, selectedDay.DayNumber);

                // Открываем DayPage через Shell
                await Shell.Current.GoToAsync(nameof(Pages.DayPage), true,
                    new Dictionary<string, object>
                    {
                { "SelectedDate", date }
                    });

                ((CollectionView)sender).SelectedItem = null;
            }
        }



    }
}
