namespace PADMA.Core.Native;

internal static class NativeLibrary
{
#if WINDOWS
    public const string LibName = "swedll64.dll";
#elif ANDROID
    public const string LibName = "libswe.so";
#elif MACCATALYST
    public const string LibName = "libswe.dylib";
#else
    public const string LibName = "swedll64.dll"; // fallback
#endif
}
