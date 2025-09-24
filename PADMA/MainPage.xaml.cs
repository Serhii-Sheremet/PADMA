using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;

namespace PADMA
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel viewModel;
        private Color[] colors = { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Purple };

        public MainPage()
        {
            InitializeComponent();
            viewModel = new CalendarViewModel();
            BindingContext = viewModel;

            UpdateTitle();
            UpdateCalendar();
            AddToolbarButtons();

            // Подписка на изменения настроек
            MessagingCenter.Subscribe<ConfigurationPage>(this, "SettingsChanged", (sender) =>
            {
                // Обновляем календарь с новым первым днем недели
                viewModel.RefreshCalendar();
                UpdateCalendar();
                UpdateTitle();
            });
        }

        private void UpdateTitle()
        {
            Title = new DateTime(viewModel.Year, viewModel.Month, 1).ToString("MMMM yyyy");
        }

        private void UpdateCalendar()
        {
            CalendarGrid.Children.Clear();

            int rows = 6;
            int cols = 7;

            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();
            for (int r = 0; r < rows; r++)
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            for (int c = 0; c < cols; c++)
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < 42; i++)
            {
                var dayItem = viewModel.Days[i];

                Frame dayFrame = new Frame
                {
                    Padding = 0,
                    Margin = 0,
                    HasShadow = false,
                    BorderColor = Colors.Black,
                    CornerRadius = 0,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Colors.White
                };

                Grid dayCell = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(2, GridUnitType.Star) }
                    },
                    RowSpacing = 0,
                    ColumnSpacing = 0
                };

                Label dayLabel = new Label
                {
                    Text = dayItem.DayNumber.ToString(),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 12,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = 2,
                    TextColor = dayItem.IsCurrentMonth ? Colors.Black : Colors.Gray
                };
                dayCell.Add(dayLabel, 0, 0);

                Grid lowerPart = new Grid
                {
                    RowSpacing = 1,
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    }
                };

                for (int j = 0; j < 6; j++)
                {
                    lowerPart.Add(new BoxView
                    {
                        Color = colors[j],
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    }, 0, j);
                }

                dayCell.Add(lowerPart, 0, 1);
                dayFrame.Content = dayCell;

                int row = i / 7;
                int col = i % 7;
                CalendarGrid.Add(dayFrame, col, row);
            }
        }

        private void AddToolbarButtons()
        {
            ToolbarItems.Clear(); // очищаем, чтобы не дублировать

            // Кнопка назад (предыдущий месяц)
            ToolbarItems.Add(new ToolbarItem("<", null, () =>
            {
                viewModel.MoveMonth(-1);
                UpdateCalendar();
                UpdateTitle();
            }));

            // Кнопка вперёд (следующий месяц)
            ToolbarItems.Add(new ToolbarItem(">", null, () =>
            {
                viewModel.MoveMonth(1);
                UpdateCalendar();
                UpdateTitle();
            }));
        }


    }
}