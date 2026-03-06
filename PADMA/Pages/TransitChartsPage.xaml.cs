using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

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

            LoadTestChartHouses();
        }

        private void LoadTestChartHouses()
        {
            var zList = DataCache.Instance.ZodiacList;
            var houses = new List<ChartHouseData>();

            for (int i = 0; i < zList.Count && i < 12; i++)
            {
                houses.Add(new ChartHouseData
                {
                    HouseNumber = i + 1,
                    ZodiacNumber = zList[i].Id,
                    Planets = new List<ChartPlanetItem>()
                });
            }

            ChartView.SetHouses(houses);
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
    }
}