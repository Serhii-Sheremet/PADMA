using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PADMA;

public partial class AppShell : Shell, INotifyPropertyChanged
{
    public ICommand ExitCommand { get; }

    private bool _isExiting;

    private string _flyoutCalendarTitle = "Calendar";
    public string FlyoutCalendarTitle
    {
        get => _flyoutCalendarTitle;
        set => SetProperty(ref _flyoutCalendarTitle, value);
    }

    private string _flyoutProfilesTitle = "Profiles";
    public string FlyoutProfilesTitle
    {
        get => _flyoutProfilesTitle;
        set => SetProperty(ref _flyoutProfilesTitle, value);
    }

    private string _flyoutSettingsTitle = "Settings";
    public string FlyoutSettingsTitle
    {
        get => _flyoutSettingsTitle;
        set => SetProperty(ref _flyoutSettingsTitle, value);
    }

    private string _flyoutExitTitle = "Exit";
    public string FlyoutExitTitle
    {
        get => _flyoutExitTitle;
        set => SetProperty(ref _flyoutExitTitle, value);
    }

    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;
        RefreshFlyoutTitles();

        // Регистрируем маршруты ProfilesPage, ProfileDetailPage и LocationSearchPage 
        Routing.RegisterRoute(nameof(ProfilesPage), typeof(ProfilesPage));
        Routing.RegisterRoute(nameof(ProfileDetailPage), typeof(ProfileDetailPage));
        Routing.RegisterRoute(nameof(LocationSearchPage), typeof(LocationSearchPage));

        // Регистрируем маршрут для DayOverviewPage и DayPage
        Routing.RegisterRoute("dayOverview", typeof(DayOverviewPage));
        Routing.RegisterRoute("day", typeof(DayPage));

        // Регистрируем маршруты для всех страниц конфигурации
        Routing.RegisterRoute(nameof(ConfigurationPage), typeof(ConfigurationPage));
        Routing.RegisterRoute(nameof(LanguagePage), typeof(LanguagePage));
        Routing.RegisterRoute(nameof(FirstDayOfWeekPage), typeof(FirstDayOfWeekPage));
        Routing.RegisterRoute(nameof(TransitsPage), typeof(TransitsPage));
        Routing.RegisterRoute(nameof(NodesPage), typeof(NodesPage));
        Routing.RegisterRoute(nameof(HoraPage), typeof(HoraPage));
        Routing.RegisterRoute(nameof(MuhurtasPage), typeof(MuhurtasPage));
        Routing.RegisterRoute(nameof(MrityuBhagaPage), typeof(MrityuBhagaPage));
        Routing.RegisterRoute(nameof(SunrisePage), typeof(SunrisePage));
        Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));

        ExitCommand = new Command(async () =>
        {
            if (_isExiting) return;

            var lang = DataCache.Instance.CurrentLanguageCode;
            string L(string nativeEn) => Localization.GetLocalizedText(nativeEn, lang);

            var ok = await DisplayAlert(
                L("Exit Application?"),
                L("Do you want to exit PADMA Application?"),
                L("Yes"),
                L("No"));

            if (!ok) return;

            _isExiting = true;
            try
            {
                AppCloser.Close();
            }
            finally
            {
                _isExiting = false;
            }
        });
    }

    public void RefreshFlyoutTitles()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        FlyoutCalendarTitle = Localization.GetLocalizedText("Calendar", lang);
        FlyoutProfilesTitle = Localization.GetLocalizedText("Profiles", lang);
        FlyoutSettingsTitle = Localization.GetLocalizedText("Settings", lang);
        FlyoutExitTitle = Localization.GetLocalizedText("Exit", lang);
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(name);
        return true;
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ConfigurationPage");
    }

    private async void OnExitMenuItemClicked(object sender, EventArgs e)
    {
        if (ExitCommand?.CanExecute(null) == true)
            ExitCommand.Execute(null);
    }

}
