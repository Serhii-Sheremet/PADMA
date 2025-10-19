using System.IO;
using Microsoft.Maui;
using PADMA.Core.Services;
using PADMA.Pages;

namespace PADMA;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // === Новый механизм автообновления базы ===
        var dbFileName = "PADMADB.db3";
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbFileName);

        // Копируем новую базу из ресурсов во временную директорию
        var tempPath = Path.Combine(FileSystem.CacheDirectory, dbFileName);
        using (var inStream = FileSystem.OpenAppPackageFileAsync(dbFileName).GetAwaiter().GetResult())
        using (var outStream = File.Create(tempPath))
        {
            inStream.CopyTo(outStream);
        }

        bool needReplace = false;

        try
        {
            // Проверяем версию локальной базы
            string localVersion = null;
            string newVersion = null;

            if (File.Exists(dbPath))
            {
                using var localDb = new SQLite.SQLiteConnection(dbPath);
                localVersion = localDb.ExecuteScalar<string>("SELECT VALUE FROM APP_META WHERE KEY = 'DB_VERSION'");
            }

            using var newDb = new SQLite.SQLiteConnection(tempPath);
            newVersion = newDb.ExecuteScalar<string>("SELECT VALUE FROM APP_META WHERE KEY = 'DB_VERSION'");

            if (localVersion == null || localVersion != newVersion)
            {
                needReplace = true;
                System.Diagnostics.Debug.WriteLine($"[DB] Updating local DB from version {localVersion ?? "none"} to {newVersion}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DB] Version check failed: {ex.Message}");
            needReplace = true; // если ошибка — заменяем базу
        }

        if (needReplace)
        {
            File.Copy(tempPath, dbPath, overwrite: true);
        }

        // === Регистрация сервисов ===
        builder.Services.AddSingleton<DatabaseService>();
        ServiceLocator.Services = builder.Services.BuildServiceProvider();

        // 2) Регистрируем один-единственный экземпляр DatabaseService
        var db = new DatabaseService();
        builder.Services.AddSingleton(db);

        var activeLang = db.GetActiveLanguageCode(); // "en" | "uk" | "pl" | "ru"
        //DataCache.Instance.CurrentLanguageCode = activeLang;          
        DataCache.Instance.LoadAll(db, activeLang);

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<DayPage>();
        builder.Services.AddSingleton<ConfigurationPage>();
        builder.Services.AddSingleton<ExitPage>();
        builder.Services.AddSingleton<AppSettingsService>();
        builder.Services.AddSingleton<NominatimService>();


        return builder.Build();
    }
}
