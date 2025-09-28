using PADMA.Services;
using System.Reflection;

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

        // Путь к базе в AppDataDirectory
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "PADMADB.db3");

        // Если базы ещё нет — копируем из ресурсов
        if (!File.Exists(dbPath))
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("PADMA.Resources.Raw.PADMADB.db3");

            if (stream != null)
            {
                using var fileStream = File.Create(dbPath);
                stream.CopyTo(fileStream);
            }
            else
            {
                throw new FileNotFoundException("Embedded resource PADMADB.db3 not found.");
            }
        }

        // Регистрируем сервис базы
        builder.Services.AddSingleton(new DatabaseService(dbPath));

        return builder.Build();
    }
}
