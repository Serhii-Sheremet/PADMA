namespace PADMA; 

public static partial class AppCloser
{
    static partial void ClosePlatform()
    {
        // On iOS, programmatically closing the app is not recommended.
        // Instead: navigate back to main, as if the user pressed Cancel.
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync("//main");
        });
    }
}
