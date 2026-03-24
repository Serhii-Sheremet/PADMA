using CommunityToolkit.Maui;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.LifecycleEvents;
using PADMA.Core.Services;
using PADMA.Pages;
using PADMA.UI.Services;
using Plugin.LocalNotification;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;


namespace PADMA;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.ConfigureLifecycleEvents(events =>
        {
            #if ANDROID
                events.AddAndroid(android =>
                {
                    android.OnCreate((activity, bundle) =>
                    {
                        Microsoft.Maui.Controls.Application.Current?.Dispatcher.Dispatch(async () =>
                        {
                            if (Microsoft.Maui.Controls.Application.Current is App app)
                                await app.EnsureDefaultProfileContextAsync();
                        });
                    });
            
                    android.OnResume(activity =>
                    {
                        Microsoft.Maui.Controls.Application.Current?.Dispatcher.Dispatch(async () =>
                        {
                            if (Microsoft.Maui.Controls.Application.Current is App app)
                                await app.EnsureDefaultProfileContextAsync();
                        });
                    });
                });
            #endif
            
            #if IOS
                        events.AddiOS(ios =>
                        {
                            ios.OnActivated(app =>
                            {
                                Microsoft.Maui.Controls.Application.Current?.Dispatcher.Dispatch(async () =>
                                {
                                    if (Microsoft.Maui.Controls.Application.Current is App mauiApp)
                                        await mauiApp.EnsureDefaultProfileContextAsync();
                                });
                            });
                        });
            #endif
        });

        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXxcdHRdQ2RcV0V+X0ZWYEo=");

        builder
            .UseMauiApp<App>()
        #if ANDROID || IOS
            .UseLocalNotification()
        #endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Harrington-Normal.ttf", "HarringtonNormal");
            })
            .UseMauiCommunityToolkit();
        
        builder.ConfigureSyncfusionCore();

        // === Механизм автообновления базы ===
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

        // Регистрируем один-единственный экземпляр DatabaseService
        var db = new DatabaseService();
        builder.Services.AddSingleton(db);

        var activeLang = db.GetActiveLanguageCode(); // "en" | "uk" | "pl" | "ru"
        DataCache.Instance.LoadAll(db, activeLang);

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<DayPage>();
        builder.Services.AddSingleton<ConfigurationPage>();
        builder.Services.AddSingleton<AppSettingsService>();
        builder.Services.AddSingleton<NominatimService>();
        builder.Services.AddSingleton<NavigationDataStore>();
        builder.Services.AddSingleton<ILocalNotificationProvider, PluginLocalNotificationProvider>();
        builder.Services.AddSingleton<IUserNoteReminderService, UserNoteReminderService>();
        builder.Services.AddSingleton<IDayComputationService, DayComputationService>();

        var app = builder.Build();

        // ВАЖНО: ServiceLocator должен ссылаться на финальный контейнер приложения
        ServiceLocator.Services = app.Services;

        return app;


    }
}