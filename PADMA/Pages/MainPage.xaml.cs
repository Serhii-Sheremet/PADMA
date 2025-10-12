using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.Linq;
using PADMA.UI;
using PADMA.Core.Services;

namespace PADMA.Pages
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel Vm => BindingContext as CalendarViewModel;

        public MainPage()
        {
            InitializeComponent();

            if (BindingContext is not CalendarViewModel)
                BindingContext = new CalendarViewModel();

            // Инициализация культуры
            Vm.InitializeCulture();

            // Подписка на изменения настроек из ConfigurationPage
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", _ =>
            {
                Vm?.RefreshCalendar();
                UpdateTitle();
                UpdateDaysHeader();
            });

            // Подписка на изменения настроек из FirstDayOfWeekPage
            MessagingCenter.Subscribe<FirstDayOfWeekPage>(this, "SettingsChanged", _ =>
            {
                Vm?.RefreshCalendar();
                UpdateTitle();
                UpdateDaysHeader();
            });

            UpdateTitle();
            AddToolbarButtons();
            UpdateDaysHeader();
        }

        private void UpdateTitle()
        {
            if (Vm == null) return;
            Title = new DateTime(Vm.Year, Vm.Month, 1).ToString("MMMM yyyy", Vm.CurrentCulture);
        }

        private void AddToolbarButtons()
        {
            ToolbarItems.Clear();

            var prev = new ToolbarItem
            {
                IconImageSource = "left_arrow.png",
                Text = "Prev"
            };
            prev.Clicked += (s, e) =>
            {
                Vm?.MoveMonth(-1);
                UpdateTitle();
                UpdateDaysHeader();
            };

            var next = new ToolbarItem
            {
                IconImageSource = "right_arrow.png",
                Text = "Next"
            };
            next.Clicked += (s, e) =>
            {
                Vm?.MoveMonth(1);
                UpdateTitle();
                UpdateDaysHeader();
            };

            ToolbarItems.Add(prev);
            ToolbarItems.Add(next);
        }

        private void UpdateDaysHeader()
        {
            if (Vm == null || DaysHeaderGrid == null)
                return;

            DaysHeaderGrid.Children.Clear();

            var culture = Vm.CurrentCulture;
            var dtf = culture.DateTimeFormat;
            var abbreviated = dtf.AbbreviatedDayNames
                .Select(d => (d.Length > 3 ? d.Substring(0, 3) : d).ToUpper(culture))
                .ToArray();

            // Получаем первый день недели из настроек
            var firstDay = Vm != null
                ? ServiceLocator.Services.GetService<DatabaseService>().GetFirstDayOfWeekFromDb()
                : dtf.FirstDayOfWeek;

            var ordered = Enumerable.Range(0, 7)
                .Select(i => abbreviated[((int)firstDay + i) % 7])
                .ToArray();

            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label
                {
                    Text = ordered[i],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    TextColor = Colors.Black
                };
                Grid.SetColumn(lbl, i);
                DaysHeaderGrid.Children.Add(lbl);
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            var selected = e.CurrentSelection[0] as DayItem;
            if (selected == null)
                return;

            ((CollectionView)sender).SelectedItem = null;

            await Shell.Current.GoToAsync($"day?Date={selected.Date:yyyy-MM-dd}");
        }
    }
}
