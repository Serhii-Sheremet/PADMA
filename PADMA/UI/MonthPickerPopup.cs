using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Views;
using PADMA.Core.Utilities;
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

        var btnCancel = new Button { Text = "Cancel" };
        btnCancel.Clicked += async (_, __) => await CloseAsync(null);

        var btnOk = new Button { Text = "OK" }; // Localization.GetLocalizedText("OK", lang)
        btnOk.Clicked += async (_, __) =>
        {
            // 1) если плагин всё-таки ведёт SelectedDates — берём оттуда
            DateTime dt = default;
            if (_cal.SelectedDates != null && _cal.SelectedDates.Count > 0)
                dt = _cal.SelectedDates[_cal.SelectedDates.Count - 1];

            // 2) fallback: берём текущие Year/Month/Day из контрола
            if (dt == default)
                dt = new DateTime(_cal.Year, _cal.Month, _cal.Day);

            await CloseAsync(dt);
        };

        Content = new Frame
        {
            Padding = 12,
            CornerRadius = 16,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    _cal,
                    new HorizontalStackLayout
                    {
                        HorizontalOptions = LayoutOptions.End,
                        Spacing = 8,
                        Children = { btnCancel, btnOk }
                    }
                }
            }
        };
    }
}
