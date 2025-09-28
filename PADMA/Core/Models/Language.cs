// Core/Models/Language.cs
namespace PADMA.Core.Models
{
    public sealed class Language
    {
        public int Id { get; set; }
        public string LanguageCode { get; set; } = ""; // e.g. "en"
        public string CultureCode  { get; set; } = ""; // e.g. "en-US"
    }

    public sealed class LanguageDesc
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }      // LANGUAGE.ID
        public string Name { get; set; } = "";
        public string LanguageCode { get; set; } = ""; // язык локализации этой строки
    }
}
