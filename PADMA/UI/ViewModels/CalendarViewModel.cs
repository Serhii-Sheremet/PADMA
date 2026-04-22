using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.TransitBuilder;
using PADMA.Core.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace PADMA.UI.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DayItem> Days { get; } = new ObservableCollection<DayItem>();

        private int _year;
        public int Year
        {
            get => _year;
            private set
            {
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        private int _month;
        public int Month
        {
            get => _month;
            private set
            {
                if (_month != value)
                {
                    _month = value;
                    OnPropertyChanged(nameof(Month));
                }
            }
        }

        private DayItem? _selectedDay;
        public DayItem? SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (ReferenceEquals(_selectedDay, value)) return;
                _selectedDay = value;
                OnPropertyChanged(nameof(SelectedDay));
            }
        }

        // === CultureCode supporting ===
        private string _cultureCode;
        public string CultureCode
        {
            get => _cultureCode;
            private set
            {
                if (_cultureCode != value)
                {
                    _cultureCode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        public sealed class NotesOverlayItem
        {
            public string Text { get; set; } = "";
            public Color BackgroundColor { get; set; } = Colors.LightGray;
        }

        private bool _isNotesOverlayVisible;
        public bool IsNotesOverlayVisible
        {
            get => _isNotesOverlayVisible;
            set { _isNotesOverlayVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotesOverlayHiddenInputTransparent)); }
        }

        public bool IsNotesOverlayHiddenInputTransparent => !IsNotesOverlayVisible;

        private string _notesOverlayTitle = "";
        public string NotesOverlayTitle
        {
            get => _notesOverlayTitle;
            set { _notesOverlayTitle = value; OnPropertyChanged(); }
        }

        public ObservableCollection<NotesOverlayItem> NotesOverlayItems { get; } = new();

        private string _activeProfileDisplay = string.Empty;
        public string ActiveProfileDisplay
        {
            get => _activeProfileDisplay;
            set { _activeProfileDisplay = value; OnPropertyChanged(); }
        }

        private double _notesOverlayMaxHeight = 400;
        public double NotesOverlayMaxHeight
        {
            get => _notesOverlayMaxHeight;
            set { _notesOverlayMaxHeight = value; OnPropertyChanged(); }
        }

        private double _notesOverlayListMaxHeight = 280;
        public double NotesOverlayListMaxHeight
        {
            get => _notesOverlayListMaxHeight;
            set { _notesOverlayListMaxHeight = value; OnPropertyChanged(); }
        }

        private string _activeLocationTimeZoneDisplay = string.Empty;
        public string ActiveLocationTimeZoneDisplay
        {
            get => _activeLocationTimeZoneDisplay;
            set { _activeLocationTimeZoneDisplay = value; OnPropertyChanged(); }
        }

        public CultureInfo CurrentCulture =>
            !string.IsNullOrWhiteSpace(CultureCode)
                ? new CultureInfo(CultureCode)
                : CultureInfo.CurrentUICulture;

        // === Culture initialization ===
        public void InitializeCulture()
        {
            try
            {
                var langCode = DataCache.Instance.CurrentLanguageCode;
                CultureCode = DataCache.Instance.GetCurrentCultureCode(langCode);
            }
            catch
            {
                CultureCode = CultureInfo.CurrentUICulture.Name;
            }

            OnPropertyChanged(nameof(CurrentCulture));
        }

        public void ReloadCultureAndRefresh()
        {
            try
            {
                var langCode = DataCache.Instance.CurrentLanguageCode;
                string newCulture = DataCache.Instance.GetCurrentCultureCode(langCode);
                if (!string.Equals(CultureCode, newCulture, StringComparison.OrdinalIgnoreCase))
                {
                    CultureCode = newCulture;
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                CultureCode = CultureInfo.CurrentUICulture.Name;
                OnPropertyChanged(nameof(CurrentCulture));
            }

            RefreshCalendar();
        }

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

        private async Task RunBusyAsync(string text, Func<Task> action)
        {
            BusyText = text;
            IsBusy = true;

            // IMPORTANT: Give the UI a chance to render the overlay
            await Task.Yield();

            try
            {
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GenerateDaysAsync(int year, int month)
        {
            if (IsBusy) return;

            var lang = DataCache.Instance.CurrentLanguageCode;
            await RunBusyAsync(Localization.GetLocalizedText("Calculating calendar…", lang), () =>
            {
                GenerateDays(year, month);
                return Task.CompletedTask;
            });
        }

        public CalendarViewModel()
        {
            DataCache.Instance.ProfileContextUpdated += () => RefreshCalendar();

            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;

            UpdateActiveProfileInfo();
            _ = GenerateDaysAsync(Year, Month);
        }

        /// <summary>
        /// Rebuild current month grid (apply first-day-of-week changes, etc.).
        /// </summary>
        public void RefreshCalendar()
        {
            UpdateActiveProfileInfo();
            OnPropertyChanged(nameof(MonthYearTitle));
            _ = GenerateDaysAsync(Year, Month);
        }

        /// <summary>
        /// Move month by offset (e.g., -1 = previous, +1 = next).
        /// </summary>
        public void MoveMonth(int offset)
        {
            var newDate = new DateTime(Year, Month, 1).AddMonths(offset);
            SetMonthYear(newDate.Year, newDate.Month);
        }

        public void RebuildCurrentMonth()
        {
            RefreshCalendar();              // will rebuild Days on the current Year/Month
        }

        public void HideNotesOverlay()
        {
            IsNotesOverlayVisible = false;
        }

        public void ShowNotesOverlayForDay(DateTime dayLocal)
        {
            var notesLabel = Localization.GetLocalizedText("Notes", DataCache.Instance.CurrentLanguageCode);

            NotesOverlayTitle = $"{notesLabel} • {dayLocal:dd.MM.yyyy}";

            NotesOverlayItems.Clear();

            var events = DataCache.Instance.UserEventsWindowCache.GetEventsForDay(dayLocal.Date);
            foreach (var ev in events.OrderBy(e => e.StartLocal))
            {
                var from = ev.StartLocal.ToString("HH:mm");
                var to = ev.EndLocal.ToString("HH:mm");

                NotesOverlayItems.Add(new NotesOverlayItem
                {
                    Text = $"{ev.Name}: {from} - {to}",
                    BackgroundColor = CalendarDrawingHelper.ColorFromArgbInt(ev.ArgbValue)
                });
            }

            IsNotesOverlayVisible = NotesOverlayItems.Count > 0;
        }

        /// <summary>
        /// Core: build a 6x7 grid (42 days) based on the selected first day of week.
        /// </summary>
        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            TimeZoneInfo? tzInfo = null;
            int birthNakshatraMoonId = 0, birthZodiacMoonId = 0, birthLagnaId = 0;

            List<NakshatraSlice>? nakshatraSlices = null;
            List<TaraBalaSlice>? taraBalaSlices = null;
            List<TithiSlice>? tithiSlices = null;
            List<KaranaSlice>? karanaSlices = null;
            List<NityaYogaSlice>? nityaYogaSlices = null;
            List<ChandraBalaSlice>? chandraBalaSlices = null;

            List<PlanetSlice>? sunSlices = null;
            List<PlanetSlice>? moonSlices = null;
            List<PlanetSlice>? marsSlices = null;
            List<PlanetSlice>? mercurySlices = null;
            List<PlanetSlice>? jupiterSlices = null;
            List<PlanetSlice>? venusSlices = null;
            List<PlanetSlice>? saturnSlices = null;
            List<PlanetSlice>? rahuSlices = null;
            List<PlanetSlice>? ketuSlices = null;
            List<LagnaSlice>? lagnaSlices = null;

            Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>? transitPack = null;
            Dictionary<DateTime, EclipseData> eclipseByLocalDay = new();

            Profile? profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            
            var lang = DataCache.Instance.CurrentLanguageCode;
            var nakById = DataCache.Instance.NakshatraDescList
                .Where(x => x.LanguageCode == lang)
                .ToDictionary(x => x.NakshatraId, x => string.IsNullOrWhiteSpace(x.ShortName) ? x.Name : x.ShortName);
            var taraById = DataCache.Instance.TaraBalaDescList
                .Where(x => x.LanguageCode == lang)
                .ToDictionary(x => x.TaraBalaId, x => string.IsNullOrWhiteSpace(x.ShortName) ? x.Name : x.ShortName);

            var tithiById = DataCache.Instance.TithiDescList
                .Where(x => x.LanguageCode == lang)
                .ToDictionary(x => x.TithiId, x => string.IsNullOrWhiteSpace(x.ShortName) ? x.Name : x.ShortName);

            var karanaById = DataCache.Instance.KaranaDescList
                .Where(x => x.LanguageCode == lang)
                .ToDictionary(x => x.KaranaId, x => x.Name ?? "");

            var nityaYogaById = DataCache.Instance.NityaYogaDescList
                .Where(x => x.LanguageCode == lang)
                .ToDictionary(x => x.NityaYogaId, x => x.Name ?? "");

            var zodiacCodeById = DataCache.Instance.ZodiacList
                .ToDictionary(
                    z => z.Id,
                    z =>
                    {
                        var code = z.ZodiacCode; // "SCO"
                        if (string.IsNullOrEmpty(code))
                            return "";

                        return char.ToUpper(code[0]) + code.Substring(1).ToLower();
                    });

            // Active Node settings
            var nodeMode = DataCache.Instance.GetActiveNodeSetting();

            IReadOnlyList<DateOnly> visibleDays;
            if (profile != null && ctx != null)
            {
                tzInfo = ctx.TimeZoneInfo;
                birthNakshatraMoonId = ctx.BirthNakshatraMoonId;
                birthZodiacMoonId = ctx.BirthZodiacMoonId;
                birthLagnaId = ctx.BirthLagnaId;

                // We calculate the calendar window (if there is a time zone)
                DateTimeOffset visibleStart, visibleEnd, bufferStart, bufferEnd;
                (visibleStart, visibleEnd, bufferStart, bufferEnd, visibleDays) =
                    CalendarWindowService.BuildWindow(year, month, DataCache.Instance.DayOfWeek, tzInfo);

                var bufferStartUtc = bufferStart.UtcDateTime;
                var bufferEndUtc = bufferEnd.UtcDateTime;

                var windowStart = bufferStart.DateTime;                 // local (profile TZ) day 00:00
                var windowEndExclusive = bufferEnd.AddDays(1).DateTime; // local (profile TZ) next day 00:00
                var cache = DataCache.Instance.UserEventsWindowCache;

                if (!cache.IsLoadedFor(ctx.ProfileId, windowStart, windowEndExclusive))
                {
                    cache.LoadWindow(ctx.ProfileId, windowStart, windowEndExclusive);
                }

                // ---- Swiss: PlanetData calculation ----
                var sunData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.SUN, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var moonData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.MOON, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var marsData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.MARS, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var mercuryData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.MERCURY, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var jupiterData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.JUPITER, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var venusData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.VENUS, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var saturnData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.SATURN, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var rahuData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.RAHU, bufferStartUtc, bufferEndUtc, nodeMode, true);
                var ketuData = rahuData?.Select(d => SwissAnalysis.CalculateKetuData(d)).ToList();

                // ---- Swiss: LagnaData calculation ----
                var lagnaData = SwissAnalysis.CalculateLagnaDataList(bufferStartUtc, bufferEndUtc, ctx.LivingLat, ctx.LivingLon, 0, 'O');

                // ---- SWISS: Calculate Eclipse Data ---- 
                var eclipsesData = SwissAnalysis.CalculateEclipses_London(bufferStartUtc, bufferEndUtc);

                // ---- Preparing Panchanga slices ----
                // ---- Nakshatra (Moon) ----
                nakshatraSlices = NakshatraTransitBuilder.BuildNakshatraSlices(moonData);
                // ---- TaraBala ----
                taraBalaSlices = TaraBalaTransitBuilder.BuildTaraBalaSlices(nakshatraSlices, birthNakshatraMoonId);
                // ---- Swiss + Tithi —---
                var tithiData = SwissAnalysis.CalculateTithiDataList_London(bufferStartUtc, bufferEndUtc, nodeMode);
                tithiSlices = TithiTransitBuilder.BuildTithiSlices(tithiData, bufferEndUtc);
                // ---- Karana ----
                karanaSlices = KaranaTransitBuilder.BuildKaranaSlices(tithiSlices);
                // ---- Swiss + Nitya Yoga ----
                var nityaYogaData = SwissAnalysis.CalculateNityaYogaDataList_London(bufferStartUtc, bufferEndUtc, nodeMode);
                nityaYogaSlices = NityaYogaTransitBuilder.BuildNityaYogaSlices(nityaYogaData, bufferEndUtc);
                // ---- Chandra Bala ----
                chandraBalaSlices = ChandraBalaTransitBuilder.BuildChandraBalaSlices(moonData, birthZodiacMoonId);

                // ---- Preparing Planet slices ----
                sunSlices = PlanetTransitBuilder.BuildPlanetSlices(sunData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                moonSlices = PlanetTransitBuilder.BuildPlanetSlices(moonData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                marsSlices = PlanetTransitBuilder.BuildPlanetSlices(marsData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                mercurySlices = PlanetTransitBuilder.BuildPlanetSlices(mercuryData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                jupiterSlices = PlanetTransitBuilder.BuildPlanetSlices(jupiterData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                venusSlices = PlanetTransitBuilder.BuildPlanetSlices(venusData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                saturnSlices = PlanetTransitBuilder.BuildPlanetSlices(saturnData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                rahuSlices = PlanetTransitBuilder.BuildPlanetSlices(rahuData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);
                ketuSlices = PlanetTransitBuilder.BuildPlanetSlices(ketuData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, nodeMode);

                // ---- Lagna slices ----
                lagnaSlices = LagnaTransitBuilder.BuildLagnaSlices(lagnaData);

                // ---- Preparing TransitPack dictionary ----
                transitPack = new Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>
                {
                    { EPlanet.SUN, sunSlices },
                    { EPlanet.MOON, moonSlices },
                    { EPlanet.MERCURY, mercurySlices },
                    { EPlanet.VENUS, venusSlices },
                    { EPlanet.MARS, marsSlices },
                    { EPlanet.JUPITER, jupiterSlices },
                    { EPlanet.SATURN, saturnSlices },
                    { EPlanet.RAHU, rahuSlices },
                    { EPlanet.KETU, ketuSlices },
                };

                // ---- Preparing Eclipces dictionary ----
                eclipseByLocalDay = (eclipsesData ?? Enumerable.Empty<EclipseData>())
                .GroupBy(e =>
                {
                    var local = TimeZoneInfo.ConvertTimeFromUtc(e.Date, tzInfo);
                    return local.Date;
                })
                .ToDictionary(g => g.Key, g => g.First());

            }
            else
            {
                // If there is no profile/location/time zone, we just build visible days only
                var firstOfMonth = new DateTime(year, month, 1);
                int shift = (7 + (int)firstOfMonth.DayOfWeek - (int)DataCache.Instance.DayOfWeek) % 7;
                var startDate = firstOfMonth.AddDays(-shift);

                var tmp = new List<DateOnly>();
                for (int i = 0; i < 42; i++)
                    tmp.Add(DateOnly.FromDateTime(startDate.AddDays(i)));

                visibleDays = tmp;
            }

            // Create 42 DayItem strictly according to visibleDays
            foreach (var d in visibleDays)
            {
                var date = d.ToDateTime(TimeOnly.MinValue);
                bool isCurrentMonth = date.Month == month && date.Year == year;
                bool isToday = date.Date == DateTime.Today;
                bool hasUserEvents = false;
                Color noteMarkerColor = DataCache.Instance.GetColor(EColor.NOTEMARKER);
                var evCache = DataCache.Instance.UserEventsWindowCache;
                if (profile != null && ctx != null && evCache != null)
                {
                    // date is local-day key (00:00)
                    hasUserEvents = evCache.HasEvents(date);
                }
                var planetMarkersText = string.Empty;   

                IList<PanchangaSegment> nakshatraSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> taraBalaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> tithiSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> karanaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> nityaYogaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> chandraBalaSegments = new List<PanchangaSegment>();
                List<LagnaSlice> lagnaForDay = new List<LagnaSlice>();

                if (profile != null && tzInfo != null)
                {
                    // Preparing Panchanga segments for the day
                    if (nakshatraSlices != null)
                    {
                        nakshatraSegments = PanchangaHelper.BuildSegmentsForDay(
                            nakshatraSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId,
                            slice => $"{slice.NakshatraId}.{nakById.GetValueOrDefault(slice.NakshatraId, "")}",
                            slice => ETransitKind.Nakshatra,
                            slice => slice.NakshatraId);
                    }

                    if (taraBalaSlices != null)
                    {
                        taraBalaSegments = PanchangaHelper.BuildSegmentsForDay(
                            taraBalaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId,
                            slice => $"{taraById.GetValueOrDefault(slice.TaraBalaId, "")} {slice.TaraBalaPercent}%",
                            slice => ETransitKind.TaraBala,
                            slice => slice.TaraBalaId);
                    }

                    if (tithiSlices != null)
                    {
                        tithiSegments = PanchangaHelper.BuildSegmentsForDay(
                            tithiSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId,
                            slice => $"{slice.TithiId}.{tithiById.GetValueOrDefault(slice.TithiId, "")}",
                            slice => ETransitKind.Tithi,
                            slice => slice.TithiId);
                    }

                    if (karanaSlices != null)
                    {
                        karanaSegments = PanchangaHelper.BuildSegmentsForDay(
                            karanaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId,
                            slice => $"{karanaById.GetValueOrDefault(slice.KaranaId, "")}",
                            slice => ETransitKind.Karana,
                            slice => slice.KaranaId);
                    }

                    if (nityaYogaSlices != null)
                    {
                        nityaYogaSegments = PanchangaHelper.BuildSegmentsForDay(
                            nityaYogaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId,
                            slice => $"{slice.NityaYogaId}.{nityaYogaById.GetValueOrDefault(slice.NityaYogaId, "")}",
                            slice => ETransitKind.NityaYoga,
                            slice => slice.NityaYogaId);
                    }

                    if (chandraBalaSlices != null)
                    {
                        string tplHouse = Localization.GetLocalizedText("Moon in {0} house", lang);
                        string tplHouseSign = Localization.GetLocalizedText("Moon in {0} house, {1}", lang);

                        chandraBalaSegments = PanchangaHelper.BuildSegmentsForDay(
                            chandraBalaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId,
                            slice =>
                            {
                                var house = slice.HouseNumber; // 1..12

                                // Show sign only for Scorpio (SCO)
                                if (slice.ZodiacCode == EZodiac.SCO)
                                {
                                    var zodiacId = (int)slice.ZodiacCode;
                                    var zodiacName = GetZodiacName(zodiacId);
                                    return string.Format(tplHouseSign, house, zodiacName);
                                }

                                return string.Format(tplHouse, house);
                            },
                            slice => ETransitKind.ChandraBala,
                            slice => slice.HouseNumber);
                    }

                    // Preparing planet markers for day
                    // Day UTC boundaries (local day of profile)
                    var dayStartLocal = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
                    var dayEndLocal = dayStartLocal.AddDays(1);

                    var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayStartLocal, tzInfo);
                    var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);

                    // Collect planet markers (6 planets, no Moon, Rahu & Ketu)
                    var markers = new List<string>(8);

                    string? m;

                    m = BuildMarkerTextForDay(EPlanet.SUN, sunSlices, dayStartUtc, dayEndUtc);
                    if (!string.IsNullOrEmpty(m)) markers.Add(m);

                    m = BuildMarkerTextForDay(EPlanet.MERCURY, mercurySlices, dayStartUtc, dayEndUtc);
                    if (!string.IsNullOrEmpty(m)) markers.Add(m);

                    m = BuildMarkerTextForDay(EPlanet.VENUS, venusSlices, dayStartUtc, dayEndUtc);
                    if (!string.IsNullOrEmpty(m)) markers.Add(m);

                    m = BuildMarkerTextForDay(EPlanet.MARS, marsSlices, dayStartUtc, dayEndUtc);
                    if (!string.IsNullOrEmpty(m)) markers.Add(m);

                    m = BuildMarkerTextForDay(EPlanet.JUPITER, jupiterSlices, dayStartUtc, dayEndUtc);
                    if (!string.IsNullOrEmpty(m)) markers.Add(m);

                    m = BuildMarkerTextForDay(EPlanet.SATURN, saturnSlices, dayStartUtc, dayEndUtc);
                    if (!string.IsNullOrEmpty(m)) markers.Add(m);

                    // Final planet markers string
                    planetMarkersText = string.Join(' ', markers);

                    lagnaForDay = (lagnaSlices ?? Enumerable.Empty<LagnaSlice>())
                        .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
                        .ToList();
                }

                // Eclipse data for the day (if any)
                eclipseByLocalDay.TryGetValue(date.Date, out var eclipse);

                int? eclipseId = eclipse?.EclipseId;
                DateTime? eclipseDate = eclipse?.Date;

                string? eclipseIcon = null;
                if (eclipse != null)
                {
                    eclipseIcon = eclipse.EclipseId switch
                    {
                        (int)EEclipse.SUNECLIPSE => "solar_eclipse_24.png",
                        (int)EEclipse.MOONECLIPSE => "lunar_eclipse_24.png",
                        _ => null
                    };
                }

                Days.Add(new DayItem
                {
                    Date = date,
                    DayNumber = date.Day,
                    IsCurrentMonth = isCurrentMonth,
                    IsToday = isToday,
                    HasUserEvents = hasUserEvents,
                    NoteMarkerColor = noteMarkerColor,
                    EclipseId = eclipseId,
                    EclipseDate = eclipseDate,    
                    EclipseIcon = eclipseIcon,
                    PlanetMarkersText = planetMarkersText,
                    NakshatraSegments = nakshatraSegments,
                    TaraBalaSegments = taraBalaSegments,
                    TithiSegments = tithiSegments,
                    KaranaSegments = karanaSegments,
                    NityaYogaSegments = nityaYogaSegments,
                    ChandraBalaSegments = chandraBalaSegments,
                    TransitPack = transitPack,
                    LagnaSlices = lagnaForDay
                });
            }
            
            OnPropertyChanged(nameof(Days));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //  Culturally appropriate calendar title
        public string MonthYearTitle
        {
            get
            {
                var raw = new DateTime(Year, Month, 1).ToString("MMMM yyyy", CurrentCulture);
                if (string.IsNullOrEmpty(raw)) return raw;
                return char.ToUpper(raw[0], CurrentCulture) + raw.Substring(1);
            }
        }

        public void SetMonthYear(int year, int month)
        {
            if (Year == year && Month == month)
                return;

            Year = year;
            Month = month;

            RefreshCalendar();

            // if MonthYearTitle can be recalculated — refresh
            OnPropertyChanged(nameof(MonthYearTitle));
        }

        private void UpdateActiveProfileInfo()
        {

            string locality = string.Empty;
            string offsetText = string.Empty;
            string transitionSuffix = string.Empty;
            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;
            
            var profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService?.Current;

            ActiveProfileDisplay = profile?.ProfileName ?? string.Empty;

            int localityId = profile?.PlaceOfLivingId ?? 0;
            locality = DataCache.Instance.LocationList
                .FirstOrDefault(x => x.Id == localityId)?.Locality ?? string.Empty;

            if (ctx != null)
            {
                tzInfo = ctx.TimeZoneInfo;
                var sampleLocal = GetMonthSampleLocalTime(Year, Month);
                var baseOffset = tzInfo.GetUtcOffset(sampleLocal);
                offsetText = FormatUtcOffset(baseOffset);

                // transition (if any)
                var transition = FindOffsetTransitionInMonth(tzInfo, Year, Month);

                if (transition.HasValue)
                {
                    transitionSuffix = GetOffsetChangeSuffix(
                        tzInfo,
                        transition.Value,
                        baseOffset);
                }
            }

            ActiveLocationTimeZoneDisplay = string.IsNullOrWhiteSpace(locality)
                    ? $"{tzInfo.Id}, {offsetText}{transitionSuffix}"
                    : $"{locality}, {offsetText}{transitionSuffix}";
        }

        private static DateTime GetMonthSampleLocalTime(int year, int month)
        {
            int day = Math.Min(15, DateTime.DaysInMonth(year, month));
            return new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Unspecified);
        }

        private static string FormatUtcOffset(TimeSpan offset)
        {
            var sign = offset >= TimeSpan.Zero ? "+" : "-";
            offset = offset.Duration();
            return $"UTC{sign}{offset.Hours:00}:{offset.Minutes:00}";
        }

        /// <summary>
        /// Returns the local DateTime of DST/offset transition within the specified month (if any).
        /// If there are multiple transitions (rare), returns the first.
        /// </summary>
        private static DateTime? FindOffsetTransitionInMonth(TimeZoneInfo tz, int year, int month)
        {
            var days = DateTime.DaysInMonth(year, month);

            // Compare offset at noon for each day (stable, avoids ambiguous hours)
            TimeSpan prevOffset = tz.GetUtcOffset(new DateTime(year, month, 1, 12, 0, 0, DateTimeKind.Unspecified));

            for (int d = 2; d <= days; d++)
            {
                var noon = new DateTime(year, month, d, 12, 0, 0, DateTimeKind.Unspecified);
                var off = tz.GetUtcOffset(noon);

                if (off != prevOffset)
                {
                    // Transition happened between day d-1 noon and day d noon.
                    // Narrow down within that 24h window.
                    var start = new DateTime(year, month, d - 1, 0, 0, 0, DateTimeKind.Unspecified);
                    var end = new DateTime(year, month, d, 23, 59, 59, DateTimeKind.Unspecified);

                    return FindTransitionByBinarySearch(tz, start, end);
                }

                prevOffset = off;
            }

            return null;
        }

        private static DateTime FindTransitionByBinarySearch(TimeZoneInfo tz, DateTime startLocal, DateTime endLocal)
        {
            // Binary search by offset change
            TimeSpan startOffset = tz.GetUtcOffset(startLocal);
            TimeSpan endOffset = tz.GetUtcOffset(endLocal);

            // Safety: if no change, return start
            if (startOffset == endOffset) return startLocal;

            DateTime lo = startLocal;
            DateTime hi = endLocal;

            // Search down to ~1 minute precision
            while ((hi - lo).TotalMinutes > 1)
            {
                var midTicks = lo.Ticks + (hi.Ticks - lo.Ticks) / 2;
                var mid = new DateTime(midTicks, DateTimeKind.Unspecified);

                var midOffset = tz.GetUtcOffset(mid);
                if (midOffset == startOffset)
                    lo = mid;
                else
                    hi = mid;
            }

            // hi is the first moment where offset differs (approx)
            return hi;
        }

        private static string GetOffsetChangeSuffix(
            TimeZoneInfo tz,
            DateTime transitionLocal,
            TimeSpan currentMonthOffset)
        {
            // offset immediately AFTER the transition
            var after = tz.GetUtcOffset(transitionLocal.AddMinutes(5));

            var delta = after - currentMonthOffset;
            if (delta == TimeSpan.Zero)
                return string.Empty;

            var hours = Math.Abs(delta.TotalHours);

            var sign = delta.TotalHours > 0 ? "+" : "-";
            return $" ({sign}{hours:0.#}h {transitionLocal:yyyy-MM-dd})";
        }

        public static string? BuildMarkerTextForDay(
            EPlanet planet,
            IReadOnlyList<PlanetSlice> slicesSorted,
            DateTime dayStartUtc,
            DateTime dayEndUtc)
        {
            if (slicesSorted == null || slicesSorted.Count == 0)
                return null;

            // base slice on day start
            PlanetSlice? baseSlice = null;
            for (int i = slicesSorted.Count - 1; i >= 0; i--)
            {
                if (slicesSorted[i].StartUtc <= dayStartUtc)
                {
                    baseSlice = slicesSorted[i];
                    break;
                }
            }
            baseSlice ??= slicesSorted[0];

            bool zodiacChanged = false;

            bool retroEnter = false; // direct -> retro
            bool retroExit = false; // retro -> direct

            // We'll keep the "last slice state within the day" to know current zodiac at the end of day
            // (needed for retro-exit to show Pl / Pl↑ / Pl↓ correctly)
            PlanetSlice endStateSlice = baseSlice;

            for (int i = 0; i < slicesSorted.Count; i++)
            {
                var s = slicesSorted[i];
                if (s.StartUtc < dayStartUtc) continue;
                if (s.StartUtc >= dayEndUtc) break;

                endStateSlice = s;

                if (s.ZodiacId != baseSlice.ZodiacId)
                    zodiacChanged = true;

                if (s.IsRetrograde != baseSlice.IsRetrograde)
                {
                    if (!baseSlice.IsRetrograde && s.IsRetrograde) retroEnter = true;
                    if (baseSlice.IsRetrograde && !s.IsRetrograde) retroExit = true;
                }
            }

            // Also: if there are no slices starting inside the day, endStateSlice should reflect the state during the day.
            // baseSlice is enough in that case.

            // Decide marker kind by strict rules
            string eventSymbol = "";
            bool showR = false;
            bool showPlainPl = false;

            if (retroEnter)
            {
                // Retro entry cancels exalt/debil markers completely
                showR = true;

                // Only allowed extra is retro + zodiac change
                if (zodiacChanged)
                    eventSymbol = "→"; // => Pl.R→
                else
                    eventSymbol = "";  // => Pl.R
            }
            else if (retroExit)
            {
                // On retro exit, we show what planet is NOW (direct), based on current zodiac
                var exNow = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)endStateSlice.ZodiacId);

                if (exNow == EExaltation.EXALTATION) eventSymbol = "↑";
                else if (exNow == EExaltation.DEBILITATION) eventSymbol = "↓";
                else if (zodiacChanged) eventSymbol = "→";
                else showPlainPl = true; // => just Pl
            }
            else
            {
                // No retro toggle today: we show only change events
                // Exalt/debil "entry" is detected relative to start-of-day state
                var startEx = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)baseSlice.ZodiacId);

                bool exaltEntry = false;
                bool debilEntry = false;

                for (int i = 0; i < slicesSorted.Count; i++)
                {
                    var s = slicesSorted[i];
                    if (s.StartUtc < dayStartUtc) continue;
                    if (s.StartUtc >= dayEndUtc) break;

                    var ex = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)s.ZodiacId);

                    if (startEx != EExaltation.EXALTATION && ex == EExaltation.EXALTATION)
                        exaltEntry = true;

                    if (startEx != EExaltation.DEBILITATION && ex == EExaltation.DEBILITATION)
                        debilEntry = true;
                }

                // ↑/↓ override → (no Pl→↑, no Pl→↓)
                if (exaltEntry) eventSymbol = "↑";
                else if (debilEntry) eventSymbol = "↓";
                else if (zodiacChanged) eventSymbol = "→";
                else return null; // no change today => no marker

                // Allowed: Pl.R→ only (retro state + zodiac change), but NOT Pl.R↑ / Pl.R↓
                if (eventSymbol == "→" && baseSlice.IsRetrograde)
                    showR = true;
            }

            // If retroExit but none of symbols and showPlainPl==false -> shouldn't happen, but safe:
            if (!showPlainPl && eventSymbol.Length == 0 && !showR)
                return null;

            // planet short name (localized, first 2 chars)
            var lang = DataCache.Instance.CurrentLanguageCode;
            string pl = DataCache.Instance.PlanetDescList
                .FirstOrDefault(p => p.LanguageCode == lang && p.PlanetId == (int)planet)?.Name ?? string.Empty;
            var text = pl.Length >= 2 ? pl.Substring(0, 2) : pl;

            if (showR)
                text += ".R";

            if (showPlainPl)
                return text; // "Pl"

            text += eventSymbol;
            return text;
        }

        private static string GetZodiacName(int zodiacId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            string zodiac = DataCache.Instance.ZodiacDescList
                .FirstOrDefault(z => z.LanguageCode == lang && z.ZodiacId == zodiacId)?.Name ?? string.Empty;
            var text = zodiac.Length >= 3 ? zodiac.Substring(0, 3) : zodiac;

            return text;
        }


    }
}
