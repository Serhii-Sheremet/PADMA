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

        // Ensure DB is copied from app package to AppData folder
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");
        if (!File.Exists(dbPath))
        {
            // PADMADB.db3 is included as MauiAsset with LogicalName = "PADMADB.db3"
            using var inStream = FileSystem.OpenAppPackageFileAsync("PADMADB.db3").GetAwaiter().GetResult();
            using var outStream = File.Create(dbPath);
            inStream.CopyTo(outStream);
        }


        // Services
        // 1) Регистрируем сервис БД (без параметров — он сам найдёт путь)
        builder.Services.AddSingleton<DatabaseService>();

        // 2) ВАЖНО: сохраняем провайдер, чтобы Shell-страницы могли брать сервисы
        ServiceLocator.Services = builder.Services.BuildServiceProvider();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<DayPage>();
        builder.Services.AddSingleton<ConfigurationPage>();
        builder.Services.AddSingleton<ExitPage>();
        builder.Services.AddSingleton<AppSettingsService>();

        var db = ServiceLocator.Services.GetRequiredService<DatabaseService>();
        
        DataCache.CurrentLanguageCode = "ru"; // временно фиксируем русский язык
        DataCache.Instance.LoadAll(db, DataCache.CurrentLanguageCode);

        

        return builder.Build();
    }
}
