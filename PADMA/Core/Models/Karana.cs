using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Class describing Karana entity (60 entities)
    /// </summary>
    [Table("KARANA")]
    public class Karana
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("TITHIID")]
        public int TithiId { get; set; }
        [Column("POSITION")]
        public int Position { get; set; }
        [Column("COLORID")]
        public int ColorId { get; set; }
    }

    [Table("KARANA_DESC")]
    public class KaranaDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("KARANAID")]
        public int KaranaId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("UPRAVITEL")]
        public string Upravitel { get; set; } = string.Empty;
        [Column("GOODFOR")]
        public string GoodFor { get; set; } = string.Empty;
        [Column("BADFOR")]
        public string BadFor { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
