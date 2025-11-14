using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describes Masa entity (12 entities)
    /// </summary>
    [Table("MASA")]
    public class Masa
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("ZODIACID")]
        public int ZodiacId { get; set; }
        [Column("SHUNYANAKSHATRA")]
        public string ShunyaNakshatra { get; set; } = string.Empty;
        [Column("SHUNYATITHI")]
        public string ShunyaTithi { get; set; } = string.Empty;
        
        public int[] ShunyaNakshatraIdArray { get; set; } = new int[0];
        
        public int[] ShunyaTithiIdArray { get; set; } = new int[0];
    }

    [Table("MASA_DESC")]
    public class MasaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("MASAID")]
        public int MasaId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
