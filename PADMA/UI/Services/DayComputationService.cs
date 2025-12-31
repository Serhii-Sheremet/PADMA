using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PADMA.Core.Enums;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Core.Models.Calendar;

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
            // System.Diagnostics.Debug.WriteLine($"[DayComputation] BuildOverview {key.ProfileId} {key.Date}");
            
            var data = new DayOverviewData(key);
            var dayStartLocal = key.Date.ToDateTime(TimeOnly.MinValue);
            var dayEndLocal = key.Date.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var transitSetting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "TRANSIT" && s.Active == 1);
            var transitMode = (EAppSetting)(transitSetting?.Id ?? (int)EAppSetting.TRANZITMOON);

            // --------------------------
            // Planets transit lines
            // --------------------------
            if (baseDay?.TransitPack != null)
            {
                // если PlanetSlice.StartUtc уже приведён к localUtc (как в календаре),
                // то сравниваем напрямую:
                var dayStart = dayStartLocal;
                var dayEnd = dayEndLocal;

                var planets9 = new[]
                {
                    EPlanet.SUN,
                    EPlanet.MOON,
                    EPlanet.MERCURY,
                    EPlanet.VENUS,
                    EPlanet.MARS,
                    EPlanet.JUPITER,
                    EPlanet.SATURN,
                    EPlanet.RAHUMEAN,
                    EPlanet.KETUMEAN
                };

                foreach (var planet in planets9)
                {
                    if (!baseDay.TransitPack.TryGetValue(planet, out var slicesSorted) ||
                        slicesSorted == null || slicesSorted.Count == 0)
                        continue;

                    var segments = BuildPlanetOverviewSegments(
                        planet,
                        slicesSorted,
                        dayStart,
                        dayEnd,
                        transitMode);

                    data.PlanetStripes.Add(new PlanetStripe
                    {
                        Planet = planet,
                        Segments = segments
                    });
                }
            }

            return data;
        }

        private static async Task<DayDetailsData> BuildDetailsAsync(DayKey key, DayItem baseDay, CancellationToken ct)
        {
            // Placeholder: later we will compute full timeline: transits, eclipses, etc.
            await Task.Yield();
            ct.ThrowIfCancellationRequested();

            return new DayDetailsData(key);
        }

        private static void ApplyTransitColors(
            PanchangaSegment seg,
            Color moonColor,
            Color lagnaColor,
            EAppSetting transitMode)
        {
            switch (transitMode)
            {
                case EAppSetting.TRANZITMOON:
                    seg.IsSplitColor = false;
                    seg.Color = moonColor;
                    seg.ColorTop = null;
                    seg.ColorBottom = null;
                    break;

                case EAppSetting.TRANZITLAGNA:
                    seg.IsSplitColor = false;
                    seg.Color = lagnaColor;
                    seg.ColorTop = null;
                    seg.ColorBottom = null;
                    break;

                case EAppSetting.TRANZITMOONANDLAGNA:
                    seg.IsSplitColor = true;
                    seg.Color = null;              // можно оставить, но не обязательно
                    seg.ColorTop = moonColor;
                    seg.ColorBottom = lagnaColor;
                    break;

                default:
                    seg.IsSplitColor = false;
                    seg.Color = moonColor;
                    seg.ColorTop = null;
                    seg.ColorBottom = null;
                    break;
            }
        }

        private static List<PanchangaSegment> BuildPlanetOverviewSegments(
            EPlanet planet,
            IReadOnlyList<PlanetSlice> slicesSorted,
            DateTime dayStartUtc,
            DateTime dayEndUtc,
            EAppSetting transitMode)
        {
            var result = new List<PanchangaSegment>();
            if (slicesSorted == null || slicesSorted.Count == 0)
                return result;

            // base slice at dayStartUtc
            PlanetSlice? baseSlice = null;
            for (int i = slicesSorted.Count - 1; i >= 0; i--)
            {
                if (slicesSorted[i].StartUtc <= dayStartUtc)
                {
                    baseSlice = slicesSorted[i];
                    break;
                }
            }
            baseSlice ??= slicesSorted[0];

            // current state (overview-level)
            int curZodiacId = baseSlice.ZodiacId;
            bool curRetro = baseSlice.IsRetrograde;
            var curEx = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)curZodiacId);

            // First segment (starts at 00:00 local), with "label on the left"
            var first = new PanchangaSegment
            {
                Start = dayStartUtc,
                End = dayEndUtc, // temporarily, will cut when first event occurs
                Text = BuildPlanetLabelText(planet, curRetro, curEx) // Pl / Pl.R / Pl↑ / Pl↓
            };

            ApplyTransitColorsForSlice(first, baseSlice, transitMode);
            result.Add(first);

            // iterate slices that start within the day, and keep only sign/retro changes
            for (int i = 0; i < slicesSorted.Count; i++)
            {
                var s = slicesSorted[i];
                if (s.StartUtc < dayStartUtc) continue;
                if (s.StartUtc >= dayEndUtc) break;

                bool zodiacChanged = (s.ZodiacId != curZodiacId);
                bool retroToggled = (s.IsRetrograde != curRetro);

                // ignore pada-level changes that don't affect overview
                if (!zodiacChanged && !retroToggled)
                    continue;

                // boundary time
                var tLocal = s.StartUtc;

                // close previous segment
                result[^1].End = tLocal;

                // compute new state
                int newZodiacId = s.ZodiacId;
                bool newRetro = s.IsRetrograde;
                var newEx = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)newZodiacId);

                // event text according to your Acceptable/Non-Acceptable rules
                var eventText = BuildPlanetEventText(
                    planet,
                    oldRetro: curRetro,
                    newRetro: newRetro,
                    zodiacChanged: zodiacChanged,
                    newEx: newEx);

                // new segment begins at event time
                var seg = new PanchangaSegment
                {
                    Start = tLocal,
                    End = dayEndUtc, // will cut by next event
                    Text = $"{tLocal:HH:mm} {eventText}"
                };

                ApplyTransitColorsForSlice(seg, s, transitMode);
                result.Add(seg);

                // advance current state
                curZodiacId = newZodiacId;
                curRetro = newRetro;
                curEx = newEx;
            }

            // ensure last ends at end-of-day
            result[^1].End = dayEndUtc;
            return result;
        }

        private static string BuildPlanetLabelText(EPlanet planet, bool isRetro, EExaltation ex)
        {
            // planet short name (localized, first 2 chars)
            var pl = GetPlanetShortName(planet);

            if (isRetro)
                return $"{pl}.R";

            if (ex == EExaltation.EXALTATION) return $"{pl}↑";
            if (ex == EExaltation.DEBILITATION) return $"{pl}↓";
            return pl;
        }

        private static void ApplyTransitColorsForSlice(PanchangaSegment seg, PlanetSlice slice, EAppSetting transitMode)
        {
            Color moonColor = DataCache.Instance.GetColor(slice.MoonColorCode);
            Color lagnaColor = DataCache.Instance.GetColor(slice.LagnaColorCode);
            ApplyTransitColors(seg, moonColor, lagnaColor, transitMode);
        }

        private static string BuildPlanetEventText(
            EPlanet planet,
            bool oldRetro,
            bool newRetro,
            bool zodiacChanged,
            EExaltation newEx)
        {
            // planet short name (localized, first 2 chars)
            var pl = GetPlanetShortName(planet);

            // Retro enter
            if (!oldRetro && newRetro)
            {
                if (zodiacChanged) return $"{pl}.R→";
                return $"{pl}.R";
            }

            // Retro exit
            if (oldRetro && !newRetro)
            {
                // show current state after exit
                if (newEx == EExaltation.EXALTATION) return $"{pl}↑";
                if (newEx == EExaltation.DEBILITATION) return $"{pl}↓";
                if (zodiacChanged) return $"{pl}→";
                return pl;
            }

            // No retro toggle today
            if (zodiacChanged)
            {
                // if retro (stable), only allowed marker is Pl.R→
                if (newRetro) return $"{pl}.R→";

                // direct: exalt/debil overrides → (no Pl→↑)
                if (newEx == EExaltation.EXALTATION) return $"{pl}↑";
                if (newEx == EExaltation.DEBILITATION) return $"{pl}↓";
                return $"{pl}→";
            }

            // should not happen if caller filtered correctly, but safe:
            return pl;
        }

        private static string GetPlanetShortName(EPlanet planet)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            string pl = DataCache.Instance.PlanetDescList
                .FirstOrDefault(p => p.LanguageCode == lang && p.PlanetId == (int)planet)?.Name ?? string.Empty;

            return pl.Length >= 2 ? pl.Substring(0, 2) : pl;
        }


    }
}
