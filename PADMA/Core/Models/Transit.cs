using SQLite;

namespace PADMA.Core.Models
{
    [Table("TRANZIT")]
    public class Transit
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("PLANETID")]
        public int PlanetId { get; set; }
        [Column("DOM")]
        public int Dom { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
        [Column("VEDHA")]
        public string Vedha { get; set; } = string.Empty;
    }

    [Table("TRANZIT_DESC")]
    public class TransitDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("TRANZITID")]
        public int TransitId { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
