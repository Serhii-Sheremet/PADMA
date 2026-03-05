using PADMA.Core.Services;

namespace PADMA;

public partial class App : Application
{
    private bool _initializedOnce;
    private readonly SemaphoreSlim _profileInitLock = new(1, 1);

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        try
        {
            _initializedOnce = true;
            await EnsureDefaultProfileContextAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileContext] OnStart failed: {ex}");
        }
    }

    protected override async void OnResume()
    {
        base.OnResume();
        try
        {
            // при возврате в приложение тоже приводим к дефолту
            await EnsureDefaultProfileContextAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileContext] OnResume failed: {ex}");
        }
    }

    public async Task EnsureDefaultProfileContextAsync()
    {
        if (!await _profileInitLock.WaitAsync(0))
            return;
        
        try
        {
            await SwissService.InitializeEphemerisPathAsync();

            var reminder = ServiceLocator.Services.GetService<IUserNoteReminderService>();
            var db = ServiceLocator.Services.GetService<DatabaseService>();

            if (db != null)
            {
                if (DataCache.Instance.ProfileList != null && DataCache.Instance.ProfileList.Count > 0)
                {
                    DataCache.Instance.ReloadProfiles(db, setActiveToDefault: true);
                    await DataCache.Instance.RebuildProfileContextAsync();
                }
            }

            if (reminder != null && DataCache.Instance.ProfileContextService.Current != null)
                await reminder.RefreshAsync();
        }
        finally
        {
            _profileInitLock.Release();
        }
    }


}
