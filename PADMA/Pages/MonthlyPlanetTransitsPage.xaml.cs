using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.MonthlyTransits;
using PADMA.UI.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

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

    private readonly MonthlyPlanetTransitsDataService _dataService = new();
    private MonthlyPlanetTransitsData? _monthlyData;
    private CancellationTokenSource? _buildCts;

    private MonthlyPlanetDaySelection? _selection;
    private MonthlyPlanetDayDetailsModel? _detailsModel;

    public ObservableCollection<object> TooltipItems { get; } = new();

    private string _tooltipTitle = string.Empty;
    public string TooltipTitle
    {
        get => _tooltipTitle;
        set
        {
            if (_tooltipTitle == value)
                return;

            _tooltipTitle = value;
            OnPropertyChanged();
        }
    }

    private bool _isTooltipVisible;
    public bool IsTooltipVisible
    {
        get => _isTooltipVisible;
        set
        {
            if (_isTooltipVisible == value)
                return;

            _isTooltipVisible = value;
            OnPropertyChanged();
        }
    }

    private double _tooltipHeight = 420;
    public double TooltipHeight
    {
        get => _tooltipHeight;
        private set
        {
            if (Math.Abs(_tooltipHeight - value) < 0.1)
                return;

            _tooltipHeight = value;
            OnPropertyChanged();
        }
    }

    public double TooltipMaxHeight { get; private set; } = 520;
    public double TooltipMaxWidth { get; private set; } = 360;
    public double TooltipBodyHeight { get; private set; } = 420;

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

    public MonthlyPlanetTransitsPage()
    {
        InitializeComponent();
        BindingContext = this;

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

        if (Shell.Current is not null)
            Shell.Current.FlyoutIsPresented = false;

        await Task.Delay(150);

        ResetTransientUiState();

        var today = DateTime.Today;

        if (Vm is not null &&
            (Vm.Year != today.Year || Vm.Month != today.Month))
        {
            Vm.SetMonthYear(today.Year, today.Month);

            await Task.Yield();
            await ResetScrollPositionsAsync();

            return;
        }

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

                    await RebuildMonthlyDataAsync();
                    await ResetScrollPositionsAsync();
                });

            return;
        }

        await RunBusyAsync(
            Localization.GetLocalizedText("Calculating transits", DataCache.Instance.CurrentLanguageCode),
            async () =>
            {
                await RebuildMonthlyDataAsync();
                await ResetScrollPositionsAsync();
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
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ResetTransientUiState();

                await RunBusyAsync(
                    Localization.GetLocalizedText("Calculating transits", DataCache.Instance.CurrentLanguageCode),
                    async () =>
                    {
                        await RebuildMonthlyDataAsync();
                        await ResetScrollPositionsAsync();
                    });
            });
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
            TopBandLabel = "Masa/Shunya",
            Data = _monthlyData,
            Selection = _selection,
            Planets = PlanetOrder
                .Select(x => new MonthlyTransitsPlanetRow
                {
                    Planet = x,
                    Name = GetPlanetName(x)
                })
                .ToList()
        };
    }

    private async void OnBodyGraphicsTapped(object? sender, TappedEventArgs e)
    {
        if (_monthlyData == null)
            return;

        var point = e.GetPosition(BodyGraphicsView);
        if (point == null)
            return;

        var layout = CreateLayout();

        var tappedSelection = MonthlyTransitsHitTestHelper.HitTestPlanetDay(
            layout,
            point.Value.X,
            point.Value.Y);

        if (tappedSelection == null)
            return;

        var isSecondTapOnSameSelection = IsSameSelection(_selection, tappedSelection);

        _selection = tappedSelection;

        // First tap or tap on another planet/day:
        // only move selection rectangle, do not build heavy details.
        if (!isSecondTapOnSameSelection)
        {
            _detailsModel = null;
            RebuildTimelineSkeleton();
            return;
        }

        // Second tap on the same selected planet/day:
        // now build details and open tooltip.
        await RunBusyAsync(
            Localization.GetLocalizedText("Calculating tooltip...", DataCache.Instance.CurrentLanguageCode),
            async () =>
            {
                await Task.Yield();

                _detailsModel = await Task.Run(() =>
                    MonthlyPlanetDayDetailsBuilder.Build(
                        _monthlyData,
                        tappedSelection.Planet,
                        tappedSelection.DayLocal));

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    RebuildTimelineSkeleton();

                    if (_detailsModel != null)
                        ShowMonthlyPlanetDayDetailsTooltip(_detailsModel);
                });
            });
    }

    private static bool IsSameSelection(
        MonthlyPlanetDaySelection? current,
        MonthlyPlanetDaySelection tapped)
    {
        return current != null &&
               current.Planet == tapped.Planet &&
               current.DayLocal.Date == tapped.DayLocal.Date;
    }

    private void OnTooltipBackdropTapped(object? sender, TappedEventArgs e)
    {
        IsTooltipVisible = false;
    }

    private void UpdateTooltipSize()
    {
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

        var screenWidth = displayInfo.Width / displayInfo.Density;
        var screenHeight = displayInfo.Height / displayInfo.Density;

        TooltipMaxWidth = Math.Min(screenWidth - 36, 380);

        // Было 0.68 — маловато для portrait.
        TooltipMaxHeight = Math.Min(screenHeight * 0.78, 620);

        var estimatedContentHeight = EstimateTooltipContentHeight();

        TooltipHeight = Math.Min(
            TooltipMaxHeight,
            Math.Max(220, estimatedContentHeight));

        TooltipBodyHeight = Math.Max(120, TooltipHeight - 62);

        OnPropertyChanged(nameof(TooltipMaxWidth));
        OnPropertyChanged(nameof(TooltipMaxHeight));
        OnPropertyChanged(nameof(TooltipBodyHeight));
    }

    private void ResetTransientUiState()
    {
        _selection = null;
        _detailsModel = null;

        IsTooltipVisible = false;
        TooltipItems.Clear();
        TooltipTitle = string.Empty;

        // Reset tooltip dimensions to safe defaults.
        TooltipMaxHeight = 620;
        TooltipMaxWidth = 380;
        TooltipBodyHeight = 420;
        TooltipHeight = 480;

        OnPropertyChanged(nameof(TooltipMaxHeight));
        OnPropertyChanged(nameof(TooltipMaxWidth));
        OnPropertyChanged(nameof(TooltipBodyHeight));
        OnPropertyChanged(nameof(TooltipHeight));
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

    private void ResetToCurrentMonth()
    {
        if (Vm is null)
            return;

        var today = DateTime.Today;

        if (Vm.Year != today.Year || Vm.Month != today.Month)
        {
            Vm.SetMonthYear(today.Year, today.Month);
        }
    }

    private double EstimateTooltipContentHeight()
    {
        // Tooltip title + top divider + frame padding.
        double h = 58;

        foreach (var item in TooltipItems)
        {
            h += item switch
            {
                MonthlyTooltipHeaderItem header => EstimateHeaderHeight(header.Text),

                MonthlyTooltipLineItem line =>
                    EstimateLineHeight($"{line.Label}: {line.Value}"),

                MonthlyTooltipTwoLineRangeItem twoLine =>
                    EstimateTwoLineRangeHeight(twoLine),

                MonthlyTooltipSimpleRangeLineItem simple =>
                    EstimateSimpleRangeLineHeight(simple),

                MonthlyTooltipDividerItem => 10,
                MonthlyTooltipSpacerItem spacer => spacer.Height,

                _ => 20
            };
        }

        return h + 18;
    }

    private static double EstimateHeaderHeight(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 24;

        return text.Length > 42 ? 42 : 28;
    }

    private static double EstimateTwoLineRangeHeight(MonthlyTooltipTwoLineRangeItem item)
    {
        // First line: label.
        // Second line: value + date range, often wraps on phone width.
        var secondLine = $"{item.Value}: {item.RangeText}";

        return secondLine.Length > 62 ? 52 : 40;
    }

    private static double EstimateSimpleRangeLineHeight(MonthlyTooltipSimpleRangeLineItem item)
    {
        var text = $"{item.Label}: {item.RangeText}";

        return text.Length > 62 ? 34 : 22;
    }

    private static double EstimateLineHeight(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 20;

        return text.Length > 55 ? 38 : 22;
    }

    private void ShowMonthlyPlanetDayDetailsTooltip(MonthlyPlanetDayDetailsModel model)
    {
        TooltipTitle = $"{model.Title} {model.Subtitle}";

        FillTooltipItemsFromMonthlyDetails(model);

        UpdateTooltipSize();

        IsTooltipVisible = true;
    }

    private void FillTooltipItemsFromMonthlyDetails(MonthlyPlanetDayDetailsModel model)
    {
        TooltipItems.Clear();

        for (int i = 0; i < model.PadaPeriodBlocks.Count; i++)
        {
            var padaBlock = model.PadaPeriodBlocks[i];

            TooltipItems.Add(new MonthlyTooltipHeaderItem
            {
                Text = $"{padaBlock.Title}"
            });

            foreach (var line in padaBlock.Lines)
            {
                if (line.HasRange)
                {
                    TooltipItems.Add(new MonthlyTooltipTwoLineRangeItem
                    {
                        Label = line.Label,
                        Value = line.Value,
                        RangeText = line.RangeText
                    });
                }
                else
                {
                    TooltipItems.Add(new MonthlyTooltipLineItem
                    {
                        Label = line.Label,
                        Value = line.Value
                    });
                }
            }

            bool hasNextPadaBlock = i < model.PadaPeriodBlocks.Count - 1;
            bool hasExtraBlocks = model.ExtraBlocks.Count > 0;

            if (hasNextPadaBlock || hasExtraBlocks)
            {
                TooltipItems.Add(new MonthlyTooltipDividerItem());
                TooltipItems.Add(new MonthlyTooltipSpacerItem { Height = 6 });
            }
        }

        for (int i = 0; i < model.ExtraBlocks.Count; i++)
        {
            var block = model.ExtraBlocks[i];

            TooltipItems.Add(new MonthlyTooltipHeaderItem
            {
                Text = block.Title
            });

            foreach (var row in block.Rows)
            {
                TooltipItems.Add(new MonthlyTooltipSimpleRangeLineItem
                {
                    Label = row.Value,
                    RangeText = row.RangeText
                });
            }

            if (i < model.ExtraBlocks.Count - 1)
            {
                TooltipItems.Add(new MonthlyTooltipDividerItem());
                TooltipItems.Add(new MonthlyTooltipSpacerItem { Height = 6 });
            }
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (IsTooltipVisible)
        {
            UpdateTooltipSize();
        }
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

    private async Task RebuildMonthlyDataAsync()
    {
        if (Vm is null)
            return;

        _buildCts?.Cancel();
        _buildCts?.Dispose();
        _buildCts = new CancellationTokenSource();

        var ct = _buildCts.Token;

        _monthlyData = await _dataService.BuildAsync(Vm.Year, Vm.Month, ct);

        RebuildTimelineSkeleton();
    }



}

public sealed class MonthlyTooltipHeaderItem
{
    public string Text { get; init; } = string.Empty;
}

public sealed class MonthlyTooltipLineItem
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public sealed class MonthlyTooltipTwoLineRangeItem
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string RangeText { get; init; } = string.Empty;
}

public sealed class MonthlyTooltipSimpleRangeLineItem
{
    public string Label { get; init; } = string.Empty;
    public string RangeText { get; init; } = string.Empty;
}

public sealed class MonthlyTooltipRangeItem
{
    public string Text { get; init; } = string.Empty;
}

public sealed class MonthlyTooltipShortDividerItem
{
}

public sealed class MonthlyTooltipDividerItem
{
}

public sealed class MonthlyTooltipSpacerItem
{
    public double Height { get; init; } = 8;
}

public sealed class MonthlyTooltipItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? HeaderTemplate { get; set; }
    public DataTemplate? LineTemplate { get; set; }
    public DataTemplate? RangeTemplate { get; set; }
    public DataTemplate? ShortDividerTemplate { get; set; }
    public DataTemplate? DividerTemplate { get; set; }
    public DataTemplate? SpacerTemplate { get; set; }

    public DataTemplate? TwoLineRangeTemplate { get; set; }
    public DataTemplate? SimpleRangeLineTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            MonthlyTooltipHeaderItem => HeaderTemplate,
            MonthlyTooltipLineItem => LineTemplate,
            MonthlyTooltipTwoLineRangeItem => TwoLineRangeTemplate,
            MonthlyTooltipSimpleRangeLineItem => SimpleRangeLineTemplate,
            MonthlyTooltipDividerItem => DividerTemplate,
            MonthlyTooltipSpacerItem => SpacerTemplate,
            _ => LineTemplate
        };
    }
}

