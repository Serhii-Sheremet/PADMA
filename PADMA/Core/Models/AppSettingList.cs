using SQLite; // из sqlite-net-pcl

namespace PADMA.Core.Models
{
    [Table("APPSETTING")]
    public class AppSettingList
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        [Column("GROUPCODE")]
        public string GroupCode { get; set; } = "";

        [Column("SETTINGCODE")]
        public string SettingCode { get; set; } = "";

        [Column("ACTIVE")]
        public int Active { get; set; }
    }
}