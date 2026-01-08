using Microsoft.Maui.Controls;

namespace PADMA.UI;

public sealed class YogaBar : ContentView
{
    private readonly GraphicsView _graphics;
    private readonly YogaBarDrawable _drawable;

    public YogaBar()
    {
        _drawable = new YogaBarDrawable();
        _graphics = new GraphicsView
        {
            Drawable = _drawable,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        Content = _graphics;
    }

    public static readonly BindableProperty DayDateProperty =
        BindableProperty.Create(nameof(DayDate), typeof(DateTime), typeof(YogaBar),
            default(DateTime), propertyChanged: (b, o, n) =>
            {
                var bar = (YogaBar)b;
                bar._drawable.DayDate = (DateTime)n;
                bar._graphics.Invalidate();
            });

    public DateTime DayDate
    {
        get => (DateTime)GetValue(DayDateProperty);
        set => SetValue(DayDateProperty, value);
    }

    public static readonly BindableProperty StripeProperty =
        BindableProperty.Create(nameof(Stripe), typeof(YogaOverviewStripe), typeof(YogaBar),
            default(YogaOverviewStripe), propertyChanged: (b, o, n) =>
            {
                var bar = (YogaBar)b;
                bar._drawable.Stripe = (YogaOverviewStripe?)n;
                bar._graphics.Invalidate();
            });

    public YogaOverviewStripe? Stripe
    {
        get => (YogaOverviewStripe?)GetValue(StripeProperty);
        set => SetValue(StripeProperty, value);
    }
}
