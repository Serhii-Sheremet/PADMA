using SQLite;

namespace PADMA.Core.Models
{
    [Table("MRITYUBHAGA")]
    public class MrityuBhaga
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PlanetId { get; set; }

        [Indexed]
        public int ZodiakId { get; set; }

        // В БД INTEGER — читаем как double, чтобы не терять точность в дальнейших сравнениях
        public double Degree { get; set; } // absolute sidereal longitude (0–360)
    }
}
