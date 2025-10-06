using SQLite;
using PADMA.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PADMA.Core.Models;


namespace PADMA.Core.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;
        private SQLiteConnection _connection;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");

            // если файла нет, скопировать из ресурсов
            var assetDb = Path.Combine(AppContext.BaseDirectory, "Resources", "Raw", "PADMADB.db3");
            if (!File.Exists(_dbPath) && File.Exists(assetDb))
                File.Copy(assetDb, _dbPath);

            _connection = new SQLiteConnection(_dbPath);
        }

        public SQLiteConnection GetConnection() => _connection;

        // --- Languages ---
        public IReadOnlyList<Language> GetLanguages()
        {
            const string sql = @"SELECT ID as Id, LANGUAGECODE as LanguageCode, CULTURECODE as CultureCode
                                 FROM LANGUAGE";
            return _connection.Query<Language>(sql);
        }

        public IReadOnlyList<LanguageDesc> GetLanguageDescs()
        {
            const string sql = @"SELECT ID as Id, LANUAGEID as LanguageId, NAME as Name, LANGUAGECODE as LanguageCode
                                 FROM LANGUAGE_DESC";
            return _connection.Query<LanguageDesc>(sql);
        }

        // --- Colors ---
        public IReadOnlyList<ColorDef> GetColors()
        {
            const string sql = @"SELECT ID as Id, CODE as Code, ARGBVALUE as ArgbValue FROM COLOR";
            return _connection.Query<ColorDef>(sql);
        }

        public IReadOnlyList<ColorDesc> GetColorDescs()
        {
            const string sql = @"SELECT ID as Id, COLORID as ColorId, NAME as Name, LANGUAGECODE as LanguageCode
                                 FROM COLOR_DESC";
            return _connection.Query<ColorDesc>(sql);
        }

        // --- Planets ---
        public IReadOnlyList<PlanetDef> GetPlanets()
        {
            const string sql = @"SELECT ID as Id, PLANETCODE as PlanetCode FROM PLANET";
            return _connection.Query<PlanetDef>(sql);
        }

        public IReadOnlyList<PlanetDesc> GetPlanetDescs()
        {
            const string sql = @"SELECT ID as Id, PLANETID as PlanetId, NAME as Name, LANGUAGECODE as LanguageCode
                                 FROM PLANET_DESC";
            return _connection.Query<PlanetDesc>(sql);
        }

        public List<AppSettingList> GetAppSettingsList()
        {
            try
            {
                return _connection.Table<AppSettingList>().ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DB read error: {ex.Message}");
                return new List<AppSettingList>();
            }
        }

        // Возвращает DayOfWeek на основе активной записи из группы WEEK
        public DayOfWeek GetFirstDayOfWeekFromDb()
        {
            var settings = GetAppSettingsList();
            var active = settings.FirstOrDefault(x => x.GroupCode == "WEEK" && x.Active == 1);

            var code = active?.SettingCode ?? "WEEKMONDAY";
            return code == "WEEKSUNDAY" ? DayOfWeek.Sunday : DayOfWeek.Monday;
        }

        // Сохраняет выбранный код (WEEKMONDAY или WEEKSUNDAY) в БД как активный
        public void SetFirstDayOfWeek(string code)
        {
            // Два простых UPDATE-а над реальной таблицей
            _connection.Execute("UPDATE APPSETTING SET ACTIVE = 0 WHERE GROUPCODE = ?", "WEEK");
            _connection.Execute("UPDATE APPSETTING SET ACTIVE = 1 WHERE GROUPCODE = ? AND SETTINGCODE = ?", "WEEK", code);
        }

        // Опционально — массовый апдейт списка 

        public void UpdateAppSettings(List<AppSettingList> settings)
        {
            try
            {
                foreach (var s in settings)
                    _connection.Update(s);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] DB update error: {ex.Message}");
            }
        }

        public void DeactivateGroup(string groupCode)
        {
            _connection.Execute("UPDATE APPSETTING SET ACTIVE = 0 WHERE GROUPCODE = ?", groupCode);
        }

        public void ActivateSetting(int id)
        {
            _connection.Execute("UPDATE APPSETTING SET ACTIVE = 1 WHERE ID = ?", id);
        }



        public List<AppText> GetAppTextsList()
        {
            List<AppText> entityList = new List<AppText>();
            using (SQLiteConnection dbCon = _connection)
            {
                dbCon.Open();
                try
                {
                    string comm = $"select ID, NATIVETEXT, FOREIGNTEXT, LANGUAGECODE from APP_TEXTS order by ID";
                    SQLiteCommand command = new SQLiteCommand(comm, dbCon);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                AppText temp = new AppText
                                {
                                    Id = reader.GetInt32(0),
                                    NativeText = reader.GetString(1),
                                    ForeignText = reader.GetString(2),
                                    LanguageCode = reader.GetString(3)
                                };
                                entityList.Add(temp);
                            }
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PADMA] DB update error: {ex.Message}");
                }
            }
            return entityList;
        }


    }
}
