using Microsoft.Maui.Layouts;
using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System.Globalization;

namespace PADMA.Pages;

public partial class BirthChartPreviewPage : ContentPage, IQueryAttributable
{
    private DateTime _originalBirthDateTime;
    private bool _isLeavingConfirmed;
    private DateTime _localBirthDateTime;
    private AppLocation? _birthLocation;

    private bool _isAspectsExpanded;
    private bool _aspectAllSelected;
    private bool _aspectSunSelected;
    private bool _aspectMoonSelected;
    private bool _aspectMarsSelected;
    private bool _aspectMercurySelected;
    private bool _aspectJupiterSelected;
    private bool _aspectVenusSelected;
    private bool _aspectSaturnSelected;
    private bool _aspectRahuSelected;
    private bool _isUpdatingAspectSelection;

    private bool _isStepUnitExpanded;

    private DateTime _currentTransitLocalDateTime = DateTime.Now;
    public DateTime CurrentTransitLocalDateTime
    {
        get => _currentTransitLocalDateTime;
        set
        {
            if (_currentTransitLocalDateTime == value)
                return;

            _currentTransitLocalDateTime = value;
            OnPropertyChanged(nameof(CurrentTransitLocalDateTime));
            OnPropertyChanged(nameof(CurrentTransitUtcDateTime));
        }
    }

    public DateTime CurrentTransitUtcDateTime => CurrentTransitLocalDateTime.ToUniversalTime();

    private ETransitStepUnit _selectedStepUnit = ETransitStepUnit.Seconds;
    private string _selectedStepUnitText = string.Empty;

    public ETransitStepUnit SelectedStepUnit
    {
        get => _selectedStepUnit;
        set
        {
            if (_selectedStepUnit == value)
                return;

            _selectedStepUnit = value;
            OnPropertyChanged(nameof(SelectedStepUnit));
        }
    }

    public string SelectedStepUnitText
    {
        get => _selectedStepUnitText;
        set
        {
            if (_selectedStepUnitText == value)
                return;

            _selectedStepUnitText = value;
            OnPropertyChanged(nameof(SelectedStepUnitText));
        }
    }

    private int _selectedStepValue = 10;
    public int SelectedStepValue
    {
        get => _selectedStepValue;
        set
        {
            var normalized = value <= 0 ? 1 : value;
            if (_selectedStepValue == normalized)
                return;

            _selectedStepValue = normalized;
            OnPropertyChanged(nameof(SelectedStepValue));
        }
    }

    private int GetParsedStepValue()
    {
        if (int.TryParse(StepValueEntry.Text, out var value) && value > 0)
            return value;

        return GetDefaultStepValue(SelectedStepUnit);
    }

    private sealed class StepUnitOption
    {
        public ETransitStepUnit Unit { get; init; }
        public string LocalizationKey { get; init; } = string.Empty;
    }

    private static int GetDefaultStepValue(ETransitStepUnit stepUnit) =>
    stepUnit switch
    {
        ETransitStepUnit.Seconds => 10,
        ETransitStepUnit.Minutes => 1,
        ETransitStepUnit.Hours => 1,
        _ => 1
    };

    private string GetLocalizedStepUnitText(ETransitStepUnit unit, string lang)
    {
        var key = unit switch
        {
            ETransitStepUnit.Seconds => "Seconds",
            ETransitStepUnit.Minutes => "Minutes",
            ETransitStepUnit.Hours => "Hours",
            _ => "Seconds"
        };

        return Localization.GetLocalizedText(key, lang);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("BirthDateTime", out var birthDateTimeObj) &&
            birthDateTimeObj is DateTime birthDateTime)
        {
            _localBirthDateTime = birthDateTime;
        }

        if (query.TryGetValue("BirthLocation", out var birthLocationObj) &&
            birthLocationObj is AppLocation birthLocation)
        {
            _birthLocation = birthLocation;
        }
    }

    public BirthChartPreviewPage()
    {
        InitializeComponent();
        BindingContext = this;

        ChartView.SizeChanged += OnChartViewSizeChanged;
        TransitNavamsaChartView.SizeChanged += OnTransitNavamsaChartSizeChanged;
    }

    private void OnChartViewSizeChanged(object? sender, EventArgs e)
    {
        if (ChartView.Width > 0)
        {
            ChartView.HeightRequest = ChartView.Width;
        }
    }

    private void OnTransitNavamsaChartSizeChanged(object? sender, EventArgs e)
    {
        if (TransitNavamsaChartView.Width > 0)
            TransitNavamsaChartView.HeightRequest = TransitNavamsaChartView.Width;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        CurrentTransitLocalDateTime = _localBirthDateTime;
        _originalBirthDateTime = _localBirthDateTime;

        try { Shell.Current.Navigating += OnShellNavigating; } catch { }

        ApplyLocalization();
        UpdateAspectSelectionUi();
        UpdateAspectsUnderline();
        RefreshStepUnitUnderline();
        UpdateStepUnitUi();

        Dispatcher.Dispatch(UpdateAspectsUnderline);
        Dispatcher.Dispatch(RefreshStepUnitUnderline);

        RebuildBirthChart();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        try { Shell.Current.Navigating -= OnShellNavigating; } catch { }
    }

    private bool HasBirthDateTimeChanged()
    {
        return CurrentTransitLocalDateTime != _originalBirthDateTime;
    }

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if(_isLeavingConfirmed)
        return;

        if (!e.CanCancel)
            return;

        if (e.Source != ShellNavigationSource.Pop)
            return;

        if (!HasBirthDateTimeChanged())
            return;

        e.Cancel();

        var lang = DataCache.Instance.CurrentLanguageCode;
        bool apply = await DisplayAlert(
            Localization.GetLocalizedText("Apply changes", lang),
            Localization.GetLocalizedText("Apply new birth date and time?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang));

        if (apply)
        {
            MessagingCenter.Send(this, "BirthDateTimeSelected", CurrentTransitLocalDateTime);
        }

        _isLeavingConfirmed = true;
        await Shell.Current.GoToAsync("..");
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Birth Chart", lang);

        AspectsLabel.Text = Localization.GetLocalizedText("Aspects", lang);
        AspectsUnderline.WidthRequest = Math.Max(38, AspectsLabel.Text.Length * 10);

        LabelAspectAll.Text = Localization.GetLocalizedText("All", lang);
        LabelAspectSun.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.SUN)?.Name ?? "Sun";
        LabelAspectMoon.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.MOON)?.Name ?? "Moon";
        LabelAspectMars.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.MARS)?.Name ?? "Mars";
        LabelAspectMercury.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.MERCURY)?.Name ?? "Mercury";
        LabelAspectJupiter.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.JUPITER)?.Name ?? "Jupiter";
        LabelAspectVenus.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.VENUS)?.Name ?? "Venus";
        LabelAspectSaturn.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.SATURN)?.Name ?? "Saturn";
        LabelAspectRahu.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.RAHU)?.Name ?? "Rahu";

        LabelPlanetsTransitPositions.Text = Localization.GetLocalizedText("Natal planet positions", lang);
        TableLabelDegree.Text = Localization.GetLocalizedText("Degree", lang);
        TableLabelRasi.Text = Localization.GetLocalizedText("Rasi", lang);
        TableLabelNakshatra.Text = Localization.GetLocalizedText("Nakshatra", lang);
        TableLabelPada.Text = Localization.GetLocalizedText("Pada", lang);
        TableLabelNavamsa.Text = Localization.GetLocalizedText("Navamsa", lang);

        StepSecondsOptionLabel.Text = Localization.GetLocalizedText("Seconds", lang);
        StepMinutesOptionLabel.Text = Localization.GetLocalizedText("Minutes", lang);
        StepHoursOptionLabel.Text = Localization.GetLocalizedText("Hours", lang);

        LabelTransitNavamsa.Text = Localization.GetLocalizedText("Natal Navamsa", lang);

        SelectedStepUnitText = GetLocalizedStepUnitText(SelectedStepUnit, lang);
        StepUnitLabel.Text = SelectedStepUnitText;
        RefreshStepUnitUnderline();
        UpdateAspectsUnderline();
    }

    private void UpdateAspectsUnderline()
    {
        Dispatcher.Dispatch(() =>
        {
            AspectsLabel.Measure(double.PositiveInfinity, double.PositiveInfinity);
            AspectsUnderline.WidthRequest = Math.Max(24, AspectsLabel.DesiredSize.Width);
        });
    }

    private void RebuildBirthChart()
    {
        if (_birthLocation == null)
            return;

        var nodeMode = DataCache.Instance.GetActiveNodeSetting();

        double birthLat = 0, birthLon = 0;
        if (_birthLocation != null &&
                double.TryParse(_birthLocation.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out birthLat) &&
                double.TryParse(_birthLocation.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out birthLon))
        {
            // Birth reference calculations
            DateTime birthUtc = TimeZoneService.ConvertLocalToUtcHistorical(CurrentTransitLocalDateTime, birthLat, birthLon);

            char hsys = 'O'; // Placidus
            var pdList = SwissAnalysis.CalculatePlanetPositionsForDate(birthUtc, birthLat, birthLon, nodeMode);
            var ascendent = SwissService.CalculateAscendantForDate(birthUtc, birthLat, birthLon, 0, hsys);

            var lagnaZodiacId = SwissUtility.GetZodiacIdFromDegree(ascendent);
            var lagnaNakshatraId = SwissUtility.GetNakshatraIdFromDegree(ascendent);
            var lagnaPadaId = SwissUtility.GetPadaIdFromDegree(ascendent);
            var lagnaPadaNumber = SwissUtility.GetPadaNumberByPadaId(lagnaPadaId);
            var lagnaNavamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(
                lagnaNakshatraId,
                lagnaPadaNumber);

            var swappedZodiacs = TransitBuilderUtility.SwapZodiacs(
                DataCache.Instance.ZodiacList.ToList(),
                lagnaZodiacId);

            var selectedAspectPlanets = GetSelectedAspectPlanets();

            var houses = TransitChartDataService.BuildCurrentTransitChartHouses(
                pdList,
                swappedZodiacs,
                selectedAspectPlanets);

            ChartView.SetHouses(houses);

            BuildTransitPlanetTable(pdList, ascendent);

            var navamsaHouses = TransitChartDataService.BuildTransitNavamsaChartHouses(
                pdList,
                lagnaNavamsaZodiacId);

            TransitNavamsaChartView.SetHouses(navamsaHouses);

            UpdateBirthContextHeader();
        }
    }

    private void UpdateBirthContextHeader()
    {
        if (_birthLocation == null)
            return;

        var culture = CultureInfo.CurrentCulture;
        string dateText = CurrentTransitLocalDateTime.ToString("d MMMM yyyy, HH:mm:ss", culture);

        lblBirthContext.Text = $"{_birthLocation.Locality}, {dateText}";
    }

    private void OnAspectsHeaderTapped(object sender, TappedEventArgs e)
    {
        _isAspectsExpanded = !_isAspectsExpanded;
        AspectsPanel.IsVisible = _isAspectsExpanded;
        AspectsExpandIcon.Text = _isAspectsExpanded ? "▲" : "▼";
    }

    private void UpdateAspectSelectionUi()
    {
        AspectAllCheck.IsVisible = _aspectAllSelected;
        AspectSunCheck.IsVisible = _aspectSunSelected;
        AspectMoonCheck.IsVisible = _aspectMoonSelected;
        AspectMarsCheck.IsVisible = _aspectMarsSelected;
        AspectMercuryCheck.IsVisible = _aspectMercurySelected;
        AspectJupiterCheck.IsVisible = _aspectJupiterSelected;
        AspectVenusCheck.IsVisible = _aspectVenusSelected;
        AspectSaturnCheck.IsVisible = _aspectSaturnSelected;
        AspectRahuCheck.IsVisible = _aspectRahuSelected;
        
        RebuildBirthChart();
    }

    private void UpdateAspectAllState()
    {
        _aspectAllSelected =
            _aspectSunSelected &&
            _aspectMoonSelected &&
            _aspectMarsSelected &&
            _aspectMercurySelected &&
            _aspectJupiterSelected &&
            _aspectVenusSelected &&
            _aspectSaturnSelected &&
            _aspectRahuSelected;
    }

    private void ToggleAspect(ref bool value)
    {
        value = !value;
    }

    private void OnAspectAllTapped(object sender, TappedEventArgs e)
    {
        if (_isUpdatingAspectSelection)
            return;

        _isUpdatingAspectSelection = true;

        _aspectAllSelected = !_aspectAllSelected;

        _aspectSunSelected = _aspectAllSelected;
        _aspectMoonSelected = _aspectAllSelected;
        _aspectMarsSelected = _aspectAllSelected;
        _aspectMercurySelected = _aspectAllSelected;
        _aspectJupiterSelected = _aspectAllSelected;
        _aspectVenusSelected = _aspectAllSelected;
        _aspectSaturnSelected = _aspectAllSelected;
        _aspectRahuSelected = _aspectAllSelected;

        UpdateAspectSelectionUi();
        _isUpdatingAspectSelection = false;
    }

    private void OnAspectSunTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectSunSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectMoonTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectMoonSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectMarsTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectMarsSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectMercuryTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectMercurySelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectJupiterTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectJupiterSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectVenusTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectVenusSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectSaturnTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectSaturnSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private void OnAspectRahuTapped(object sender, TappedEventArgs e)
    {
        ToggleAspect(ref _aspectRahuSelected);
        UpdateAspectAllState();
        UpdateAspectSelectionUi();
        RebuildBirthChart();
    }

    private List<EPlanet> GetSelectedAspectPlanets()
    {
        var result = new List<EPlanet>();

        if (_aspectAllSelected)
        {
            result.Add(EPlanet.SUN);
            result.Add(EPlanet.MOON);
            result.Add(EPlanet.MARS);
            result.Add(EPlanet.MERCURY);
            result.Add(EPlanet.JUPITER);
            result.Add(EPlanet.VENUS);
            result.Add(EPlanet.SATURN);
            result.Add(EPlanet.RAHU);
            return result;
        }

        if (_aspectSunSelected) result.Add(EPlanet.SUN);
        if (_aspectMoonSelected) result.Add(EPlanet.MOON);
        if (_aspectMarsSelected) result.Add(EPlanet.MARS);
        if (_aspectMercurySelected) result.Add(EPlanet.MERCURY);
        if (_aspectJupiterSelected) result.Add(EPlanet.JUPITER);
        if (_aspectVenusSelected) result.Add(EPlanet.VENUS);
        if (_aspectSaturnSelected) result.Add(EPlanet.SATURN);
        if (_aspectRahuSelected) result.Add(EPlanet.RAHU);

        return result;
    }

    private void ShiftTransitTime(int direction)
    {
        SelectedStepValue = GetParsedStepValue();
        StepValueEntry.Text = SelectedStepValue.ToString();

        int value = SelectedStepValue * direction;

        CurrentTransitLocalDateTime = SelectedStepUnit switch
        {
            ETransitStepUnit.Seconds => CurrentTransitLocalDateTime.AddSeconds(value),
            ETransitStepUnit.Minutes => CurrentTransitLocalDateTime.AddMinutes(value),
            ETransitStepUnit.Hours => CurrentTransitLocalDateTime.AddHours(value),
            _ => CurrentTransitLocalDateTime
        };

        RebuildBirthChart();
    }

    private void OnStepUnitTapped(object sender, EventArgs e)
    {
        _isStepUnitExpanded = !_isStepUnitExpanded;

        if (_isStepUnitExpanded)
        {
            PositionStepUnitPanel();
            StepUnitPanel.IsVisible = true;
        }
        else
        {
            StepUnitPanel.IsVisible = false;
        }

        StepUnitArrow.Text = _isStepUnitExpanded ? "▲" : "▼";
    }

    private void OnStepBackClicked(object sender, EventArgs e)
    {
        ShiftTransitTime(-1);
    }

    private void OnStepForwardClicked(object sender, EventArgs e)
    {
        ShiftTransitTime(1);
    }

    private void UpdateStepUnitUi()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        StepUnitLabel.Text = GetLocalizedStepUnitText(SelectedStepUnit, lang);
        StepUnitArrow.Text = _isStepUnitExpanded ? "▲" : "▼";
        StepValueEntry.Text = SelectedStepValue.ToString();
    }

    private void SetStepUnit(ETransitStepUnit unit)
    {
        SelectedStepUnit = unit;
        SelectedStepValue = GetDefaultStepValue(unit);
        StepValueEntry.Text = SelectedStepValue.ToString();

        var lang = DataCache.Instance.CurrentLanguageCode;
        SelectedStepUnitText = GetLocalizedStepUnitText(unit, lang);
        StepUnitLabel.Text = SelectedStepUnitText;

        _isStepUnitExpanded = false;
        StepUnitPanel.IsVisible = false;
        StepUnitArrow.Text = "▼";

        RefreshStepUnitUnderline();
    }

    private void OnStepSecondsTapped(object sender, TappedEventArgs e) => SetStepUnit(ETransitStepUnit.Seconds);
    private void OnStepMinutesTapped(object sender, TappedEventArgs e) => SetStepUnit(ETransitStepUnit.Minutes);
    private void OnStepHoursTapped(object sender, TappedEventArgs e) => SetStepUnit(ETransitStepUnit.Hours);

    public sealed class TransitPlanetTableRow
    {
        public string Planet { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Zodiac { get; set; } = string.Empty;
        public string Nakshatra { get; set; } = string.Empty;
        public string Pada { get; set; } = string.Empty;
        public string Navamsha { get; set; } = string.Empty;
    }

    public List<TransitPlanetTableRow> TransitPlanetRows { get; set; } = new();

    private void BuildTransitPlanetTable(List<PlanetData> pdList, double ascendant)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        var planet = Localization.GetLocalizedText("Lg", lang);
        var lagnaId = SwissUtility.GetZodiacIdFromDegree(ascendant);

        var lagnaZodiacId = SwissUtility.GetZodiacIdFromDegree(ascendant);
        var lagnaNakshatraId = SwissUtility.GetNakshatraIdFromDegree(ascendant);
        var lagnaPadaId = SwissUtility.GetPadaIdFromDegree(ascendant);
        var lagnaPadaNumber = SwissUtility.GetPadaNumberByPadaId(lagnaPadaId);
        var lagnaNavamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(lagnaNakshatraId, lagnaPadaNumber);

        var lagnaRow = new TransitPlanetTableRow
        {
            Planet = planet,
            Degree = FormatDegree(ascendant),
            Zodiac = GetZodiacDisplay(lagnaZodiacId),
            Nakshatra = GetNakshatraDisplay(lagnaNakshatraId),
            Pada = SwissUtility.GetPadaNumberByPadaId(lagnaPadaId).ToString(),
            Navamsha = GetZodiacDisplay(lagnaNavamsaZodiacId)
        };

        TransitPlanetRows = pdList
            .OrderBy(p => p.PlanetId)
            .Select(p => new TransitPlanetTableRow
            {
                Planet = GetPlanetShortName(p.PlanetId),
                Degree = FormatDegree(p.Longitude),
                Zodiac = GetZodiacDisplay(p.ZodiacId),
                Nakshatra = GetNakshatraDisplay(p.NakshatraId),
                Pada = SwissUtility.GetPadaNumberByPadaId(p.PadaId).ToString(),
                Navamsha = GetZodiacDisplay(p.NavamsaZodiacId)
            })
            .ToList();

        TransitPlanetRows.Insert(0, lagnaRow);

        TransitPlanetTableView.ItemsSource = null;
        TransitPlanetTableView.ItemsSource = TransitPlanetRows;
    }

    private string GetPlanetShortName(int planetId)
    {
        var name = PanchangaHelper.GetPlanetDescEntity(planetId)?.Name ?? planetId.ToString();
        return name.Length >= 2 ? name.Substring(0, 2) : name;
    }

    private string GetZodiacDisplay(int zodiacId)
    {
        return $"{zodiacId}.{PanchangaHelper.GetZodiacDescEntity(zodiacId).Name}";
    }

    private string GetNakshatraDisplay(int nakshatraId)
    {
        return $"{nakshatraId}.{PanchangaHelper.GetNakshatraDescEntity(nakshatraId).ShortName}";
    }

    private string FormatDegree(double longitude)
    {
        var totalDegrees = longitude % 30.0;
        if (totalDegrees < 0)
            totalDegrees += 30.0;

        int deg = (int)totalDegrees;
        double m1 = (totalDegrees - deg) * 60.0;
        int min = (int)m1;
        int sec = (int)Math.Round((m1 - min) * 60.0);

        if (sec == 60)
        {
            sec = 0;
            min++;
        }

        if (min == 60)
        {
            min = 0;
            deg++;
        }

        return $"{deg}°{min:00}'{sec:00}\"";
    }

    private Point GetAbsolutePositionToRoot(VisualElement element, VisualElement root)
    {
        double x = element.X;
        double y = element.Y;

        Element? parent = element.Parent;
        while (parent is VisualElement ve && parent != root)
        {
            x += ve.X;
            y += ve.Y;
            parent = ve.Parent;
        }

        return new Point(x, y);
    }

    private void PositionStepUnitPanel()
    {
        if (StepUnitHeader == null || CurrentTransitOverlayRoot == null)
            return;

        var p = GetAbsolutePositionToRoot(StepUnitHeader, CurrentTransitOverlayRoot);

        // немного правее начала header и сразу под ним
        double panelX = p.X + 6;
        double panelY = p.Y + StepUnitHeader.Height + 2;

        AbsoluteLayout.SetLayoutBounds(StepUnitPanel, new Rect(panelX, panelY, 110, -1));
        AbsoluteLayout.SetLayoutFlags(StepUnitPanel, AbsoluteLayoutFlags.None);
    }

    private void RefreshStepUnitUnderline()
    {
        Dispatcher.Dispatch(() =>
        {
            StepUnitLabel.Measure(double.PositiveInfinity, double.PositiveInfinity);
            StepUnitline.WidthRequest = StepUnitLabel.DesiredSize.Width;
        });
    }





}