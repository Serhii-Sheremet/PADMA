using PADMA.Core.Models;
using PADMA.Core.Services;
using System.Linq;

namespace PADMA.Core.Utilities
{
    public static class Localization
    {
        public static string Get(string nativeText, string langCode = "en")
        {
            var list = DataCache.Instance.AppTextsList;
            var match = list.FirstOrDefault(x => x.NativeText == nativeText && x.LanguageCode == langCode);
            return match?.ForeignText ?? nativeText;
        }
    }
}

