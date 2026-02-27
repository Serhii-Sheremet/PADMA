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

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();

            // 1) Границы окна
            var now = DateTime.Now;
            var windowStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
            var windowEndExclusive = windowStart.AddDays(HORIZON_DAYS); // 7 дней

            // 2) Reminder offset (глобальный)
            int? offsetMinutes = GetReminderMinutesFromCache();
            if (offsetMinutes == null)
            {
                await _provider.CancelAllAsync();
                return;
            }

            // 3) Профили
            var profiles = DataCache.Instance.ProfileList;
            if (profiles == null || profiles.Count == 0)
                return;

            // 4) Собираем items по всем профилям
            var items = new List<ReminderItem>();

            foreach (var p in profiles)
            {
                ct.ThrowIfCancellationRequested();

                var events = _db.GetUserEventsForRange(p.Id, windowStart, windowEndExclusive);

                // у тебя сейчас p.ProfileName — оставляю как в файле
                var pName = p.ProfileName ?? string.Empty;

                foreach (var ev in events)
                {
                    ct.ThrowIfCancellationRequested();

                    var fireLocal = EnsureLocal(ev.StartLocal).AddMinutes(-offsetMinutes.Value);

                    // пропускаем прошлое
                    if (fireLocal <= now.AddSeconds(1))
                        continue;

                    items.Add(new ReminderItem
                    {
                        FireLocal = ToMinuteSlot(fireLocal),
                        ProfileName = pName,
                        EventName = ev.Name ?? string.Empty,
                    });
                }
            }

            if (items.Count == 0)
            {
                return;
            }

            // 5) Группировка по минутному слоту
            var groups = items
                .GroupBy(x => x.FireLocal)
                .OrderBy(g => g.Key)
                .Take(MAX_COUNT)
                .ToList();

            // 6) Пересобираем пул целиком
            await _provider.CancelAllAsync();

            foreach (var g in groups)
            {
                ct.ThrowIfCancellationRequested();

                var slot = g.Key;

                var lines = g
                    .Select(x => $"{x.ProfileName} — {x.EventName}")
                    .Distinct()
                    .Take(8)
                    .ToList();

                int more = g.Count() - lines.Count;
                if (more > 0)
                    lines.Add($"+{more} more");

                string timeText = slot.ToString("HH:mm");

                string title = g.Count() == 1
                    ? $"🔔 {timeText}"
                    : $"🔔 {timeText} — {g.Count()}";

                string body = string.Join("\n", lines);

                int notifId = MakeNotificationId(slot);

                await _provider.ScheduleAsync(notifId, slot, title, body);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private sealed class ReminderItem
    {
        public DateTime FireLocal { get; set; } // minute slot, Kind=Local
        public string ProfileName { get; set; } = "";
        public string EventName { get; set; } = "";
    }

    private static DateTime EnsureLocal(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dt, DateTimeKind.Local);

        return dt.ToLocalTime();
    }

    private static DateTime ToMinuteSlot(DateTime local)
        => new DateTime(local.Year, local.Month, local.Day, local.Hour, local.Minute, 0, DateTimeKind.Local);

    private static int MakeNotificationId(DateTime slotLocal)
    {
        long t = slotLocal.Ticks;
        int id = unchecked((int)(t ^ (t >> 32)));
        return id & 0x7fffffff;
    }



}
