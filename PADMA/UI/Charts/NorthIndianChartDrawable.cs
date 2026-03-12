using Microsoft.Maui.Graphics;
using PADMA.Core.Models;
using PADMA.Core.Utilities;
using PADMA.Core.Services;

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
            DrawPlanets(canvas, chartRect);

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

            const float globalYOffset = 4f;

            canvas.FontSize = 10;
            canvas.FontColor = Colors.Black;

            var xs = GetDomNumbersPositionXCoordinate(rect.Left, rect.Width);
            var ys = GetDomNumbersPositionYCoordinate(rect.Top, rect.Height);

            for (int i = 0; i < Houses.Count; i++)
            {
                canvas.DrawString(
                    Houses[i].ZodiacNumber.ToString(),
                    xs[i],
                    ys[i] + globalYOffset,
                    HorizontalAlignment.Center);
            }
        }

        private static float[] GetDomNumbersPositionXCoordinate(float posX, float width)
        {
            return new float[12]
            {
                posX + (width / 2f),
                posX + (width / 4f),
                posX + (width / 4f - 12f),
                posX + (width / 2f - 12f),
                posX + (width / 4f - 12f),
                posX + (width / 4f),
                posX + (width / 2f),
                posX + (width - width / 4f),
                posX + (width - width / 4f + 12f),
                posX + (width / 2f + 12f),
                posX + (width - width / 4f + 12f),
                posX + (width - width / 4f)
            };
        }

        private static float[] GetDomNumbersPositionYCoordinate(float posY, float height)
        {
            return new float[12]
            {
                posY + (height / 2f - 12f),
                posY + (height / 4f - 12f),
                posY + (height / 4f),
                posY + (height / 2f),
                posY + (height - height / 4f),
                posY + (height - height / 4f + 12f),
                posY + (height / 2f + 12f),
                posY + (height - height / 4f + 12f),
                posY + (height - height / 4f),
                posY + (height / 2f),
                posY + (height / 4f),
                posY + (height / 4f - 12f)
            };
        }

        private void DrawPlanets(ICanvas canvas, RectF rect)
        {
            if (Houses == null || Houses.Count != 12)
                return;

            const float fontSize = 14f;
            canvas.FontSize = fontSize;

            foreach (var house in Houses)
            {
                if (house.Planets == null || house.Planets.Count == 0)
                    continue;

                switch (house.HouseNumber)
                {
                    case 1:
                        DrawCenteredDiamondHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width / 2f,
                            rect.Top + rect.Height / 4f);
                        break;

                    case 4:
                        DrawCenteredDiamondHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width / 4f,
                            rect.Top + rect.Height / 2f);
                        break;

                    case 7:
                        DrawCenteredDiamondHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width / 2f,
                            rect.Top + rect.Height * 3f / 4f);
                        break;

                    case 10:
                        DrawCenteredDiamondHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width * 3f / 4f,
                            rect.Top + rect.Height / 2f);
                        break;

                    case 2:
                        DrawTopTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width / 4f);
                        break;

                    case 12:
                        DrawTopTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width * 3f / 4f);
                        break;

                    case 6:
                        DrawBottomTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width / 4f);
                        break;

                    case 8:
                        DrawBottomTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Left + rect.Width * 3f / 4f);
                        break;

                    case 3:
                        DrawLeftTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Top + rect.Height / 4f);
                        break;

                    case 5:
                        DrawLeftTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Top + rect.Height * 3f / 4f);
                        break;

                    case 9:
                        DrawRightTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Top + rect.Height * 3f / 4f);
                        break;

                    case 11:
                        DrawRightTriangleHouse(
                            canvas, rect, house.Planets, fontSize,
                            rect.Top + rect.Height / 4f);
                        break;
                }
            }
        }

        private void DrawCenteredDiamondHouse(
            ICanvas canvas,
            RectF rect,
            List<ChartPlanetItem> planets,
            float fontSize,
            float centerX,
            float centerY)
        {
            var tokens = BuildPlanetTokens(canvas, planets, fontSize);
            if (tokens.Count == 0)
                return;

            float textHeight = tokens.Max(t => t.Height);
            const float spacing = 4f;

            float mainLineWidth = rect.Width / 2f - 40f;
            float sideLineWidth = (rect.Width / 2f * (rect.Height / 4f - textHeight) / (rect.Height / 4f)) - 40f;

            int index = 0;

            var mainRow = TakeCenteredRowTokens(tokens, ref index, mainLineWidth, spacing);
            DrawCenteredRow(canvas, mainRow, centerX, centerY, spacing);

            if (index >= tokens.Count)
                return;

            var topRow = TakeCenteredRowTokens(tokens, ref index, sideLineWidth, spacing);
            DrawCenteredRow(canvas, topRow, centerX, centerY - textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var bottomRow = TakeCenteredRowTokens(tokens, ref index, sideLineWidth, spacing);
            DrawCenteredRow(canvas, bottomRow, centerX, centerY + textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var rest = tokens.Skip(index).ToList();
            DrawCenteredRow(canvas, rest, centerX, centerY + 2f * textHeight, spacing);
        }

        private enum HouseRowAlignment
        {
            Center,
            Left,
            Right
        }

        private sealed class HouseRowSpec
        {
            public float AnchorX { get; init; }
            public float Y { get; init; }
            public float MaxWidth { get; init; }
            public HouseRowAlignment Alignment { get; init; }
        }

        private sealed class PlanetToken
        {
            public required ChartPlanetItem Item { get; init; }
            public required string Text { get; init; }
            public float Width { get; set; }
            public float Height { get; set; }
        }

        private List<PlanetToken> BuildPlanetTokens(ICanvas canvas, List<ChartPlanetItem> planets, float fontSize)
        {
            var tokens = new List<PlanetToken>();

            foreach (var p in planets)
            {
                var text = BuildPlanetText(p);
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                var size = canvas.GetStringSize(text, null, fontSize);

                tokens.Add(new PlanetToken
                {
                    Item = p,
                    Text = text,
                    Width = size.Width,
                    Height = size.Height
                });
            }

            return tokens;
        }

        private void DrawCenteredRow(
            ICanvas canvas,
            List<PlanetToken> rowTokens,
            float centerX,
            float centerY,
            float spacing = 4f)
        {
            if (rowTokens.Count == 0)
                return;

            const float globalYOffset = 12f;

            float usedLeft = 0f;
            float usedRight = 0f;
            bool leftTurn = false;
            bool rightTurn = false;

            for (int i = 0; i < rowTokens.Count; i++)
            {
                var token = rowTokens[i];
                float drawX;

                if (!leftTurn && !rightTurn)
                {
                    drawX = centerX - token.Width / 2f;
                    usedLeft += token.Width / 2f;
                    usedRight += token.Width / 2f;
                    leftTurn = true;
                }
                else if (leftTurn && !rightTurn)
                {
                    drawX = centerX - usedLeft - spacing - token.Width;
                    usedLeft += spacing + token.Width;
                    leftTurn = false;
                    rightTurn = true;
                }
                else
                {
                    drawX = centerX + usedRight + spacing;
                    usedRight += spacing + token.Width;
                    leftTurn = true;
                    rightTurn = false;
                }

                canvas.FontColor = GetPlanetDrawColor(token.Item);
                canvas.DrawString(
                    token.Text,
                    drawX,
                    centerY - token.Height / 2f + globalYOffset,
                    HorizontalAlignment.Left);
            }
        }

        private static List<PlanetToken> TakeCenteredRowTokens(
            List<PlanetToken> source,
            ref int startIndex,
            float maxWidth,
            float spacing = 4f)
        {
            var result = new List<PlanetToken>();
            float usedLeft = 0f;
            float usedRight = 0f;
            bool leftTurn = false;
            bool rightTurn = false;

            int i = startIndex;
            while (i < source.Count)
            {
                var token = source[i];
                float candidateLeft = usedLeft;
                float candidateRight = usedRight;

                if (!leftTurn && !rightTurn)
                {
                    candidateLeft += token.Width / 2f;
                    candidateRight += token.Width / 2f;
                }
                else if (leftTurn && !rightTurn)
                {
                    candidateLeft += spacing + token.Width;
                }
                else
                {
                    candidateRight += spacing + token.Width;
                }

                if (candidateLeft + candidateRight > maxWidth)
                    break;

                result.Add(token);
                usedLeft = candidateLeft;
                usedRight = candidateRight;

                if (!leftTurn && !rightTurn)
                {
                    leftTurn = true;
                }
                else if (leftTurn && !rightTurn)
                {
                    leftTurn = false;
                    rightTurn = true;
                }
                else
                {
                    leftTurn = true;
                    rightTurn = false;
                }

                i++;
            }

            startIndex = i;
            return result;
        }

        private void DrawLeftAlignedRow(
            ICanvas canvas,
            List<PlanetToken> rowTokens,
            float leftX,
            float centerY,
            float spacing = 4f)
        {
            if (rowTokens.Count == 0)
                return;

            const float globalYOffset = 12f;

            float x = leftX;

            foreach (var token in rowTokens)
            {
                canvas.FontColor = GetPlanetDrawColor(token.Item);
                canvas.DrawString(
                    token.Text,
                    x,
                    centerY - token.Height / 2f + globalYOffset,
                    HorizontalAlignment.Left);

                x += token.Width + spacing;
            }
        }

        private void DrawRightAlignedRow(
            ICanvas canvas,
            List<PlanetToken> rowTokens,
            float rightX,
            float centerY,
            float spacing = 4f)
        {
            if (rowTokens.Count == 0)
                return;

            const float globalYOffset = 12f;

            float totalWidth = 0f;
            for (int i = 0; i < rowTokens.Count; i++)
            {
                totalWidth += rowTokens[i].Width;
                if (i < rowTokens.Count - 1)
                    totalWidth += spacing;
            }

            float x = rightX - totalWidth;

            foreach (var token in rowTokens)
            {
                canvas.FontColor = GetPlanetDrawColor(token.Item);
                canvas.DrawString(
                    token.Text,
                    x,
                    centerY - token.Height / 2f + globalYOffset,
                    HorizontalAlignment.Left);

                x += token.Width + spacing;
            }
        }

        private static List<PlanetToken> TakeLinearRowTokens(
            List<PlanetToken> source,
            ref int startIndex,
            float maxWidth,
            float spacing = 4f)
        {
            var result = new List<PlanetToken>();
            float usedWidth = 0f;

            int i = startIndex;
            while (i < source.Count)
            {
                var token = source[i];
                float candidateWidth = usedWidth;

                if (result.Count > 0)
                    candidateWidth += spacing;

                candidateWidth += token.Width;

                if (candidateWidth > maxWidth && result.Count > 0)
                    break;

                result.Add(token);
                usedWidth = candidateWidth;
                i++;
            }

            startIndex = i;
            return result;
        }

        private void DrawLeftTriangleHouse(
            ICanvas canvas,
            RectF rect,
            List<ChartPlanetItem> planets,
            float fontSize,
            float centerY)
        {
            var tokens = BuildPlanetTokens(canvas, planets, fontSize);
            if (tokens.Count == 0)
                return;

            float textHeight = tokens.Max(t => t.Height);
            const float spacing = 4f;

            float mainLineWidth = rect.Width / 4f - 20f;
            float top1LineWidth = rect.Width / 4f * (rect.Height / 4f - textHeight) / (rect.Height / 4f);
            float top2LineWidth = rect.Width / 4f * (rect.Height / 4f - 2f * textHeight) / (rect.Height / 4f);

            float leftX = rect.Left + 4f;

            int index = 0;

            var mainRow = TakeLinearRowTokens(tokens, ref index, mainLineWidth, spacing);
            DrawLeftAlignedRow(canvas, mainRow, leftX, centerY, spacing);

            if (index >= tokens.Count)
                return;

            var top1Row = TakeLinearRowTokens(tokens, ref index, top1LineWidth, spacing);
            DrawLeftAlignedRow(canvas, top1Row, leftX, centerY - textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var bottom1Row = TakeLinearRowTokens(tokens, ref index, top1LineWidth, spacing);
            DrawLeftAlignedRow(canvas, bottom1Row, leftX, centerY + textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var top2Row = TakeLinearRowTokens(tokens, ref index, top2LineWidth, spacing);
            DrawLeftAlignedRow(canvas, top2Row, leftX, centerY - 2f * textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var rest = tokens.Skip(index).ToList();
            DrawLeftAlignedRow(canvas, rest, leftX, centerY + 2f * textHeight, spacing);
        }

        private void DrawRightTriangleHouse(
            ICanvas canvas,
            RectF rect,
            List<ChartPlanetItem> planets,
            float fontSize,
            float centerY)
        {
            var tokens = BuildPlanetTokens(canvas, planets, fontSize);
            if (tokens.Count == 0)
                return;

            float textHeight = tokens.Max(t => t.Height);
            const float spacing = 4f;

            float mainLineWidth = rect.Width / 4f - 20f;
            float top1LineWidth = rect.Width / 4f * (rect.Height / 4f - textHeight) / (rect.Height / 4f);
            float top2LineWidth = rect.Width / 4f * (rect.Height / 4f - 2f * textHeight) / (rect.Height / 4f);

            float rightX = rect.Right - 4f;

            int index = 0;

            var mainRow = TakeLinearRowTokens(tokens, ref index, mainLineWidth, spacing);
            DrawRightAlignedRow(canvas, mainRow, rightX, centerY, spacing);

            if (index >= tokens.Count)
                return;

            var top1Row = TakeLinearRowTokens(tokens, ref index, top1LineWidth, spacing);
            DrawRightAlignedRow(canvas, top1Row, rightX, centerY - textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var bottom1Row = TakeLinearRowTokens(tokens, ref index, top1LineWidth, spacing);
            DrawRightAlignedRow(canvas, bottom1Row, rightX, centerY + textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var top2Row = TakeLinearRowTokens(tokens, ref index, top2LineWidth, spacing);
            DrawRightAlignedRow(canvas, top2Row, rightX, centerY - 2f * textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var rest = tokens.Skip(index).ToList();
            DrawRightAlignedRow(canvas, rest, rightX, centerY + 2f * textHeight, spacing);
        }

        private void DrawTopTriangleHouse(
            ICanvas canvas,
            RectF rect,
            List<ChartPlanetItem> planets,
            float fontSize,
            float centerX)
        {
            var tokens = BuildPlanetTokens(canvas, planets, fontSize);
            if (tokens.Count == 0)
                return;

            float textHeight = tokens.Max(t => t.Height);
            const float spacing = 4f;

            float mainLineWidth = rect.Width / 2f * (rect.Height / 4f - 6f) / (rect.Height / 4f) - 40f;
            float bottom1LineWidth = rect.Width / 2f * (rect.Height / 4f - textHeight - 6f) / (rect.Height / 4f) - 40f;
            float bottom2LineWidth = rect.Width / 2f * (rect.Height / 4f - 2f * textHeight - 6f) / (rect.Height / 4f) - 20f;

            int index = 0;

            var mainRow = TakeCenteredRowTokens(tokens, ref index, mainLineWidth, spacing);
            DrawCenteredRow(canvas, mainRow, centerX, rect.Top + 4f + textHeight / 2f, spacing);

            if (index >= tokens.Count)
                return;

            var bottom1Row = TakeCenteredRowTokens(tokens, ref index, bottom1LineWidth, spacing);
            DrawCenteredRow(canvas, bottom1Row, centerX, rect.Top + textHeight + 4f + textHeight / 2f, spacing);

            if (index >= tokens.Count)
                return;

            var bottom2Row = TakeCenteredRowTokens(tokens, ref index, bottom2LineWidth, spacing);
            DrawCenteredRow(canvas, bottom2Row, centerX, rect.Top + 2f * textHeight + 4f + textHeight / 2f, spacing);

            if (index >= tokens.Count)
                return;

            var rest = tokens.Skip(index).ToList();
            DrawCenteredRow(canvas, rest, centerX, rect.Top + 3f * textHeight + 4f + textHeight / 2f, spacing);
        }

        private void DrawBottomTriangleHouse(
            ICanvas canvas,
            RectF rect,
            List<ChartPlanetItem> planets,
            float fontSize,
            float centerX)
        {
            var tokens = BuildPlanetTokens(canvas, planets, fontSize);
            if (tokens.Count == 0)
                return;

            float textHeight = tokens.Max(t => t.Height);
            const float spacing = 4f;

            float mainLineWidth = rect.Width / 2f * (rect.Height / 4f - 6f) / (rect.Height / 4f) - 40f;
            float top1LineWidth = rect.Width / 2f * (rect.Height / 4f - textHeight - 6f) / (rect.Height / 4f) - 40f;
            float top2LineWidth = rect.Width / 2f * (rect.Height / 4f - 2f * textHeight - 6f) / (rect.Height / 4f) - 20f;

            float baseCenterY = rect.Bottom - textHeight / 2f - 4f;

            int index = 0;

            var mainRow = TakeCenteredRowTokens(tokens, ref index, mainLineWidth, spacing);
            DrawCenteredRow(canvas, mainRow, centerX, baseCenterY, spacing);

            if (index >= tokens.Count)
                return;

            var top1Row = TakeCenteredRowTokens(tokens, ref index, top1LineWidth, spacing);
            DrawCenteredRow(canvas, top1Row, centerX, baseCenterY - textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var top2Row = TakeCenteredRowTokens(tokens, ref index, top2LineWidth, spacing);
            DrawCenteredRow(canvas, top2Row, centerX, baseCenterY - 2f * textHeight, spacing);

            if (index >= tokens.Count)
                return;

            var rest = tokens.Skip(index).ToList();
            DrawCenteredRow(canvas, rest, centerX, baseCenterY - 3f * textHeight, spacing);
        }

        private static Color GetPlanetDrawColor(ChartPlanetItem item)
        {
            if (item.IsActiveAspect)
                return Colors.Gray;

            return DataCache.Instance.GetColor(item.ColorCode);
        }

        private static string BuildPlanetText(ChartPlanetItem item)
        {
            var planetName = PanchangaHelper.GetPlanetDescEntity((int)item.PlanetCode)?.Name ?? string.Empty;

            var shortName = planetName.Length >= 2
                ? planetName.Substring(0, 2)
                : planetName;

            bool isRetro = item.Retro == "R";

            if (isRetro)
                return shortName + "R";

            if (!string.IsNullOrWhiteSpace(item.Exaltation))
                return shortName + item.Exaltation;

            return shortName;
        }
        


    }
}