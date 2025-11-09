using SQLite;

namespace PADMA.Core.Models
{
    /// <summary>
    /// Планеты в астрологическом смысле (включая Солнце, Луну, узлы и т.п.)
    /// PLANET: ID, PLANETCODE
    /// PLANET_DESC: PLANETID, NAME, LANGUAGECODE
    /// </summary>
    [Table("PLANET")]
    public class Planet
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("PLANETCODE")]
        public string PlanetCode { get; set; } = string.Empty; // e.g. "SUN","MOON","RAHU","KETU"
    }

    [Table("PLANET_DESC")]
    public class PlanetDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("PLANETID")]
        public int PlanetId { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;       // локализованное имя
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
