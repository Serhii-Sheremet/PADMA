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

        _yearLabel = new Label
        {
            FontSize = 26,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            MinimumWidthRequest = 110
        };

        _previousButton = new Button
        {
            Text = "‹",
            FontSize = 30,
            Padding = new Thickness(12, 2),
            MinimumWidthRequest = 52
        };
        _previousButton.Clicked += (_, _) => ChangeYear(-1);

        _nextButton = new Button
        {
            Text = "›",
            FontSize = 30,
            Padding = new Thickness(12, 2),
            MinimumWidthRequest = 52
        };
        _nextButton.Clicked += (_, _) => ChangeYear(1);

        var btnCancel = new Button
        {
            Text = Localization.GetLocalizedText(
                "Cancel",
                DataCache.Instance.CurrentLanguageCode),
            FontSize = 12,
            MinimumHeightRequest = 32,
            Padding = new Thickness(12, 6)
        };
        btnCancel.Clicked += async (_, _) => await CloseAsync(null);

        var btnOk = new Button
        {
            Text = "OK",
            FontSize = 12,
            MinimumHeightRequest = 32,
            Padding = new Thickness(14, 6)
        };
        btnOk.Clicked += async (_, _) => await CloseAsync(_selectedYear);

        var yearSelector = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 10,
            Padding = new Thickness(4, 8)
        };

        yearSelector.Add(_previousButton, 0, 0);
        yearSelector.Add(_yearLabel, 1, 0);
        yearSelector.Add(_nextButton, 2, 0);

        var footer = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            Padding = new Thickness(4, 0, 4, 2)
        };

        footer.Add(btnCancel, 0, 0);
        footer.Add(btnOk, 1, 0);

        Content = new Frame
        {
            Padding = 10,
            CornerRadius = 16,
            Content = new VerticalStackLayout
            {
                Spacing = 8,
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