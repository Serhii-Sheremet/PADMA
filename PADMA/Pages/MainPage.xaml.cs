using Microsoft.Maui.Controls;
using PADMA.Core.Analysis;
using PADMA.Core.Models;
using PADMA.Core.Native;
using PADMA.Core.Services;
using PADMA.UI;
using System;
using System.Globalization;
using System.Linq;

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

            MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
            {
                _needsRefreshAfterConfig = true;

                Vm?.ReloadCultureAndRefresh();
                UpdateTitle();
                UpdateDaysHeader();
            });

            UpdateTitle();
            AddToolbarButtons();
            UpdateDaysHeader();
        }

        public static async Task RunAsync()
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



        protected override void OnAppearing()
        {
            base.OnAppearing();


            _ = RunAsync();


            try
            {
                if (_needsRefreshAfterConfig)
                {
                    // На всякий случай подтянем свежие настройки и тексты из БД в кэш
                    var db = ServiceLocator.Services.GetService<DatabaseService>();
                    DataCache.Instance.Refresh(db);

                    Vm?.ReloadCultureAndRefresh();
                    UpdateTitle();
                    UpdateDaysHeader();

                    _needsRefreshAfterConfig = false; // сброс флага после обновления
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] MainPage.OnAppearing error: {ex.Message}");
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
