using Microsoft.Maui.Layouts;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System.Globalization;

namespace PADMA.Pages;

public partial class FeedbackPage : ContentPage
{
    private bool _isCategoryExpanded;
    private string _selectedCategoryKey = "Bug report";
    private readonly IFeedbackService _feedbackService = new EmailJsFeedbackService();
    private bool _isSending;

    public FeedbackPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
        return true;
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

        RefreshCategoryUnderline();
        Dispatcher.Dispatch(RefreshCategoryUnderline);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<object>(this, "SettingsChanged");
    }

    private void ApplyLocalization()
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        Title = Localization.GetLocalizedText("Send feedback", lang);
        pageDescLabel.Text = Localization.GetLocalizedText(
            "Use this form to send a question, suggestion, bug report or feedback.",
            lang);

        categoryLabel.Text = Localization.GetLocalizedText("Category", lang);
        messageLabel.Text = Localization.GetLocalizedText("Message", lang);
        emailLabel.Text = Localization.GetLocalizedText("Your email", lang);

        messageEditor.Placeholder = $"{Localization.GetLocalizedText("Write your message here", lang)}...";
        emailEntry.Placeholder = Localization.GetLocalizedText("Email is optional", lang);

        sendButton.Text = Localization.GetLocalizedText("Send", lang);

        CategoryBugLabel.Text = Localization.GetLocalizedText("Bug report", lang);
        CategorySuggestionLabel.Text = Localization.GetLocalizedText("Suggestion", lang);
        CategoryQuestionLabel.Text = Localization.GetLocalizedText("Question", lang);
        CategoryOtherLabel.Text = Localization.GetLocalizedText("Other", lang);

        CategoryValueLabel.Text = Localization.GetLocalizedText(_selectedCategoryKey, lang);
        RefreshCategoryUnderline();
    }

    private void OnCategoryTapped(object sender, TappedEventArgs e)
    {
        _isCategoryExpanded = !_isCategoryExpanded;

        if (_isCategoryExpanded)
        {
            PositionCategoryPanel();
            CategoryPanel.IsVisible = true;
        }
        else
        {
            CategoryPanel.IsVisible = false;
        }

        CategoryArrow.Text = _isCategoryExpanded ? "▲" : "▼";
    }

    private void SetCategory(string key)
    {
        _selectedCategoryKey = key;

        string lang = DataCache.Instance.CurrentLanguageCode;
        CategoryValueLabel.Text = Localization.GetLocalizedText(key, lang);

        _isCategoryExpanded = false;
        CategoryPanel.IsVisible = false;
        CategoryArrow.Text = "▼";

        RefreshCategoryUnderline();
    }

    private void OnCategoryBugTapped(object sender, TappedEventArgs e) => SetCategory("Bug report");
    private void OnCategorySuggestionTapped(object sender, TappedEventArgs e) => SetCategory("Suggestion");
    private void OnCategoryQuestionTapped(object sender, TappedEventArgs e) => SetCategory("Question");
    private void OnCategoryOtherTapped(object sender, TappedEventArgs e) => SetCategory("Other");

    private Point GetAbsolutePositionToRoot(VisualElement element, VisualElement root)
    {
        double x = element.X;
        double y = element.Y;

        Element? parent = element.Parent;
        while (parent is VisualElement ve && parent != root)
        {
            x += ve.X;
            y += ve.Y;
            parent = ve.Parent;
        }

        return new Point(x, y);
    }

    private void PositionCategoryPanel()
    {
        if (CategoryHeader == null || PageOverlayRoot == null)
            return;

        var p = GetAbsolutePositionToRoot(CategoryHeader, PageOverlayRoot);

        double panelX = p.X + 2;
        double panelY = p.Y + CategoryHeader.Height + 2;

        AbsoluteLayout.SetLayoutBounds(CategoryPanel, new Rect(panelX, panelY, 170, -1));
        AbsoluteLayout.SetLayoutFlags(CategoryPanel, AbsoluteLayoutFlags.None);
    }

    private void RefreshCategoryUnderline()
    {
        Dispatcher.Dispatch(() =>
        {
            CategoryValueLabel.Measure(double.PositiveInfinity, double.PositiveInfinity);
            CategoryUnderline.WidthRequest = Math.Max(24, CategoryValueLabel.DesiredSize.Width);
        });
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        if (_isSending)
            return;

        string lang = DataCache.Instance.CurrentLanguageCode;

        if (string.IsNullOrWhiteSpace(messageEditor.Text))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Message is required.", lang),
                "OK");
            return;
        }

        string? email = emailEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(email) && !IsValidEmail(email))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Please enter a valid email address.", lang),
                "OK");
            return;
        }

        var feedback = new FeedbackMessage
        {
            Category = Localization.GetLocalizedText(_selectedCategoryKey, lang),
            Message = messageEditor.Text.Trim(),
            Email = email,
            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            Language = lang,
            AppVersion = AppInfo.Current.VersionString
        };

        try
        {
            _isSending = true;
            sendButton.IsEnabled = false;
            sendButton.Text = $"{Localization.GetLocalizedText("Sending", lang)}...";

            bool success = await _feedbackService.SendAsync(feedback);

            if (!success)
            {
                await DisplayAlert(
                    Localization.GetLocalizedText("Error", lang),
                    Localization.GetLocalizedText("Failed to send feedback. Please try again.", lang),
                    "OK");
                return;
            }

            await DisplayAlert(
                Localization.GetLocalizedText("Feedback", lang),
                Localization.GetLocalizedText("Thank you for your feedback.", lang),
                "OK");

            messageEditor.Text = string.Empty;
            emailEntry.Text = string.Empty;

            _selectedCategoryKey = "Bug report";
            CategoryValueLabel.Text = Localization.GetLocalizedText(_selectedCategoryKey, lang);
            RefreshCategoryUnderline();

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _isSending = false;
            sendButton.IsEnabled = true;
            sendButton.Text = Localization.GetLocalizedText("Send", lang);
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void OnPageTapped(object sender, TappedEventArgs e)
    {
        KeyboardHelper.HideKeyboard();
        messageEditor?.Unfocus();
        emailEntry?.Unfocus();
    }


}