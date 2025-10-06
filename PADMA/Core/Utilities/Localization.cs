using PADMA.Core.Models;
using PADMA.Core.Services;
using System.Linq;

namespace PADMA.Core.Utilities
{
    public static class Localization
    {
        public static string GetLocalizedText(string nativeText, string langCode)
        {
            var appTextsList = DataCache.Instance.AppTextsList;
            if (appTextsList == null || appTextsList.Count == 0)
                return nativeText;

            var translation = appTextsList
                .FirstOrDefault(x => x.NativeText.Equals(nativeText, StringComparison.OrdinalIgnoreCase)
                                  && x.LanguageCode.Equals(langCode, StringComparison.OrdinalIgnoreCase));

            return translation?.ForeignText ?? nativeText;
        }
    }
}

