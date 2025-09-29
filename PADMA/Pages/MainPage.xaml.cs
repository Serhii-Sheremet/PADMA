using Microsoft.Maui.Controls;
using System;
using PADMA.Core.ViewModels;
using PADMA.UI.Models;

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

            // Обновление календаря при изменении настроек
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", _ =>
            {
                Vm?.RefreshCalendar();
                UpdateTitle();
            });

            UpdateTitle();
            AddToolbarButtons();
        }

        private void UpdateTitle()
        {
            if (Vm == null) return;
            Title = new DateTime(Vm.Year, Vm.Month, 1).ToString("MMMM yyyy");
        }

        private void AddToolbarButtons()
        {
            ToolbarItems.Clear();

            ToolbarItems.Add(new ToolbarItem("<", null, () =>
            {
                Vm?.MoveMonth(-1);
                UpdateTitle();
            }));

            ToolbarItems.Add(new ToolbarItem(">", null, () =>
            {
                Vm?.MoveMonth(1);
                UpdateTitle();
            }));
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            var selected = e.CurrentSelection[0] as DayItem;
            if (selected == null)
                return;

            // Сбрасываем выделение, чтобы не «залипало»
            ((CollectionView)sender).SelectedItem = null;

            // Навигация на DayPage, заголовок = реальная дата
            await Shell.Current.GoToAsync($"day?date={selected.Date:yyyy-MM-dd}");
        }
    }
}
