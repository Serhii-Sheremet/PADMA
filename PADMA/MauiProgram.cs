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
                    /*
                    android.OnResume(activity =>
                    {
                        Microsoft.Maui.Controls.Application.Current?.Dispatcher.Dispatch(async () =>
                        {
                            if (Microsoft.Maui.Controls.Application.Current is App app)
                                await app.EnsureDefaultProfileContextAsync();
                        });
                    });*/
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

        var preservedLocations = new List<LocationBackupRow>();
        var preservedProfiles = new List<ProfileBackupRow>();
        var preservedUserEvents = new List<UserEventBackupRow>();
        var preservedAppSettings = new List<AppSettingBackupRow>();
        var preservedColors = new List<ColorBackupRow>();

        if (needReplace)
        {
            try
            {
                if (File.Exists(dbPath))
                {
                    using var oldDb = new SQLite.SQLiteConnection(dbPath);

                    preservedLocations = oldDb.Query<LocationBackupRow>(@"
                        SELECT
                            ID,
                            LOCALITY,
                            LATITUDE,
                            LONGITUDE,
                            REGION,
                            STATE,
                            COUNTRY,
                            COUNTRYCODE,
                            LANGUAGECODE,
                            LOCALITY_NORM,
                            REGION_NORM,
                            STATE_NORM,
                            COUNTRY_NORM
                        FROM LOCATION
                    ");

                    preservedProfiles = oldDb.Query<ProfileBackupRow>(@"
                        SELECT
                            ID,
                            PROFILENAME,
                            PERSONNAME,
                            PERSONSURNAME,
                            DATEOFBIRTH,
                            PLACEOFBIRTHID,
                            PLACEOFLIVINGID,
                            MESSAGE,
                            CHECKED
                        FROM PROFILE
                    ");

                    preservedUserEvents = oldDb.Query<UserEventBackupRow>(@"
                        SELECT
                            ID,
                            PROFILEID,
                            DATESTART,
                            DATEEND,
                            NAME,
                            MESSAGE,
                            ARGBVALUE
                        FROM USER_EVENTS
                    ");

                    preservedAppSettings = oldDb.Query<AppSettingBackupRow>(@"
                        SELECT
                            GROUPCODE,
                            SETTINGCODE,
                            ACTIVE
                        FROM APPSETTING
                    ");

                    preservedColors = oldDb.Query<ColorBackupRow>(@"
                        SELECT
                            CODE,
                            ARGBVALUE
                        FROM COLOR
                    ");

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB] Preserve user data failed: {ex.Message}");
            }

            File.Copy(tempPath, dbPath, overwrite: true);

            try
            {
                using var newLocalDb = new SQLite.SQLiteConnection(dbPath);

                newLocalDb.RunInTransaction(() =>
                {
                    newLocalDb.Execute("PRAGMA foreign_keys = OFF;");

                    newLocalDb.Execute("DELETE FROM USER_EVENTS;");
                    newLocalDb.Execute("DELETE FROM PROFILE;");
                    newLocalDb.Execute("DELETE FROM LOCATION;");

                    newLocalDb.Execute("DELETE FROM sqlite_sequence WHERE name='USER_EVENTS';");
                    newLocalDb.Execute("DELETE FROM sqlite_sequence WHERE name='PROFILE';");
                    newLocalDb.Execute("DELETE FROM sqlite_sequence WHERE name='LOCATION';");

                    foreach (var loc in preservedLocations)
                    {
                        newLocalDb.Execute(@"
                            INSERT INTO LOCATION
                            (
                                ID,
                                LOCALITY,
                                LATITUDE,
                                LONGITUDE,
                                REGION,
                                STATE,
                                COUNTRY,
                                COUNTRYCODE,
                                LANGUAGECODE,
                                LOCALITY_NORM,
                                REGION_NORM,
                                STATE_NORM,
                                COUNTRY_NORM
                            )
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);",
                            loc.ID,
                            loc.LOCALITY,
                            loc.LATITUDE,
                            loc.LONGITUDE,
                            loc.REGION,
                            loc.STATE,
                            loc.COUNTRY,
                            loc.COUNTRYCODE,
                            loc.LANGUAGECODE,
                            loc.LOCALITY_NORM,
                            loc.REGION_NORM,
                            loc.STATE_NORM,
                            loc.COUNTRY_NORM
                        );
                    }

                    foreach (var profile in preservedProfiles)
                    {
                        newLocalDb.Execute(@"
                            INSERT INTO PROFILE
                            (
                                ID,
                                PROFILENAME,
                                PERSONNAME,
                                PERSONSURNAME,
                                DATEOFBIRTH,
                                PLACEOFBIRTHID,
                                PLACEOFLIVINGID,
                                MESSAGE,
                                CHECKED
                            )
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);",
                            profile.ID,
                            profile.PROFILENAME,
                            profile.PERSONNAME,
                            profile.PERSONSURNAME,
                            profile.DATEOFBIRTH,
                            profile.PLACEOFBIRTHID,
                            profile.PLACEOFLIVINGID,
                            profile.MESSAGE,
                            profile.CHECKED
                        );
                    }

                    foreach (var ev in preservedUserEvents)
                    {
                        newLocalDb.Execute(@"
                            INSERT INTO USER_EVENTS
                            (
                                ID,
                                PROFILEID,
                                DATESTART,
                                DATEEND,
                                NAME,
                                MESSAGE,
                                ARGBVALUE
                            )
                            VALUES (?, ?, ?, ?, ?, ?, ?);",
                            ev.ID,
                            ev.PROFILEID,
                            ev.DATESTART,
                            ev.DATEEND,
                            ev.NAME,
                            ev.MESSAGE,
                            ev.ARGBVALUE
                        );
                    }

                    foreach (var setting in preservedAppSettings)
                    {
                        newLocalDb.Execute(@"
                            UPDATE APPSETTING
                            SET ACTIVE = ?
                            WHERE GROUPCODE = ?
                              AND SETTINGCODE = ?;",
                            setting.ACTIVE,
                            setting.GROUPCODE,
                            setting.SETTINGCODE
                        );
                    }

                    foreach (var color in preservedColors)
                    {
                        newLocalDb.Execute(@"
                            UPDATE COLOR
                            SET ARGBVALUE = ?
                            WHERE CODE = ?;",
                            color.ARGBVALUE,
                            color.CODE
                        );
                    }

                    newLocalDb.Execute("PRAGMA foreign_keys = ON;");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB] Restore user data failed: {ex.Message}");
            }
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
        builder.Services.AddSingleton<UserEventRetentionService>();
        builder.Services.AddSingleton<IDayComputationService, DayComputationService>();

        var app = builder.Build();

        // ВАЖНО: ServiceLocator должен ссылаться на финальный контейнер приложения
        ServiceLocator.Services = app.Services;

        return app;
    }

    private sealed class LocationBackupRow
    {
        public int ID { get; set; }
        public string LOCALITY { get; set; } = string.Empty;
        public string LATITUDE { get; set; } = string.Empty;
        public string LONGITUDE { get; set; } = string.Empty;
        public string REGION { get; set; } = string.Empty;
        public string STATE { get; set; } = string.Empty;
        public string COUNTRY { get; set; } = string.Empty;
        public string COUNTRYCODE { get; set; } = string.Empty;
        public string LANGUAGECODE { get; set; } = string.Empty;
        public string LOCALITY_NORM { get; set; } = string.Empty;
        public string REGION_NORM { get; set; } = string.Empty;
        public string STATE_NORM { get; set; } = string.Empty;
        public string COUNTRY_NORM { get; set; } = string.Empty;
    }

    private sealed class ProfileBackupRow
    {
        public int ID { get; set; }
        public string PROFILENAME { get; set; } = string.Empty;
        public string PERSONNAME { get; set; } = string.Empty;
        public string PERSONSURNAME { get; set; } = string.Empty;
        public string DATEOFBIRTH { get; set; } = string.Empty;
        public int? PLACEOFBIRTHID { get; set; }
        public int? PLACEOFLIVINGID { get; set; }
        public string MESSAGE { get; set; } = string.Empty;
        public int CHECKED { get; set; }
    }

    private sealed class UserEventBackupRow
    {
        public int ID { get; set; }
        public int PROFILEID { get; set; }
        public string DATESTART { get; set; } = string.Empty;
        public string DATEEND { get; set; } = string.Empty;
        public string NAME { get; set; } = string.Empty;
        public string MESSAGE { get; set; } = string.Empty;
        public int ARGBVALUE { get; set; }
    }

    private sealed class AppSettingBackupRow
    {
        public string GROUPCODE { get; set; } = string.Empty;
        public string SETTINGCODE { get; set; } = string.Empty;
        public int ACTIVE { get; set; }
    }

    private sealed class ColorBackupRow
    {
        public string CODE { get; set; } = string.Empty;
        public int ARGBVALUE { get; set; }
    }

}