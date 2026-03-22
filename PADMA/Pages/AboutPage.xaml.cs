using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages;

public partial class AboutPage : ContentPage
{
    public AboutPage()
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

        var startYear = 2017;
        var currentYear = DateTime.Now.Year;

        lblCopyright.Text =
            currentYear > startYear
                ? $"© {startYear}–{currentYear} PADma"
                : $"© {startYear} PADma";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main", true);
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await Shell.Current.GoToAsync("//main");
        });
        return true;
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = $"{Localization.GetLocalizedText("About application", lang)} PADma";

        appNameLabel.Text = Localization.GetLocalizedText("Personal Astrological Diary", lang);
        appSubNameLabel.Text = $"{Localization.GetLocalizedText("mobile application", lang)}";
        
        appDesclabel.Text = Localization.GetLocalizedText("Application for selecting favorable timing for new beginnings.", lang);

        ideaLabel.Text = Localization.GetLocalizedText("Idea", lang);
        ideaValueLabel.Text = Localization.GetLocalizedText("Halyna Sheremet", lang);
        devLabel.Text = Localization.GetLocalizedText("Development", lang);
        devValueLabel.Text = Localization.GetLocalizedText("Serhii Sheremet", lang);

        lblContactsLabel.Text = Localization.GetLocalizedText("Contact us", lang);
        btnSendFeedback.Text = Localization.GetLocalizedText("Send feedback", lang);

        lblSupportLabel.Text = $"{Localization.GetLocalizedText("Support", lang)} PADma";
        btnSupport.Text = Localization.GetLocalizedText("Support the project", lang);
        lblSubSupportLabel.Text = Localization.GetLocalizedText("Voluntary support", lang);

        lblRights.Text = Localization.GetLocalizedText("Licensed under MIT License", lang);

        lblPrivacyPolicyLabel.Text = Localization.GetLocalizedText("Privacy Policy", lang);
        btnPrivcyPolicy.Text = Localization.GetLocalizedText("Privacy Policy", lang);

        lblQuoteLabel.Text = Localization.GetLocalizedText("Created with love for practical Jyotish work.", lang);
        versionLabel.Text = $"{Localization.GetLocalizedText("Version", lang)}: {AppInfo.VersionString}";
    }

    private async void OnSendFeedbackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(FeedbackPage), true);
    }

    private async void OnSupportClicked(object sender, EventArgs e)
    {
        await Browser.Default.OpenAsync("https://paypal.me/ssheremet71");
    }

    private async void OnPrivacyPolicyClicked(object sender, EventArgs e)
    {
        var url = "https://serhii-sheremet.github.io/PADMA/privacy-policy.html"; 

        try
        {
            await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

}