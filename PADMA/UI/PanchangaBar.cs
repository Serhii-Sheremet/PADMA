using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace PADMA.UI
{
    public class PanchangaBar : ContentView
    {
        private readonly GraphicsView _graphicsView;
        private readonly PanchangaBarDrawable _drawable;

        public PanchangaBar()
        {
            _drawable = new PanchangaBarDrawable();
            _graphicsView = new GraphicsView
            {
                Drawable = _drawable,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            Content = _graphicsView;
        }

        public static readonly BindableProperty DayDateProperty =
            BindableProperty.Create(
                nameof(DayDate),
                typeof(DateTime),
                typeof(PanchangaBar),
                default(DateTime),
                propertyChanged: OnDayDateChanged);

        public DateTime DayDate
        {
            get => (DateTime)GetValue(DayDateProperty);
            set => SetValue(DayDateProperty, value);
        }

        private static void OnDayDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var bar = (PanchangaBar)bindable;
            bar._drawable.DayDate = (DateTime)newValue;
            bar._graphicsView.Invalidate(); // перерисовать
        }

        public static readonly BindableProperty SegmentsProperty =
            BindableProperty.Create(
                nameof(Segments),
                typeof(IList<PanchangaSegment>),
                typeof(PanchangaBar),
                default(IList<PanchangaSegment>),
                propertyChanged: OnSegmentsChanged);

        public IList<PanchangaSegment> Segments
        {
            get => (IList<PanchangaSegment>)GetValue(SegmentsProperty);
            set => SetValue(SegmentsProperty, value);
        }

        private static void OnSegmentsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var bar = (PanchangaBar)bindable;
            bar._drawable.Segments = newValue as IList<PanchangaSegment>;
            bar._graphicsView.Invalidate();
        }
    }
}
