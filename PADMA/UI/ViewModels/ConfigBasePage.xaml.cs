using PADMA.Core.Utilities;
using PADMA.Core.Services;

namespace PADMA.UI.Templates
{
    public partial class ConfigBasePage : ContentPage
    {
        protected bool IsClosingByButton = false;

        public Grid OverlayHostGrid => OverlayHost;

        public static readonly BindableProperty IsCompactProperty =
        BindableProperty.Create(nameof(IsCompact), typeof(bool), typeof(ConfigBasePage), false,
            propertyChanged: (b, o, n) => ((ConfigBasePage)b).ApplyLayoutMode());

        public bool IsCompact
        {
            get => (bool)GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        public static readonly BindableProperty ShowTitleProperty =
        BindableProperty.Create(nameof(ShowTitle), typeof(bool), typeof(ConfigBasePage), true,
            propertyChanged: (b, o, n) => ((ConfigBasePage)b).TitleLabel.IsVisible = (bool)n);

        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        public ConfigBasePage()
        {
            InitializeComponent();
            ApplyLayoutMode();
            //this.BackgroundImageSource = "background.png";
        }

        private void ApplyLayoutMode()
        {
            if (RootStack == null) return;

            if (IsCompact)
            {
                Padding = new Thickness(0);
                TitleLabel.Margin = new Thickness(0, 0, 0, 2);
                RootStack.Spacing = 6; 
                RootStack.Padding = new Thickness(12, 2, 12, 10);
                TitleLabel.HorizontalOptions = LayoutOptions.Start;
            }
            else
            {
                Padding = new Thickness(10);
                RootStack.Padding = new Thickness(20);
                RootStack.Spacing = 20;
                TitleLabel.Margin = new Thickness(10, 10, 0, 10);
                TitleLabel.HorizontalOptions = LayoutOptions.Center;
            }
        }

        protected virtual async void OnCloseClicked(object sender, EventArgs e)
        {
            IsClosingByButton = true;
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// ”ниверсальный метод диалога подтверждени€ изменений
        /// </summary>
        /// <param name="titleKey"> люч дл€ заголовка</param>
        /// <param name="messageKey"> люч дл€ сообщени€</param>
        /// <returns>True Ч сохранить, False Ч не сохран€ть</returns>
        protected async Task<bool> TrySaveChangesAsync(string titleKey, string messageKey)
        {
            string titleText = Localization.GetLocalizedText(titleKey, DataCache.Instance.CurrentLanguageCode);
            string messageText = Localization.GetLocalizedText(messageKey, DataCache.Instance.CurrentLanguageCode);
            string yesText = Localization.GetLocalizedText("Yes", DataCache.Instance.CurrentLanguageCode);
            string noText = Localization.GetLocalizedText("No", DataCache.Instance.CurrentLanguageCode);

            return await DisplayAlert(titleText, messageText, yesText, noText);
        }

    }
}
