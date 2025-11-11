using SQLite;

namespace PADMA.Core.Models
{
    [Table("TRANSIT")]
    public class Transit
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("PLANETID")]
        public int PlanetId { get; set; }
        [Column("HOUSE")]
        public int House { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
        [Column("VEDHA")]
        public string Vedha { get; set; } = string.Empty;
    }

    [Table("TRANSIT_DESC")]
    public class TransitDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("TRANSITID")]
        public int TransitId { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
