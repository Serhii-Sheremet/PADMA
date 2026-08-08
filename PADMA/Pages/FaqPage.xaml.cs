using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages;

public partial class FaqPage : ContentPage
{
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    private string _busyText = "Please wait…";
    public string BusyText
    {
        get => _busyText;
        set
        {
            if (_busyText == value) return;
            _busyText = value;
            OnPropertyChanged(nameof(BusyText));
        }
    }

    public FaqPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var lang = DataCache.Instance.CurrentLanguageCode;
        MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
        MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
        {
            lang = DataCache.Instance.CurrentLanguageCode;
            ApplyLocalization();

        });
        ApplyLocalization();
        
        
        string faqFile = lang switch
        {
            "uk" => "faq_uk.html",
            "pl" => "faq_pl.html",
            "ru" => "faq_ru.html",
            _ => "faq_en.html"
        };

        LoadFaqAsync(faqFile);
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Frequently Asked Questions", lang);
    }

    private async void LoadFaqAsync(string faqFile)
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(faqFile);
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            FaqWebView.Source = new HtmlWebViewSource
            {
                Html = html
            };
        }
        catch (Exception ex)
        {
            await DisplayAlert("FAQ", ex.Message, "OK");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await CloseAsync();
        });
        return true;
    }

    private async Task CloseAsync()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        await RunBusyAsync(Localization.GetLocalizedText("Please wait…", lang), async () =>
        {
            await Shell.Current.GoToAsync("//main");
            await Task.Yield();
        });
    }

    private async Task RunBusyAsync(string text, Func<Task> action)
    {
        BusyText = text;
        IsBusy = true;

        await Task.Yield(); // give the UI time to show the overlay

        try { await action(); }
        finally { IsBusy = false; }
    }

}