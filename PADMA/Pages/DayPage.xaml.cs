using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Graphics;

namespace PADMA.Pages
{
    [QueryProperty(nameof(Date), "Date")]
    public partial class DayPage : ContentPage
    {
        private DateTime _date;
        private bool _syncingHorizontalScroll;

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                Title = _date.ToString("dd MMMM yyyy"); // title always selected date
            }
        }

        public DayPage()
        {
            InitializeComponent();
            BindingContext = this;

            BuildTimeScale();
            BuildEventsGrid();
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Close DayPage completely and return to calendar
            await Shell.Current.GoToAsync("//main", true);
        }

        private async void OnTransitBodyScrolled(object? sender, ScrolledEventArgs e)
        {
            if (_syncingHorizontalScroll) return;
            try
            {
                _syncingHorizontalScroll = true;
                await TransitHeaderScroll.ScrollToAsync(e.ScrollX, 0, false);
                await TransitStickyScroll.ScrollToAsync(e.ScrollX, 0, false);
            }
            finally { _syncingHorizontalScroll = false; }
        }

        private async void OnTransitHeaderScrolled(object? sender, ScrolledEventArgs e)
        {
            if (_syncingHorizontalScroll) return;
            try
            {
                _syncingHorizontalScroll = true;
                await TransitBodyScroll.ScrollToAsync(e.ScrollX, 0, false);
                await TransitStickyScroll.ScrollToAsync(e.ScrollX, 0, false);
            }
            finally { _syncingHorizontalScroll = false; }
        }

        private async void OnTransitStickyScrolled(object? sender, ScrolledEventArgs e)
        {
            if (_syncingHorizontalScroll) return;
            try
            {
                _syncingHorizontalScroll = true;
                await TransitBodyScroll.ScrollToAsync(e.ScrollX, 0, false);
                await TransitHeaderScroll.ScrollToAsync(e.ScrollX, 0, false);
            }
            finally { _syncingHorizontalScroll = false; }
        }

        private const int StepMinutes = 15;
        private const double PixelsPerMinute = 1.0;

        private void BuildTimeScale()
        {
            if (TimeScaleLayout == null) return;

            TimeScaleLayout.Children.Clear();

            // Full day height in pixels
            var totalMinutes = 24 * 60;
            var totalHeight = totalMinutes * PixelsPerMinute;

            // Important: set layout height so ScrollView knows content size
            TimeScaleLayout.HeightRequest = totalHeight;

            // Width available inside the time column
            // (we'll keep ticks inside this width)
            var colWidth = 40.0;
            var shortTick = 10.0;
            var midTick = 18.0;
            var fullTick = colWidth; // full width for full hour

            for (int minute = 0; minute <= totalMinutes; minute += StepMinutes)
            {
                var y = minute * PixelsPerMinute;

                bool isHour = (minute % 60 == 0);
                bool isHalfHour = (!isHour && minute % 30 == 0);

                var tickWidth = isHour ? fullTick : (isHalfHour ? midTick : shortTick);
                var tickOpacity = isHour ? 0.55 : (isHalfHour ? 0.35 : 0.25);

                //var lineColor = (Color)Application.Current.Resources["TimelineLineColor"];
                var lineColor = Colors.DeepSkyBlue; // fallback
                if (Application.Current?.Resources.TryGetValue("TimelineLineColor", out var v) == true)
                {
                    if (v is Color c) lineColor = c;
                }

                // Tick line (horizontal)
                var tick = new BoxView
                {
                    Color = lineColor,
                    Opacity = tickOpacity,
                    HeightRequest = 1,
                    WidthRequest = tickWidth
                };

                // Place tick RIGHT-aligned in the time column
                var x = colWidth - tickWidth;
                AbsoluteLayout.SetLayoutBounds(tick, new Rect(x, y, tickWidth, 1));
                AbsoluteLayout.SetLayoutFlags(tick, AbsoluteLayoutFlags.None);
                TimeScaleLayout.Children.Add(tick);

                // Hour label only at full hours
                if (isHour && minute < totalMinutes)
                {
                    var t = TimeSpan.FromMinutes(minute);

                    const double labelHeight = 14;     // под FontSize 11Ц12 обычно ок
                    const double labelPaddingTop = 0;  // небольшой зазор над линией

                    var label = new Label
                    {
                        Text = $"{(int)t.TotalHours:00}:00",
                        FontSize = 11,
                        TextColor = Colors.Black,
                        Opacity = 0.80,
                        Padding = new Thickness(2, 0, 0, 0),
                        VerticalTextAlignment = TextAlignment.End
                    };

                    var labelY = Math.Max(0, y - labelHeight - labelPaddingTop);

                    AbsoluteLayout.SetLayoutBounds(label, new Rect(0, labelY, colWidth, labelHeight));
                    AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.None);
                    TimeScaleLayout.Children.Add(label);
                }

            }
        }

        private void BuildEventsGrid()
        {
            if (EventsGridLayout == null) return;

            EventsGridLayout.Children.Clear();

            var totalMinutes = 24 * 60;
            var totalHeight = totalMinutes * PixelsPerMinute;
            EventsGridLayout.HeightRequest = totalHeight;

            var width = 80.0; // должна совпадать с WidthRequest events-колонки

            // ѕытаемс€ вз€ть тот же цвет, что и дл€ шкалы (если ресурс не используем Ч можно поставить FromArgb)
            var lineColor = Color.FromArgb("#1E88E5");

            for (int minute = 0; minute <= totalMinutes; minute += StepMinutes)
            {
                var y = minute * PixelsPerMinute;

                bool isHour = (minute % 60 == 0);
                bool isHalfHour = (!isHour && minute % 30 == 0);

                // Events grid делаем более м€гкой, чем сама шкала
                var opacity = isHour ? 0.20 : (isHalfHour ? 0.12 : 0.06);

                var line = new BoxView
                {
                    Color = lineColor,
                    Opacity = opacity,
                    HeightRequest = 1,
                    WidthRequest = width
                };

                AbsoluteLayout.SetLayoutBounds(line, new Rect(0, y, width, 1));
                AbsoluteLayout.SetLayoutFlags(line, AbsoluteLayoutFlags.None);
                EventsGridLayout.Children.Add(line);
            }
        }






    }
}
