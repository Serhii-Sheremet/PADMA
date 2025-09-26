using SQLite;
using System.Collections.Generic;

namespace PADMA.Services
{
    public class DatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_dbPath}");
        }

        // Example: read languages
        public List<(int Id, string LanguageCode, string CultureCode)> GetLanguages()
        {
            var result = new List<(int, string, string)>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ID, LANGUAGECODE, CULTURECODE FROM LANGUAGE";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add((
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                ));
            }

            return result;
        }
    }
}
