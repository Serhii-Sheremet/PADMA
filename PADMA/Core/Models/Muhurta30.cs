using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describes Muhurta30 entity (30 entities)
    /// </summary>
    [Table("MUHURTA30")]
    public class Muhurta30
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("MUHURTA30CODE")]
        public string Muhurta30Code { get; set; } = string.Empty;
        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("MUHURTA30_DESC")]
    public class Muhurta30Desc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("MUHURTA30ID")]
        public int Muhurta30Id { get; set; }
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
