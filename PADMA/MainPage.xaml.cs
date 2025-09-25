using Microsoft.Maui.Controls;
using System;

namespace PADMA
{
    public partial class MainPage : ContentPage
    {
        private readonly CalendarViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
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
