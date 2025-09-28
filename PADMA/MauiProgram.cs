using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using PADMA.Core.Services; // DatabaseService, ServiceLocator
using System.IO;

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

        // Register DatabaseService (copy seed DB from app package to AppDataDirectory once)
        builder.Services.AddSingleton<DatabaseService>(_ =>
        {
            const string dbFileName = "PADMADB.db3";
            string destPath = Path.Combine(FileSystem.Current.AppDataDirectory, dbFileName);

            if (!File.Exists(destPath))
            {
                // Copy embedded MauiAsset -> writable app data
                using var src = FileSystem.Current.OpenAppPackageFileAsync(dbFileName).GetAwaiter().GetResult();
                using var dst = File.Create(destPath);
                src.CopyTo(dst);
            }

            return new DatabaseService(destPath);
        });

        var app = builder.Build();

        // expose IServiceProvider globally for pages with parameterless constructors
        ServiceLocator.Services = app.Services;

        return app;
    }
}
