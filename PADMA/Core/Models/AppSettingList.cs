namespace PADMA.Core.Models
{
    public class AppSettingList
    {
        public int Id { get; set; }
        public string GroupCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Active { get; set; }  // 1 = активен, 0 = выключен
    }
}
