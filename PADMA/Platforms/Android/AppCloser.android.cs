namespace PADMA; 

public static partial class AppCloser
{
    static partial void ClosePlatform()
    {
        var activity = Platform.CurrentActivity;
        activity?.FinishAndRemoveTask();   // корректный выход на Android
        // activity?.Finish();             // fallback при желании
    }
}
