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
            await DataCache.Instance.RebuildProfileContextAsync();

            var reminder = ServiceLocator.Services.GetService<IUserNoteReminderService>();
            if (reminder != null)
                await reminder.RefreshAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileContext] Rebuild failed: {ex}");
        }
    }

}
