namespace PADMA.Core.Models
{
    public sealed class AppColor
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";   // e.g. "RED_PRIMARY"
        public int ArgbValue { get; set; }       // e.g. 0xFF112233 (AARRGGBB)
    }

    public sealed class ColorDesc
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public string Name { get; set; } = "";
        public string LanguageCode { get; set; } = "";
    }
}
