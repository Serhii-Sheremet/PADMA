using Microsoft.Maui.Layouts;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.TransitBuilder;
using PADMA.Core.Utilities;
using PADMA.UI;
using PADMA.UI.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PADMA.Pages
{
    public partial class DayPage : ContentPage, IQueryAttributable
    {
        private bool _isClosing;
        private bool _autoCenterRequested;
        private bool _syncingHorizontalScroll;
        private readonly Dictionary<VisualElement, Border> _highlights = new();
        
        private DayOverviewData? _overview;
        public DayOverviewData? Overview
        {
            get => _overview;
            set { _overview = value; OnPropertyChanged(); }
        }

        private List<PanchangaSegment> _horaSegments = new();
        public List<PanchangaSegment> HoraSegments
        {
            get => _horaSegments;
            set { _horaSegments = value; OnPropertyChanged(); }
        }

        private List<PanchangaSegment> _muhurta30Segments = new();
        public List<PanchangaSegment> Muhurta30Segments
        {
            get => _muhurta30Segments;
            set { _muhurta30Segments = value; OnPropertyChanged(); }
        }

        private List<PanchangaSegment> _ghati60Segments = new();
        public List<PanchangaSegment> Ghati60Segments
        {
            get => _ghati60Segments;
            set { _ghati60Segments = value; OnPropertyChanged(); }
        }

        private List<YogaOverviewStripe> _yogaStripes = new();
        public List<YogaOverviewStripe> YogaStripes
        {
            get => _yogaStripes;
            set { _yogaStripes = value; OnPropertyChanged(); }
        }

        private List<PanchangaSegment> _yogaSegments = new();
        public List<PanchangaSegment> YogaSegments
        {
            get => _yogaSegments;
            set { _yogaSegments = value; OnPropertyChanged(); }
        }

        private List<MuhurtaOverviewStripe> _muhurtaStripes = new();
        public List<MuhurtaOverviewStripe> MuhurtaStripes
        {
            get => _muhurtaStripes;
            set { _muhurtaStripes = value; OnPropertyChanged(); }
        }

        private List<PanchangaSegment> _muhurtaSegments = new();
        public List<PanchangaSegment> MuhurtaSegments
        {
            get => _muhurtaSegments;
            set { _muhurtaSegments = value; OnPropertyChanged(); }
        }

        private List<PanchangaSegment> _lagnaSegments = new();
        public List<PanchangaSegment> LagnaSegments
        {
            get => _lagnaSegments;
            set { _lagnaSegments = value; OnPropertyChanged(); }
        }

        // -------  Planet segments  -------
        private List<PanchangaSegment> _sunSegments = new();
        public List<PanchangaSegment> SunSegments
        {
            get => _sunSegments;
            set { _sunSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _moonSegments = new();
        public List<PanchangaSegment> MoonSegments
        {
            get => _moonSegments;
            set { _moonSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _marsSegments = new();
        public List<PanchangaSegment> MarsSegments
        {
            get => _marsSegments;
            set { _marsSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _mercurySegments = new();
        public List<PanchangaSegment> MercurySegments
        {
            get => _mercurySegments;
            set { _mercurySegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _jupiterSegments = new();
        public List<PanchangaSegment> JupiterSegments
        {
            get => _jupiterSegments;
            set { _jupiterSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _venusSegments = new();
        public List<PanchangaSegment> VenusSegments
        {
            get => _venusSegments;
            set { _venusSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _saturnSegments = new();
        public List<PanchangaSegment> SaturnSegments
        {
            get => _saturnSegments;
            set { _saturnSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _rahuSegments = new();
        public List<PanchangaSegment> RahuSegments
        {
            get => _rahuSegments;
            set { _rahuSegments = value; OnPropertyChanged(); }
        }
        private List<PanchangaSegment> _ketuSegments = new();
        public List<PanchangaSegment> KetuSegments
        {
            get => _ketuSegments;
            set { _ketuSegments = value; OnPropertyChanged(); }
        }
        // ---------------------------------

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

        private double _tooltipMaxHeight = 500;
        public double TooltipMaxHeight
        {
            get => _tooltipMaxHeight;
            set { if (Math.Abs(_tooltipMaxHeight - value) < 0.1) return; _tooltipMaxHeight = value; OnPropertyChanged(); }
        }

        private double _tooltipMaxWidth = 520;
        public double TooltipMaxWidth
        {
            get => _tooltipMaxWidth;
            set { if (Math.Abs(_tooltipMaxWidth - value) < 0.1) return; _tooltipMaxWidth = value; OnPropertyChanged(); }
        }

        private double _tooltipBodyMaxHeight = 300;
        public double TooltipBodyMaxHeight
        {
            get => _tooltipBodyMaxHeight;
            set { if (Math.Abs(_tooltipBodyMaxHeight - value) < 0.1) return; _tooltipBodyMaxHeight = value; OnPropertyChanged(); }
        }

        private double _tooltipBodyHeight = -1; // -1 = Auto
        public double TooltipBodyHeight
        {
            get => _tooltipBodyHeight;
            set { if (Math.Abs(_tooltipBodyHeight - value) < 0.1) return; _tooltipBodyHeight = value; OnPropertyChanged(); }
        }

        private bool _isTooltipVisible;
        public bool IsTooltipVisible
        {
            get => _isTooltipVisible;
            set { _isTooltipVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsTooltipHiddenInputTransparent)); }
        }

        // чтобы overlay не мешал скроллам когда скрыт
        public bool IsTooltipHiddenInputTransparent => !IsTooltipVisible;
        public bool HasTooltipHeader => !string.IsNullOrWhiteSpace(TooltipTitle) || !string.IsNullOrWhiteSpace(TooltipRange);
        public bool HasTooltipHeaderAndBlocks => HasTooltipHeader && HasTooltipBlocks;

        private string _tooltipTitle = string.Empty;
        public string TooltipTitle
        {
            get => _tooltipTitle;
            set { if (_tooltipTitle == value) return; _tooltipTitle = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasTooltipHeader)); }
        }

        private string _tooltipRange = string.Empty;
        public string TooltipRange
        {
            get => _tooltipRange;
            set { if (_tooltipRange == value) return; _tooltipRange = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasTooltipHeader)); }
        }

        public sealed class TooltipDivider { }
        public sealed class TooltipInnerDivider { }
        public sealed class TooltipSpacer
        {
            public double Height { get; init; } = 8;
        }
        public sealed class TooltipRangeLine
        {
            public string Text { get; init; } = string.Empty;
        }
        public sealed class TooltipRangeLinePlain
        {
            public string Text { get; init; } = string.Empty;
        }
        public ObservableCollection<object> TooltipItems { get; } = new();
        public bool HasTooltipBlocks => TooltipItems.Count > 0;

        private PanchangaSegment? _selectedSegment;
        private VisualElement? _selectedSegmentView;
        private int _selectedLineId;

        private bool _eventsSizeChangedHooked;
        private UserEvent? _selectedUserEvent;
        private int? _selectedSlotStartMin;   // minute of day (0..1439), multiple of 15
        private int? _selectedSlotEndMin;     // start+15 typically
        private string? _lastSelectionKey;    // for "second tap"
        private UserEvent? _editingEvent;
        private int _editingArgb;

        private bool _userEventEditorIsEdit;    // редактируем существующий?
        private int? _editingUserEventId;       // ID редактируемого (если есть)

        private string _origTitle = string.Empty;
        private string _origMessage = string.Empty;
        private TimeSpan _origStartTime;
        private TimeSpan _origEndTime;
        private int _origArgb;

        private readonly Dictionary<int, Frame> _eventFramesById = new();
        private BoxView? _slotSelectionBox;
        private bool _overlayLayoutAdjusted;
        private double _lastW, _lastH;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private string _busyText = "Please wait…";
        public string BusyText
        {
            get => _busyText;
            set
            {
                if (_busyText == value) return;
                _busyText = value;
                OnPropertyChanged(nameof(BusyText));
            }
        }

        public string NotesLabel => Localization.GetLocalizedText("Notes", DataCache.Instance.CurrentLanguageCode);

        private void ApplyLocalizedLabels()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var culture = new CultureInfo(lang);
            if (Day != null)
                Title = Day.Date.ToString("dd MMMM yyyy", culture);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // New flow: token -> bundle
            if (query.TryGetValue("token", out var t) && t is string token && !string.IsNullOrWhiteSpace(token))
            {
                var store = ServiceLocator.Services.GetService<NavigationDataStore>();
                if (store != null && store.TryGet(token, out DayNavBundle? bundle) && bundle != null)
                {
                    Day = bundle.Day;
                    if (Day != null)
                    {
                        BuildEventsSegments(Day.Date);
                    }
                    
                    var transitPack = Day?.TransitPack;

                    Overview = bundle.Overview;
                    
                    YogaStripes = (Overview?.YogaStripes ?? new ObservableCollection<YogaOverviewStripe>()).ToList();
                    MuhurtaStripes = (Overview?.MuhurtaStripes ?? new ObservableCollection<MuhurtaOverviewStripe>())
                            .Where(s => s.MuhurtaId != 0)      // фильтр Abhijit "не образуется"
                            .ToList();

                    if (bundle.Overview?.SunriseUtc != null)
                        SunriseUtc = DateTime.SpecifyKind(bundle.Overview.SunriseUtc.Value, DateTimeKind.Utc);

                    if (bundle.Overview?.SunsetUtc != null)
                        SunsetUtc = DateTime.SpecifyKind(bundle.Overview.SunsetUtc.Value, DateTimeKind.Utc);

                    var dayStart = Day!.Date;          
                    var dayEnd = dayStart.AddDays(1);

                    YogaSegments = BuildYogaSegments(dayStart, dayEnd, YogaStripes);
                    MuhurtaSegments = BuildMuhurtaSegments(dayStart, dayEnd, MuhurtaStripes);

                    // ---- Hora segments (локальные, зависят от TZ профиля) ----
                    var ctx = DataCache.Instance.ProfileContextService.Current;
                    if (ctx?.TimeZoneInfo != null)
                    {
                        var tz = ctx.TimeZoneInfo;

                        // Day.Date считается локальной датой (00:00)
                        var dayLocal = Day!.Date;
                        if (dayLocal.Kind == DateTimeKind.Utc)
                            dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayLocal, tz);

                        var horaSlices = HoraTransitBuilder.BuildForLocalDay(
                            dayLocal.Date,
                            tz,
                            ctx.LivingLat,
                            ctx.LivingLon,
                            altitude: 0);

                        // text = локализованное имя планеты
                        HoraSegments = PanchangaHelper.BuildSegmentsForDay(
                            horaSlices,
                            dayLocal.Date,
                            tz,
                            DataCache.Instance,
                            getColorCode: s => s.ColorCode,
                            getText: s =>
                            {
                                var planet = HoraPlanetMapper.ToPlanet(s.PlanetCode);

                                var planetId = DataCache.Instance.PlanetList
                                    .FirstOrDefault(p => p.PlanetCode == planet.ToString())?.Id ?? 0;

                                return planetId > 0
                                    ? PanchangaHelper.GetPlanetDescEntity(planetId)?.Name ?? string.Empty
                                    : planet.ToString();
                            },
                            getKind: s => ETransitKind.Hora,
                            includeStartTimeInText: false
                        );
                    }
                    // ------------------------------------------------------------

                    // ---- Muhurta30 segments (локальные, зависят от TZ профиля) ----
                    if (ctx?.TimeZoneInfo != null)
                    {
                        var tz = ctx.TimeZoneInfo;

                        var dayLocal = Day!.Date;
                        if (dayLocal.Kind == DateTimeKind.Utc)
                            dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayLocal, tz);

                        var slices = Muhurta30TransitBuilder.BuildForLocalDay(
                            dayLocal.Date,
                            tz,
                            ctx.LivingLat,
                            ctx.LivingLon,
                            altitude: 0);

                        Muhurta30Segments = PanchangaHelper.BuildSegmentsForDay(
                            slices,
                            dayLocal.Date,
                            tz,
                            DataCache.Instance,
                            getColorCode: s => (EColor)s.ColorId,
                            getText: s => $"{s.Muhurta30Id}.{PanchangaHelper.GetMuhurta30DescEntity(s.Muhurta30Id)?.ShortName}" ?? string.Empty,
                            getKind: s => ETransitKind.Muhurta30,
                            includeStartTimeInText: false,
                            getId: s => s.Muhurta30Id
                        );
                    }
                    // ------------------------------------------------------------

                    // ---- Ghati60 segments (локальные, зависят от TZ профиля) ----
                    if (ctx?.TimeZoneInfo != null)
                    {
                        var tz = ctx.TimeZoneInfo;

                        var dayLocal = Day!.Date;
                        if (dayLocal.Kind == DateTimeKind.Utc)
                            dayLocal = TimeZoneInfo.ConvertTimeFromUtc(dayLocal, tz);

                        var slices = Ghati60TransitBuilder.BuildForLocalDay(
                            dayLocal.Date,
                            tz,
                            ctx.LivingLat,
                            ctx.LivingLon,
                            altitude: 0);

                        Ghati60Segments = PanchangaHelper.BuildSegmentsForDay(
                            slices,
                            dayLocal.Date,
                            tz,
                            DataCache.Instance,
                            getColorCode: s => (EColor)s.ColorId,
                            getText: s => $"{s.Ghati60Id}.{ PanchangaHelper.GetGhati60DescDescEntity(s.Ghati60Id)?.ShortName}" ?? string.Empty,
                            getKind: s => ETransitKind.Ghati60,
                            includeStartTimeInText: false,
                            getId: s => s.Ghati60Id
                        );
                    }
                    // ------------------------------------------------------------

                    // --------- Lagna segments ----------------------------------
                    var lagnaSlices = Day?.LagnaSlices;
                    if (lagnaSlices != null && lagnaSlices.Count > 0)
                    {
                        LagnaSegments = BuildLagnaSegments(dayStart, dayEnd, lagnaSlices);
                    }
                    else
                    {
                        LagnaSegments = new List<PanchangaSegment>();
                    }
                    // -----------------------------------------------------------

                    // --------- Planet segments ----------------------------------
                    IReadOnlyList<PlanetSlice> sunSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.SUN, out sunSlices) && sunSlices != null)
                    {
                        SunSegments = BuildPlanetSegments(EPlanet.SUN, dayStart, dayEnd, sunSlices);
                    }
                    IReadOnlyList<PlanetSlice> moonSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.MOON, out moonSlices) && moonSlices != null)
                    {
                        MoonSegments = BuildPlanetSegments(EPlanet.MOON, dayStart, dayEnd, moonSlices);
                    }
                    IReadOnlyList<PlanetSlice> marsSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.MARS, out marsSlices) && marsSlices != null)
                    {
                        MarsSegments = BuildPlanetSegments(EPlanet.MARS, dayStart, dayEnd, marsSlices);
                    }
                    IReadOnlyList<PlanetSlice> mercurySlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.MERCURY, out mercurySlices) && mercurySlices != null)
                    {
                        MercurySegments = BuildPlanetSegments(EPlanet.MERCURY, dayStart, dayEnd, mercurySlices);
                    }
                    IReadOnlyList<PlanetSlice> jupiterSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.JUPITER, out jupiterSlices) && jupiterSlices != null)
                    {
                        JupiterSegments = BuildPlanetSegments(EPlanet.JUPITER, dayStart, dayEnd, jupiterSlices);
                    }
                    IReadOnlyList<PlanetSlice> venusSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.VENUS, out venusSlices) && venusSlices != null)
                    {
                        VenusSegments = BuildPlanetSegments(EPlanet.VENUS, dayStart, dayEnd, venusSlices);
                    }
                    IReadOnlyList<PlanetSlice> saturnSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.SATURN, out saturnSlices) && saturnSlices != null)
                    {
                        SaturnSegments = BuildPlanetSegments(EPlanet.SATURN, dayStart, dayEnd, saturnSlices);
                    }
                    IReadOnlyList<PlanetSlice> rahuSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.RAHU, out rahuSlices) && rahuSlices != null)
                    {
                        RahuSegments = BuildPlanetSegments(EPlanet.RAHU, dayStart, dayEnd, rahuSlices);
                    }
                    IReadOnlyList<PlanetSlice> ketuSlices;
                    if (transitPack != null && transitPack.TryGetValue(EPlanet.KETU, out ketuSlices) && ketuSlices != null)
                    {
                        KetuSegments = BuildPlanetSegments(EPlanet.KETU, dayStart, dayEnd, ketuSlices);
                    }
                    // -------------------------------------------------

                    ApplyDayNightBackgroundIfPossible();

                    IconsLayer.Children.Clear();
                    AddIconAtTime(IconsLayer, SunriseUtc, "sunrise_24.png", 24, 24, anchorY: 24 * 0.65);
                    AddIconAtTime(IconsLayer, SunsetUtc, "sunrise_24.png", 24, 24, anchorY: 24 * 0.65);
                    AddEclipseIconIfAny();

                    RenderTransitLane();
                    UpdateAllSticky(0);
                    RequestAutoCenter();

                    // optional: store.Remove(token); // если освобождать память сразу
                    return;
                }
            }

            // Backward compatibility (old flow)
            if (query.TryGetValue("Day", out var d) && d is DayItem day)
                Day = day;

            if (query.TryGetValue("SunriseUtc", out var sr) && sr is DateTime sunriseUtc)
                SunriseUtc = DateTime.SpecifyKind(sunriseUtc, DateTimeKind.Utc);

            if (query.TryGetValue("SunsetUtc", out var ss) && ss is DateTime sunsetUtc)
                SunsetUtc = DateTime.SpecifyKind(sunsetUtc, DateTimeKind.Utc);

            ApplyDayNightBackgroundIfPossible();
            RenderTransitLane();
            UpdateAllSticky(0);
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

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (height <= 0) return;

            TooltipMaxHeight = height * (width > height ? 0.85 : 0.70);
            TooltipMaxWidth = Math.Max(300, Math.Min(520, width * 0.92));

            const double headerReserve = 150; // Title+Range+Divider+Padding
            TooltipBodyMaxHeight = Math.Max(80, TooltipMaxHeight - headerReserve);

            // пересчёт HeightRequest после смены ориентации
            UpdateTooltipBodyHeight();

            if (Math.Abs(width - _lastW) < 0.5 && Math.Abs(height - _lastH) < 0.5)
                return;

            _lastW = width;
            _lastH = height;

            AdjustUserEventOverlayLayout(width, height);
        }

        private void UpdateTooltipBodyHeight()
        {
            if (!IsTooltipVisible) return;
            if (!HasTooltipBlocks) { TooltipBodyHeight = -1; return; }
            if (TooltipBodyStack == null) return;

            double availableWidth = Math.Max(0, TooltipMaxWidth - 40);

            var size = TooltipBodyStack.Measure(availableWidth, double.PositiveInfinity);
            var desired = size.Height;

            TooltipBodyHeight = Math.Min(desired, TooltipBodyMaxHeight);
        }

        private void OnTooltipBackdropTapped(object? sender, EventArgs e)
        {
            IsTooltipVisible = false;
        }

        public DayPage()
        {
            InitializeComponent();
            if (!_eventsSizeChangedHooked)
            {
                _eventsSizeChangedHooked = true;

                EventsBackgroundLayout.SizeChanged += (_, __) =>
                {
                    if (Day == null) return;
                    if (EventsBackgroundLayout.Width <= 0) return;

                    BuildEventsSegments(Day.Date);
                };
            }

            BindingContext = this;

            TooltipItems.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(HasTooltipBlocks));
                OnPropertyChanged(nameof(HasTooltipHeader)); // не обязательно, но не мешает
                OnPropertyChanged(nameof(HasTooltipHeaderAndBlocks));
            };

            MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
            MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
            {
                // язык уже сохранён в БД + Refresh вызывается на MainPage или в LanguagePage
                // тут нам достаточно пересобрать отображаемые колонки
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnPropertyChanged(nameof(NotesLabel));
                    BuildTransitColumns();

                    // чтобы подписи/содержимое тоже обновились
                    RenderTransitLane();
                });
            });

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
            UpdateAllSticky(e.ScrollY);
        }

        private void UpdateAllSticky(double scrollY)
        {
            if (Day == null) return;

            //UpdateStickyForLane((int)EDVLineName.LAGNA, LagnaSegments, scrollY); // -- will not be shown
            UpdateStickyForLane((int)EDVLineName.NAKSHATRA, Day.NakshatraSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.TARABALA, Day.TaraBalaSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.TITHI, Day.TithiSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.KARANA, Day.KaranaSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.NITYAYOGA, Day.NityaYogaSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.CHANDRABALA, Day.ChandraBalaSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.YOGA, YogaSegments, scrollY);
            //UpdateStickyForLane((int)EDVLineName.MUHURTA, MuhurtaSegments, scrollY);  // -- will not be shown
            UpdateStickyForLane((int)EDVLineName.HORA, HoraSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.MUHURTA30, Muhurta30Segments, scrollY);
            UpdateStickyForLane((int)EDVLineName.GHATI60, Ghati60Segments, scrollY);
            UpdateStickyForLane((int)EDVLineName.SUNPADA, SunSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.MOONPADA, MoonSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.MARSPADA, MarsSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.MERCURYPADA, MercurySegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.JUPITERPADA, JupiterSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.VENUSPADA, VenusSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.SATURNPADA, SaturnSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.RAHUPADA, RahuSegments, scrollY);
            UpdateStickyForLane((int)EDVLineName.KETUPADA, KetuSegments, scrollY);
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
            UpdateAllSticky(target);

            Dispatcher.Dispatch(() =>
            {
                // заставляем MAUI пересчитать размеры/arrange для sticky слоя
                TransitStickyScroll?.InvalidateMeasure();
                TransitStickyScroll?.ForceLayout();

                StickyOverlay?.InvalidateMeasure();
            });
        }

        private async Task CloseAsync()
        {
            if (_isClosing) return;
            _isClosing = true;

            var lang = DataCache.Instance.CurrentLanguageCode;
            await RunBusyAsync(Localization.GetLocalizedText("Please wait…", lang), async () =>
            {
                try
                {
                    // Close DayPage completely and return to calendar
                    await Shell.Current.GoToAsync("//main");
                }
                finally
                {
                    _isClosing = false;
                }
                await Task.Yield();
            });
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            ResetTooltip();
            await CloseAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            // Нельзя давать Shell "pop" назад на DayOverview — это ломает carousel.
            Dispatcher.Dispatch(async () =>
            {
                if (_isClosing) return;
                await CloseAsync();
            });

            return true; // мы обработали, стандартный Back отменяем
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
            BuildColumnBackground(IconsBackgroundLayout, ySunrise, ySunset, dayColor, nightColor, 24);
            BuildColumnBackground(TimeBackgroundLayout, ySunrise, ySunset, dayColor, nightColor, 36);

            // фон для Events column через BoxView
            EventsNightBackground.BackgroundColor = nightColor;
            EventsNightBackground.HeightRequest = TimelineHeight;
            EventsNightBackground.TranslationY = 0;

            EventsDayBackground.BackgroundColor = dayColor;
            EventsDayBackground.HeightRequest = Math.Max(0, ySunset - ySunrise);
            EventsDayBackground.TranslationY = ySunrise;

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
                var desc = DataCache.Instance.DVLineNameDescList?.FirstOrDefault(d => d.DVLineNameId == id && d.LanguageCode == lang) ?? null;

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

        private void RenderTransitLane()
        {
            if (Day == null) return;

            _highlights.Clear();

            RenderPanchangaLane((int)EDVLineName.LAGNA, LagnaSegments);
            RenderPanchangaLane((int)EDVLineName.NAKSHATRA, Day.NakshatraSegments);
            RenderPanchangaLane((int)EDVLineName.TARABALA, Day.TaraBalaSegments);
            RenderPanchangaLane((int)EDVLineName.TITHI, Day.TithiSegments);
            RenderPanchangaLane((int)EDVLineName.KARANA, Day.KaranaSegments);
            RenderPanchangaLane((int)EDVLineName.NITYAYOGA, Day.NityaYogaSegments);
            RenderPanchangaLane((int)EDVLineName.CHANDRABALA, Day.ChandraBalaSegments);
            RenderPanchangaLane((int)EDVLineName.YOGA, YogaSegments);
            RenderPanchangaLane((int)EDVLineName.MUHURTA, MuhurtaSegments);
            RenderPanchangaLane((int)EDVLineName.HORA, HoraSegments);
            RenderPanchangaLane((int)EDVLineName.MUHURTA30, Muhurta30Segments);
            RenderPanchangaLane((int)EDVLineName.GHATI60, Ghati60Segments);
            RenderPanchangaLane((int)EDVLineName.SUNPADA, SunSegments);
            RenderPanchangaLane((int)EDVLineName.MOONPADA, MoonSegments);
            RenderPanchangaLane((int)EDVLineName.MARSPADA, MarsSegments);
            RenderPanchangaLane((int)EDVLineName.MERCURYPADA, MercurySegments);
            RenderPanchangaLane((int)EDVLineName.JUPITERPADA, JupiterSegments);
            RenderPanchangaLane((int)EDVLineName.VENUSPADA, VenusSegments);
            RenderPanchangaLane((int)EDVLineName.SATURNPADA, SaturnSegments);
            RenderPanchangaLane((int)EDVLineName.RAHUPADA, RahuSegments);
            RenderPanchangaLane((int)EDVLineName.KETUPADA, KetuSegments);
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

                    var top = new BoxView { Color = seg.ColorTop };
                    var bottom = new BoxView { Color = seg.ColorBottom };

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
                        Color = seg.Color ?? Colors.Transparent
                    };
                }

                var wrapper = new Grid
                {
                    Padding = 0,
                    Margin = 0,
                    RowSpacing = 0,
                    ColumnSpacing = 0
                };

                // фон сегмента
                if (block is View v)
                {
                    v.HorizontalOptions = LayoutOptions.Fill;
                    v.VerticalOptions = LayoutOptions.Fill;
                }
                wrapper.Children.Add(block);

                // рамка выделения поверх
                var highlight = new Border
                {
                    Stroke = new SolidColorBrush(Colors.Gold),
                    StrokeThickness = 2,          // можно 1, если тонко
                    Background = null,
                    Padding = 0,
                    Margin = 0,
                    InputTransparent = true,
                    Opacity = 0.0,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };
                wrapper.Children.Add(highlight);
                _highlights[wrapper] = highlight;

                // TAP on wrapper
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => OnSegmentTapped(lineId, seg, wrapper);
                wrapper.GestureRecognizers.Add(tap);

                // Position wrapper in the lane (AbsoluteLayout)
                AbsoluteLayout.SetLayoutBounds(wrapper, new Rect(0, y, 80, h));
                AbsoluteLayout.SetLayoutFlags(wrapper, AbsoluteLayoutFlags.None);
                lane.Children.Add(wrapper);

                // текст в начале сегмента (если он начинается внутри суток, а не в 00:00)
                var text = GetSegmentLabelText(seg);
                if (startMin > 0 && startMin < 1440)
                {
                    labels.Add((startMin * PixelsPerMinute, text));
                }
                // линия БЕЗ текста в конце сегмента (если не конец суток)
                if (endMin > 0 && endMin < 1440)
                {
                    labels.Add((endMin * PixelsPerMinute, string.Empty));
                }
            }

            // separator at segment end (to avoid merging same-color segments)
            foreach (var (y, text) in labels)
            {
                // линия
                var sep = new BoxView
                {
                    Color = lineColor,
                    HeightRequest = 1,
                    InputTransparent = true,
                    ZIndex = 10
                };
                AbsoluteLayout.SetLayoutBounds(sep, new Rect(0, y, 80, 1));
                lane.Children.Add(sep);

                // текст рисуем только если есть что рисовать
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                // текст под линией
                var lbl = new Label
                {
                    Text = text,
                    FontSize = 10,
                    TextColor = Colors.Black,
                    HorizontalTextAlignment = TextAlignment.Center,
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

        private readonly Dictionary<ETransitKind, Func<PanchangaSegment, string>> _labelResolvers
            = new()
        {
            // here will be resolvers for all transit lines
            {
                ETransitKind.Lagna,
                seg => seg.Text
            },
            {
                ETransitKind.Nakshatra,
                seg =>
                {
                    var nak = PanchangaHelper.GetNakshatraDescEntity(seg.TransitId);
                    return nak != null ? $"{nak.NakshatraId}. {nak.ShortName}" : string.Empty;
                }
            },
            {
                ETransitKind.TaraBala,
                seg =>
                {
                    var tb = PanchangaHelper.GetTaraBalaDescEntity(seg.TransitId);
                    var pct = TryParsePercent(seg.Text);
                    return tb != null ? $"{tb.ShortName} {pct}%" : string.Empty;
                }
            },
            {
                ETransitKind.Tithi,
                seg =>
                {
                    var ti = PanchangaHelper.GetTithiDescEntity(seg.TransitId);
                    return ti != null ? $"{ti.TithiId}. {ti.ShortName}" : string.Empty;
                }
            },
            {
                ETransitKind.Karana,
                seg =>
                {
                    var ka = PanchangaHelper.GetKaranaDescEntity(seg.TransitId);
                    return ka != null ? $"{ka.Name}" : string.Empty;
                }
            },
            {
                ETransitKind.NityaYoga,
                seg =>
                {
                    var ny = PanchangaHelper.GetNityaYogaDescEntity(seg.TransitId);
                    return ny != null ? $"{ny.NityaYogaId}. {ny.Name}" : string.Empty;
                }
            },
            {
                ETransitKind.ChandraBala,
                seg =>
                {
                    var text = StripLeadingTime(seg.Text);
                    return $"{text}";
                }
            },
            {
                ETransitKind.Yoga,
                seg =>
                {
                    // seg.Text may contain a comma-separated list of YogaIds (for overlays)
                    var ids = (seg.Text ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(s => int.TryParse(s, out var id) ? id : 0)
                        .Where(id => id > 0)
                        .Distinct()
                        .ToList();

                    if (ids.Count == 0)
                        ids.Add(seg.TransitId);

                    var names = ids
                        .Select(id => PanchangaHelper.GetYogaDescEntity(id)?.ShortName)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .ToList();

                    return string.Join(" ", names);
                }
            },
            {
                ETransitKind.Muhurta,
                seg =>
                {
                    var ids = (seg.Text ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(s => int.TryParse(s, out var id) ? id : 0)
                        .Where(id => id > 0)
                        .Distinct()
                        .Take(2)
                        .ToList();

                    if (ids.Count == 0)
                        ids.Add(seg.TransitId);

                    var names = ids
                        .Select(id => PanchangaHelper.GetMuhurtaDescEntity(id)?.ShortName)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .ToList();

                    return string.Join(" ", names);
                }
            },
            {
                ETransitKind.Hora,
                seg =>
                {
                    return seg.Text;
                }
            },
            {
                ETransitKind.Muhurta30,
                seg =>
                {
                    return seg.Text;
                }
            },
            {
                ETransitKind.Ghati60,
                seg =>
                {
                    return seg.Text;
                }
            },
            {
                ETransitKind.Planet,
                seg =>
                {
                    return seg.Text;
                }
            }
        };

        private string GetSegmentLabelText(PanchangaSegment seg)
        {
            if (seg == null) return string.Empty;

            if (_labelResolvers.TryGetValue(seg.TransitKind, out var resolver))
                return resolver(seg);

            return string.Empty;
        }

        private void UpdateStickyForLane(int lineId, IList<PanchangaSegment> segments, double scrollY)
        {
            if (Day == null) return;

            var idx = TransitColumns.ToList().FindIndex(c => c.LineId == lineId);
            if (idx < 0) return;

            if (segments == null || segments.Count == 0)
            {
                TransitColumns[idx].StickyText = string.Empty;
                return;
            }

            var minute = scrollY / PixelsPerMinute;
            var t = Day.Date.AddMinutes(minute);

            var seg = segments.FirstOrDefault(s => s.Start <= t && t < s.End);

            // ВАЖНО: если "дыра" — убираем sticky
            if (seg == null)
            {
                TransitColumns[idx].StickyText = string.Empty;
                return;
            }

            TransitColumns[idx].StickyText = GetSegmentLabelText(seg);
        }

        private void OnSegmentTapped(int lineId, PanchangaSegment seg, VisualElement view)
        {
            ClearEventsSelectionVisuals();
            CloseUserEventOverlay();
            _lastSelectionKey = null;

            // 1-й тап: выделяем
            if (_selectedSegment == null || !ReferenceEquals(_selectedSegment, seg) || _selectedLineId != lineId)
            {
                ClearSelection();

                _selectedSegment = seg;
                _selectedLineId = lineId;
                _selectedSegmentView = view;

                HighlightSelection(view, true);

                // при выборе сегмента тултип закрываем
                IsTooltipVisible = false;
                return;
            }

            // 2-й тап по тому же сегменту: открываем тултип
            switch (lineId)
            {
                //switch for all lines
                case (int)EDVLineName.LAGNA:
                    ShowLagnaPadaTooltip(seg);
                    break;

                case (int)EDVLineName.NAKSHATRA:
                    ShowNakshatraTooltip(seg);
                    break;

                case (int)EDVLineName.TARABALA:
                    ShowTaraBalaTooltip(seg);
                    break;

                case (int)EDVLineName.TITHI:
                    ShowTithiTooltip(seg);
                    break;

                case (int)EDVLineName.KARANA:
                    ShowKaranaTooltip(seg);
                    break;

                case (int)EDVLineName.NITYAYOGA:
                    ShowNityaYogaTooltip(seg);
                    break;

                case (int)EDVLineName.CHANDRABALA:
                    ShowChandraBalaTooltip(seg);
                    break;

                case (int)EDVLineName.YOGA:
                    ShowYogaTooltip(seg);
                    break;

                case (int)EDVLineName.MUHURTA:
                    ShowMuhurtaTooltip(seg);
                    break;

                case (int)EDVLineName.HORA:
                    ShowHoraTooltip(seg);
                    break;

                case (int)EDVLineName.MUHURTA30:
                    ShowMuhurta30Tooltip(seg);
                    break;

                case (int)EDVLineName.GHATI60:
                    ShowGhati60Tooltip(seg);
                    break;

                case (int)EDVLineName.SUNPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.MOONPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.MARSPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.MERCURYPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.JUPITERPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.VENUSPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.SATURNPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.RAHUPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;

                case (int)EDVLineName.KETUPADA:
                    //ShowPlanetPadaTooltip(seg);
                    _ = ShowPlanetPadaTooltipBusyAsync(seg);
                    break;
            }
            IsTooltipVisible = true;
        }

        private void ClearSelection()
        {
            if (_selectedSegmentView != null)
                HighlightSelection(_selectedSegmentView, false);

            _selectedSegment = null;
            _selectedSegmentView = null;
            _selectedLineId = 0;
        }

        private void HighlightSelection(VisualElement view, bool selected)
        {
            if (_highlights.TryGetValue(view, out var h))
                h.Opacity = selected ? 1.0 : 0.0;
        }

        private void AddIconAtTime(
            AbsoluteLayout layer,
            DateTime? timeUtc,
            string iconResource,
            double columnWidth,
            double iconSize,
            double anchorY = -1)
        {
            if (!timeUtc.HasValue) 
                return;

            var tUtc = timeUtc.Value;
            if (tUtc.Kind == DateTimeKind.Unspecified)
                tUtc = DateTime.SpecifyKind(tUtc, DateTimeKind.Utc);

            var y = TimeToY(tUtc);

            var img = new Image
            {
                Source = iconResource,
                WidthRequest = iconSize,
                HeightRequest = iconSize
            };

            if (anchorY < 0) anchorY = iconSize / 2.0; // по умолчанию — центр

            AbsoluteLayout.SetLayoutBounds(
                img,
                new Rect(
                    (columnWidth - iconSize) / 2.0,
                    y - anchorY,     
                    iconSize,
                    iconSize));
            
            AbsoluteLayout.SetLayoutFlags(img, AbsoluteLayoutFlags.None);
            layer.Children.Add(img);
        }

        private double TimeToY(DateTime timeUtc)
        {
            if (Day == null) return 0;

            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (ctx?.TimeZoneInfo != null)
                tzInfo = ctx.TimeZoneInfo;

            var dayStartLocal = Day.Date.Date;
            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayStartLocal, tzInfo);

            var h = TimelineHeight; 

            return CalendarDrawingHelper.ConvertTimeToPixelsY(h, timeUtc, dayStartUtc);
        }

        private void AddEclipseIconIfAny()
        {
            if (Day == null) return;

            if (Day.EclipseId == null || Day.EclipseId == 0) return;   // если int? / int
            if (string.IsNullOrWhiteSpace(Day.EclipseIcon)) return;

            // если EclipseDate nullable:
            if (Day.EclipseDate == null) return;

            var tUtc = Day.EclipseDate.Value;

            // на всякий случай: если Kind не задан — считаем, что это UTC (как у SunriseUtc/SunsetUtc)
            if (tUtc.Kind == DateTimeKind.Unspecified)
                tUtc = DateTime.SpecifyKind(tUtc, DateTimeKind.Utc);

            AddIconAtTime(IconsLayer, Day.EclipseDate, Day.EclipseIcon, 24, 22);
        }

        private void ShowNakshatraTooltip(PanchangaSegment seg)
        {
            var nak = PanchangaHelper.GetNakshatraDescEntity(seg.TransitId);
            if (nak == null)
                return;

            TooltipTitle = $"{nak.NakshatraId}.{nak.Name}";
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            FillTooltipBlocks(nak, NakshatraTooltipFields);
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<NakshatraDesc, string?> GetValue)[] NakshatraTooltipFields =
        {
            ("Ruler",       d => d.Ruler),
            ("Nature",      d => d.Nature),
            ("Description", d => d.Description),
            ("Favorable",   d => d.GoodFor),
            ("Unfavorable", d => d.BadFor),
        };

        private void ShowTaraBalaTooltip(PanchangaSegment seg)
        {
            var tb = PanchangaHelper.GetTaraBalaDescEntity(seg.TransitId);
            if (tb == null)
                return;

            var pct = TryParsePercent(seg.Text);
            TooltipTitle = pct.HasValue ? $"{tb.Name} {pct.Value}%" : $"{tb.Name}";
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            FillTooltipBlocks(tb, TaraBalaTooltipFields);
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<TaraBalaDesc, string?> GetValue)[] TaraBalaTooltipFields =
        {
            ("Description", d => d.Description),
        };

        private static int? TryParsePercent(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            // ищем что-то вроде "75%" или " 75 % "
            var m = System.Text.RegularExpressions.Regex.Match(
                text, @"(?<!\d)(\d{1,3})\s*%");

            if (!m.Success)
                return null;

            if (int.TryParse(m.Groups[1].Value, out var p) && p >= 0 && p <= 100)
                return p;

            return null;
        }

        private void ShowTithiTooltip(PanchangaSegment seg)
        {
            var ti = PanchangaHelper.GetTithiDescEntity(seg.TransitId);
            if (ti == null)
                return;

            TooltipTitle = $"{ti.TithiId}.{ti.Name}";
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            FillTooltipBlocks(ti, TithiTooltipFields);
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<TithiDesc, string?> GetValue)[] TithiTooltipFields =
        {
            ("Ruler",       d => d.Ruler),
            ("Type",        d => d.Type),
            ("Favorable",   d => d.GoodFor),
            ("Unfavorable", d => d.BadFor),
        };

        private void ShowKaranaTooltip(PanchangaSegment seg)
        {
            var ka = PanchangaHelper.GetKaranaDescEntity(seg.TransitId);
            if (ka == null)
                return;

            TooltipTitle = $"{ka.Name}";
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            FillTooltipBlocks(ka, KaranaTooltipFields);
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<KaranaDesc, string?> GetValue)[] KaranaTooltipFields =
        {
            ("Ruler",       d => d.Ruler),
            ("Favorable",   d => d.GoodFor),
            ("Unfavorable", d => d.BadFor),
        };

        private void ShowNityaYogaTooltip(PanchangaSegment seg)
        {
            var ny = PanchangaHelper.GetNityaYogaEntity(seg.TransitId);
            var nyd = PanchangaHelper.GetNityaYogaDescEntity(seg.TransitId);
            if (ny == null || nyd == null)
                return;

            TooltipTitle = $"{nyd.NityaYogaId}.{nyd.Name}";
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            TooltipItems.Clear();

            // 1) Управитель (планета)
            var rulerName = PanchangaHelper.GetPlanetDescEntity(ny.YogiPlanetId)?.Name;
            AddTooltipBlock("Ruler", rulerName);

            // 2) Накшатра
            var nakName = PanchangaHelper.GetNakshatraDescEntity(ny.NakshatraId)?.Name;
            AddTooltipBlock("Nakshatra", nakName);

            FillTooltipBlocks(nyd, NityaYogaTooltipFields, false);
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<NityaYogaDesc, string?> GetValue)[] NityaYogaTooltipFields =
        {
            ("Deity",       d => d.Deity),
            ("Meaning",     d => d.Meaning),
            ("Description", d => d.Description),
        };

        private void ShowChandraBalaTooltip(PanchangaSegment seg)
        {
            TooltipItems.Clear();
            var text = StripLeadingTime(seg.Text);
            TooltipTitle = $"{text}";
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static string StripLeadingTime(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var s = text.Trim();

            var idx = s.IndexOf(' ');
            if (idx < 0)
                return s;

            var first = s.Substring(0, idx);

            // проверяем, что первое "слово" похоже на время
            // допустимы: H:mm, HH:mm, H:mm:ss, HH:mm:ss
            if (TimeSpan.TryParse(first, out _))
                return s.Substring(idx + 1).Trim();

            return s;
        }

        private void FillTooltipBlocks<T>(
            T entity,
            (string NativeLabel, Func<T, string?> GetValue)[] fields,
             bool clearFirst = true)
        {
            if (clearFirst)
                TooltipItems.Clear();

            var lang = DataCache.Instance.CurrentLanguageCode;

            foreach (var f in fields)
            {
                var value = f.GetValue(entity)?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var label = Localization.GetLocalizedText(f.NativeLabel, lang);

                var fs = new FormattedString();
                fs.Spans.Add(new Span
                {
                    Text = $"{label}:",
                    TextDecorations = TextDecorations.Underline
                });
                fs.Spans.Add(new Span
                {
                    Text = " "
                });
                fs.Spans.Add(new Span
                {
                    Text = value
                });

                TooltipItems.Add(fs);
            }
        }
        private void ResetTooltip()
        {
            IsTooltipVisible = false;
            TooltipItems.Clear();
            TooltipTitle = string.Empty;
            TooltipRange = string.Empty;
        }

        private void AddTooltipBlock(string nativeLabel, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            
            if(value.Equals("#"))
                value = string.Empty;

            var lang = DataCache.Instance.CurrentLanguageCode;
            var label = Localization.GetLocalizedText(nativeLabel, lang);

            var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = $"{label}:", TextDecorations = TextDecorations.Underline });
            fs.Spans.Add(new Span { Text = " " });
            fs.Spans.Add(new Span { Text = value.Trim() });

            TooltipItems.Add(fs);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetTooltip();
        }

        protected override void OnDisappearing()
        {
            ResetTooltip();
            base.OnDisappearing();
        }

        private static List<PanchangaSegment> BuildMuhurtaSegments(
            DateTime dayStartLocal,
            DateTime dayEndLocal,
            IReadOnlyList<MuhurtaOverviewStripe> stripes)
        {
            var overlapColorId = (int)EColor.PINK;
            var points = new List<DateTime> { dayStartLocal, dayEndLocal };

            foreach (var it in stripes)
            {
                if (it.StartLocal.HasValue) points.Add(it.StartLocal.Value);
                if (it.EndLocal.HasValue) points.Add(it.EndLocal.Value);
            }

            points = points.Distinct().OrderBy(x => x).ToList();

            var result = new List<PanchangaSegment>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                var a = points[i];
                var b = points[i + 1];
                if (b <= a) continue;

                var active = stripes
                    .Where(it => it.StartLocal.HasValue && it.EndLocal.HasValue
                              && it.StartLocal.Value <= a && b <= it.EndLocal.Value)
                    .ToList();

                if (active.Count == 0)
                    continue;

                var id1 = active[0].MuhurtaId;
                var c1 = active[0].ColorId;

                int colorId;
                string idsText;

                if (active.Count == 1)
                {
                    colorId = c1;
                    idsText = id1.ToString();
                }
                else
                {
                    var id2 = active[1].MuhurtaId;
                    var c2 = active[1].ColorId;

                    idsText = $"{id1},{id2}";
                    colorId = (c1 == c2) ? c1 : overlapColorId;
                }

                result.Add(new PanchangaSegment
                {
                    TransitId = id1,

                    // важно для tooltip
                    TransitStart = a,
                    TransitEnd = b,

                    // важно для RenderPanchangaLane (он использует seg.Start/seg.End)
                    Start = a,
                    End = b,

                    Text = idsText,
                    Color = DataCache.Instance.GetColor((EColor)colorId),

                    // чтобы GetSegmentLabelText заработал
                    TransitKind = ETransitKind.Muhurta
                });
            }
            return result;
        }

        private void ShowMuhurtaTooltip(PanchangaSegment seg)
        {
            // ids: "id1" или "id1,id2"
            var ids = (seg.Text ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .Take(2)
                .ToList();

            if (ids.Count == 0)
                ids.Add(seg.TransitId);

            // собрать реальные мухурты из stripes (чтобы корректно сортировать по StartLocal)
            var items = MuhurtaStripes
                .Where(s => ids.Contains(s.MuhurtaId) && s.StartLocal.HasValue && s.EndLocal.HasValue)
                .Select(s => new
                {
                    s.MuhurtaId,
                    Start = s.StartLocal!.Value,
                    End = s.EndLocal!.Value
                })
                .OrderBy(x => x.Start)
                .ToList();

            if (items.Count == 0)
                return;

            IsTooltipVisible = true;
            TooltipTitle = string.Empty;
            TooltipRange = string.Empty;
            TooltipItems.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                var muItem = PanchangaHelper.GetMuhurtaDescEntity(it.MuhurtaId);
                if (muItem == null) continue;

                TooltipItems.Add(FsHeader(muItem.Name));
                TooltipItems.Add(new TooltipRangeLinePlain { Text = $"{it.Start:dd.MM.yyyy HH:mm:ss} – {it.End:dd.MM.yyyy HH:mm:ss}" });

                if (i < items.Count - 1)
                {
                    TooltipItems.Add(new TooltipDivider());
                    TooltipItems.Add(new TooltipSpacer { Height = 8 });
                }
            }
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static FormattedString FsHeader(string text)
        {
            var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = text, FontAttributes = FontAttributes.Bold, FontSize = 14 });
            return fs;
        }

        private static FormattedString FsRange(string text)
        {
            var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = text, FontSize = 11, TextColor = Color.FromRgba(0, 0, 0, 217) }); // 217/255 ~ 0.85 opacity
            return fs;
        }

        private static FormattedString FsBody(string text)
        {
            var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = text, FontSize = 12 });
            return fs;
        }

        private static List<PanchangaSegment> BuildYogaSegments(
            DateTime dayStartLocal,
            DateTime dayEndLocal,
            IReadOnlyList<YogaOverviewStripe> stripes)
        {
            // Build a single overlay lane from multiple yoga stripes.
            // If active yogas have the same ColorId -> use it.
            // If there is a mix of ColorId -> use YOGAMERGE (neutralization).

            var overlapColorId = (int)EColor.YOGAMERGE;

            // Flatten all yoga time intervals for the day
            var intervals = new List<(int YogaId, int ColorId, string Title, DateTime Start, DateTime End)>();
            foreach (var s in stripes)
            {
                if (s?.Segments == null || s.Segments.Count == 0)
                    continue;

                foreach (var seg in s.Segments)
                {
                    // clamp to [dayStartLocal, dayEndLocal]
                    var a = seg.StartLocal < dayStartLocal ? dayStartLocal : seg.StartLocal;
                    var b = seg.EndLocal > dayEndLocal ? dayEndLocal : seg.EndLocal;
                    if (b <= a)
                        continue;

                    intervals.Add((s.YogaId, s.ColorId, s.Title ?? string.Empty, a, b));
                }
            }

            if (intervals.Count == 0)
                return new List<PanchangaSegment>();

            // Collect split points
            var points = new List<DateTime> { dayStartLocal, dayEndLocal };
            foreach (var it in intervals)
            {
                points.Add(it.Start);
                points.Add(it.End);
            }

            points = points.Distinct().OrderBy(x => x).ToList();

            var result = new List<PanchangaSegment>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                var a = points[i];
                var b = points[i + 1];
                if (b <= a) continue;

                // Active yogas that fully cover this sub-interval
                var active = intervals
                    .Where(it => it.Start <= a && b <= it.End)
                    .ToList();

                if (active.Count == 0)
                    continue;

                var activeIds = active.Select(x => x.YogaId).Distinct().OrderBy(x => x).ToList();
                var distinctColors = active.Select(x => x.ColorId).Distinct().ToList();

                var colorId = (distinctColors.Count == 1) ? distinctColors[0] : overlapColorId;
                var idsText = string.Join(",", activeIds);
                var firstId = activeIds.Count > 0 ? activeIds[0] : 0;

                result.Add(new PanchangaSegment
                {
                    TransitId = firstId,

                    // for tooltip: exact overlay interval
                    TransitStart = a,
                    TransitEnd = b,

                    // for RenderPanchangaLane
                    Start = a,
                    End = b,

                    Text = idsText,
                    Color = DataCache.Instance.GetColor((EColor)colorId),
                    TransitKind = ETransitKind.Yoga
                });
            }
            return result;
        }

        private void ShowYogaTooltip(PanchangaSegment seg)
        {
            // seg.Text: comma-separated YogaIds (for overlays)
            var ids = (seg.Text ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                ids.Add(seg.TransitId);

            // Find the corresponding Yoga stripes and the concrete intervals that cover this segment
            var items = YogaStripes
                .Where(s => ids.Contains(s.YogaId))
                .Select(s =>
                {
                    var cover = s.Segments.FirstOrDefault(x => x.StartLocal <= seg.Start && seg.End <= x.EndLocal);
                    return new
                    {
                        s.YogaId,
                        s.ColorId,
                        Vara = s.Vara,
                        NakshatraId = s.NakshatraId,
                        TithiId = s.TithiId,
                        Start = cover?.StartLocal ?? seg.Start,
                        End = cover?.EndLocal ?? seg.End
                    };
                })
                .OrderBy(x => x.Start)
                .ToList();

            if (items.Count == 0)
                return;

            var distinctColors = items.Select(x => x.ColorId).Where(c => c != 0).Distinct().ToList();
            bool isMixed = distinctColors.Count > 1;

            IsTooltipVisible = true;
            TooltipTitle = string.Empty;
            TooltipRange = string.Empty;
            TooltipItems.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                var yogaItem = PanchangaHelper.GetYogaDescEntity(it.YogaId);
                if (yogaItem == null) continue;

                TooltipItems.Add(FsHeader(yogaItem.Name));
                TooltipItems.Add(new TooltipRangeLine { Text = $"{it.Start:dd.MM.yyyy HH:mm:ss} – {it.End:dd.MM.yyyy HH:mm:ss}" });

                // Vara
                var vara = GetVaraName(it.Vara);
                AddTooltipBlock("Vara", vara);

                // Nakshatra
                if (it.NakshatraId != 0)
                {
                    var nakEntity = PanchangaHelper.GetNakshatraDescEntity(it.NakshatraId);
                    var nakName = nakEntity != null ? $"{it.NakshatraId}.{nakEntity.Name}" : string.Empty;
                    AddTooltipBlock("Nakshatra", nakName);
                }

                // Tithi
                if (it.TithiId != 0)
                {
                    var tiEntity = PanchangaHelper.GetTithiDescEntity(it.TithiId);
                    var tiName = tiEntity != null ? $"{it.TithiId}.{tiEntity.Name}" : string.Empty;
                    AddTooltipBlock("Tithi", tiName);
                }

                FillTooltipBlocks(yogaItem, YogaTooltipFields, clearFirst: false);

                // divider между йогами — но НЕ после последней
                if (i < items.Count - 1)
                {
                    TooltipItems.Add(new TooltipDivider());
                    TooltipItems.Add(new TooltipSpacer { Height = 8 });
                }
            }
            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private string GetVaraName(DayOfWeek dow)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var culture = new CultureInfo(lang);

            var name = culture.DateTimeFormat.GetDayName(dow);

            if (string.IsNullOrEmpty(name))
                return name;

            return char.ToUpper(name[0], culture) + name.Substring(1);
        }

        private static readonly (string NativeLabel, Func<YogaDesc, string?> GetValue)[] YogaTooltipFields =
        {
            ("Description", d => d.Description)
        };

        private List<PanchangaSegment> BuildPlanetSegments(
            EPlanet planet,
            DateTime dayStartLocal,
            DateTime dayEndLocal,
            IReadOnlyList<PlanetSlice> slices)
        {
            var ctx = DataCache.Instance.ProfileContextService.Current;
            TimeZoneInfo? tzInfo = ctx.TimeZoneInfo;
            var segments = new List<PanchangaSegment>();
            var transitMode = DataCache.Instance.GetActiveTransitSettings();

            foreach (var s in slices)
            {
                var startUtc = DateTime.SpecifyKind(s.StartUtc, DateTimeKind.Utc);
                var endUtc = DateTime.SpecifyKind(s.EndUtc, DateTimeKind.Utc);

                // FIX: последний slice может быть нулевой длительности (EndUtc == StartUtc)
                // Тогда для отрисовки текущего транзита растягиваем его до конца суток.
                if (endUtc <= startUtc)
                {
                    // dayEndLocal -> UTC в таймзоне профиля
                    var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);
                    endUtc = dayEndUtc;
                }

                // переводим в local
                var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tzInfo);
                var endLocal = TimeZoneInfo.ConvertTimeFromUtc(endUtc, tzInfo);

                // клипуем в рамки суток
                if (endLocal <= dayStartLocal || startLocal >= dayEndLocal) continue;
                if (startLocal < dayStartLocal) startLocal = dayStartLocal;
                if (endLocal > dayEndLocal) endLocal = dayEndLocal;

                Color color = GetPadaColor(s.NakshatraId, s.PadaId);
                string text = PreparePlanetSegmentText(s);
                segments.Add(new PanchangaSegment
                {
                    Start = startLocal,
                    End = endLocal,
                    TransitKind = ETransitKind.Planet,
                    TransitId = s.PlanetId,
                    TransitStart = s.StartUtc,
                    TransitEnd = s.EndUtc,
                    Text = text,
                    Color = color
                });
            }
            return segments;
        }

        private string PreparePlanetSegmentText(PlanetSlice slice)
        {
            int padaNum = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);
            string text = $"{PanchangaHelper.GetNakshatraDescEntity(slice.NakshatraId).ShortName} {padaNum.ToString()}{GetLocalizedPadaChar()}";
            return text;
        }

        private Color GetPadaColor(int nakshatraId, int padaId)
        {
            int padaNumber = SwissUtility.GetPadaNumberByPadaId(padaId);
            int colorId = DataCache.Instance.PadaList.FirstOrDefault(i => i.PadaNumber == padaNumber && i.NakshatraId == nakshatraId).ColorId;
            return DataCache.Instance.GetColor((EColor)colorId);
        }

        private string GetLocalizedPadaChar()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            switch (lang)
            {
                case "en": return "P";
                case "uk": return "П"; 
                case "pl": return "P";
                case "ru": return "П";
            }
            return string.Empty;
        }

        private async Task ShowPlanetPadaTooltipBusyAsync(PanchangaSegment seg)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var text = Localization.GetLocalizedText("Calculating tooltip...", lang);

            await RunBusyAsync(text, () =>
            {
                ShowPlanetPadaTooltip(seg);   
                return Task.CompletedTask;
            });
        }

        private void ShowPlanetPadaTooltip(PanchangaSegment seg)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var ctx = DataCache.Instance.ProfileContextService?.Current;
            TimeZoneInfo? tzInfo = ctx.TimeZoneInfo;

            // seg.TransitId для планетных линий = PlanetId
            var planet = (EPlanet)seg.TransitId;

            var day = Day;
            var transitPack = day?.TransitPack;
            if (transitPack == null)
                return;

            if (!transitPack.TryGetValue(planet, out var slices) || slices == null || slices.Count == 0)
                return;

            // Найдем slice, по которому был построен сегмент
            var slice = slices.FirstOrDefault(s =>
                s.StartUtc == seg.TransitStart && s.EndUtc == seg.TransitEnd);

            // fallback: если по равенству не нашлось (на всякий случай)
            if (slice == null)
            {
                var t = seg.TransitStart;
                slice = slices.FirstOrDefault(s => s.StartUtc <= t && s.EndUtc >= t);
                if (slice == null)
                    return;
            }

            // переводим в local
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(seg.TransitStart, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(seg.TransitEnd, tzInfo);

            // Заголовок и диапазон – в стиле остальных тултипов
            var pName = PanchangaHelper.GetPlanetDescEntity((int)planet)?.Name ?? planet.ToString();
            TooltipTitle = pName;
            TooltipRange = $"{startLocal:dd.MM.yyyy HH:mm:ss} – {endLocal:dd.MM.yyyy HH:mm:ss}";

            TooltipItems.Clear();

            // ---- Блок 1: Zodiac / Nakshatra / Pada / Navamsa ----
            var zodiacName = PanchangaHelper.GetZodiacDescEntity(slice.ZodiacId)?.Name;
            var nak = PanchangaHelper.GetNakshatraDescEntity(slice.NakshatraId);
            var nakText = nak != null ? $"{nak.NakshatraId}.{nak.Name}" : string.Empty;

            // Pada number (1..4) из PadaId (1..108)
            int padaNum = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);

            // Navamsa zodiac + exaltation mark
            int navZodId = slice.NavamsaZodiacId;
            var navName = PanchangaHelper.GetZodiacDescEntity(navZodId)?.Name;

            // ExaltationUtility.GetPlanetExaltation ожидает EZodiac
            string eventSymbol = string.Empty;
            if (navZodId >= 1 && navZodId <= 12)
            {
                var exNow = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)navZodId);
                if (exNow == EExaltation.EXALTATION) eventSymbol = "↑";
                else if (exNow == EExaltation.DEBILITATION) eventSymbol = "↓";
                else eventSymbol = string.Empty;
            }

            AddTooltipBlock("Zodiac Sign", zodiacName);
            AddTooltipBlock("Nakshatra", nakText);
            AddTooltipBlock("Pada", padaNum.ToString());
            AddTooltipBlock("Navamsa", $"{navName}{eventSymbol}");

            // ---- Блок 2: Special Navamsa / Bad Navamsa / Drekkana ----
            var pEntity = DataCache.Instance.PadaList.FirstOrDefault(p => p.Id == slice.PadaId);
            string specNav = pEntity != null ? PlanetTooltipUtility.GetSpecNavamsha(pEntity) : string.Empty;
            specNav = specNav?.Trim().TrimStart(',').Trim();

            if (!string.IsNullOrWhiteSpace(specNav))
                AddTooltipBlock("Special Navamsa", specNav);

            // Birth pada from Moon
            int birthPadaMoonId = ctx.BirthPadaMoonId;

            // Birth pada from Lagna
            int birthPadaLagnaId = ctx.BirthPadaLagnaId;

            // Bad Navamsa (Malefic Navamsa) / Drekkana
            Func<string, string> L = key => Localization.GetLocalizedText(key, lang);
            string badNav = PlanetTooltipUtility.GetBadNavamsha(slice.PadaId, birthPadaMoonId, birthPadaLagnaId, L);
            badNav = (badNav ?? string.Empty).Trim().TrimEnd(',').Trim();
            if (!string.IsNullOrWhiteSpace(badNav))
                AddTooltipBlock("Malefic Navamsa", badNav);

            var dList = PlanetTooltipUtility.GetBadDrekkanaList(slice.PadaId, birthPadaMoonId, birthPadaLagnaId);
            if (dList != null && dList.Count > 0)
            {
                var parts = new List<string>();
                foreach (var de in dList)
                {
                    var suffix = de.IsLagna
                        ? Localization.GetLocalizedText("Drekkana from Lagna", lang)
                        : Localization.GetLocalizedText("Drekkana from Natal Moon", lang);

                    parts.Add($"{de.Drekkana} {suffix}");
                }
                AddTooltipBlock("Drekkana", string.Join(", ", parts));
            }

            // ---- Блок 3: Transit sections (Moon / Lagna) ----
            var tranzitSetting = DataCache.Instance.GetActiveTransitSettings(); // TRANZITMOON / TRANZITLAGNA / TRANZITMOONANDLAGNA
            bool useLagna = tranzitSetting == EAppSetting.TRANZITLAGNA;

            void AddTransitSection(bool forLagna)
            {
                int domX = forLagna ? slice.HouseFromLagna : slice.HouseFromMoon;

                AddTooltipBlock(forLagna ? "House from Lagna" : "House from Moon", domX.ToString());

                var trX = DataCache.Instance.TransitList
                    .FirstOrDefault(t => t.PlanetId == slice.PlanetId && t.House == domX);

                if (trX == null)
                    return;

                var trDescX = PanchangaHelper.GetTransitDescEntity(trX.Id)?.Description;
                if (!string.IsNullOrWhiteSpace(trDescX))
                    AddTooltipBlock("Transit Description", trDescX);

                if (string.IsNullOrWhiteSpace(trX.Vedha) || !int.TryParse(trX.Vedha, out int vedhaDom))
                    return;

                var nodeType = slice.NodeType;
                var planetKey = (EPlanet)slice.PlanetId;

                if (!transitPack.TryGetValue(planetKey, out var slicesForPlanet) || slicesForPlanet == null || slicesForPlanet.Count == 0)
                    return;

                var (rangeStartUtc, rangeEndUtc) =
                    PlanetTooltipUtility.GetContinuousHouseRangeUtc(
                        slicesForPlanet,
                        seg.TransitStart,
                        isLagna: forLagna);

                var vedhaList = PlanetTooltipUtility.PrepareVedhaPlanetList(
                    targetPlanetId: slice.PlanetId,
                    targetStartUtc: rangeStartUtc,
                    targetEndUtc: rangeEndUtc,
                    transitPack: transitPack,
                    vedhaDom: vedhaDom,
                    isLagna: forLagna,
                    nodeType: nodeType);

                vedhaList = PlanetTooltipUtility.MergeVedhaIntervals(vedhaList);

                var dayStartLocal = DateTime.SpecifyKind(Day.Date.Date, DateTimeKind.Unspecified);
                var dayEndLocal = dayStartLocal.AddDays(1);

                var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayStartLocal, tzInfo);
                var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);

                static DateTime AsUtc(DateTime dt) =>
                    dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

                vedhaList = vedhaList
                    .Where(v =>
                    {
                        var s = AsUtc(v.DateStart);
                        var e = AsUtc(v.DateEnd);
                        return e > dayStartUtc && s < dayEndUtc;
                    })
                    .ToList();

                if (vedhaList.Count == 0)
                    return;

                AddTooltipBlock(forLagna ? "Vedha from Lagna" : "Vedha from Moon", "#");

                foreach (var ve in vedhaList)
                {
                    var vpName = PanchangaHelper.GetPlanetDescEntity((int)ve.PlanetCode)?.Name ?? ve.PlanetCode.ToString();

                    var veStartLocal = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(ve.DateStart, DateTimeKind.Utc), tzInfo);

                    var veEndLocal = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.SpecifyKind(ve.DateEnd, DateTimeKind.Utc), tzInfo);

                    AddTooltipBlock(
                        vpName,
                        $"({veStartLocal:dd.MM.yyyy HH:mm:ss} – {veEndLocal:dd.MM.yyyy HH:mm:ss})");
                }
            }

            if (tranzitSetting == EAppSetting.TRANZITMOONANDLAGNA)
            {
                AddTransitSection(forLagna: false); // Moon block
                AddTransitSection(forLagna: true);  // Lagna block
            }
            else
            {
                AddTransitSection(forLagna: useLagna);
            }

            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private void ShowHoraTooltip(PanchangaSegment seg)
        {
            TooltipItems.Clear();
            TooltipTitle = seg.Text ?? string.Empty;
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private void ShowMuhurta30Tooltip(PanchangaSegment seg)
        {
            var m30desc = PanchangaHelper.GetMuhurta30DescEntity(seg.TransitId);
            if (m30desc == null) return;

            TooltipTitle = m30desc.Name;
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";
            
            TooltipItems.Clear();
            FillTooltipBlocks(m30desc, Muhurta30TooltipFields, false);

            UpdateTooltipBodyHeight();
            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<Muhurta30Desc, string?> GetValue)[] Muhurta30TooltipFields =
        {
            ("Description", d => d.Description),
        };

        private void ShowGhati60Tooltip(PanchangaSegment seg)
        {
            var gdesc = PanchangaHelper.GetGhati60DescDescEntity(seg.TransitId);
            if (gdesc == null) return;

            TooltipTitle = gdesc.Name;
            TooltipRange = $"{seg.TransitStart:dd.MM.yyyy HH:mm:ss} – {seg.TransitEnd:dd.MM.yyyy HH:mm:ss}";

            TooltipItems.Clear();
            FillTooltipBlocks(gdesc, Ghati60TooltipFields, false);

            Dispatcher.Dispatch(UpdateTooltipBodyHeight);
        }

        private static readonly (string NativeLabel, Func<Ghati60Desc, string?> GetValue)[] Ghati60TooltipFields =
        {
            ("Description", d => d.Description),
        };

        private List<PanchangaSegment> BuildLagnaSegments(
            DateTime dayStartLocal,
            DateTime dayEndLocal,
            IReadOnlyList<LagnaSlice> slices)
        {
            var ctx = DataCache.Instance.ProfileContextService.Current;
            TimeZoneInfo? tzInfo = ctx.TimeZoneInfo;

            var segments = new List<PanchangaSegment>();

            foreach (var s in slices)
            {
                var startUtc = DateTime.SpecifyKind(s.StartUtc, DateTimeKind.Utc);
                var endUtc = DateTime.SpecifyKind(s.EndUtc, DateTimeKind.Utc);

                // на всякий случай: если последний slice нулевой — растягиваем до конца суток
                if (endUtc <= startUtc)
                {
                    var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);
                    endUtc = dayEndUtc;
                }

                // UTC -> local
                var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tzInfo);
                var endLocal = TimeZoneInfo.ConvertTimeFromUtc(endUtc, tzInfo);

                // клипуем в рамки суток
                if (endLocal <= dayStartLocal || startLocal >= dayEndLocal) continue;
                if (startLocal < dayStartLocal) startLocal = dayStartLocal;
                if (endLocal > dayEndLocal) endLocal = dayEndLocal;

                Color color = GetPadaColor(s.NakshatraId, s.PadaId);

                int padaNum = SwissUtility.GetPadaNumberByPadaId(s.PadaId);
                var nakShort = PanchangaHelper.GetNakshatraDescEntity(s.NakshatraId)?.ShortName ?? string.Empty;
                string nakText = nakShort.Length > 5 ? nakShort[..5] : nakShort; 
                string text = $"{s.ZodiacId}-{nakText}-{padaNum}-{s.NavamsaZodiacId}";

                segments.Add(new PanchangaSegment
                {
                    Start = startLocal,
                    End = endLocal,
                    TransitKind = ETransitKind.Lagna,
                    TransitId = s.PadaId, // для лагны в TransitId храним PadaId
                    TransitStart = s.StartUtc,
                    TransitEnd = s.EndUtc,
                    Text = text,
                    Color = color
                });
            }

            return segments;
        }

        private void ShowLagnaPadaTooltip(PanchangaSegment seg)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            var ctx = DataCache.Instance.ProfileContextService?.Current;
            TimeZoneInfo? tzInfo = ctx.TimeZoneInfo;

            var day = Day;
            var slices = day?.LagnaSlices;
            if (slices == null || slices.Count == 0) return;

            // Найдём slice по старт/энд (как у планет)
            var slice = slices.FirstOrDefault(s => s.StartUtc == seg.TransitStart && s.EndUtc == seg.TransitEnd);
            if (slice == null)
            {
                // fallback по включению времени
                var t = seg.TransitStart;
                slice = slices.FirstOrDefault(s => s.StartUtc <= t && s.EndUtc >= t);
                if (slice == null) return;
            }

            // local range
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(seg.TransitStart, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(seg.TransitEnd, tzInfo);

            TooltipTitle = Localization.GetLocalizedText("Lagna", lang); 
            TooltipRange = $"{startLocal:dd.MM.yyyy HH:mm:ss} – {endLocal:dd.MM.yyyy HH:mm:ss}";

            TooltipItems.Clear();

            var zodiacName = PanchangaHelper.GetZodiacDescEntity(slice.ZodiacId)?.Name;
            var nak = PanchangaHelper.GetNakshatraDescEntity(slice.NakshatraId);
            var nakText = nak != null ? $"{nak.NakshatraId}.{nak.Name}" : string.Empty;

            int padaNum = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);

            var navName = PanchangaHelper.GetZodiacDescEntity(slice.NavamsaZodiacId)?.Name;

            AddTooltipBlock("Zodiac Sign", zodiacName);
            AddTooltipBlock("Nakshatra", nakText);
            AddTooltipBlock("Pada", padaNum.ToString());
            AddTooltipBlock("Navamsa", $"{navName}");

            // optional: Special / Malefic Navamsa + Drekkana (как у планет)
            var pEntity = DataCache.Instance.PadaList.FirstOrDefault(p => p.Id == slice.PadaId);
            string specNav = pEntity != null ? PlanetTooltipUtility.GetSpecNavamsha(pEntity) : string.Empty;
            specNav = specNav?.Trim().TrimStart(',').Trim();
            if (!string.IsNullOrWhiteSpace(specNav))
                AddTooltipBlock("Special Navamsa", specNav);

            if (ctx != null)
            {
                int birthPadaMoonId = ctx.BirthPadaMoonId;
                int birthPadaLagnaId = ctx.BirthPadaLagnaId;
                Func<string, string> L = key => Localization.GetLocalizedText(key, lang);

                string badNav = PlanetTooltipUtility.GetBadNavamsha(slice.PadaId, birthPadaMoonId, birthPadaLagnaId, L);
                badNav = (badNav ?? string.Empty).Trim().TrimEnd(',').Trim();
                if (!string.IsNullOrWhiteSpace(badNav))
                    AddTooltipBlock("Malefic Navamsa", badNav);

                var dList = PlanetTooltipUtility.GetBadDrekkanaList(slice.PadaId, birthPadaMoonId, birthPadaLagnaId);
                if (dList != null && dList.Count > 0)
                {
                    var parts = new List<string>();
                    foreach (var de in dList)
                    {
                        var suffix = de.IsLagna
                            ? Localization.GetLocalizedText("Drekkana from Lagna", lang)
                            : Localization.GetLocalizedText("Drekkana from Natal Moon", lang);

                        parts.Add($"{de.Drekkana} {suffix}");
                    }
                    AddTooltipBlock("Drekkana", string.Join(", ", parts));
                }
                UpdateTooltipBodyHeight();
                Dispatcher.Dispatch(UpdateTooltipBodyHeight);
            }
        }

        private List<UserEvent> LoadUserEventsForDay(DateTime dayLocal)
        {
            return DataCache.Instance.UserEventsWindowCache?.GetEventsForDay(dayLocal)
                   ?? new List<UserEvent>();
        }

        private void BuildEventsSegments(DateTime dayDate)
        {
            EventsBackgroundLayout.Children.Clear();

            var dayLocal = new DateTime(dayDate.Year, dayDate.Month, dayDate.Day, 0, 0, 0);
            var dayEnd = dayLocal.AddDays(1);

            var events = DataCache.Instance.UserEventsWindowCache?.GetEventsForDay(dayLocal)
                         ?? new List<UserEvent>();

            if (events.Count == 0)
                return;

            // Column width base
            var totalWidth = EventsBackgroundLayout.Width;
            if (totalWidth <= 0)
                totalWidth = 80; // your Events column width

            // Optional: clamp intervals for overlap calculation (recommended)
            var placed = UserEventOverlapHelper.AssignColumnsForDay(events, dayLocal);

            foreach (var p in placed)
            {
                var ev = p.Event;

                var start = ev.StartLocal < dayLocal ? dayLocal : ev.StartLocal;
                var end = ev.EndLocal > dayEnd ? dayEnd : ev.EndLocal;
                if (end <= start) continue;

                var startMin = (start - dayLocal).TotalMinutes;
                var endMin = (end - dayLocal).TotalMinutes;

                var top = startMin * PixelsPerMinute;
                var height = Math.Max(2, (endMin - startMin) * PixelsPerMinute);

                var colWidth = totalWidth / p.ColumnCount;
                var left = p.ColumnIndex * colWidth;

                var frame = new Frame
                {
                    Padding = new Thickness(2, 1),
                    CornerRadius = 4,
                    HasShadow = false,
                    BackgroundColor = CalendarDrawingHelper.ColorFromArgbInt(ev.ArgbValue),
                    BorderColor = Colors.Transparent,
                    Content = new Label
                    {
                        Text = ev.Name,
                        FontSize = 10,
                        LineBreakMode = LineBreakMode.TailTruncation,
                        MaxLines = 2,
                        TextColor = Colors.Black
                    }
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, __) => OnUserEventTapped(ev);
                frame.GestureRecognizers.Add(tap);
                _eventFramesById[ev.Id] = frame;

                AbsoluteLayout.SetLayoutBounds(frame, new Rect(left, top, colWidth, height));
                AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.None);

                EventsBackgroundLayout.Children.Add(frame);
            }
        }

        private void OnEventsBackgroundTapped(object? sender, TappedEventArgs e)
        {
            if (Day == null) return;

            ClearSelection();
            IsTooltipVisible = false;

            // получаем Y координату тапа (самый простой путь — через e.GetPosition)
            // В MAUI TapGestureRecognizer: e.GetPosition((VisualElement)sender)
            var pos = e.GetPosition(EventsBackgroundLayout);
            if (pos == null) return;

            var y = pos.Value.Y;

            // переводим в минуты
            var minute = (int)Math.Floor(y / PixelsPerMinute);

            // ограничиваем 0..1439
            minute = Math.Max(0, Math.Min(1439, minute));

            // округление вниз к 15 минутам
            var startMin = (minute / 15) * 15;
            var endMin = Math.Min(startMin + 15, 1440);

            SelectSlot(startMin, endMin);

            var key = $"slot:{startMin}";
            if (_lastSelectionKey == key)
            {
                // второй тап по тому же слоту -> открыть editor создания
                OpenUserEventEditorForSlot(startMin, endMin);
            }

            _lastSelectionKey = key;
        }

        private void OnUserEventTapped(UserEvent ev)
        {
            ClearSelection();
            IsTooltipVisible = false;
            
            SelectUserEvent(ev);
            var key = $"ev:{ev.Id}";
            if (_lastSelectionKey == key)
            {
                // второй тап по тому же событию -> открыть editor редактирования
                OpenUserEventEditorForExisting(ev);
            }

            _lastSelectionKey = key;
        }

        private void SelectUserEvent(UserEvent ev)
        {
            ClearEventsSelectionVisuals();

            _selectedUserEvent = ev;
            _selectedSlotStartMin = null;
            _selectedSlotEndMin = null;

            if (_eventFramesById.TryGetValue(ev.Id, out var frame))
                frame.BorderColor = Colors.Gold;  // золотая рамка как у тебя
        }

        private void SelectSlot(int startMin, int endMin)
        {
            ClearEventsSelectionVisuals();

            _selectedUserEvent = null;
            _selectedSlotStartMin = startMin;
            _selectedSlotEndMin = endMin;

            var top = startMin * PixelsPerMinute;
            var height = Math.Max(2, (endMin - startMin) * PixelsPerMinute);

            var width = EventsBackgroundLayout.Width;
            if (width <= 0) width = 80;

            _slotSelectionBox = new BoxView
            {
                Color = Color.FromRgb(255, 253, 240), // почти белый, очень деликатный
                InputTransparent = true
            };

            const double inset = 1.0;

            var x = inset;                 
            var y = top + inset;
            var w = Math.Max(0, width - inset * 2);
            var h = Math.Max(0, height - inset * 2);

            AbsoluteLayout.SetLayoutBounds(_slotSelectionBox, new Rect(x, y, w, h));
            AbsoluteLayout.SetLayoutFlags(_slotSelectionBox, AbsoluteLayoutFlags.None);

            EventsBackgroundLayout.Children.Add(_slotSelectionBox);
        }

        private void ClearEventsSelectionVisuals()
        {
            // снять рамки с event frames
            foreach (var kv in _eventFramesById)
                kv.Value.BorderColor = Colors.Transparent;

            // убрать слот-selection
            if (_slotSelectionBox != null)
            {
                EventsBackgroundLayout.Children.Remove(_slotSelectionBox);
                _slotSelectionBox = null;
            }
        }

        private const int DefaultEventArgb = unchecked((int)0xFFA8D5A2); // pastel green
        private void OpenUserEventEditorForSlot(int startMin, int endMin)
        {
            if (Day == null) return;

            var dayLocal = new DateTime(Day.Date.Year, Day.Date.Month, Day.Date.Day, 0, 0, 0);

            var start = dayLocal.AddMinutes(startMin);
            var end = dayLocal.AddMinutes(endMin);

            _editingEvent = null; // поле
            ShowUserEventOverlay(start, end, name: "", message: "", argb: DefaultEventArgb);
        }

        private void OpenUserEventEditorForExisting(UserEvent ev)
        {
            _editingEvent = ev;

            // заполняем UI как сейчас...
            TitleEntry.Text = ev.Name ?? string.Empty;
            MessageEditor.Text = ev.Message ?? string.Empty;
            StartTimePicker.Time = ev.StartLocal.TimeOfDay;
            EndTimePicker.Time = ev.EndLocal.TimeOfDay;
            _editingArgb = ev.ArgbValue;
            // + подсветка выбранного цвета

            // снимаем оригиналы
            _origTitle = (ev.Name ?? string.Empty);
            _origMessage = (ev.Message ?? string.Empty);
            _origStartTime = ev.StartLocal.TimeOfDay;
            _origEndTime = ev.EndLocal.TimeOfDay;
            _origArgb = ev.ArgbValue;

            ShowUserEventOverlay(ev.StartLocal, ev.EndLocal, ev.Name, ev.Message, ev.ArgbValue);
        }

        private string L(string nativeText)
        {
            return Localization.GetLocalizedText(nativeText, DataCache.Instance.CurrentLanguageCode);
        }

        private void ApplyUserEventOverlayLocalization()
        {
            // placeholders
            TitleEntry.Placeholder = L("Title");
            MessageEditor.Placeholder = L("Description");

            // buttons
            DeleteButton.Text = L("Delete");
            CancelButton.Text = L("Cancel");
            OkButton.Text = L("OK"); 
        }

        private void ShowUserEventOverlay(DateTime start, DateTime end, string name, string message, int argb)
        {
            UserEventOverlay.IsVisible = true;

            UserEventHeader.Text = _editingEvent == null ? L("New note") : L("Edit note");
            ApplyUserEventOverlayLocalization();

            StartTimePicker.Time = start.TimeOfDay;
            EndTimePicker.Time = end.TimeOfDay;

            TitleEntry.Text = name;
            MessageEditor.Text = message;

            _editingArgb = argb;
            BuildColorPalette(argb);

            DeleteButton.IsVisible = _editingEvent != null;
        }

        private readonly int[] _eventColors =
        {
            unchecked((int)0xFFA8D5A2), // pastel green
            unchecked((int)0xFFA7C7E7), // pastel blue
            unchecked((int)0xFFFFD6A5), // pastel orange
            unchecked((int)0xFFFFADAD), // pastel red/pink
            unchecked((int)0xFFD7B5E8), // pastel purple
            unchecked((int)0xFFA0E7E5), // pastel cyan
            unchecked((int)0xFFFFF3B0)  // pastel yellow
        };

        private void BuildColorPalette(int selectedArgb)
        {
            ColorPalette.Children.Clear();

            foreach (var argb in _eventColors)
            {
                var frame = new Frame
                {
                    WidthRequest = 28,
                    HeightRequest = 28,
                    CornerRadius = 14,
                    Padding = 0,
                    HasShadow = false,
                    BackgroundColor = CalendarDrawingHelper.ColorFromArgbInt(argb),
                    BorderColor = argb == selectedArgb ? Colors.Black : Colors.Transparent
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, __) =>
                {
                    _editingArgb = argb;

                    // refresh borders
                    foreach (var child in ColorPalette.Children)
                    {
                        if (child is Frame f)
                            f.BorderColor = Colors.Transparent;
                    }

                    frame.BorderColor = Colors.Black;
                };

                frame.GestureRecognizers.Add(tap);
                ColorPalette.Children.Add(frame);
            }
        }

        private bool _timeSyncInProgress;
        private static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(15);

        private void OnStartTimeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TimePicker.Time)) return;
            if (_timeSyncInProgress) return;

            try
            {
                _timeSyncInProgress = true;

                var start = StartTimePicker.Time;
                var end = EndTimePicker.Time;

                if (start >= end)
                {
                    var newEnd = start + MinDuration;

                    // если нельзя выходить за сутки — зажимаем в 23:59
                    if (newEnd.TotalMinutes >= 24 * 60)
                        newEnd = new TimeSpan(23, 59, 0);

                    EndTimePicker.Time = newEnd;
                }
            }
            finally
            {
                _timeSyncInProgress = false;
            }
        }

        private void OnEndTimeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TimePicker.Time)) return;
            if (_timeSyncInProgress) return;

            try
            {
                _timeSyncInProgress = true;

                var start = StartTimePicker.Time;
                var end = EndTimePicker.Time;

                if (end <= start)
                {
                    var newStart = end - MinDuration;

                    // если нельзя уходить ниже 00:00 — зажимаем
                    if (newStart < TimeSpan.Zero)
                        newStart = TimeSpan.Zero;

                    StartTimePicker.Time = newStart;
                }
            }
            finally
            {
                _timeSyncInProgress = false;
            }
        }

        private void OnUserEventCancel(object sender, EventArgs e)
        {
            CloseUserEventOverlay();
        }

        private void OnUserEventOk(object sender, EventArgs e)
        {
            SaveUserEventFromEditor();
        }

        private void OnUserEventDelete(object sender, EventArgs e)
        {
            if (_editingEvent == null)
                return;

            var db = ServiceLocator.Services.GetService(typeof(DatabaseService)) as DatabaseService;
            if (db == null)
                return;

            db.DeleteUserEvent(_editingEvent.Id);

            var cache = DataCache.Instance.UserEventsWindowCache;
            cache?.Invalidate();
            cache?.ReloadLastWindow();

            CloseUserEventOverlay();
            BuildEventsSegments(Day.Date);

            MessagingCenter.Send<object>(this, "UserEventsChanged");
        }

        private void CloseUserEventOverlay()
        {
            UserEventOverlay.IsVisible = false;
            _editingEvent = null;
        }

        private void SaveUserEventFromEditor()
        {
            if (Day == null)
                return;

            var db = ServiceLocator.Services.GetService(typeof(DatabaseService)) as DatabaseService;
            if (db == null)
                return;

            var dayLocal = new DateTime(Day.Date.Year, Day.Date.Month, Day.Date.Day, 0, 0, 0);
            int profileId = DataCache.Instance.ProfileContextService.Current.ProfileId;

            // читаем UI
            var startTime = StartTimePicker.Time;
            var endTime = EndTimePicker.Time;

            var start = dayLocal.Add(startTime);
            var end = dayLocal.Add(endTime);

            // safety: if user picked end <= start, force +15 min
            if (end <= start)
            {
                end = start.AddMinutes(15);
                // можно ещё обновить EndTimePicker.Time = end.TimeOfDay; (по желанию)
                endTime = end.TimeOfDay;
            }

            var title = (TitleEntry.Text ?? string.Empty);
            var message = (MessageEditor.Text ?? string.Empty);
            var argb = _editingArgb;

            // --- ВАЖНО: если это EDIT и ничего не изменилось — просто закрываем ---
            if (_editingEvent != null)
            {
                bool noChanges =
                    title == _origTitle &&
                    message == _origMessage &&
                    startTime == _origStartTime &&
                    endTime == _origEndTime &&
                    argb == _origArgb;

                if (noChanges)
                {
                    CloseUserEventOverlay();
                    return;
                }
            }

            if (_editingEvent == null)
            {
                // CREATE
                var ev = new UserEvent
                {
                    ProfileId = profileId,
                    StartLocal = start,
                    EndLocal = end,
                    Name = title,
                    Message = message,
                    ArgbValue = argb
                };

                db.InsertUserEvent(ev);
            }
            else
            {
                // UPDATE
                _editingEvent.StartLocal = start;
                _editingEvent.EndLocal = end;
                _editingEvent.Name = title;
                _editingEvent.Message = message;
                _editingEvent.ArgbValue = argb;

                db.UpdateUserEvent(_editingEvent);
            }

            var cache = DataCache.Instance.UserEventsWindowCache;
            cache?.Invalidate();
            cache?.ReloadLastWindow();

            CloseUserEventOverlay();
            BuildEventsSegments(Day.Date);

            MessagingCenter.Send<object>(this, "UserEventsChanged");
        }

        private void OnEventColorChanged(object sender, Syncfusion.Maui.Inputs.ColorChangedEventArgs e)
        {
            _editingArgb = CalendarDrawingHelper.ColorToArgbInt(e.NewColor);
        }

        private void OnUserEventOverlayTapped(object sender, TappedEventArgs e)
        {
            KeyboardHelper.HideKeyboard();
        }

        private void AdjustUserEventOverlayLayout(double width, double height)
        {
            if (UserEventCard == null || MessageEditor == null) return;

            // landscape if width > height
            bool isLandscape = width > height;

            // Editor height: portrait bigger, landscape smaller
            MessageEditor.HeightRequest = isLandscape ? 64 : 96;

            // Limit card height so it never goes out of screen
            // Keep margins: 18 top/bottom + a little safety
            var maxCardHeight = Math.Max(200, height - 60);
            UserEventCard.MaximumHeightRequest = maxCardHeight;

            // Optional: slightly reduce padding in landscape
            UserEventCard.Padding = isLandscape ? new Thickness(10) : new Thickness(12);
        }

        private async Task RunBusyAsync(string text, Func<Task> action)
        {
            BusyText = text;
            IsBusy = true;

            await Task.Yield(); // дать UI шанс отрисовать overlay

            try { await action(); }
            finally { IsBusy = false; }
        }


    }
}
