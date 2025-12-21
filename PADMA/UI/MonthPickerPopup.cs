using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Views;
using PADMA.Core.Utilities;
using PADMA.Core.Services;
using Microsoft.Maui.Controls;

using CalendarControl = Plugin.Maui.Calendar.Controls.Calendar;

public sealed class MonthPickerPopup : Popup<DateTime?>
{
    private readonly CalendarControl _cal;
    private DateTime? _selected;

    public MonthPickerPopup(CultureInfo? culture, int year, int month)
    {
        culture ??= CultureInfo.CurrentCulture;

        _cal = new CalendarControl
        {
            Culture = culture,
            Year = year,
            Month = month
        };

        _cal.EventsScrollViewVisible = false;   // <-- вот это чаще всего убирает “↑”
        _cal.SwipeUpToHideEnabled = false;      // можно оставить как дополнительную защиту
        _cal.FooterSectionTemplate = new DataTemplate(() => new ContentView { IsVisible = false });

        _cal.DaysTitleLabelStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 11d }, // под Android обычно отлично
                new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.NoWrap },
                new Setter { Property = Label.MaxLinesProperty, Value = 1 },
                new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
            }
        };

        _cal.WeekendTitleStyle = new Style(typeof(Label))
        {
            Setters =
            {
                new Setter { Property = Label.FontSizeProperty, Value = 11d },
                new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.NoWrap },
                new Setter { Property = Label.MaxLinesProperty, Value = 1 },
                new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
            }
        };

        _cal.FirstDayOfWeek = DataCache.Instance.DayOfWeek;

        // гарантируем не-null
        _cal.SelectedDates ??= new ObservableCollection<DateTime>();

        // стартовая “выбранная”
        _cal.Year = year;
        _cal.Month = month;
        _cal.Day = 1; // важно
        _selected = new DateTime(year, month, 1);
        _cal.SelectedDates.Clear();
        _cal.SelectedDates.Add(_selected.Value);

        // Попробуем ловить изменения коллекции (если сработает — супер)
        _cal.SelectedDates.CollectionChanged += (_, __) =>
        {
            var dt = _cal.SelectedDates.LastOrDefault();
            if (dt != default) _selected = dt;
        };

        var btnCancel = new Button { Text = Localization.GetLocalizedText("Cancel", DataCache.Instance.CurrentLanguageCode) };
        btnCancel.Clicked += async (_, __) => await CloseAsync(null);

        var btnToday = new Button { Text = Localization.GetLocalizedText("Today", DataCache.Instance.CurrentLanguageCode) };
        btnToday.Clicked += async (_, __) =>
        {
            await CloseAsync(DateTime.Today);
        };

        var btnOk = new Button { Text = "OK" };
        btnOk.Clicked += async (_, __) =>
        {
            // День нам не важен — используем всегда 1
            var dt = new DateTime(_cal.Year, _cal.Month, 1);
            await CloseAsync(dt);
        };

        static void ApplyWideFooterButtonStyle(Button b)
        {
            b.FontSize = 12; 
            b.LineBreakMode = LineBreakMode.WordWrap; 
            b.HorizontalOptions = LayoutOptions.Fill;
            b.VerticalOptions = LayoutOptions.Center;
            b.MinimumHeightRequest = 32;

            // часто помогает от "уродского" переноса:
            b.Padding = new Thickness(10, 6); // чуть компактнее
        }

        static void ApplyOkFooterButtonStyle(Button b)
        {
            b.FontSize = 12;
            b.LineBreakMode = LineBreakMode.NoWrap;
            b.HorizontalOptions = LayoutOptions.End;   // прижать вправо
            b.VerticalOptions = LayoutOptions.Center;
            b.MinimumHeightRequest = 32;
            b.Padding = new Thickness(14, 6);          // можно чуть шире по бокам
        }

        ApplyWideFooterButtonStyle(btnCancel);
        ApplyWideFooterButtonStyle(btnToday);
        ApplyOkFooterButtonStyle(btnOk);

        var footer = new Grid
        {
            ColumnSpacing = 8,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star), // Cancel
                new ColumnDefinition(GridLength.Star), // Today
                new ColumnDefinition(GridLength.Auto), // OK
            },
            HorizontalOptions = LayoutOptions.Fill
        };

        footer.Add(btnCancel, 0, 0);
        footer.Add(btnToday, 1, 0);
        footer.Add(btnOk, 2, 0);


        Content = new Frame
        {
            Padding = 8,
            CornerRadius = 16,
            Content = new VerticalStackLayout
            {
                Spacing = 4,
                Children =
                {
                    _cal,
                    footer
                }
            }
        };


        


    }
}
