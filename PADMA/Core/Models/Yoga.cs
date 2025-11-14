using SQLite;

namespace PADMA.Core.Models
{
    [Table("YOGA")]
    public class Yoga
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("YOGACODE")]
        public string YogaCode { get; set; } = string.Empty;
        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("YOGA_DESC")]
    public class YogaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("YOGAID")]
        public int YogaId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("SHORTNAME")]
        public string ShortName { get; set; } = string.Empty;
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
