using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describes Muhurta entity (5 entities)
    /// </summary>
    [Table("MUHURTA")]
    public class Muhurta
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("MUHURTACODE")]
        public string MuhurtaCode { get; set; } = string.Empty;
        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("MUHURTA_DESC")]
    public class MuhurtaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("MUHURTAID")]
        public int MuhurtaId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("SHORTNAME")]
        public string ShortName { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
