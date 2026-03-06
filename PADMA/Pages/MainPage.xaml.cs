using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using PADMA.Core.Enums;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI;
using PADMA.UI.Services;
using PADMA.UI.ViewModels;
using System.Globalization;

namespace PADMA.Pages
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel Vm => BindingContext as CalendarViewModel;
        private bool _needsRefreshAfterConfig = false;
        private bool _notificationsPermissionChecked;

        public MainPage()
        {
            InitializeComponent();

            if (BindingContext is not CalendarViewModel)
                BindingContext = new CalendarViewModel();

            // Culture initialization
            Vm.InitializeCulture();
            var culture = Vm.CurrentCulture ?? CultureInfo.CurrentCulture;
            var popup = new MonthPickerPopup(culture, Vm.Year, Vm.Month);
            var dayService = ServiceLocator.Services.GetService<IDayComputationService>();

            MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
            MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
            {
                dayService?.InvalidateAll();
                _needsRefreshAfterConfig = true;

                if (Shell.Current is AppShell shell)
                    shell.RefreshFlyoutTitles();
            });
            
            MessagingCenter.Unsubscribe<object>(this, "ProfileChanged");
            MessagingCenter.Subscribe<object>(this, "ProfileChanged", _ =>
            {
                dayService?.InvalidateAll();

                Vm?.ReloadCultureAndRefresh();
                UpdateDaysHeader();
            });

            MessagingCenter.Unsubscribe<object>(this, "UserEventsChanged");
            MessagingCenter.Subscribe<object>(this, "UserEventsChanged", _ =>
            {
                Vm?.RebuildCurrentMonth(); // пересоберёт Days, и HasUserEvents подтянется из кеша
                UpdateDaysHeader();
            });

            AddToolbarButtons();
            UpdateDaysHeader();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_notificationsPermissionChecked)
            {
                _notificationsPermissionChecked = true;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    #if ANDROID
                        try
                        {
                            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                            if (status != PermissionStatus.Granted)
                                await Permissions.RequestAsync<Permissions.PostNotifications>();
                        }
                        catch { }
                    #endif

                    try
                    {
                        var provider = ServiceLocator.Services.GetService<ILocalNotificationProvider>();
                        if (provider != null)
                            await provider.EnsurePermissionsAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PADMA] EnsurePermissions error: {ex.Message}");
                    }
                });
            }

            if (_needsRefreshAfterConfig)
            {
                _needsRefreshAfterConfig = false; // drop flag to avoid infinitive loops

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // allow UI to settle before doing heavy work
                    await Task.Yield();

                    // refresh data cache
                    var db = ServiceLocator.Services.GetService<DatabaseService>();
                    DataCache.Instance.Refresh(db);

                    Vm?.ReloadCultureAndRefresh();
                    UpdateDaysHeader();
                });
            }

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (BindingContext is CalendarViewModel vm && height > 0)
            {
                // 70% экрана под карточку, остальное - поля/заголовок/воздух
                vm.NotesOverlayMaxHeight = height * 0.7;
                vm.NotesOverlayListMaxHeight = height * 0.45;
            }
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

            // Taking first day of week from settings
            var firstDay = ServiceLocator.Services
                .GetService<DatabaseService>()
                .GetFirstDayOfWeekFromDb();

            var ordered = Enumerable.Range(0, 7)
                .Select(i => abbreviated[((int)firstDay + i) % 7])
                .ToArray();

            for (int i = 0; i < 7; i++)
            {
                // Border around each cell
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

            // important: reseting the CollectionView selection so that the next tap on the same element will trigger SelectionChanged again.
            ((CollectionView)sender).SelectedItem = null;

            // if tapped day is already selected, navigate to DayOverview
            if (Vm.SelectedDay != null && Vm.SelectedDay.Date == tapped.Date)
            {
                var navStore = ServiceLocator.Services.GetService<NavigationDataStore>();

                string? windowToken = null;
                if (navStore != null)
                {
                    var daysSnapshot = Vm.Days.ToList();
                    var idx = daysSnapshot.FindIndex(d => d.Date.Date == tapped.Date.Date);
                    if (idx < 0) idx = 0;

                    var window = new DayWindowContext
                    {
                        Days = daysSnapshot,
                        SelectedIndex = idx
                    };

                    windowToken = navStore.Put(window);
                }

                var parameters = new Dictionary<string, object>{{ "Day", tapped }};

                if (!string.IsNullOrWhiteSpace(windowToken))
                    parameters["WindowToken"] = windowToken;

                await Shell.Current.GoToAsync("dayOverview", true, parameters);
                return;
            }

            // orderwise, just select the tapped day
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

        protected override bool OnBackButtonPressed()
        {
            Dispatcher.Dispatch(async () =>
            {
                var lang = DataCache.Instance.CurrentLanguageCode;
                string L(string nativeEn) => Localization.GetLocalizedText(nativeEn, lang);

                var ok = await DisplayAlert(
                    L("Exit application?"),
                    L("Do you want to exit PADMA Application?"),
                    L("Yes"),
                    L("No"));

                if (ok)
                    AppCloser.Close();
            });
            return true;
        }

        private void OnNotesTriangleTapped(object sender, EventArgs e)
        {
            if (Vm == null) return;
            if (sender is not BindableObject bo) return;
            if (bo.BindingContext is not DayItem day) return;

            Vm.ShowNotesOverlayForDay(day.Date);
        }

        private void OnNotesOverlayBackdropTapped(object sender, EventArgs e)
        {
            Vm?.HideNotesOverlay();
        }



    }
}
