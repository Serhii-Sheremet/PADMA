using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Represents a single Pada entry (total 108).
    /// </summary>
    [Table("PADA")]
    public class Pada
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }

        [Column("ZODIAKID")]
        public int ZodiakId { get; set; }

        [Column("NAKSHATRAID")]
        public int NakshatraId { get; set; }

        [Column("PADANUMBER")]
        public int PadaNumber { get; set; }

        [Column("DREKKANA")]
        public int Drekkana { get; set; }

        [Column("SPECIALNAVAMSA")]
        public string SpecialNavamsa { get; set; } = string.Empty;

        [Column("NAVAMSA")]
        public int Navamsa { get; set; }

        [Column("COLORID")]
        public int ColorId { get; set; }
    }
}
