using Microsoft.Maui.Controls;
using PADMA.Core.Models;

namespace PADMA.UI.Charts
{
    public sealed class NorthIndianChartView : GraphicsView
    {
        private readonly NorthIndianChartDrawable _drawable;

        public NorthIndianChartView()
        {
            _drawable = new NorthIndianChartDrawable();
            Drawable = _drawable;

            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Start;
        }

        public Color ChartBackgroundColor
        {
            get => _drawable.BackgroundColor;
            set
            {
                _drawable.BackgroundColor = value;
                Invalidate();
            }
        }

        public Color ChartLineColor
        {
            get => _drawable.LineColor;
            set
            {
                _drawable.LineColor = value;
                Invalidate();
            }
        }

        public float ChartLineStrokeWidth
        {
            get => _drawable.LineStrokeWidth;
            set
            {
                _drawable.LineStrokeWidth = value;
                Invalidate();
            }
        }

        public void SetHouses(List<ChartHouseData> houses)
        {
            _drawable.Houses = houses;
            Invalidate();
        }
    }
}