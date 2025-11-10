using SQLite;

namespace PADMA.Core.Models
{
    [Table("LANGUAGE")]
    public class Language
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty; // e.g. "en"
        [Column("CULTURECODE")]
        public string CultureCode  { get; set; } = string.Empty; // e.g. "en-US"
    }

    [Table("LANGUAGE_DESC")]
    public class LanguageDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("LANGUAGEID")]
        public int LanguageId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty; 
    }
}
