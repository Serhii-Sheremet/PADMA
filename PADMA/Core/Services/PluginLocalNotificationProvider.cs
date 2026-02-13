using Plugin.LocalNotification;

namespace PADMA.Core.Services;

public sealed class PluginLocalNotificationProvider : ILocalNotificationProvider
{
    public async Task<bool> EnsurePermissionsAsync()
    {
        // Wiki: AreNotificationsEnabled + RequestNotificationPermission :contentReference[oaicite:2]{index=2}
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            return await LocalNotificationCenter.Current.RequestNotificationPermission();

        return true;
    }

    public async Task ScheduleAsync(int notificationId, DateTime fireTimeLocal, string title, string body)
    {
        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = title,
            Description = body,
            Schedule =
            {
                NotifyTime = fireTimeLocal
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
