using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PADMA.Pages
{
    [QueryProperty(nameof(Day), "Day")]
    public partial class DayPage : ContentPage, IQueryAttributable
    {
        private bool _autoCenterRequested;
        private bool _syncingHorizontalScroll;
        public DateTime? SunriseUtc { get; private set; }
        public DateTime? SunsetUtc { get; private set; }

        public sealed class TransitColumnVm : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

            public int LineId { get; init; }
            public string Code { get; init; } = string.Empty;
            public string ShortName { get; init; } = string.Empty;
            public string Name { get; init; } = string.Empty;

            string _stickyText = string.Empty;
            public string StickyText
            {
                get => _stickyText;
                set { if (_stickyText == value) return; _stickyText = value; Raise(); }
            }
        }

        // То, к чему привязывается XAML (Header + Body)
        public ObservableCollection<TransitColumnVm> TransitColumns { get; } = new();

        private DayItem? _day;
        public DayItem? Day
        {
            get => _day;
            private set
            {
                _day = value;
                ApplyLocalizedLabels();
                OnPropertyChanged(nameof(Day));
            }
        }

        private void ApplyLocalizedLabels()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var culture = new CultureInfo(lang);
            if (Day != null)
                Title = Day.Date.ToString("dd MMMM yyyy", culture);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Day", out var d) && d is DayItem day)
                Day = day;

            if (query.TryGetValue("SunriseUtc", out var sr) && sr is DateTime sunriseUtc)
                SunriseUtc = DateTime.SpecifyKind(sunriseUtc, DateTimeKind.Utc);

            if (query.TryGetValue("SunsetUtc", out var ss) && ss is DateTime sunsetUtc)
                SunsetUtc = DateTime.SpecifyKind(sunsetUtc, DateTimeKind.Utc);

            // после получения параметров можно применить фон
            ApplyDayNightBackgroundIfPossible();

            // тут же можно вызвать построение данных/блоков транзитов, уже имея Day
            RenderNakshatraLane();
            UpdateStickyForNakshatra(0); // старт в начале суток

            RequestAutoCenter(); 
        }

        private void RequestAutoCenter()
        {
            _autoCenterRequested = true;

            Dispatcher.Dispatch(async () =>
            {
                // дать странице разложиться
                await Task.Delay(50);

                if (!_autoCenterRequested) return;
                _autoCenterRequested = false;

                await CenterOnNowIfTodayAsync();
            });
        }

        public DayPage()
        {
            InitializeComponent();
            BindingContext = this;

            BuildTransitColumns();
            BuildTimeScale();
            BuildEventsGrid();
        }

        private bool IsTodayForProfile(DayItem day)
        {
            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (ctx?.TimeZoneInfo != null)
                tzInfo = ctx.TimeZoneInfo;
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo).Date;

            var dayLocal = day.Date;
            if (dayLocal.Kind == DateTimeKind.Utc)
                dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayLocal, tzInfo);
            else
                dayLocal = dayLocal; // Unspecified считаем уже локальной датой

            return dayLocal.Date == nowLocal;
        }
        
        private void OnTimelineScrolled(object? sender, ScrolledEventArgs e)
        {
            // e.ScrollY — это то, что нужно для sticky
            UpdateStickyForNakshatra(e.ScrollY);


        }

        private async Task CenterOnNowIfTodayAsync()
        {
            if (Day == null) return;

            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (ctx?.TimeZoneInfo != null)
                tzInfo = ctx.TimeZoneInfo;

            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo);

            // центрируем только если открыли сегодняшний день
            if (!IsTodayForProfile(Day)) return;

            var minute = nowLocal.TimeOfDay.TotalMinutes;
            var y = minute * PixelsPerMinute;

            var viewport = TimelineScroll.Height;
            if (viewport <= 0) viewport = 600;

            var target = y - viewport / 2;
            if (target < 0) target = 0;

            var maxTarget = Math.Max(0, TimelineHeight - viewport);
            if (target > maxTarget) target = maxTarget;

            await TimelineScroll.ScrollToAsync(0, target, false);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Close DayPage completely and return to calendar
            await Shell.Current.GoToAsync("//main", true);
        }
        
        private void ApplyDayNightBackgroundIfPossible()
        {
            if (SunriseUtc == null || SunsetUtc == null)
                return;

            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (ctx?.TimeZoneInfo != null)
                tzInfo = ctx.TimeZoneInfo;

            if (tzInfo == null)
                return;

            var sunriseLocal = TimeZoneInfo.ConvertTimeFromUtc(SunriseUtc.Value, tzInfo);
            var sunsetLocal = TimeZoneInfo.ConvertTimeFromUtc(SunsetUtc.Value, tzInfo);

            var ySunrise = sunriseLocal.TimeOfDay.TotalMinutes * PixelsPerMinute;
            var ySunset = sunsetLocal.TimeOfDay.TotalMinutes * PixelsPerMinute;

            var dayColor = Color.FromArgb("#EAF6FF");
            var nightColor = Color.FromArgb("#D7ECFF");

            // фон для Time/Events через AbsoluteLayout
            BuildColumnBackground(TimeBackgroundLayout, ySunrise, ySunset, dayColor, nightColor, 40);
            BuildColumnBackground(EventsBackgroundLayout, ySunrise, ySunset, dayColor, nightColor, 80);

            // фон для TransitBodyGrid через BoxView
            TransitNightBackground.Color = nightColor;
            TransitNightBackground.HeightRequest = TimelineHeight;
            TransitNightBackground.TranslationY = 0;

            TransitDayBackground.Color = dayColor;
            TransitDayBackground.HeightRequest = Math.Max(0, ySunset - ySunrise);
            TransitDayBackground.TranslationY = ySunrise;
        }

        private void BuildColumnBackground(
            AbsoluteLayout layout,
            double ySunrise,
            double ySunset,
            Color dayColor,
            Color nightColor,
            double width)
        {
            layout.Children.Clear();
            layout.HeightRequest = TimelineHeight;

            // Night base full height
            var night = new BoxView { Color = nightColor };
            AbsoluteLayout.SetLayoutBounds(night, new Rect(0, 0, width, TimelineHeight));
            layout.Children.Add(night);

            // Day overlay
            var day = new BoxView { Color = dayColor };
            var dayHeight = Math.Max(0, ySunset - ySunrise);
            AbsoluteLayout.SetLayoutBounds(day, new Rect(0, ySunrise, width, dayHeight));
            layout.Children.Add(day);
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
        private const double PixelsPerMinute = 2.0;
        private const int TotalMinutes = 24 * 60;     // 1440
        private const double TimelineHeight = TotalMinutes * PixelsPerMinute; // 2880

        private void BuildTimeScale()
        {
            if (TimeScaleLayout == null) return;

            TimeScaleLayout.Children.Clear();

            // Full day height in pixels
            //var totalMinutes = 24 * 60;
            var totalHeight = TimelineHeight;// totalMinutes * PixelsPerMinute;

            // Important: set layout height so ScrollView knows content size
            TimeScaleLayout.HeightRequest = totalHeight;

            // Width available inside the time column
            // (we'll keep ticks inside this width)
            var colWidth = 36.0;
            var shortTick = 10.0;
            var midTick = 18.0;
            var fullTick = colWidth; // full width for full hour

            for (int minute = 0; minute <= TotalMinutes; minute += StepMinutes)
            {
                var y = minute * PixelsPerMinute;

                bool isHour = (minute % 60 == 0);
                bool isHalfHour = (!isHour && minute % 30 == 0);

                var tickWidth = isHour ? fullTick : (isHalfHour ? midTick : shortTick);
                var tickOpacity = isHour ? 0.85 : (isHalfHour ? 0.85 : 0.85);
                var lineColor = (Color)Application.Current.Resources["TimelineLineColor"];
                
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
                if (isHour && minute < TotalMinutes)
                {
                    var t = TimeSpan.FromMinutes(minute);

                    const double labelHeight = 14;     // под FontSize 11–12 обычно ок
                    const double labelPaddingTop = 0;  // небольшой зазор над линией

                    var label = new Label
                    {
                        Text = $"{(int)t.TotalHours:00}:00",
                        FontSize = 11,
                        TextColor = lineColor,
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

            var lineColor = (Color)Application.Current.Resources["TimelineLineColor"];

            for (int minute = 0; minute <= totalMinutes; minute += StepMinutes)
            {
                var y = minute * PixelsPerMinute;

                bool isHour = (minute % 60 == 0);
                bool isHalfHour = (!isHour && minute % 30 == 0);

                // Events grid делаем более мягкой, чем сама шкала
                var opacity = isHour ? 0.85 : (isHalfHour ? 0.85 : 0.85);

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

        private void BuildTransitColumns()
        {
            TransitColumns.Clear();
            var lang = DataCache.Instance.CurrentLanguageCode;

            // берём все enum-значения < 100 (то есть 1..21), USER=100 не включаем
            var ids = Enum.GetValues(typeof(EDVLineName))
                          .Cast<EDVLineName>()
                          .Select(x => (int)x)
                          .Where(id => id > 0 && id < 100)
                          .Distinct()
                          .OrderBy(id => id)
                          .ToList();

            foreach (var id in ids)
            {
                var code = DataCache.Instance.DVLineNameList?.FirstOrDefault(x => x.Id == id)?.Code ?? id.ToString();
                var desc =DataCache.Instance.DVLineNameDescList?.FirstOrDefault(d => d.DVLineNameId == id && d.LanguageCode == lang) ?? null;

                var name = desc?.Name;
                if (string.IsNullOrWhiteSpace(name))
                    name = code; // fallback

                var shortName = desc?.ShortName;
                if (string.IsNullOrWhiteSpace(shortName))
                    shortName = code; // fallback

                TransitColumns.Add(new TransitColumnVm
                {
                    LineId = id,
                    Code = code,
                    Name = name!,
                    ShortName = shortName!
                });
            }
        }

        private void RenderNakshatraLane()
        {
            if (Day == null) return;

            // Накшатра = EDVLineName.NAKSHATRA = 2
            RenderPanchangaLane((int)EDVLineName.NAKSHATRA, Day.NakshatraSegments);
        }

        private void RenderPanchangaLane(int lineId, IList<PanchangaSegment> segments)
        {
            if (TransitColumnsHost == null) return;

            // найдём индекс колонки по LineId
            var columnIndex = TransitColumns.ToList().FindIndex(c => c.LineId == lineId);
            if (columnIndex < 0) return;

            // получаем визуальный Grid конкретной колонки из хоста
            if (columnIndex >= TransitColumnsHost.Children.Count) return;
            if (TransitColumnsHost.Children[columnIndex] is not Grid columnGrid) return;

            // первый child в template — это AbsoluteLayout, который мы добавили
            var lane = columnGrid.Children.OfType<AbsoluteLayout>().FirstOrDefault();
            if (lane == null) return;

            lane.Children.Clear();

            var dayStart = Day!.Date;              // 00:00 выбранного дня (локально)
            var dayEnd = dayStart.AddDays(1);
            var lineColor = (Color)Application.Current.Resources["TimelineLineColor"];
            var labels = new List<(double y, string text)>();

            // рисуем сегменты
            foreach (var seg in segments)
            {
                // minutes relative to dayStart (can be <0 or >1440)
                var startMin = (seg.Start - dayStart).TotalMinutes;
                var endMin = (seg.End - dayStart).TotalMinutes;

                // clip to [0..1440]
                if (endMin <= 0 || startMin >= 1440) continue;   // полностью вне суток

                startMin = Math.Max(0, startMin);
                endMin = Math.Min(1440, endMin);

                if (endMin <= startMin) continue;

                var y = startMin * PixelsPerMinute;
                var h = (endMin - startMin) * PixelsPerMinute;
                if (h < 1) h = 1;

                View block;

                // поддержка "split" (если понадобится, уже есть в PanchangaSegment)
                if (seg.IsSplitColor && seg.ColorTop != null && seg.ColorBottom != null)
                {
                    var splitGrid = new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Star },
                            new RowDefinition { Height = GridLength.Star }
                        }
                    };

                    var top = new BoxView { Color = seg.ColorTop, Opacity = 0.95 };
                    var bottom = new BoxView { Color = seg.ColorBottom, Opacity = 0.95 };

                    splitGrid.Children.Add(top);
                    splitGrid.Children.Add(bottom);

                    Grid.SetRow(top, 0);
                    Grid.SetRow(bottom, 1);

                    block = splitGrid;

                }
                else
                {
                    block = new BoxView
                    {
                        Color = seg.Color ?? Colors.Transparent,
                        Opacity = 0.95
                    };
                }

                AbsoluteLayout.SetLayoutBounds(block, new Rect(0, y, 80, h));
                AbsoluteLayout.SetLayoutFlags(block, AbsoluteLayoutFlags.None);
                lane.Children.Add(block);

                // граница перед сегментом (кроме самого первого)
                // текст в начале сегмента (если он начинается внутри суток, а не в 00:00)
                if (startMin > 0 && startMin < 1440)
                {
                    var text = GetSegmentLabelText(seg);
                    labels.Add((startMin * PixelsPerMinute, text));
                }
                
            }

            // separator at segment end (to avoid merging same-color segments)
            foreach (var (y, text) in labels)
            {
                // линия
                var sep = new BoxView
                {
                    Color = lineColor,
                    Opacity = 0.85,
                    HeightRequest = 1,
                    InputTransparent = true,
                    ZIndex = 10
                };
                AbsoluteLayout.SetLayoutBounds(sep, new Rect(0, y, 80, 1));
                lane.Children.Add(sep);

                // текст под линией
                var lbl = new Label
                {
                    Text = text,
                    FontSize = 10,
                    TextColor = Colors.Black,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Opacity = 0.85,
                    Margin = new Thickness(2, 1, 2, 0),   // 1px вниз от линии
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaxLines = 1,
                    InputTransparent = true,
                    ZIndex = 11
                };

                AbsoluteLayout.SetLayoutBounds(lbl, new Rect(2, y + 1, 76, 16));
                lane.Children.Add(lbl);
            }

        }

        private string GetSegmentLabelText(PanchangaSegment seg)
        {
            var text = (seg.Text ?? string.Empty).Trim();
            if (text.Length == 0) return string.Empty;

            // У DayOverview текст часто вида: "HH:mm <Id>.<Name>"
            // Нам на DayPage время не нужно — оставляем последнюю часть после пробелов.
            // (если там несколько пробелов/табов — тоже ок)
            var parts = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
                text = parts[^1];

            return text;
        }

        private void UpdateStickyForNakshatra(double scrollY)
        {
            if (Day == null) return;

            var minute = scrollY / PixelsPerMinute;
            var t = Day.Date.AddMinutes(minute);

            var seg = Day.NakshatraSegments.FirstOrDefault(s => s.Start <= t && t < s.End);
            if (seg == null) return;

            var idx = TransitColumns.ToList().FindIndex(c => c.LineId == (int)EDVLineName.NAKSHATRA);
            if (idx < 0) return;

            TransitColumns[idx].StickyText = GetSegmentLabelText(seg);  
        }







    }
}
