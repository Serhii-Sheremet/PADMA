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
        Title = $"{Localization.GetLocalizedText("About application", lang)} PADMA";

        appNameLabel.Text = Localization.GetLocalizedText("Personal Astrological Diary", lang);
        appSubNameLabel.Text = $"({Localization.GetLocalizedText("Mobile Application", lang)})";
        
        appDesclabel.Text = Localization.GetLocalizedText("Application for selecting favorable timing for new beginnings.", lang);

        ideaLabel.Text = Localization.GetLocalizedText("Idea", lang);
        ideaValueLabel.Text = Localization.GetLocalizedText("Halyna Sheremet", lang);
        devLabel.Text = Localization.GetLocalizedText("Development", lang);
        devValueLabel.Text = Localization.GetLocalizedText("Serhii Sheremet", lang);

        lblContactsLabel.Text = Localization.GetLocalizedText("Contacts", lang);
        btnSendFeedback.Text = Localization.GetLocalizedText("Send feedback", lang);


        lblQuoteLabel.Text = Localization.GetLocalizedText("Created with love for practical Jyotish work.", lang);
        versionLabel.Text = $"{Localization.GetLocalizedText("Version", lang)}: {AppInfo.VersionString}";
    }

    private async void OnSendFeedbackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(FeedbackPage), true);
    }



}