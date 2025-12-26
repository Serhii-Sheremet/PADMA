using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PADMA.UI.Services
{
    /// <summary>
    /// Progressive day computation with in-memory caching.
    /// Key = (ProfileId, Date). Language is UI-only (DataCache.Instance.CurrentLanguageCode).
    /// </summary>
    public class DayComputationService : IDayComputationService
    {
        private readonly object _sync = new();

        private readonly Dictionary<DayKey, Lazy<Task<DayOverviewData>>> _overviewCache = new();
        private readonly Dictionary<DayKey, Lazy<Task<DayDetailsData>>> _detailsCache = new();

        public Task<DayOverviewData> GetOverviewAsync(DayKey key, DayItem baseDay, CancellationToken ct = default)
        {
            Lazy<Task<DayOverviewData>> lazy;

            lock (_sync)
            {
                if (!_overviewCache.TryGetValue(key, out lazy))
                {
                    lazy = new Lazy<Task<DayOverviewData>>(() => BuildOverviewAsync(key, baseDay, ct));
                    _overviewCache[key] = lazy;
                }
            }

            return lazy.Value;
        }

        public Task<DayDetailsData> GetDetailsAsync(DayKey key, DayItem baseDay, CancellationToken ct = default)
        {
            Lazy<Task<DayDetailsData>> lazy;

            lock (_sync)
            {
                if (!_detailsCache.TryGetValue(key, out lazy))
                {
                    lazy = new Lazy<Task<DayDetailsData>>(() => BuildDetailsAsync(key, baseDay, ct));
                    _detailsCache[key] = lazy;
                }
            }

            return lazy.Value;
        }

        public void InvalidateProfile(int profileId)
        {
            lock (_sync)
            {
                var overviewKeys = _overviewCache.Keys.Where(k => k.ProfileId == profileId).ToList();
                foreach (var k in overviewKeys) _overviewCache.Remove(k);

                var detailsKeys = _detailsCache.Keys.Where(k => k.ProfileId == profileId).ToList();
                foreach (var k in detailsKeys) _detailsCache.Remove(k);
            }
        }

        public void InvalidateAll()
        {
            lock (_sync)
            {
                _overviewCache.Clear();
                _detailsCache.Clear();
            }
        }

        // ==========================
        // Builders (placeholders)
        // ==========================

        private static async Task<DayOverviewData> BuildOverviewAsync(DayKey key, DayItem baseDay, CancellationToken ct)
        {
            // Placeholder: later we will compute Muhurta/Yogas using TransitEngine/Swiss services.
            // For now, just return empty structure (but cached).
            await Task.Yield();
            ct.ThrowIfCancellationRequested();

            System.Diagnostics.Debug.WriteLine($"[DayComputation] BuildOverview {key.ProfileId} {key.Date}");

            return new DayOverviewData(key);
        }

        private static async Task<DayDetailsData> BuildDetailsAsync(DayKey key, DayItem baseDay, CancellationToken ct)
        {
            // Placeholder: later we will compute full timeline: transits, eclipses, etc.
            await Task.Yield();
            ct.ThrowIfCancellationRequested();

            return new DayDetailsData(key);
        }
    }
}
