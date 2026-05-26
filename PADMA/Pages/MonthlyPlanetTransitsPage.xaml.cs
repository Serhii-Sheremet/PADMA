using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI;
using PADMA.UI.ViewModels;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using System.ComponentModel;
using System.Globalization;

namespace PADMA.Pages;

public partial class MonthlyPlanetTransitsPage : ContentPage
{
    private MonthlyPlanetTransitsViewModel Vm => BindingContext as MonthlyPlanetTransitsViewModel;
    private bool _needsRefreshAfterConfig;

    private bool _isPageAppeared;
    private bool _isBuildingTimeline;
    private bool _timelineBuildPending;

    private readonly MonthlyTransitsHeaderDrawable _headerDrawable = new();
    private readonly MonthlyTransitsLabelsDrawable _labelsDrawable = new();
    private readonly MonthlyTransitsBodyDrawable _bodyDrawable = new();

    private bool _syncingHorizontalScroll;
    private bool _syncingVerticalScroll;

    private bool _isClosing;
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

    private string _busyText = "Please waitЕ";
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

    private static readonly EPlanet[] PlanetOrder =
    [
        EPlanet.SUN,
        EPlanet.MOON,
        EPlanet.MARS,
        EPlanet.MERCURY,
        EPlanet.JUPITER,
        EPlanet.VENUS,
        EPlanet.SATURN,
        EPlanet.RAHU,
        EPlanet.KETU
    ];

    public MonthlyPlanetTransitsPage()
    {
        InitializeComponent();

        if (BindingContext is not MonthlyPlanetTransitsViewModel)
            BindingContext = new MonthlyPlanetTransitsViewModel();

        Vm.InitializeCulture();
        Vm.PropertyChanged += OnViewModelPropertyChanged;

        MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
        MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
        {
            _needsRefreshAfterConfig = true;

            if (Shell.Current is AppShell shell)
                shell.RefreshFlyoutTitles();
        });

        MessagingCenter.Unsubscribe<object>(this, "ProfileChanged");
        MessagingCenter.Subscribe<object>(this, "ProfileChanged", _ =>
        {
            Vm?.ReloadCultureAndRefresh();
        });

        AddToolbarButtons();
        InitializeGraphicsViews();
        RebuildTimelineSkeleton();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _isPageAppeared = true;

        // ƒать Shell шанс закрыть burger menu перед т€желым построением UI.
        if (Shell.Current is not null)
            Shell.Current.FlyoutIsPresented = false;

        await Task.Delay(150);

        if (_needsRefreshAfterConfig)
        {
            _needsRefreshAfterConfig = false;

            await RunBusyAsync(
                Localization.GetLocalizedText("Please wait...", DataCache.Instance.CurrentLanguageCode),
                async () =>
                {
                    await Task.Yield();

                    var db = ServiceLocator.Services.GetService<DatabaseService>();
                    DataCache.Instance.Refresh(db);

                    Vm?.ReloadCultureAndRefresh();

                    await RebuildTimelineAsync();
                });

            return;
        }

        await RunBusyAsync(
            Localization.GetLocalizedText("Calculating transits", DataCache.Instance.CurrentLanguageCode),
            async () =>
            {
                await RebuildTimelineAsync();
            });
    }

    private async Task RebuildTimelineAsync()
    {
        if (_isBuildingTimeline)
        {
            _timelineBuildPending = true;
            return;
        }

        try
        {
            _isBuildingTimeline = true;

            do
            {
                _timelineBuildPending = false;

                await Task.Yield();

                RebuildTimelineSkeleton();

                await Task.Yield();
            }
            while (_timelineBuildPending);
        }
        finally
        {
            _isBuildingTimeline = false;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MonthlyPlanetTransitsViewModel.Year) ||
            e.PropertyName == nameof(MonthlyPlanetTransitsViewModel.Month) ||
            e.PropertyName == nameof(MonthlyPlanetTransitsViewModel.CultureCode))
        {
            MainThread.BeginInvokeOnMainThread(RebuildTimelineSkeleton);
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (_isClosing)
            return true;

        Dispatcher.Dispatch(async () =>
        {
            await CloseAsync();
        });
        return true;
    }

    private async Task CloseAsync()
    {
        if (_isClosing)
            return;

        _isClosing = true;

        try
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            await RunBusyAsync(Localization.GetLocalizedText("Please waitЕ", lang), async () =>
            {
                await Shell.Current.GoToAsync("//main");
                await Task.Yield();
            });
        }
        finally
        {
            _isClosing = false;
        }
    }

    private async Task RunBusyAsync(string text, Func<Task> action)
    {
        BusyText = text;
        IsBusy = true;

        await Task.Yield(); // дать UI шанс отрисовать overlay

        try { await action(); }
        finally { IsBusy = false; }
    }

    private void AddToolbarButtons()
    {
        ToolbarItems.Clear();

        var prev = new ToolbarItem
        {
            IconImageSource = "left_arrow.png",
        };
        prev.Clicked += (s, e) =>
        {
            if (Vm is null) return;

            var dt = new DateTime(Vm.Year, Vm.Month, 1).AddMonths(-1);
            Vm.SetMonthYear(dt.Year, dt.Month);
        };

        var next = new ToolbarItem
        {
            IconImageSource = "right_arrow.png",
        };
        next.Clicked += (s, e) =>
        {
            if (Vm is null) return;

            var dt = new DateTime(Vm.Year, Vm.Month, 1).AddMonths(1);
            Vm.SetMonthYear(dt.Year, dt.Month);
        };

        ToolbarItems.Add(prev);
        ToolbarItems.Add(next);
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

    private void InitializeGraphicsViews()
    {
        HeaderGraphicsView.Drawable = _headerDrawable;
        LabelsGraphicsView.Drawable = _labelsDrawable;
        BodyGraphicsView.Drawable = _bodyDrawable;
    }

    private void RebuildTimelineSkeleton()
    {
        if (Vm is null)
            return;

        var layout = CreateLayout();

        HeaderGraphicsView.WidthRequest = layout.ContentWidth;
        HeaderGraphicsView.HeightRequest = layout.HeaderHeight;

        LabelsGraphicsView.WidthRequest = layout.LabelWidth;
        LabelsGraphicsView.HeightRequest = layout.ContentHeight;

        BodyGraphicsView.WidthRequest = layout.ContentWidth;
        BodyGraphicsView.HeightRequest = layout.ContentHeight;

        _headerDrawable.Layout = layout;
        _labelsDrawable.Layout = layout;
        _bodyDrawable.Layout = layout;

        HeaderGraphicsView.Invalidate();
        LabelsGraphicsView.Invalidate();
        BodyGraphicsView.Invalidate();
    }

    private MonthlyTransitsLayout CreateLayout()
    {
        return new MonthlyTransitsLayout
        {
            Year = Vm.Year,
            Month = Vm.Month,
            Culture = Vm.CurrentCulture,
            TopBandLabel = Localization.GetLocalizedText("Masa/Shunya", DataCache.Instance.CurrentLanguageCode),
            Planets = PlanetOrder
                .Select(x => new MonthlyTransitsPlanetRow
                {
                    Planet = x,
                    Name = GetPlanetName(x)
                })
                .ToList()
        };
    }

    private static string GetShortDayOfWeek(DateTime date, CultureInfo culture)
    {
        var text = culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek);

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        text = text.Replace(".", string.Empty);

        if (text.Length <= 2)
            return text;

        return text[..2];
    }

    private static string GetPlanetName(EPlanet planet)
    {
        return DataCache.Instance.PlanetDescList
            .FirstOrDefault(x =>
                x.PlanetId == (int)planet &&
                x.LanguageCode == DataCache.Instance.CurrentLanguageCode)
            ?.Name
            ?? planet.ToString();
    }

    private async void OnBodyHorizontalScrolled(object sender, ScrolledEventArgs e)
    {
        if (_syncingHorizontalScroll)
            return;

        try
        {
            _syncingHorizontalScroll = true;
            await HeaderHorizontalScroll.ScrollToAsync(e.ScrollX, 0, false);
        }
        finally
        {
            _syncingHorizontalScroll = false;
        }
    }

    private async void OnHeaderHorizontalScrolled(object sender, ScrolledEventArgs e)
    {
        if (_syncingHorizontalScroll)
            return;

        try
        {
            _syncingHorizontalScroll = true;
            await BodyHorizontalScroll.ScrollToAsync(e.ScrollX, 0, false);
        }
        finally
        {
            _syncingHorizontalScroll = false;
        }
    }

    private async void OnBodyVerticalScrolled(object sender, ScrolledEventArgs e)
    {
        if (_syncingVerticalScroll)
            return;

        try
        {
            _syncingVerticalScroll = true;
            await LabelsVerticalScroll.ScrollToAsync(0, e.ScrollY, false);
        }
        finally
        {
            _syncingVerticalScroll = false;
        }
    }

    private async void OnLabelsVerticalScrolled(object sender, ScrolledEventArgs e)
    {
        if (_syncingVerticalScroll)
            return;

        try
        {
            _syncingVerticalScroll = true;
            await BodyVerticalScroll.ScrollToAsync(0, e.ScrollY, false);
        }
        finally
        {
            _syncingVerticalScroll = false;
        }
    }



}
