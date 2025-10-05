using PADMA.Core.Models;

namespace PADMA.Core.Services
{
    public class AppSettingsService
    {
        private readonly DatabaseService _db;

        public AppSettingsService(DatabaseService db)
        {
            _db = db;
        }

        // Загружаем все настройки
        public List<AppSettingList> LoadSettings()
            => _db.GetAppSettingsList();

        // Получаем активную настройку по группе (например, "WEEK")
        public AppSettingList GetActiveSetting(string groupCode)
            => _db.GetAppSettingsList()
                   .FirstOrDefault(s => s.GroupCode == groupCode && s.Active == 1);

        // Активируем конкретную настройку
        public void SetActiveSetting(int settingId)
        {
            var setting = _db.GetAppSettingsList().FirstOrDefault(s => s.Id == settingId);
            if (setting == null) return;

            _db.DeactivateGroup(setting.GroupCode);
            _db.ActivateSetting(setting.Id);
        }
    }
}
