namespace PADMA.Core.Models
{
    /// <summary>
    /// Планеты в астрологическом смысле (включая Солнце, Луну, узлы и т.п.)
    /// PLANET: ID, PLANETCODE
    /// PLANET_DESC: PLANETID, NAME, LANGUAGECODE
    /// </summary>
    public sealed class Planet
    {
        public int Id { get; set; }
        public string PlanetCode { get; set; } = ""; // e.g. "SUN","MOON","RAHU","KETU"
    }

    public sealed class PlanetDesc
    {
        public int Id { get; set; }
        public int PlanetId { get; set; }
        public string Name { get; set; } = "";       // локализованное имя
        public string LanguageCode { get; set; } = "";
    }
}
