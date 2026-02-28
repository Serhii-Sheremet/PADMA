using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.UI.Templates;

namespace PADMA.Pages
{
	public partial class ColorSettingsPage : ConfigBasePage
	{
		public ColorSettingsPage()
		{
			InitializeComponent();
            ApplyLocalization();

        }

        private void ApplyLocalization()
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            Title = Localization.GetLocalizedText("Color settings", lang);

        }





    }
}