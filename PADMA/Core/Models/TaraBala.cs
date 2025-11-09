using SQLite;

namespace PADMA.Core.Models
{
    [Table("TARABALA")]
    public class TaraBala
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("TARABALA_DESC")]
    public class TaraBalaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("TARABALAID")]
        public int TaraBalaId { get; set; }
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
