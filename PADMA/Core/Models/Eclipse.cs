using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describing Eclipse entity (1 = Moon, 2 = Sun)
    /// </summary>
    [Table("ECLIPSE")]
    public class Eclipse
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("ECLIPSECODE")]
        public string EclipseCode { get; set; } = string.Empty;
    }

    [Table("ECLIPSE_DESC")]
    public class EclipseDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("ECLIPSEID")]
        public int EclipseId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
