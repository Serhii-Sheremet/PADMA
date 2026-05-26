using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI;
using PADMA.UI.ViewModels;

namespace PADMA.Pages;

public partial class MonthlyPlanetTransitsPage : ContentPage
{
    private MonthlyPlanetTransitsViewModel Vm => BindingContext as MonthlyPlanetTransitsViewModel;
    private bool _needsRefreshAfterConfig;

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

    public MonthlyPlanetTransitsPage()
    {
        InitializeComponent();

        if (BindingContext is not MonthlyPlanetTransitsViewModel)
            BindingContext = new MonthlyPlanetTransitsViewModel();

        Vm.InitializeCulture();

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
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_needsRefreshAfterConfig)
        {
            _needsRefreshAfterConfig = false;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Yield();

                var db = ServiceLocator.Services.GetService<DatabaseService>();
                DataCache.Instance.Refresh(db);

                Vm?.ReloadCultureAndRefresh();
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




}
