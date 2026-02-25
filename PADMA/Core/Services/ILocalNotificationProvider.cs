namespace PADMA.Core.Services;

public interface ILocalNotificationProvider
{
    Task<bool> EnsurePermissionsAsync();
    Task ScheduleAsync(int notificationId, DateTime fireTimeLocal, string title, string body);
    Task CancelAsync(int notificationId);
    Task CancelManyAsync(IEnumerable<int> notificationIds);
    Task CancelAllAsync();

}
