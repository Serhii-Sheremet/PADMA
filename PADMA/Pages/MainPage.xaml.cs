using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using GeoTimeZone;
using NodaTime;
using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Native;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace PADMA.Pages
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel Vm => BindingContext as CalendarViewModel;
        private bool _needsRefreshAfterConfig = false;

        public MainPage()
        {
            InitializeComponent();

            if (BindingContext is not CalendarViewModel)
                BindingContext = new CalendarViewModel();

            // Инициализация культуры
            Vm.InitializeCulture();
            var culture = Vm.CurrentCulture ?? CultureInfo.CurrentCulture;
            var popup = new MonthPickerPopup(culture, Vm.Year, Vm.Month);

            MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
            MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
            {
                _needsRefreshAfterConfig = true;

                Vm?.ReloadCultureAndRefresh();
                UpdateDaysHeader();
            });
            
            MessagingCenter.Unsubscribe<object>(this, "ProfileChanged");
            MessagingCenter.Subscribe<object>(this, "ProfileChanged", _ =>
            {
                Vm?.ReloadCultureAndRefresh();
                UpdateDaysHeader();
            });

            AddToolbarButtons();
            UpdateDaysHeader();
        }

        // Test method for Swiss Ephemeris PlanetData calculations
        public static async Task RunPlanetTestAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Swiss Ephemeris Test: Moon states (Lahiri) ===");

                await SwissService.InitializeEphemerisPathAsync();
                SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

                // Быстрый smoke-test перед длинным расчётом
                var jd0 = SwissEphemerisNative.swe_julday(2025, 10, 29, 0.0, SweConst.SE_GREG_CAL);
                var xx = new double[6];
                var serr = new System.Text.StringBuilder(256);
                int rc = SwissEphemerisNative.swe_calc_ut(jd0, SweConst.SE_MOON,
                           SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SIDEREAL | SweConst.SEFLG_SPEED, xx, serr);
                System.Diagnostics.Debug.WriteLine($"[SMOKE] rc={rc} lon={xx[0]:F4} err={serr}");

                var list = SwissAnalysis.CalculatePlanetDataList_London(
                    planetId: 2,
                    startUtc: new DateTime(2025, 10, 27, 0, 0, 0, DateTimeKind.Utc),
                    endUtc: new DateTime(2025, 11, 03, 0, 0, 0, DateTimeKind.Utc));

                foreach (var d in list)
                    System.Diagnostics.Debug.WriteLine(d.ToString());

                System.Diagnostics.Debug.WriteLine($"[DONE] Total states: {list.Count}");

                SwissService.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[TEST][ERROR] " + ex.ToString());
                // при желании: выводить в UI Alert/Label
            }
        }

        /// <summary>
        /// Simple debug test: calculates all Tithi changes for a given UTC range.
        /// </summary>
        public static async Task RunTithiTestAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== Swiss Ephemeris Test: Tithi states (Lahiri, London) ===");

                await SwissService.InitializeEphemerisPathAsync();
                SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

                // --- smoke-тест для проверки инициализации ---
                var jd0 = SwissEphemerisNative.swe_julday(2025, 10, 29, 0.0, SweConst.SE_GREG_CAL);
                var xx = new double[6];
                var serr = new System.Text.StringBuilder(256);
                int rc = SwissEphemerisNative.swe_calc_ut(
                    jd0,
                    SweConst.SE_MOON,
                    SweConst.SEFLG_SWIEPH | SweConst.SEFLG_SIDEREAL | SweConst.SEFLG_SPEED,
                    xx,
                    serr);
                System.Diagnostics.Debug.WriteLine($"[SMOKE] rc={rc} lon={xx[0]:F4} err={serr}");
                // ---------------------------------------------

                DateTime fromDate = new(2025, 10, 27, 0, 0, 0, DateTimeKind.Utc);
                DateTime toDate = new(2025, 11, 3, 0, 0, 0, DateTimeKind.Utc);

                var tithiDataList = SwissAnalysis.CalculateTithiDataList_London(fromDate, toDate);

                foreach (var t in tithiDataList)
                    System.Diagnostics.Debug.WriteLine(
                        $"{t.DateTimeUtc:yyyy-MM-dd HH:mm:ss} | Δ={t.MoonSunDifference:F4}° | Tithi={t.TithiId}");

                System.Diagnostics.Debug.WriteLine($"[DONE] Total tithis: {tithiDataList.Count}");

                SwissService.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[TEST][ERROR] " + ex);
            }
        }

        async void TestAscendant()
        {
            // 1) Координаты Чёрный Остров, Украина
            double lat = 49.506984;
            double lon = 26.764657;
            double alt = 0.0;
            char hsys = 'O'; // Placidus

            // 2) Локальная дата рождения (БЕЗ смещения)
            var localBirth = new LocalDateTime(1971, 12, 5, 0, 40, 0);

            // 3) Исторический таймзон по координатам → IANA
            string iana = TimeZoneLookup.GetTimeZone(lat, lon).Result; // напр. "Europe/Kyiv"
            var zone = DateTimeZoneProviders.Tzdb[iana];

            // 4) Локальное → UTC через NodaTime (учтёт исторический DST/офсеты)
            var zoned = zone.AtLeniently(localBirth);
            DateTime utcBirth = zoned.ToInstant().ToDateTimeUtc();

            // (опционально) убедимся, что пути к эфемеридам и сидерика активны
            await SwissService.InitializeEphemerisPathAsync();
            SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);

            // 5) Расчёт асцендента
            double asc = SwissService.CalculateAscendantForDate(utcBirth, lat, lon, alt, hsys);

            // 6) Знак и градус внутри знака
            double ascNorm = SwissService.NormalizeDegrees(asc);
            int sign = SwissUtility.GetZodiacIdFromDegree(ascNorm); // 1..12
            double degInSign = ascNorm - (sign - 1) * 30.0;

            // 7) Вывод
            Console.WriteLine($"IANA: {iana}");
            Console.WriteLine($"Local: {localBirth:yyyy-MM-dd HH:mm:ss}  →  UTC: {utcBirth:yyyy-MM-dd HH:mm:ss}Z");
            Console.WriteLine($"Ascendant: {ascNorm:F6}°  |  Sign #{sign}  |  {degInSign:F2}° in sign");
        }

        async void TestSunriseSunsetUniversal()
        {
            // Координаты Варшавы
            double lon = 21.0;
            double lat = 52.25;
            double alt = 0;

            // Период — проверяем конец октября и ноябрь 2025
            var from = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2025, 11, 30, 0, 0, 0, DateTimeKind.Utc);

            Console.WriteLine("=== Sunrise / Sunset for Warsaw (universal DST-safe test) ===");

            for (var date = from; date <= to; date = date.AddDays(1))
            {
                // Берем середину суток — это стабилизирует расчёты Swiss Ephemeris
                var dateMid = date.AddHours(12);

                var srUtc = SwissService.CalculateSunriseForDateAndLocation(dateMid, lat, lon, alt);
                var ssUtc = SwissService.CalculateSunsetForDateAndLocation(dateMid, lat, lon, alt);

                if (srUtc is DateTime sru && ssUtc is DateTime ssu)
                {
                    // Наш новый универсальный метод конвертации UTC → Local
                    var srLocal = TimeZoneService.ConvertUtcToLocalSmart(
                        DateTime.SpecifyKind(sru, DateTimeKind.Utc), lat, lon);
                    var ssLocal = TimeZoneService.ConvertUtcToLocalSmart(
                        DateTime.SpecifyKind(ssu, DateTimeKind.Utc), lat, lon);

                    Console.WriteLine($"{date:yyyy-MM-dd} | Sunrise: {srLocal:HH:mm} | Sunset: {ssLocal:HH:mm}");
                }
                else
                {
                    Console.WriteLine($"{date:yyyy-MM-dd} | No data");
                }
            }

            Console.WriteLine("=== End of test ===");
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            // _ = RunPlanetTestAsync();
            // _ = RunTithiTestAsync();
            //TestAscendant();
            //TestSunriseSunsetUniversal();

            /*
            var from = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddDays(31);
            var yogas = SwissAnalysis.CalculateNityaYogaDataList_London(from, to);
            foreach (var y in yogas)
                Console.WriteLine(y);
            */
            /*
            var from = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = from.AddDays(30);

            var mercuryMb = SwissAnalysis.CalculateMrityuBhagaDataList_London((int)EPlanet.MOON, from, to);
            foreach (var r in mercuryMb)
                Console.WriteLine("[TEST] " + r);
            */

            /*
            var from = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2030, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            var eclipses = SwissAnalysis.CalculateEclipses_London(from, to);
            foreach (var e in eclipses)
                Console.WriteLine($"{(EEclipse)e.EclipseId} | {e.Date:yyyy-MM-dd HH:mm}");
            */

            if (_needsRefreshAfterConfig)
            {
                _needsRefreshAfterConfig = false; // сразу сбросить, чтобы не зациклиться

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // дать MainPage реально отрисоваться (и показать overlay календаря)
                    await Task.Yield();

                    // На всякий случай подтянем свежие настройки и тексты из БД в кэш
                    var db = ServiceLocator.Services.GetService<DatabaseService>();
                    DataCache.Instance.Refresh(db);

                    Vm?.ReloadCultureAndRefresh();
                    UpdateDaysHeader();
                });
            }
            
        }
        
        private void UpdateTitle()
        {
            if (Vm == null) return;

            var rawTitle = new DateTime(Vm.Year, Vm.Month, 1).ToString("MMMM yyyy", Vm.CurrentCulture);
            if (!string.IsNullOrEmpty(rawTitle))
                Title = char.ToUpper(rawTitle[0], Vm.CurrentCulture) + rawTitle.Substring(1);
            else
                Title = rawTitle;
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
                var dt = new DateTime(Vm.Year, Vm.Month, 1).AddMonths(-1);
                Vm.SetMonthYear(dt.Year, dt.Month);
                UpdateDaysHeader();
            };

            var next = new ToolbarItem
            {
                IconImageSource = "right_arrow.png",
                Text = "Next"
            };
            next.Clicked += (s, e) =>
            {
                var dt = new DateTime(Vm.Year, Vm.Month, 1).AddMonths(1);
                Vm.SetMonthYear(dt.Year, dt.Month);
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

            // Берём первый день недели из настроек (как и раньше)
            var firstDay = ServiceLocator.Services
                .GetService<DatabaseService>()
                .GetFirstDayOfWeekFromDb();

            var ordered = Enumerable.Range(0, 7)
                .Select(i => abbreviated[((int)firstDay + i) % 7])
                .ToArray();

            for (int i = 0; i < 7; i++)
            {
                // Граница как у ячеек
                var cell = new Border
                {
                    Stroke = Colors.Black,
                    StrokeThickness = 0.5,
                    Padding = 0,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Content = new Label
                    {
                        Text = ordered[i],
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 13,
                        TextColor = Colors.Black
                    }
                };

                Grid.SetColumn(cell, i);
                DaysHeaderGrid.Children.Add(cell);
            }
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Vm is null) return;

            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            var tapped = e.CurrentSelection[0] as DayItem;
            if (tapped == null)
                return;

            // важно: сбрасываем выделение CollectionView, чтобы следующий тап по тому же элементу снова вызвал SelectionChanged
            ((CollectionView)sender).SelectedItem = null;

            // 1) если тапнули по уже выбранному дню -> открываем DayOverview
            if (Vm.SelectedDay != null && Vm.SelectedDay.Date == tapped.Date)
            {
                await Shell.Current.GoToAsync("dayOverview", true,
                        new Dictionary<string, object> { { "Day", tapped } });
                return;
            }

            // 2) иначе просто выбираем день (подсветка)
            Vm.SelectedDay = tapped;
        }

        private async void OnMonthTitleTapped(object sender, EventArgs e)
        {
            if (Vm is null) return;

            var popup = new MonthPickerPopup(Vm.CurrentCulture, Vm.Year, Vm.Month);

            var res = await this.ShowPopupAsync<DateTime?>(
                popup,
                PopupOptions.Empty,
                CancellationToken.None);

            if (res.WasDismissedByTappingOutsideOfPopup)
                return;

            var dt = res.Result;
            if (dt.HasValue)
                Vm.SetMonthYear(dt.Value.Year, dt.Value.Month);
        }



    }
}
