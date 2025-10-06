using SQLite;

namespace PADMA.Core.Models
{
    [Table("APP_TEXTS")]
    public class AppText
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NativeText { get; set; }
        public string ForeignText { get; set; }
        public string LanguageCode { get; set; }
    }

}
