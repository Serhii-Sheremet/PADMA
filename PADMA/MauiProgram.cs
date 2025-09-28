using Microsoft.Maui.Controls;
using PADMA.Services;

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

        // Пути к БД
        string dbFileName = "PADMADB.db3";
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, dbFileName);
        string seedDbPath = Path.Combine(FileSystem.AppPackageDirectory, "Resources", "Raw", dbFileName);

        // Копируем, если ещё не было
        if (!File.Exists(dbPath))
            File.Copy(seedDbPath, dbPath);

        // Регистрируем сервисы
        builder.Services.AddSingleton(new DatabaseService(dbPath));
        builder.Services.AddSingleton<MainPage>();

        // Загружаем кеш данных (язык можно будет выбрать из настроек позже)
        DataCache.Instance.LoadAll(builder.Services.BuildServiceProvider().GetRequiredService<DatabaseService>(), preferredUiLang: "en");

        return builder.Build();
    }
}
