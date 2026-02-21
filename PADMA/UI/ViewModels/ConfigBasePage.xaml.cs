using Microsoft.Maui.Controls;
using PADMA.Core.Utilities;
using System.Threading.Tasks;
using PADMA.Core.Services;

namespace PADMA.UI.Templates
{
    public partial class ConfigBasePage : ContentPage
    {
        protected bool IsClosingByButton = false;

        public ConfigBasePage()
        {
            InitializeComponent();
            //this.BackgroundImageSource = "background.png";
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
