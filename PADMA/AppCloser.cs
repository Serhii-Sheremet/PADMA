namespace PADMA; 

public static partial class AppCloser
{
    public static void Close()
    {
        ClosePlatform();
    }

    // ÂÀÆÍÎ: áåç public/internal/private !
    static partial void ClosePlatform();
}
