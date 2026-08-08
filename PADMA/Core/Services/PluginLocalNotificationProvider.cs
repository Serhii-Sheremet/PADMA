using Plugin.LocalNotification;

#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
#endif

namespace PADMA.Core.Services;

public sealed class PluginLocalNotificationProvider : ILocalNotificationProvider
{
    #if ANDROID
    private static bool CanScheduleExactAlarms()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.S) // Android 12 (API 31)
            return true;

        var alarmManager = (AlarmManager?)Android.App.Application.Context
            .GetSystemService(Context.AlarmService);

        return alarmManager?.CanScheduleExactAlarms() ?? true;
    }

    private static void RequestExactAlarmPermission()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.S)
            return;

        // Opens system screen: "Allow exact alarms" / "Alarms & reminders"
        var intent = new Intent(Android.Provider.Settings.ActionRequestScheduleExactAlarm);
        intent.AddFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
    }
    #endif

    public async Task<bool> EnsurePermissionsAsync()
    {
        // Wiki: AreNotificationsEnabled + RequestNotificationPermission :contentReference[oaicite:2]{index=2}
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            return await LocalNotificationCenter.Current.RequestNotificationPermission();

        #if ANDROID
        // Exact alarms are required for reliable scheduled notifications on Android 12+
        if (!CanScheduleExactAlarms())
        {
            RequestExactAlarmPermission();
            // We don't wait for a result here — fire and forget; the user grants the permission via system settings.
        }
        #endif

        return true;
    }

    public async Task ScheduleAsync(int notificationId, DateTime fireTimeLocal, string title, string body)
    {
        var local = fireTimeLocal;

        if (local.Kind == DateTimeKind.Unspecified)
            local = DateTime.SpecifyKind(local, DateTimeKind.Local);
        else
            local = local.ToLocalTime();

        if (local <= DateTime.Now.AddSeconds(1))
            local = DateTime.Now.AddSeconds(5);

        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Description = body,
            Android = { ChannelId = "default" },
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = local
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public Task CancelAsync(int notificationId)
    {
        _ = LocalNotificationCenter.Current.Cancel(notificationId);
        return Task.CompletedTask;
    }

    public Task CancelManyAsync(IEnumerable<int> notificationIds)
    {
        _ = LocalNotificationCenter.Current.Cancel(notificationIds.ToArray());
        return Task.CompletedTask;
    }

    public Task CancelAllAsync()
    {
        LocalNotificationCenter.Current.CancelAll();
        LocalNotificationCenter.Current.ClearAll();
        return Task.CompletedTask;
    }

}
