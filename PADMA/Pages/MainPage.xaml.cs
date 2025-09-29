using Microsoft.Maui.Controls;
using PADMA.UI.Models;

namespace PADMA.Pages
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
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is DayItem selectedDay)
            {
                // переход на DayPage и передаём дату
                await Shell.Current.GoToAsync("day", true,
                    new Dictionary<string, object>
                    {
                        { "Date", selectedDay.Date }
                    });
            }

            // сброс выделения (убираем системный оранжевый хайлайт)
            ((CollectionView)sender).SelectedItem = null;
        }

        private void UpdateTitle()
        {
            Title = new DateTime(viewModel.Year, viewModel.Month, 1)
                .ToString("MMMM yyyy");
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
