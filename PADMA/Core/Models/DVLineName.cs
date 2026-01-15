using SQLite;

namespace PADMA.Core.Models
{
    [Table("DVLINENAME")]
    public class DVLineName
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("CODE")]
        public string Code { get; set; } = string.Empty;
    }

    [Table("DVLINENAME_DESC")]
    public class DVLineNameDesc
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int Id { get; set; }
        [Column("DVLINENAMEID")]
        public int DVLineNameId { get; set; }
        [Column("SHORTNAME")]
        public string ShortName { get; set; } = string.Empty;
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;
        [Column("LANGUAGECODE")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
