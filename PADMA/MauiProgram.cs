using PADMA.Services;
using PADMA.Core.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(f =>
            {
                f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                f.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Path to copied DB ("PADMADB.db3") — как у тебя сейчас реализовано копирование в AppDataDirectory
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");
        var db = new DatabaseService(dbPath);
        builder.Services.AddSingleton(db);

        // Load cache once (choose UI language; например, "en" по умолчанию)
        DataCache.Instance.LoadAll(db, preferredUiLang: "en");

        return builder.Build();
    }
}
