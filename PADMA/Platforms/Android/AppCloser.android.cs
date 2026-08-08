namespace PADMA; 

public static partial class AppCloser
{
    static partial void ClosePlatform()
    {
        var activity = Platform.CurrentActivity;
        activity?.FinishAndRemoveTask();   // removes the task from Android's recent apps
        // activity?.Finish();             // fallback option, currently unused
    }
}
