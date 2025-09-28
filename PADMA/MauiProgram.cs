using PADMA.Core.Services;

namespace PADMA;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

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

        // 📌 Копируем БД, если её ещё нет
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");
        if (!File.Exists(dbPath))
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("PADMADB.db3").Result;
            using var newFile = File.Create(dbPath);
            stream.CopyTo(newFile);
        }

        // 📌 Регистрация сервисов
        builder.Services.AddSingleton(new DatabaseService(dbPath));

        // 📌 Регистрация страниц
        builder.Services.AddSingleton<MainPage>();

        var app = builder.Build();

        // сохраняем ServiceProvider для ServiceLocator
        Services = app.Services;

        return app;
    }
}
