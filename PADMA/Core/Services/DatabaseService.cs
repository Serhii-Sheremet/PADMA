using SQLite;
using PADMA.Core.Models;
using System.Collections.Generic;
using System.Linq;


namespace PADMA.Core.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;
        private SQLiteConnection _connection;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
            _connection = new SQLiteConnection(_dbPath);
            _connection.CreateTable<Language>();
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
            return _connection.Table<AppSettingList>().ToList();
        }

        public void DeactivateGroup(string groupCode)
        {
            _connection.Execute("UPDATE APPSETTING SET ACTIVE = 0 WHERE GROUPCODE = ?", groupCode);
        }

        public void ActivateSetting(int id)
        {
            _connection.Execute("UPDATE APPSETTING SET ACTIVE = 1 WHERE ID = ?", id);
        }


    }
}
