using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System.ComponentModel;
using System.Globalization;

namespace PADMA.Pages
{
    public partial class TransitChartsPage : ConfigBasePage
    {
        private bool _showCurrentTransits = true;
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

        private bool _isNatalReferenceExpanded;
        private int _selectedNatalReferenceId = 0; // 0 = Lagna

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
                OnPropertyChanged(nameof(CurrentTransitDateDisplay));
                OnPropertyChanged(nameof(CurrentTransitTimeDisplay));
            }
        }

        public DateTime CurrentTransitUtcDateTime => CurrentTransitLocalDateTime.ToUniversalTime();

        private string _selectedStepUnit = "Seconds";
        public string SelectedStepUnit
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

        public string CurrentTransitDateDisplay =>
            CurrentTransitLocalDateTime.ToString("dddd, dd MMMM yyyy", CultureInfo.CurrentCulture);

        public string CurrentTransitTimeDisplay =>
            CurrentTransitLocalDateTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);

        private readonly List<string> _stepUnits = new()
        {
            "Seconds",
            "Minutes",
            "Hours",
            "Days",
            "Months",
            "Years"
        };

        private int GetParsedStepValue()
        {
            if (int.TryParse(StepValueEntry.Text, out var value) && value > 0)
                return value;

            return GetDefaultStepValue(SelectedStepUnit);
        }

        private static int GetDefaultStepValue(string stepUnit) =>
            stepUnit switch
            {
                "Seconds" => 10,
                "Minutes" => 1,
                "Hours" => 1,
                "Days" => 1,
                "Months" => 1,
                "Years" => 1,
                _ => 1
            };

        private bool _isDateEditorOpen;
        private bool _isTimeEditorOpen;

        private void ShowDateEditor()
        {
            if (_isDateEditorOpen)
                return;

            _isDateEditorOpen = true;

            TransitDateLabel.IsVisible = false;
            transitDatePicker.IsVisible = true;
            transitDatePicker.Opacity = 0.01;
            transitDatePicker.Date = CurrentTransitLocalDateTime.Date;

            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(80), () =>
            {
                if (_isDateEditorOpen)
                    transitDatePicker.Focus();
            });
        }

        private void HideDateEditor()
        {
            _isDateEditorOpen = false;
            transitDatePicker.IsVisible = false;
            TransitDateLabel.IsVisible = true;
        }

        private void ShowTimeEditor()
        {
            if (_isTimeEditorOpen)
                return;

            _isTimeEditorOpen = true;

            TransitTimeLabel.IsVisible = false;
            transitTimePicker.IsVisible = true;
            transitTimePicker.Opacity = 0.01;
            transitTimePicker.Time = CurrentTransitLocalDateTime.TimeOfDay;

            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(80), () =>
            {
                if (_isTimeEditorOpen)
                    transitTimePicker.Focus();
            });
        }

        private void HideTimeEditor()
        {
            _isTimeEditorOpen = false;
            transitTimePicker.IsVisible = false;
            TransitTimeLabel.IsVisible = true;
        }

        private void OnTransitDateTapped(object sender, TappedEventArgs e)
        {
            ShowDateEditor();
        }

        private void OnTransitTimeTapped(object sender, TappedEventArgs e)
        {
            ShowTimeEditor();
        }

        public TransitChartsPage()
        {
            InitializeComponent();
            BindingContext = this;

            ChartView.SizeChanged += OnChartViewSizeChanged;
        }

        private void OnChartViewSizeChanged(object? sender, EventArgs e)
        {
            if (ChartView.Width > 0)
            {
                ChartView.HeightRequest = ChartView.Width;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ApplyLocalization();
            UpdateTabState();
            UpdateAspectSelectionUi();
            UpdateNatalReferenceUnderline();
            UpdateStepUnitUi();

            transitDatePicker.Date = CurrentTransitLocalDateTime.Date;
            transitTimePicker.Time = CurrentTransitLocalDateTime.TimeOfDay;

            HideDateEditor();
            HideTimeEditor();

            Dispatcher.Dispatch(UpdateNatalReferenceUnderline);
            LoadSelectedTabContent();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            Title = Localization.GetLocalizedText("Transit charts", lang);
            CurrentTabLabel.Text = Localization.GetLocalizedText("Current Transits", lang);
            NatalTabLabel.Text = Localization.GetLocalizedText("Transits from Natal", lang);
            
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

            NatalReferenceLabel.Text = GetNatalReferenceDisplayName(_selectedNatalReferenceId);

            NatalRefLagnaLabel.Text = Localization.GetLocalizedText("Lagna", lang);
            NatalRefSunLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.SUN)?.Name ?? "Sun";
            NatalRefMoonLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.MOON)?.Name ?? "Moon";
            NatalRefMarsLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.MARS)?.Name ?? "Mars";
            NatalRefMercuryLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.MERCURY)?.Name ?? "Mercury";
            NatalRefJupiterLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.JUPITER)?.Name ?? "Jupiter";
            NatalRefVenusLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.VENUS)?.Name ?? "Venus";
            NatalRefSaturnLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.SATURN)?.Name ?? "Saturn";
            NatalRefRahuLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.RAHU)?.Name ?? "Rahu";
            NatalRefKetuLabel.Text = PanchangaHelper.GetPlanetDescEntity((int)EPlanet.KETU)?.Name ?? "Ketu";

        }

        private void UpdateNatalReferenceState()
        {
            var isEnabled = !_showCurrentTransits;

            NatalReferenceHeader.InputTransparent = !isEnabled;

            NatalReferenceLabel.TextColor = isEnabled ? Colors.Black : Colors.Gray;
            NatalReferenceArrow.TextColor = isEnabled ? Colors.Black : Colors.Gray;
            NatalReferenceUnderline.Color = isEnabled
                ? Color.FromArgb("#C8A23A")
                : Color.FromArgb("#D0D0D0");

            if (!isEnabled)
            {
                _isNatalReferenceExpanded = false;
                NatalReferencePanel.IsVisible = false;
                NatalReferenceArrow.Text = "▼";
            }
        }

        private void UpdateNatalReferenceUnderline()
        {
            if (NatalReferenceHeader.Width > 0)
            {
                NatalReferenceUnderline.WidthRequest = NatalReferenceHeader.Width;
                return;
            }

            var measureLabel = new Label
            {
                Text = NatalReferenceLabel.Text,
                FontSize = NatalReferenceLabel.FontSize,
                LineBreakMode = LineBreakMode.NoWrap
            };

            var size = measureLabel.Measure(double.PositiveInfinity, double.PositiveInfinity);
            NatalReferenceUnderline.WidthRequest = Math.Max(24, size.Width + 8);
        }

        private string GetNatalReferenceDisplayName(int id)
        {
            if (id == 0)
                return Localization.GetLocalizedText("Lagna", DataCache.Instance.CurrentLanguageCode);

            return PanchangaHelper.GetPlanetDescEntity(id)?.Name ?? "Lagna";
        }

        private void OnNatalReferenceTapped(object sender, TappedEventArgs e)
        {
            if (_showCurrentTransits)
                return;

            _isNatalReferenceExpanded = !_isNatalReferenceExpanded;
            NatalReferencePanel.IsVisible = _isNatalReferenceExpanded;
            NatalReferenceArrow.Text = _isNatalReferenceExpanded ? "▲" : "▼";
        }

        private void SelectNatalReference(int id)
        {
            _selectedNatalReferenceId = id;
            NatalReferenceLabel.Text = GetNatalReferenceDisplayName(id);

            _isNatalReferenceExpanded = false;
            NatalReferencePanel.IsVisible = false;
            NatalReferenceArrow.Text = "▼";

            UpdateNatalReferenceUnderline();

            if (!_showCurrentTransits)
            {
                //LoadNatalTransitChart(id);
            }
        }

        private void OnNatalReferenceLagnaTapped(object sender, TappedEventArgs e) => SelectNatalReference(0);
        private void OnNatalReferenceSunTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.SUN);
        private void OnNatalReferenceMoonTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.MOON);
        private void OnNatalReferenceMarsTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.MARS);
        private void OnNatalReferenceMercuryTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.MERCURY);
        private void OnNatalReferenceJupiterTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.JUPITER);
        private void OnNatalReferenceVenusTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.VENUS);
        private void OnNatalReferenceSaturnTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.SATURN);
        private void OnNatalReferenceRahuTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.RAHU);
        private void OnNatalReferenceKetuTapped(object sender, TappedEventArgs e) => SelectNatalReference((int)EPlanet.KETU);

        private void UpdateTabState()
        {
            var selectedText = Colors.Black;
            var unselectedText = Colors.Gray;

            var selectedIndicator = Color.FromArgb("#C8A23A");   // gold-like
            var unselectedIndicator = Color.FromArgb("#D0D0D0"); // light gray

            CurrentTabLabel.TextColor = _showCurrentTransits ? selectedText : unselectedText;
            NatalTabLabel.TextColor = _showCurrentTransits ? unselectedText : selectedText;

            CurrentTabIndicator.Color = _showCurrentTransits ? selectedIndicator : unselectedIndicator;
            NatalTabIndicator.Color = _showCurrentTransits ? unselectedIndicator : selectedIndicator;

            UpdateNatalReferenceState();
        }

        private void OnCurrentTransitsTapped(object sender, TappedEventArgs e)
        {
            if (_showCurrentTransits)
                return;

            _showCurrentTransits = true;
            ApplyLocalization();
            UpdateTabState();
            LoadSelectedTabContent();
        }

        private void OnNatalTransitsTapped(object sender, TappedEventArgs e)
        {
            if (!_showCurrentTransits)
                return;

            _showCurrentTransits = false;
            ApplyLocalization();
            UpdateTabState();
            LoadSelectedTabContent();
        }

        private void LoadSelectedTabContent()
        {
            if (_showCurrentTransits)
            {
                LoadCurrentTransitChart(CurrentTransitUtcDateTime);
            }
            else
            {
                //LoadNatalTransitPlaceholder(); 
            }
        }

        private void LoadCurrentTransitChart(DateTime transitUtc)
        {
            Profile? profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (profile == null || ctx == null)
                return;
            
            var nodeMode = DataCache.Instance.GetActiveNodeSetting();

            var pdList = SwissAnalysis.CalculatePlanetPositionsForDate(transitUtc, ctx.LivingLat, ctx.LivingLon, nodeMode);
            var ascendant = SwissService.CalculateAscendantForDate(transitUtc, ctx.LivingLat, ctx.LivingLon, 0, 'O');
            var lagnaId = SwissUtility.GetZodiacIdFromDegree(ascendant); 
            
            var swappedZodiacs = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), lagnaId);
            var selectedAspectPlanets = GetSelectedAspectPlanets();

            var houses = TransitChartDataService.BuildCurrentTransitChartHouses(
                pdList,
                swappedZodiacs,
                selectedAspectPlanets);

            ChartView.SetHouses(houses);
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

            if (_showCurrentTransits)
            {
                LoadCurrentTransitChart(CurrentTransitUtcDateTime);
            }
        }

        private void OnAspectSunTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectSunSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectMoonTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectMoonSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectMarsTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectMarsSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectMercuryTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectMercurySelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectJupiterTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectJupiterSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectVenusTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectVenusSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectSaturnTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectSaturnSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnAspectRahuTapped(object sender, TappedEventArgs e)
        {
            ToggleAspect(ref _aspectRahuSelected);
            UpdateAspectAllState();
            UpdateAspectSelectionUi();
            if (_showCurrentTransits) LoadCurrentTransitChart(CurrentTransitUtcDateTime); 
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
                "Seconds" => CurrentTransitLocalDateTime.AddSeconds(value),
                "Minutes" => CurrentTransitLocalDateTime.AddMinutes(value),
                "Hours" => CurrentTransitLocalDateTime.AddHours(value),
                "Days" => CurrentTransitLocalDateTime.AddDays(value),
                "Months" => CurrentTransitLocalDateTime.AddMonths(value),
                "Years" => CurrentTransitLocalDateTime.AddYears(value),
                _ => CurrentTransitLocalDateTime
            };

            transitDatePicker.Date = CurrentTransitLocalDateTime.Date;
            transitTimePicker.Time = CurrentTransitLocalDateTime.TimeOfDay;

            LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnStepUnitTapped(object sender, EventArgs e)
        {
            _isStepUnitExpanded = !_isStepUnitExpanded;
            StepUnitPanel.IsVisible = _isStepUnitExpanded;
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

        private void OnTransitDateSelected(object sender, DateChangedEventArgs e)
        {
            CurrentTransitLocalDateTime = new DateTime(
                e.NewDate.Year,
                e.NewDate.Month,
                e.NewDate.Day,
                CurrentTransitLocalDateTime.Hour,
                CurrentTransitLocalDateTime.Minute,
                CurrentTransitLocalDateTime.Second);

            HideDateEditor();
            LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnTransitDateUnfocused(object sender, FocusEventArgs e)
        {
            HideDateEditor();
        }

        private void OnTransitTimeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TimePicker.Time))
                return;

            var time = transitTimePicker.Time;

            CurrentTransitLocalDateTime = new DateTime(
                CurrentTransitLocalDateTime.Year,
                CurrentTransitLocalDateTime.Month,
                CurrentTransitLocalDateTime.Day,
                time.Hours,
                time.Minutes,
                time.Seconds);

            HideTimeEditor();
            LoadCurrentTransitChart(CurrentTransitUtcDateTime);
        }

        private void OnTransitTimeUnfocused(object sender, FocusEventArgs e)
        {
            HideTimeEditor();
        }

        private void UpdateStepUnitUi()
        {
            StepUnitLabel.Text = SelectedStepUnit;
            StepUnitArrow.Text = _isStepUnitExpanded ? "▲" : "▼";
            StepValueEntry.Text = SelectedStepValue.ToString();
        }

        private void SetStepUnit(string unit)
        {
            SelectedStepUnit = unit;
            SelectedStepValue = GetDefaultStepValue(unit);

            _isStepUnitExpanded = false;
            StepUnitPanel.IsVisible = false;

            UpdateStepUnitUi();
        }

        private void OnStepSecondsTapped(object sender, TappedEventArgs e) => SetStepUnit("Seconds");
        private void OnStepMinutesTapped(object sender, TappedEventArgs e) => SetStepUnit("Minutes");
        private void OnStepHoursTapped(object sender, TappedEventArgs e) => SetStepUnit("Hours");
        private void OnStepDaysTapped(object sender, TappedEventArgs e) => SetStepUnit("Days");
        private void OnStepMonthsTapped(object sender, TappedEventArgs e) => SetStepUnit("Months");
        private void OnStepYearsTapped(object sender, TappedEventArgs e) => SetStepUnit("Years");




    }
}