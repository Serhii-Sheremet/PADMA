using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages;

public partial class PrivacyPolicyPage : ContentPage
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

    public PrivacyPolicyPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        ApplyLocalization();
        await LoadPrivacyPolicyAsync();
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Privacy Policy", lang);
    }

    private async Task LoadPrivacyPolicyAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("privacy_policy.html");
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            PrivacyPolicyWebView.Source = new HtmlWebViewSource
            {
                Html = html
            };
        }
        catch (Exception ex)
        {
            await DisplayAlert("Privacy Policy", ex.Message, "OK");
        }
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
            await Shell.Current.GoToAsync("..");
            await Task.Yield();
        });
    }

    private async Task RunBusyAsync(string text, Func<Task> action)
    {
        BusyText = text;
        IsBusy = true;

        await Task.Yield();

        try { await action(); }
        finally { IsBusy = false; }
    }
}
