using SQLite;

namespace PADMA
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _connection;

        public DatabaseService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");

            // Копируем встроенную БД в рабочую папку, если её ещё нет
            if (!File.Exists(dbPath))
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("PADMADB.db3").Result;
                using var fileStream = File.Create(dbPath);
                stream.CopyTo(fileStream);
            }

            _connection = new SQLiteConnection(dbPath);
        }

        public List<Language> GetLanguages()
        {
            return _connection.Table<Language>().ToList();
        }
    }

    public class Language
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string LanguageCode { get; set; }
        public string CultureCode { get; set; }
    }
}
