using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using PADMA.Core.Enums;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.ViewModels;
using PADMA.UI.YearlyTransits;
using System.ComponentModel;

namespace PADMA.Pages;

public partial class YearlyPlanetTransitsPage : ContentPage
{
    private YearlyPlanetTransitsViewModel Vm => BindingContext as YearlyPlanetTransitsViewModel;

    private readonly YearlyTransitsHeaderDrawable _headerDrawable = new();
    private readonly YearlyTransitsLabelsDrawable _labelsDrawable = new();
    private readonly YearlyTransitsBodyDrawable _bodyDrawable = new();

    private bool _syncingHorizontalScroll;
    private bool _syncingVerticalScroll;
    private bool _hasInitialized;
    private bool _needsRefreshAfterConfig;
    private int? _selectedMonth;

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

    public YearlyPlanetTransitsPage()
    {
        InitializeComponent();

        BindingContext = new YearlyPlanetTransitsViewModel();
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
            _selectedMonth = null;
            Vm?.ReloadCultureAndRefresh();
            RebuildTimeline();
        });

        AddToolbarButtons();
        InitializeGraphicsViews();
        RebuildTimeline();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await Task.Delay(150);

        if (Shell.Current is not null)
            Shell.Current.FlyoutIsPresented = false;

        if (!_hasInitialized)
        {
            _hasInitialized = true;
            Vm?.SetYear(DateTime.Today.Year);
        }

        if (_needsRefreshAfterConfig)
        {
            _needsRefreshAfterConfig = false;

            var db = ServiceLocator.Services.GetService<DatabaseService>();
            DataCache.Instance.Refresh(db);

            _selectedMonth = null;
            Vm?.ReloadCultureAndRefresh();
        }

        RebuildTimeline();
        
        await Task.Yield();
        await ResetScrollPositionsAsync();
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
            await RunBusyAsync(Localization.GetLocalizedText("Please wait…", lang), async () =>
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

        await Task.Yield(); // äàòü UI øàíñ îòðèñîâàòü overlay

        try { await action(); }
        finally { IsBusy = false; }
    }

    private void AddToolbarButtons()
    {
        ToolbarItems.Clear();

        var previous = new ToolbarItem
        {
            IconImageSource = "left_arrow.png"
        };
        previous.Clicked += (_, _) => ChangeYear(-1);

        var next = new ToolbarItem
        {
            IconImageSource = "right_arrow.png"
        };
        next.Clicked += (_, _) => ChangeYear(1);

        ToolbarItems.Add(previous);
        ToolbarItems.Add(next);
    }

    private async void OnYearTitleTapped(object? sender, TappedEventArgs e)
    {
        if (Vm is null || IsBusy)
            return;

        var popup = new YearPickerPopup(Vm.Year);

        var popupResult = await this.ShowPopupAsync<int?>(
                popup,
                PopupOptions.Empty,
                CancellationToken.None);

        if (popupResult.WasDismissedByTappingOutsideOfPopup ||
            popupResult.Result is not int year ||
            year == Vm.Year)
        {
            return;
        }

        _selectedMonth = null;

        await RunBusyAsync(
            Localization.GetLocalizedText(
                "Please wait…",
                DataCache.Instance.CurrentLanguageCode),
            async () =>
            {
                Vm.SetYear(year);

                await Task.Yield();
                await ResetScrollPositionsAsync();
            });
    }

    private void ChangeYear(int delta)
    {
        if (Vm is null)
            return;

        var newYear = Vm.Year + delta;

        if (newYear < DateTime.MinValue.Year ||
            newYear > DateTime.MaxValue.Year)
        {
            return;
        }

        _selectedMonth = null;
        Vm.SetYear(newYear);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(YearlyPlanetTransitsViewModel.Year) ||
            e.PropertyName == nameof(YearlyPlanetTransitsViewModel.CultureCode))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                RebuildTimeline();
                await ResetScrollPositionsAsync();
            });
        }
    }

    private void InitializeGraphicsViews()
    {
        HeaderGraphicsView.Drawable = _headerDrawable;
        LabelsGraphicsView.Drawable = _labelsDrawable;
        BodyGraphicsView.Drawable = _bodyDrawable;
    }

    private void RebuildTimeline()
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

    private YearlyTransitsLayout CreateLayout()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        var topBandLabel = $"{Localization.GetLocalizedText("Masa", lang)}/{Localization.GetLocalizedText("Shunya", lang)}";

        return new YearlyTransitsLayout
        {
            Year = Vm.Year,
            Culture = Vm.CurrentCulture,
            TopBandLabel = topBandLabel,
            SelectedMonth = _selectedMonth,
            Planets = PlanetOrder
                .Select(planet => new YearlyTransitsPlanetRow
                {
                    Planet = planet,
                    Name = GetPlanetName(planet)
                })
                .ToList()
        };
    }

    private async void OnBodyGraphicsTapped(object? sender, TappedEventArgs e)
    {
        // The no-profile screen is visual-only. It must never navigate.
        if (!HasActiveProfile())
            return;

        var point = e.GetPosition(BodyGraphicsView);
        if (point == null)
            return;

        var layout = CreateLayout();
        var tappedMonth = YearlyTransitsHitTestHelper.HitTestMonth(
            layout,
            point.Value.X,
            point.Value.Y);

        if (!tappedMonth.HasValue)
            return;

        var isSecondTapOnSameMonth = _selectedMonth == tappedMonth.Value;
        _selectedMonth = tappedMonth.Value;
        RebuildTimeline();

        if (!isSecondTapOnSameMonth)
            return;

        await Shell.Current.GoToAsync(
            $"//monthlyPlanetTransits?year={Vm.Year}&month={tappedMonth.Value}",
            true);
    }

    private static bool HasActiveProfile()
    {
        var profiles = DataCache.Instance.ProfileList;
        return profiles?.Any(profile => profile.Checked) == true;
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

    private async Task ResetScrollPositionsAsync()
    {
        _syncingHorizontalScroll = true;
        _syncingVerticalScroll = true;

        try
        {
            await HeaderHorizontalScroll.ScrollToAsync(0, 0, false);
            await BodyHorizontalScroll.ScrollToAsync(0, 0, false);
            await LabelsVerticalScroll.ScrollToAsync(0, 0, false);
            await BodyVerticalScroll.ScrollToAsync(0, 0, false);
        }
        finally
        {
            _syncingHorizontalScroll = false;
            _syncingVerticalScroll = false;
        }
    }

    private void OnBodyHorizontalScrollSizeChanged(object? sender, EventArgs e)
    {
        var viewportHeight = BodyHorizontalScroll.Height;

        if (viewportHeight <= 0)
            return;

        // The inner vertical ScrollView must be constrained to the visible
        // matrix viewport; otherwise Android may measure it at content height
        // and vertical scrolling will not activate.
        BodyVerticalScroll.HeightRequest = viewportHeight;
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
