namespace PADMA; 

public static partial class AppCloser
{
    public static void Close()
    {
        ClosePlatform();
    }

    // Important: no access modifier (public/internal/private)!
    static partial void ClosePlatform();
}
