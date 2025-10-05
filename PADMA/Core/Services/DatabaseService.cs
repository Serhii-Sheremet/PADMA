using PADMA.Core.Models;
using SQLite;


namespace PADMA.Core.Services
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _connection;

        public DatabaseService(string dbPath)
        {
            _connection = new SQLiteConnection(dbPath);
        }

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
            using var db = new SQLiteConnection(_dbPath);
            return db.Table<AppSettingList>().ToList();
        }

        public void DeactivateGroup(string groupCode)
        {
            using var db = new SQLiteConnection(_dbPath);
            db.Execute("UPDATE APPSETTING SET ACTIVE = 0 WHERE GROUPCODE = ?", groupCode);
        }

        public void ActivateSetting(int id)
        {
            using var db = new SQLiteConnection(_dbPath);
            db.Execute("UPDATE APPSETTING SET ACTIVE = 1 WHERE ID = ?", id);
        }


    }
}
