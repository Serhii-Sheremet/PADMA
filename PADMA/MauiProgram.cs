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

        // Ensure DB is copied
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");
        if (!File.Exists(dbPath))
        {
            using var inStream = FileSystem.OpenAppPackageFileAsync("PADMADB.db3").GetAwaiter().GetResult();
            using var outStream = File.Create(dbPath);
            inStream.CopyTo(outStream);
        }

        // Register services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<AppSettingsService>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<DayPage>();
        builder.Services.AddSingleton<ConfigurationPage>();
        builder.Services.AddSingleton<ExitPage>();

        // Build provider and initialize cache
        var provider = builder.Services.BuildServiceProvider();
        ServiceLocator.Services = provider;

        var db = provider.GetService<DatabaseService>();
        DataCache.Instance.LoadAll(db);

        return builder.Build();
    }



}
