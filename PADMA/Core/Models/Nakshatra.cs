using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Represents a single Nakshatra entry (total 27).
    /// </summary>
    [Table("NAKSHATRA")]
    public class Nakshatra
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }

        [Column("NAKSHATRACODE")]
        public string NakshatraCode { get; set; } = string.Empty;

        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("NAKSHATRA_DESC")]
    public class NakshatraDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("NAKSHATRAID")]
        public int NakshatraId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("SHORTNAME")]
        public string ShortName { get; set; } = string.Empty;
        [Column("UPRAVITEL")]
        public string Upravitel { get; set; } = string.Empty;
        [Column("NATURE")]
        public string Nature { get; set; } = string.Empty;
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
        [Column("GOODFOR")]
        public string GoodFor { get; set; } = string.Empty;
        [Column("BADFOR")]
        public string BadFor { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }

}
