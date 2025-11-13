using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describing Tithi entity (30 entities)
    /// </summary>
    [Table("TITHI")]
    public class Tithi
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("TITHI_DESC")]
    public class TithiDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("TITHIID")]
        public int TithiId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("SHORTNAME")]
        public string ShortName { get; set; } = string.Empty;
        [Column("RULER")]
        public string Ruler { get; set; } = string.Empty;
        [Column("TYPE")]
        public string Type { get; set; } = string.Empty;
        [Column("GOODFOR")]
        public string GoodFor { get; set; } = string.Empty;
        [Column("BADFOR")]
        public string BadFor { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
