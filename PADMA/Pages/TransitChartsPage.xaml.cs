using PADMA.Core.Models;
using PADMA.Core.Analysis;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;
using System.Globalization;

namespace PADMA.Pages
{
    public partial class TransitChartsPage : ConfigBasePage
    {
        private bool _showCurrentTransits = true;

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

            var now = DateTime.Now;

            var pdList = SwissAnalysis.CalculatePlanetPositionsForDate(now, ctx.LivingLat, ctx.LivingLon, nodeMode);
            var ascendant = SwissService.CalculateAscendantForDate(now, ctx.LivingLat, ctx.LivingLon, 0, 'O');
            var lagnaId = SwissUtility.GetZodiacIdFromDegree(ascendant); 
            
            var swappedZodiacs = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), lagnaId);

            var houses = TransitChartDataService.BuildCurrentTransitChartHouses(
                pdList,
                swappedZodiacs);

            ChartView.SetHouses(houses);
        }


    }
}