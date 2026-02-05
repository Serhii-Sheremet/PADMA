namespace PADMA; 

public static partial class AppCloser
{
    static partial void ClosePlatform()
    {
        // На iOS не закрываем приложение программно.
        // Можно: ничего не делать, или вернуть на главную.
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync("//main");
        });
    }
}
