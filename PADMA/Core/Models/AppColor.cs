using SQLite;

namespace PADMA.Core.Models
{
    [Table("COLOR")]
    public class AppColor
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("CODE")]
        public string Code { get; set; } = string.Empty;   // e.g. "RED"
        [Column("ARGBVALUE")]
        public int ArgbValue { get; set; }       // e.g. -45233
    }

    [Table("COLOR_DESC")]
    public class AppColorDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
