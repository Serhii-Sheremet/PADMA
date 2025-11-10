using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describes Ghati60 entity (60 entities)
    /// </summary>
    [Table("GHATI60")]
    public class Ghati60
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("GHATI60CODE")]
        public string Ghati60Code { get; set; } = string.Empty;
        [Column("POSITION")]
        public int Position { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
    }
    
    [Table("GHATI60_DESC")]
    public class Ghati60Desc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("GHATI60ID")]
        public int Ghati60Id { get; set; }
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
