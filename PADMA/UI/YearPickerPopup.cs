using CommunityToolkit.Maui.Views;
using PADMA.Core.Utilities;
using PADMA.Core.Services;

public sealed class YearPickerPopup : Popup<int?>
{
    private int MinYear = DateTime.MinValue.Year;
    private int MaxYear = DateTime.MaxValue.Year;

    private int _selectedYear;
    private readonly Label _yearLabel;
    private readonly Button _previousButton;
    private readonly Button _nextButton;

    public YearPickerPopup(int initialYear)
    {
        _selectedYear = Math.Clamp(initialYear, MinYear, MaxYear);
        Size size = new Size(250, 150);

        _yearLabel = new Label
        {
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap,
            WidthRequest = 90,
            MinimumWidthRequest = 90
        };

        _previousButton = new Button
        {
            Text = "‹",
            FontSize = 26,
            Padding = new Thickness(8, 0),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 48,
            HeightRequest = 44,
            MinimumWidthRequest = 48,
            MinimumHeightRequest = 44
        };
        _previousButton.Clicked += (_, _) => ChangeYear(-1);

        _nextButton = new Button
        {
            Text = "›",
            FontSize = 26,
            Padding = new Thickness(8, 0),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 48,
            HeightRequest = 44,
            MinimumWidthRequest = 48,
            MinimumHeightRequest = 44
        };
        _nextButton.Clicked += (_, _) => ChangeYear(1);

        var btnCancel = new Button
        {
            Text = Localization.GetLocalizedText(
                "Cancel",
                DataCache.Instance.CurrentLanguageCode),
            FontSize = 12,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 86,
            MinimumHeightRequest = 36,
            Padding = new Thickness(10, 4),
            BackgroundColor = Colors.White,
            TextColor = Colors.DarkSlateGray,
            BorderColor = Colors.MediumPurple,
            BorderWidth = 1,
            CornerRadius = 8
        };
        btnCancel.Clicked += async (_, _) => await CloseAsync(null);

        var btnOk = new Button
        {
            Text = "OK",
            FontSize = 12,
            WidthRequest = 64,
            MinimumHeightRequest = 36,
            Padding = new Thickness(10, 4),
            CornerRadius = 8
        };
        btnOk.Clicked += async (_, _) => await CloseAsync(_selectedYear);

        var yearSelector = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(48)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(48))
            },
            ColumnSpacing = 18,
            Padding = new Thickness(4, 2, 4, 0)
        };

        yearSelector.Add(_previousButton, 0, 0);
        yearSelector.Add(_yearLabel, 1, 0);
        yearSelector.Add(_nextButton, 2, 0);

        var footer = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(104)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(64))
            },
            ColumnSpacing = 8,
            Padding = new Thickness(4, 0, 4, 2)
        };

        footer.Add(btnCancel, 0, 0);
        footer.Add(btnOk, 2, 0);

        Content = new Frame
        {
            Padding = 8,
            CornerRadius = 14,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Padding = 0,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    yearSelector,
                    footer
                }
            }
        };

        RefreshUi();
    }

    private void ChangeYear(int delta)
    {
        var newYear = Math.Clamp(_selectedYear + delta, MinYear, MaxYear);

        if (newYear == _selectedYear)
            return;

        _selectedYear = newYear;
        RefreshUi();
    }

    private void RefreshUi()
    {
        _yearLabel.Text = _selectedYear.ToString();

        _previousButton.IsEnabled = _selectedYear > MinYear;
        _nextButton.IsEnabled = _selectedYear < MaxYear;
    }
}