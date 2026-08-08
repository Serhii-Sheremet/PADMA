using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages;

public partial class PaymentPage : ContentPage
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

    public PaymentPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
        MessagingCenter.Subscribe<object>(this, "SettingsChanged", _ =>
        {
            ApplyLocalization();
        });

        ApplyLocalization();
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

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = $"{Localization.GetLocalizedText("Support", lang)} PADma";

        lblDesclabel.Text = $"PADma {Localization.GetLocalizedText("is an independent project.", lang)}";
        lblSubDesclabel.Text = Localization.GetLocalizedText("If you find it useful, you can support its development.", lang);
        btnPayPalSupport.Text = $"{Localization.GetLocalizedText("Support via", lang)} PayPal";
        btnKoFiSupport.Text = $"{Localization.GetLocalizedText("Support via", lang)} Ko-fi";
        lblQuoteLabel.Text = Localization.GetLocalizedText("Support is completely voluntary.", lang);
    }

    private async void OnPayPalSupportClicked(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync("https://paypal.me/ssheremet71");
    }

    private async void OnKoFiSupportClicked(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync("https://ko-fi.com/ssheremet");
    }

    private async Task CloseAsync()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        await RunBusyAsync(Localization.GetLocalizedText("Please wait…", lang), async () =>
        {
            await Shell.Current.GoToAsync("..");
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