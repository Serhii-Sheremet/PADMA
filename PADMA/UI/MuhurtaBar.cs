using Microsoft.Maui.Controls;

namespace PADMA.UI;

public sealed class MuhurtaBar : ContentView
{
    private readonly GraphicsView _graphics;
    private readonly MuhurtaBarDrawable _drawable;

    public MuhurtaBar()
    {
        _drawable = new MuhurtaBarDrawable();
        _graphics = new GraphicsView
        {
            Drawable = _drawable,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        Content = _graphics;
    }

    public static readonly BindableProperty DayDateProperty =
        BindableProperty.Create(nameof(DayDate), typeof(DateTime), typeof(MuhurtaBar),
            default(DateTime), propertyChanged: (b, o, n) =>
            {
                var bar = (MuhurtaBar)b;
                bar._drawable.DayDate = (DateTime)n;
                bar._graphics.Invalidate();
            });

    public DateTime DayDate
    {
        get => (DateTime)GetValue(DayDateProperty);
        set => SetValue(DayDateProperty, value);
    }

    public static readonly BindableProperty StripeProperty =
        BindableProperty.Create(nameof(Stripe), typeof(MuhurtaOverviewStripe), typeof(MuhurtaBar),
            default(MuhurtaOverviewStripe), propertyChanged: (b, o, n) =>
            {
                var bar = (MuhurtaBar)b;
                bar._drawable.Stripe = (MuhurtaOverviewStripe?)n;
                bar._graphics.Invalidate();
            });

    public MuhurtaOverviewStripe? Stripe
    {
        get => (MuhurtaOverviewStripe?)GetValue(StripeProperty);
        set => SetValue(StripeProperty, value);
    }
}
