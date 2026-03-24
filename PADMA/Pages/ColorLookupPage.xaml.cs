using Microsoft.Maui.Controls;
using Syncfusion.Maui.Inputs;
using PADMA.Core.Utilities;
using PADMA.Core.Services;

namespace PADMA.Pages;

[QueryProperty(nameof(StartArgb), "StartArgb")]
public partial class ColorLookupPage : ContentPage
{
    private int _startArgb;
    private int _selectedArgb;

    public string StartArgb
    {
        // Shell передаёт query как string, парсим
        get => _startArgb.ToString();
        set
        {
            if (int.TryParse(value, out var v))
                _startArgb = v;
        }
    }

    public ColorLookupPage()
    {
        InitializeComponent();
        BindingContext = this;
        ApplyLocalization();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Dispatcher.Dispatch(() =>
        {
            var c = CalendarDrawingHelper.ColorFromArgbInt(_startArgb);
            picker.SelectedColor = c;
            SelectedColorPreview.BackgroundColor = c;
            _selectedArgb = _startArgb;
        });
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Select a color", lang);
        SelectButon.Text = Localization.GetLocalizedText("Select", lang);
    }

    private void OnColorChanged(object? sender, ColorChangedEventArgs e)
    {
        _selectedArgb = CalendarDrawingHelper.ColorToArgbInt(e.NewColor);
        if (e.NewColor is not null)
        {
            SelectedColorPreview.BackgroundColor = e.NewColor; 
        }
    }

    private async void OnSelectClicked(object sender, EventArgs e)
    {
        var argb = _selectedArgb == 0 ? _startArgb : _selectedArgb;

        MessagingCenter.Send(this, "ColorSelected", (argb));

        await Shell.Current.GoToAsync("..", true);
    }


}