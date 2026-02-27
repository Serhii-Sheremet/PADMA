using PADMA.Core.Services;

namespace PADMA;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        //_ = SwissService.InitializeEphemerisPathAsync(); // запустится в фоне
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        try
        {
            await SwissService.InitializeEphemerisPathAsync();
            var reminder = ServiceLocator.Services.GetService<IUserNoteReminderService>(); // чтобы подписки точно установились
            var db = ServiceLocator.Services.GetService<DatabaseService>();
            if (db != null)
            {
                DataCache.Instance.ReloadProfiles(db, setActiveToDefault: true);
            }
            await DataCache.Instance.RebuildProfileContextAsync();

            // refresh только если профиль реально есть
            if (reminder != null && DataCache.Instance.ProfileContextService.Current != null)
                await reminder.RefreshAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileContext] Rebuild failed: {ex}");
        }
    }

}
