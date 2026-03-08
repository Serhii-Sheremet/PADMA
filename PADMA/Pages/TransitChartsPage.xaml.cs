using PADMA.Core.Models;
using PADMA.Core.Analysis;
using PADMA.Core.Services;
using PADMA.Core.Enums;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System.Globalization;

namespace PADMA.Pages
{
    public partial class TransitChartsPage : ConfigBasePage
    {
        private bool _showCurrentTransits = true;
        private bool _isAspectsExpanded;
        private bool _isUpdatingAspectChecks;

        public TransitChartsPage()
        {
            InitializeComponent();
            
            ChartView.SizeChanged += OnChartViewSizeChanged;

            ApplyLocalization();
            UpdateTabState();
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

            LoadSelectedTabContent();
        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            Title = Localization.GetLocalizedText("Transit charts", lang);
            CurrentTabLabel.Text = Localization.GetLocalizedText("Current Transits", lang);
            NatalTabLabel.Text = Localization.GetLocalizedText("Transits from Natal Positions", lang);
            
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

        }

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
        }

        private void OnCurrentTransitsTapped(object sender, TappedEventArgs e)
        {
            if (_showCurrentTransits)
                return;

            _showCurrentTransits = true;
            ApplyLocalization();
            UpdateTabState();
        }

        private void OnNatalTransitsTapped(object sender, TappedEventArgs e)
        {
            if (!_showCurrentTransits)
                return;

            _showCurrentTransits = false;
            ApplyLocalization();
            UpdateTabState();
        }

        private void LoadSelectedTabContent()
        {
            if (_showCurrentTransits)
            {
                LoadCurrentTransitChart();
            }
            else
            {
                //LoadNatalTransitPlaceholder(); 
            }
        }

        private void LoadCurrentTransitChart()
        {
            Profile? profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (profile == null || ctx == null)
                return;
            
            var nodeMode = DataCache.Instance.GetActiveNodeSetting();
            var nowUtc = DateTime.Now.ToUniversalTime();

            var pdList = SwissAnalysis.CalculatePlanetPositionsForDate(nowUtc, ctx.LivingLat, ctx.LivingLon, nodeMode);
            var ascendant = SwissService.CalculateAscendantForDate(nowUtc, ctx.LivingLat, ctx.LivingLon, 0, 'O');
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

        private void OnAspectCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (_isUpdatingAspectChecks)
                return;

            try
            {
                _isUpdatingAspectChecks = true;

                if (sender == CheckAspectAll)
                {
                    bool value = CheckAspectAll.IsChecked;

                    CheckAspectSun.IsChecked = value;
                    CheckAspectMoon.IsChecked = value;
                    CheckAspectMars.IsChecked = value;
                    CheckAspectMercury.IsChecked = value;
                    CheckAspectJupiter.IsChecked = value;
                    CheckAspectVenus.IsChecked = value;
                    CheckAspectSaturn.IsChecked = value;
                    CheckAspectRahu.IsChecked = value;
                }
                else
                {
                    bool allChecked =
                        CheckAspectSun.IsChecked &&
                        CheckAspectMoon.IsChecked &&
                        CheckAspectMars.IsChecked &&
                        CheckAspectMercury.IsChecked &&
                        CheckAspectJupiter.IsChecked &&
                        CheckAspectVenus.IsChecked &&
                        CheckAspectSaturn.IsChecked &&
                        CheckAspectRahu.IsChecked;

                    CheckAspectAll.IsChecked = allChecked;
                }
            }
            finally
            {
                _isUpdatingAspectChecks = false;
            }

            if (_showCurrentTransits)
            {
                LoadCurrentTransitChart();
            }
        }

        private bool AreAnyAspectsSelected()
        {
            return CheckAspectAll.IsChecked ||
                   CheckAspectSun.IsChecked ||
                   CheckAspectMoon.IsChecked ||
                   CheckAspectMars.IsChecked ||
                   CheckAspectMercury.IsChecked ||
                   CheckAspectJupiter.IsChecked ||
                   CheckAspectVenus.IsChecked ||
                   CheckAspectSaturn.IsChecked ||
                   CheckAspectRahu.IsChecked;
        }

        private List<EPlanet> GetSelectedAspectPlanets()
        {
            var result = new List<EPlanet>();

            if (CheckAspectAll.IsChecked)
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

            if (CheckAspectSun.IsChecked) result.Add(EPlanet.SUN);
            if (CheckAspectMoon.IsChecked) result.Add(EPlanet.MOON);
            if (CheckAspectMars.IsChecked) result.Add(EPlanet.MARS);
            if (CheckAspectMercury.IsChecked) result.Add(EPlanet.MERCURY);
            if (CheckAspectJupiter.IsChecked) result.Add(EPlanet.JUPITER);
            if (CheckAspectVenus.IsChecked) result.Add(EPlanet.VENUS);
            if (CheckAspectSaturn.IsChecked) result.Add(EPlanet.SATURN);
            if (CheckAspectRahu.IsChecked) result.Add(EPlanet.RAHU);

            return result;
        }


    }
}