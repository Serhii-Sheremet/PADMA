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

        // Copy seed DB if needed
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");
        if (!File.Exists(dbPath))
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("PADMADB.db3").Result;
            using var newFile = File.Create(dbPath);
            stream.CopyTo(newFile);
        }

        // DI registrations
        builder.Services.AddSingleton(new DatabaseService(dbPath));
        builder.Services.AddSingleton<MainPage>();

        var app = builder.Build();

        // make DI accessible to ServiceLocator BEFORE App() runs
        ServiceLocator.Services = app.Services;

        return app;
    }
}
