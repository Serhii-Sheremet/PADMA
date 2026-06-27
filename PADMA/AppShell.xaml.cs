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

    private string _flyoutTransitChartsTitle = "Transit charts";
    public string FlyoutTransitChartsTitle
    {
        get => _flyoutTransitChartsTitle;
        set => SetProperty(ref _flyoutTransitChartsTitle, value);
    }

    private string _flyoutMonthlyTransitsTitle = "Transits for month";
    public string FlyoutMonthlyTransitsTitle
    {
        get => _flyoutMonthlyTransitsTitle;
        set => SetProperty(ref _flyoutMonthlyTransitsTitle, value);
    }

    private string _flyoutYearlyTransitsTitle = "Transits for year";
    public string FlyoutYearlyTransitsTitle
    {
        get => _flyoutYearlyTransitsTitle;
        set => SetProperty(ref _flyoutYearlyTransitsTitle, value);
    }

    private string _flyoutSettingsTitle = "Settings";
    public string FlyoutSettingsTitle
    {
        get => _flyoutSettingsTitle;
        set => SetProperty(ref _flyoutSettingsTitle, value);
    }

    private string _flyoutAboutTitle = "About";
    public string FlyoutAboutTitle
    {
        get => _flyoutAboutTitle;
        set => SetProperty(ref _flyoutAboutTitle, value);
    }

    private string _flyoutFAQTitle = "faq";
    public string FlyoutFAQTitle
    {
        get => _flyoutFAQTitle;
        set => SetProperty(ref _flyoutFAQTitle, value);
    }

    private string _flyoutExitTitle = "Exit";
    public string FlyoutExitTitle
    {
        get => _flyoutExitTitle;
        set => SetProperty(ref _flyoutExitTitle, value);
    }

    private string _flyoutAppTitle = "Personal Astrological Diary";
    public string FlyoutAppTitle
    {
        get => _flyoutAppTitle;
        set => SetProperty(ref _flyoutAppTitle, value);
    }

    private string _flyoutAppSubtitle = "mobile application";
    public string FlyoutAppSubtitle
    {
        get => _flyoutAppSubtitle;
        set => SetProperty(ref _flyoutAppSubtitle, value);
    }

    private string _flyoutVersionText = "Version: 1.0.0";
    public string FlyoutVersionText
    {
        get => _flyoutVersionText;
        set => SetProperty(ref _flyoutVersionText, value);
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
        Routing.RegisterRoute(nameof(BirthChartPreviewPage), typeof(BirthChartPreviewPage));

        // Регистрируем маршрут для TransitChartsPage
        Routing.RegisterRoute(nameof(TransitChartsPage), typeof(TransitChartsPage));

        // Регистрируем маршрут для MonthlyPlanetTransitsPage
        Routing.RegisterRoute(nameof(MonthlyPlanetTransitsPage), typeof(MonthlyPlanetTransitsPage));

        // Регистрируем маршрут для YearlyPlanetTransitsPage
        Routing.RegisterRoute(nameof(YearlyPlanetTransitsPage), typeof(YearlyPlanetTransitsPage));

        // Регистрируем маршрут для AboutPage
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(FeedbackPage), typeof(FeedbackPage));
        Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));

        // Регистрируем маршрут для FaqPage
        Routing.RegisterRoute(nameof(FaqPage), typeof(FaqPage));

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
        Routing.RegisterRoute(nameof(ColorSettingsPage), typeof(ColorSettingsPage));
        Routing.RegisterRoute(nameof(ColorLookupPage), typeof(ColorLookupPage));
        Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));
        Routing.RegisterRoute(nameof(RetentionPage), typeof(RetentionPage));

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
                // Reset to DB-default profile before closing (so next launch starts from default)
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                if (db != null)
                {
                    var def = db.GetProfiles().FirstOrDefault(p => p.Checked);
                    if (def != null)
                        DataCache.Instance.ActiveProfile = def;
                }

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

        //FlyoutAppTitle = Localization.GetLocalizedText("Personal Astrological Diary", lang);
        FlyoutVersionText = $"{Localization.GetLocalizedText("Version", lang)}: {AppInfo.VersionString}";

        FlyoutCalendarTitle = Localization.GetLocalizedText("Calendar", lang);
        FlyoutProfilesTitle = Localization.GetLocalizedText("Profiles", lang);
        FlyoutTransitChartsTitle = Localization.GetLocalizedText("Transit charts", lang);
        FlyoutMonthlyTransitsTitle = Localization.GetLocalizedText("Transits for month", lang);
        FlyoutYearlyTransitsTitle = Localization.GetLocalizedText("Transits for year", lang);
        FlyoutSettingsTitle = Localization.GetLocalizedText("Settings", lang);
        FlyoutAboutTitle = $"{Localization.GetLocalizedText("About application", lang)}...";
        FlyoutFAQTitle = Localization.GetLocalizedText("FAQs", lang);
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
