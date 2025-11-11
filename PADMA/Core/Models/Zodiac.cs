using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describing Zodiac entity (12 entities)
    /// </summary>
    [Table("ZODIAK")]
    public class Zodiac
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("ZODIAKCODE")]
        public string ZodiacCode { get; set; } = string.Empty;
    }

    [Table("ZODIAK_DESC")]
    public class ZodiacDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("ZODIAKID")]
        public int ZodiacId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
