using PADMA.Services;

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

        // Register DatabaseService
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADDB.db");

        // Copy from app bundle to writable folder if needed
        string seedDb = Path.Combine(FileSystem.Current.AppPackageDirectory, "Data", "PADDB.db");
        if (!File.Exists(dbPath))
            File.Copy(seedDb, dbPath);

        builder.Services.AddSingleton(new DatabaseService(dbPath));

        return builder.Build();
    }
}
