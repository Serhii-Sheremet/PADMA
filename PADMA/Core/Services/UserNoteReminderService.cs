using Plugin.LocalNotification;
using System.Diagnostics;

namespace PADMA.Core.Services;

public sealed class UserNoteReminderService : IUserNoteReminderService
{
    private const int HORIZON_DAYS = 7;
    private const int MAX_COUNT = 64;

    private readonly DatabaseService _db;
    private readonly ILocalNotificationProvider _provider;
    private readonly SemaphoreSlim _lock = new(1, 1);

    // простая коалесценция сигналов
    private int _refreshQueued = 0;

    public UserNoteReminderService(DatabaseService db, ILocalNotificationProvider provider)
    {
        _db = db;
        _provider = provider;

        MessagingCenter.Subscribe<object>(this, "ProfileChanged", _ => RequestRefresh());
        MessagingCenter.Subscribe<object>(this, "UserEventsChanged", _ => RequestRefresh());

        DataCache.Instance.ProfileContextUpdated += RequestRefresh;
    }

    private void RequestRefresh()
    {
        if (Interlocked.Exchange(ref _refreshQueued, 1) == 1)
            return;

        _ = Task.Run(async () =>
        {
            try { await RefreshAsync(); }
            catch (Exception ex) { Debug.WriteLine($"[Reminder] Refresh failed: {ex}"); }
            finally { Interlocked.Exchange(ref _refreshQueued, 0); }
        });
    }

    public async Task CancelAllAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            await _provider.CancelAllAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (ctx == null)
                return;

            int? reminderMinutes = GetReminderMinutesFromCache(); // OFF => null
            if (reminderMinutes == null)
            {
                await _provider.CancelAllAsync();
                return;
            }

            var tz = ctx.TimeZoneInfo;
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var windowEndExclusiveLocal = nowLocal.Date.AddDays(HORIZON_DAYS + 1);

            // грузим события из БД (они хранятся в local time)
            var notes = _db.GetUserEventsForRange(ctx.ProfileId, nowLocal, windowEndExclusiveLocal);

            // строим список уведомлений
            var planned = notes
                .Select(n => new
                {
                    Note = n,
                    FireTimeLocal = n.StartLocal.AddMinutes(-reminderMinutes.Value)
                })
                .Where(x =>
                    x.Note.StartLocal < windowEndExclusiveLocal &&
                    x.FireTimeLocal > nowLocal // строго в будущем
                )
                .OrderBy(x => x.FireTimeLocal)
                .Take(MAX_COUNT)
                .ToList();

            // Stage 1: cancel + reschedule
            await _provider.CancelAllAsync();

            if (planned.Count == 0)
                return;

            // permissions
            var ok = await _provider.EnsurePermissionsAsync();
            if (!ok)
                return;

            // Текст сообщения
            const string title = "Personal Astrological Diary";

            foreach (var item in planned)
            {
                ct.ThrowIfCancellationRequested();

                var from = item.Note.StartLocal.ToString("HH:mm");
                var to = item.Note.EndLocal.ToString("HH:mm");

                var name = string.IsNullOrWhiteSpace(item.Note.Name) ? "Reminder" : item.Note.Name;
                var body = $"{from}–{to} • {name}";

                // notificationId = note.Id
                await _provider.ScheduleAsync(item.Note.Id, item.FireTimeLocal, title, body);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private static int? GetReminderMinutesFromCache()
    {
        var s = DataCache.Instance.AppSettingsList
            .FirstOrDefault(x => x.GroupCode == "NOTEREMINDER" && x.Active == 1);

        var code = s?.SettingCode?.ToUpperInvariant() ?? "OFF";

        return code switch
        {
            "OFF" => null,
            "MIN5" => 5,
            "MIN15" => 15,
            "MIN30" => 30,
            _ => null
        };
    }
}
