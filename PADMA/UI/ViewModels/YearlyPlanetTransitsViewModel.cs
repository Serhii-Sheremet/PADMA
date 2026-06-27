using PADMA.Core.Services;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PADMA.UI.ViewModels;

public sealed class YearlyPlanetTransitsViewModel : INotifyPropertyChanged
{
    private int _year;
    public int Year
    {
        get => _year;
        private set => SetProperty(ref _year, value);
    }

    private string _cultureCode = CultureInfo.CurrentUICulture.Name;
    public string CultureCode
    {
        get => _cultureCode;
        private set
        {
            if (SetProperty(ref _cultureCode, value))
            {
                OnPropertyChanged(nameof(CurrentCulture));
                OnPropertyChanged(nameof(YearTitle));
            }
        }
    }

    private string _activeProfileDisplay = string.Empty;
    public string ActiveProfileDisplay
    {
        get => _activeProfileDisplay;
        private set => SetProperty(ref _activeProfileDisplay, value);
    }

    private string _activeLocationTimeZoneDisplay = string.Empty;
    public string ActiveLocationTimeZoneDisplay
    {
        get => _activeLocationTimeZoneDisplay;
        private set => SetProperty(ref _activeLocationTimeZoneDisplay, value);
    }

    public CultureInfo CurrentCulture =>
        !string.IsNullOrWhiteSpace(CultureCode)
            ? new CultureInfo(CultureCode)
            : CultureInfo.CurrentUICulture;

    public string YearTitle => Year.ToString(CultureInfo.InvariantCulture);

    public YearlyPlanetTransitsViewModel()
    {
        Year = DateTime.Today.Year;

        DataCache.Instance.ProfileContextUpdated += ReloadCultureAndRefresh;

        InitializeCulture();
        UpdateActiveProfileInfo();
    }

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
        OnPropertyChanged(nameof(YearTitle));
    }

    public void ReloadCultureAndRefresh()
    {
        InitializeCulture();
        UpdateActiveProfileInfo();
    }

    public void SetYear(int year)
    {
        if (Year == year)
            return;

        Year = year;
        OnPropertyChanged(nameof(YearTitle));
        UpdateActiveProfileInfo();
    }

    private void UpdateActiveProfileInfo()
    {
        var profile = DataCache.Instance.ActiveProfile;
        var ctx = DataCache.Instance.ProfileContextService?.Current;

        ActiveProfileDisplay = profile?.ProfileName ?? string.Empty;

        var locality = string.Empty;
        int localityId = profile?.PlaceOfLivingId ?? 0;
        if (localityId > 0)
        {
            locality = DataCache.Instance.LocationList
                .FirstOrDefault(x => x.Id == localityId)?.Locality ?? string.Empty;
        }

        if (ctx == null)
        {
            ActiveLocationTimeZoneDisplay = locality;
            return;
        }

        // A noon sample in the middle of the year avoids midnight/DST edge cases.
        var sampleLocal = new DateTime(Year, 7, 1, 12, 0, 0, DateTimeKind.Unspecified);
        var offset = ctx.TimeZoneInfo.GetUtcOffset(sampleLocal);
        var offsetText = FormatUtcOffset(offset);

        ActiveLocationTimeZoneDisplay = string.IsNullOrWhiteSpace(locality)
            ? $"{ctx.TimeZoneInfo.Id}, {offsetText}"
            : $"{locality}, {offsetText}";
    }

    private static string FormatUtcOffset(TimeSpan offset)
    {
        var sign = offset >= TimeSpan.Zero ? "+" : "-";
        offset = offset.Duration();
        return $"UTC{sign}{offset.Hours:00}:{offset.Minutes:00}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
