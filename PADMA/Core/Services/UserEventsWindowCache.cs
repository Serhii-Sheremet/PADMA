using System;
using System.Collections.Generic;
using System.Linq;
using PADMA.Core.Models;

namespace PADMA.Core.Services
{
    /// <summary>
    /// Keeps user events in memory for a specific profile and a specific window (bufferStart..bufferEndExclusive).
    /// Provides fast day lookup for Calendar (indicator) and DayPage (list for the day).
    /// </summary>
    public sealed class UserEventsWindowCache
    {
        private readonly DatabaseService _db;

        private int _profileId;
        private DateTime _windowStart;        // inclusive, 00:00:00
        private DateTime _windowEndExclusive; // exclusive, 00:00:00 next day

        private List<UserEvent> _events = new();
        private Dictionary<DateTime, List<UserEvent>> _byDay = new(); // key = day (00:00:00 local)

        public UserEventsWindowCache(DatabaseService db)
        {
            _db = db;
        }

        public bool IsLoadedFor(int profileId, DateTime windowStart, DateTime windowEndExclusive)
        {
            return _profileId == profileId
                   && _windowStart == NormalizeDay(windowStart)
                   && _windowEndExclusive == NormalizeDay(windowEndExclusive)
                   && _byDay.Count > 0;
        }

        public void LoadWindow(int profileId, DateTime windowStart, DateTime windowEndExclusive)
        {
            _profileId = profileId;
            _windowStart = NormalizeDay(windowStart);
            _windowEndExclusive = NormalizeDay(windowEndExclusive);

            _events = _db.GetUserEventsForRange(profileId, _windowStart, _windowEndExclusive);
            BuildDayIndex();
        }

        public void Invalidate()
        {
            _events.Clear();
            _byDay.Clear();
            _profileId = 0;
            _windowStart = DateTime.MinValue;
            _windowEndExclusive = DateTime.MinValue;
        }

        public bool HasEvents(DateTime dayLocal)
        {
            var day = NormalizeDay(dayLocal);
            return _byDay.ContainsKey(day);
        }

        public List<UserEvent> GetEventsForDay(DateTime dayLocal)
        {
            var day = NormalizeDay(dayLocal);
            return _byDay.TryGetValue(day, out var list) ? list : new List<UserEvent>();
        }

        private void BuildDayIndex()
        {
            _byDay = new Dictionary<DateTime, List<UserEvent>>();

            foreach (var ev in _events)
            {
                if (ev.StartLocal == DateTime.MinValue || ev.EndLocal == DateTime.MinValue)
                    continue;

                // clamp to window to avoid extra looping
                var start = ev.StartLocal < _windowStart ? _windowStart : ev.StartLocal;
                var end = ev.EndLocal > _windowEndExclusive ? _windowEndExclusive : ev.EndLocal;

                // If invalid after clamp, skip
                if (end <= start)
                    continue;

                // We add event to every day it intersects (for Calendar indicator + day list)
                var day = NormalizeDay(start);
                var lastDay = NormalizeDay(end.AddSeconds(-1)); // endExclusive -> inclusive day

                while (day <= lastDay)
                {
                    if (!_byDay.TryGetValue(day, out var list))
                    {
                        list = new List<UserEvent>();
                        _byDay[day] = list;
                    }
                    list.Add(ev);
                    day = day.AddDays(1);
                }
            }

            // Optional: keep events ordered per day by start
            foreach (var kv in _byDay)
                kv.Value.Sort((a, b) => a.StartLocal.CompareTo(b.StartLocal));
        }

        private static DateTime NormalizeDay(DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
    }
}
