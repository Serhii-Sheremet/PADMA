using SQLite;

namespace PADMA.Core.Models
{
    [Table("SPECIALNAVAMSA_DESC")]
    public class SpecialNavamsaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("SPECIALNAVAMSAID")]
        public int SpecialNavamsaId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }

}
