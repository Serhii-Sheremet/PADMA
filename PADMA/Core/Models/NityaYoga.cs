using SQLite;

namespace PADMA.Core.Models
{

    /// <summary>
    /// Represents a single Nitya Yoga entry (total 27).
    /// NITYAYOGA_DESC: NITYAYOGAID, NAME, LANGUAGECODE, etc
    /// </summary>
    [Table("NITYAYOGA")]
    public class NityaYoga
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("CODE")]
        public string Code { get; set; } = string.Empty;
        [Column("COLORID")]
        public int ColorId { get; set; }
        [Column("NAKSHATRAID")]
        public int NakshatraId { get; set; }
        [Column("YOGIPLANETID")]
        public int YogiPlanetId { get; set; }
        [Column("AVAYOGIPLANETID")]
        public int AvaYogiPlanetId { get; set; }
    }

    [Table("NITYAYOGA_DESC")]
    public class NityaYogaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("NITYAYOGAID")]
        public int NityaYogaId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("DEITY")]
        public string Deity { get; set; } = string.Empty;
        [Column("MEANING")]
        public string Meaning { get; set; } = string.Empty;
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
