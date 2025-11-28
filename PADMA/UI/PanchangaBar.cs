using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using PADMA.Core.Utilities;

namespace PADMA.UI
{
    public class PanchangaBar : ContentView
    {
        private readonly AbsoluteLayout _layout;

        public PanchangaBar()
        {
            _layout = new AbsoluteLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            Content = _layout;

            // высоту пока зафиксируем, позже можем вынести в XAML
            HeightRequest = 4;

            SizeChanged += (_, __) => Redraw();
        }

        #region Bindable properties

        // Дата дня, для которого рисуем полоску
        public static readonly BindableProperty DayDateProperty =
            BindableProperty.Create(
                nameof(DayDate),
                typeof(DateTime),
                typeof(PanchangaBar),
                default(DateTime),
                propertyChanged: OnBarPropertyChanged);

        public DateTime DayDate
        {
            get => (DateTime)GetValue(DayDateProperty);
            set => SetValue(DayDateProperty, value);
        }

        // Список сегментов
        public static readonly BindableProperty SegmentsProperty =
            BindableProperty.Create(
                nameof(Segments),
                typeof(IList<PanchangaSegment>),
                typeof(PanchangaBar),
                default(IList<PanchangaSegment>),
                propertyChanged: OnBarPropertyChanged);

        public IList<PanchangaSegment> Segments
        {
            get => (IList<PanchangaSegment>)GetValue(SegmentsProperty);
            set => SetValue(SegmentsProperty, value);
        }

        private static void OnBarPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PanchangaBar bar)
            {
                bar.Redraw();
            }
        }

        #endregion

        private void Redraw()
        {
            _layout.Children.Clear();

            if (Width <= 0 || Height <= 0)
                return;

            if (Segments == null || Segments.Count == 0)
                return;

            var dayStart = DayDate.Date;
            var dayEnd = dayStart.AddDays(1);
            double width = Width;
            double height = Height;

            foreach (var segment in Segments)
            {
                // ограничиваем сегмент рамками суток
                var start = segment.Start < dayStart ? dayStart : segment.Start;
                var end = segment.End > dayEnd ? dayEnd : segment.End;

                if (end <= start)
                    continue;

                double x1 = CalendarDrawingHelper.ConvertTimeToPixels(width, start, dayStart);
                double x2 = CalendarDrawingHelper.ConvertTimeToPixels(width, end, dayStart);
                double segWidth = Math.Max(1, x2 - x1);

                // сам цветной сегмент
                var box = new BoxView
                {
                    Color = segment.Color
                };

                AbsoluteLayout.SetLayoutBounds(box, new Rect(x1, 0, segWidth, height));
                AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.None);
                _layout.Children.Add(box);

                // тонкая вертикальная линия на границе сегмента (кроме последнего пикселя справа)
                var line = new BoxView
                {
                    Color = Colors.Black,
                    WidthRequest = 1
                };

                AbsoluteLayout.SetLayoutBounds(line, new Rect(x2 - 0.5, 0, 1, height));
                AbsoluteLayout.SetLayoutFlags(line, AbsoluteLayoutFlags.None);
                _layout.Children.Add(line);
            }
        }
    }
}
