using Microsoft.Maui.Graphics;
using PADMA.Core.Models;

namespace PADMA.UI.Charts
{
    public sealed class NorthIndianChartDrawable : IDrawable
    {
        public Color BackgroundColor { get; set; } = Color.FromArgb("#FAFAD2");
        public Color LineColor { get; set; } = Colors.Black;
        public float LineStrokeWidth { get; set; } = 1f;

        public List<ChartHouseData>? Houses { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
            var left = dirtyRect.Left + (dirtyRect.Width - size) / 2f;
            var top = dirtyRect.Top + (dirtyRect.Height - size) / 2f;

            var chartRect = new RectF(left, top, size, size);

            // fill only chart area
            canvas.FillColor = BackgroundColor;
            canvas.FillRectangle(chartRect);

            canvas.StrokeColor = LineColor;
            canvas.StrokeSize = LineStrokeWidth;

            DrawChartGrid(canvas, chartRect);
            DrawZodiacNumbers(canvas, chartRect);

            canvas.RestoreState();
        }

        private static void DrawChartGrid(ICanvas canvas, RectF rect)
        {
            var x = rect.Left;
            var y = rect.Top;
            var w = rect.Width;
            var h = rect.Height;

            var midX = x + w / 2f;
            var midY = y + h / 2f;

            var topMid = new PointF(midX, y);
            var rightMid = new PointF(x + w, midY);
            var bottomMid = new PointF(midX, y + h);
            var leftMid = new PointF(x, midY);

            var topLeft = new PointF(x, y);
            var topRight = new PointF(x + w, y);
            var bottomRight = new PointF(x + w, y + h);
            var bottomLeft = new PointF(x, y + h);

            // outer border
            canvas.DrawRectangle(rect);

            // diamond
            canvas.DrawLine(topMid.X, topMid.Y, rightMid.X, rightMid.Y);
            canvas.DrawLine(rightMid.X, rightMid.Y, bottomMid.X, bottomMid.Y);
            canvas.DrawLine(bottomMid.X, bottomMid.Y, leftMid.X, leftMid.Y);
            canvas.DrawLine(leftMid.X, leftMid.Y, topMid.X, topMid.Y);

            // corner-to-diamond lines
            canvas.DrawLine(topLeft.X, topLeft.Y, midX, midY);
            canvas.DrawLine(topRight.X, topRight.Y, midX, midY);
            canvas.DrawLine(bottomRight.X, bottomRight.Y, midX, midY);
            canvas.DrawLine(bottomLeft.X, bottomLeft.Y, midX, midY);
        }

        private void DrawZodiacNumbers(ICanvas canvas, RectF rect)
        {
            if (Houses == null || Houses.Count != 12)
                return;

            var x = rect.Left;
            var y = rect.Top;
            var w = rect.Width;
            var h = rect.Height;

            var topLeftNode = new PointF(x + w * 0.25f, y + h * 0.25f);
            var topRightNode = new PointF(x + w * 0.75f, y + h * 0.25f);
            var centerNode = new PointF(x + w * 0.50f, y + h * 0.50f);
            var bottomLeftNode = new PointF(x + w * 0.25f, y + h * 0.75f);
            var bottomRightNode = new PointF(x + w * 0.75f, y + h * 0.75f);

            canvas.FontColor = Colors.Black;
            canvas.FontSize = 14;

            ChartHouseData? GetHouse(int houseNumber)
                => Houses.FirstOrDefault(house => house.HouseNumber == houseNumber);

            void DrawHouseNumber(int houseNumber, float px, float py)
            {
                var house = GetHouse(houseNumber);
                if (house == null)
                    return;

                canvas.DrawString(
                    house.ZodiacNumber.ToString(),
                    px,
                    py,
                    24,
                    20,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);
            }

            // Top-left node: H2, H3
            DrawHouseNumber(2, topLeftNode.X - w * 0.045f, topLeftNode.Y - h * 0.070f); // правее
            DrawHouseNumber(3, topLeftNode.X - w * 0.090f, topLeftNode.Y - h * 0.020f); // левее

            // Top-right node: H12, H11
            DrawHouseNumber(12, topRightNode.X - w * 0.040f, topRightNode.Y - h * 0.070f); // левее
            DrawHouseNumber(11, topRightNode.X + w * 0.010f, topRightNode.Y - h * 0.020f);

            // Center node: H1 top, H4 left, H10 right, H7 bottom
            DrawHouseNumber(1, centerNode.X - 12, centerNode.Y - h * 0.090f); // ближе к узлу
            DrawHouseNumber(4, centerNode.X - w * 0.090f, centerNode.Y - h * 0.030f); // выше
            DrawHouseNumber(10, centerNode.X + w * 0.015f, centerNode.Y - h * 0.030f); // выше и левее
            DrawHouseNumber(7, centerNode.X - 12, centerNode.Y + h * 0.045f); // ближе к узлу

            // Bottom-left node: H6, H5
            DrawHouseNumber(6, bottomLeftNode.X - w * 0.045f, bottomLeftNode.Y + h * 0.015f); // правее
            DrawHouseNumber(5, bottomLeftNode.X - w * 0.090f, bottomLeftNode.Y - h * 0.020f); // левее

            // Bottom-right node: H8, H9
            DrawHouseNumber(8, bottomRightNode.X - w * 0.045f, bottomRightNode.Y + h * 0.015f); // левее
            DrawHouseNumber(9, bottomRightNode.X + w * 0.010f, bottomRightNode.Y - h * 0.025f); // левее
        }






    }
}