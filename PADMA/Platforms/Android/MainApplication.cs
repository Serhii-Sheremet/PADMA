using Android.App;
using Android.OS;
using Android.Runtime;

namespace PADMA
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public const string DefaultChannelId = "default"; // Plugin.LocalNotification default

        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // Android 8+ requires notification channels
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    DefaultChannelId,
                    "Default",
                    NotificationImportance.Default);

                channel.Description = "PADMA notifications";

                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
