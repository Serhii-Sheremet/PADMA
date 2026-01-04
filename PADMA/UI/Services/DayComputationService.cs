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
        private static bool IsNodePlanet(EPlanet planet) => planet == EPlanet.RAHU || planet == EPlanet.KETU;

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
            await Task.Yield();
            ct.ThrowIfCancellationRequested();
            
            var lang = DataCache.Instance.CurrentLanguageCode;
            var data = new DayOverviewData(key);
            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;
            var ctx = DataCache.Instance.ProfileContextService.Current;
            if (ctx?.TimeZoneInfo != null)
                tzInfo = ctx.TimeZoneInfo;

            var dayStartLocal = baseDay.Date.Date;      // 00:00 локального дня
            var dayEndLocal = dayStartLocal.AddDays(1);
            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayStartLocal, tzInfo);
            var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);
            var transitMode = DataCache.Instance.GetActiveTransitSettings();

            data.SunriseLabelText = Localization.GetLocalizedText("Sunrise:", lang);
            data.SunsetLabelText = Localization.GetLocalizedText("Sunset:", lang);
            try
            {
                // Важно: координаты "Living"
                double lat = ctx.LivingLat;
                double lon = ctx.LivingLon;

                // SwissService 
                var sunriseUtc = SwissService.CalculateSunriseForDateAndLocation(dayStartUtc, lat, lon);
                var sunsetUtc = SwissService.CalculateSunsetForDateAndLocation(dayStartUtc, lat, lon);

                // convert to local time
                var sunriseLocal = sunriseUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(sunriseUtc.Value, tzInfo) : (DateTime?)null;
                var sunsetLocal = sunsetUtc.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(sunsetUtc.Value, tzInfo) : (DateTime?)null;

                // Time formating
                data.SunriseText = sunriseLocal.HasValue ? sunriseLocal.Value.ToString("HH:mm:ss") : "--:--:--";
                data.SunsetText = sunsetLocal.HasValue ? sunsetLocal.Value.ToString("HH:mm:ss") : "--:--:--";
            }
            catch
            {
                data.SunriseText = "--:--:--";
                data.SunsetText = "--:--:--";
            }


            // --------------------------
            // Planets transit lines
            // --------------------------
            if (baseDay?.TransitPack != null)
            {
                var dayStart = dayStartUtc;
                var dayEnd = dayEndUtc;

                var planets9 = new[]
                {
                    EPlanet.SUN,
                    EPlanet.MOON,
                    EPlanet.MERCURY,
                    EPlanet.VENUS,
                    EPlanet.MARS,
                    EPlanet.JUPITER,
                    EPlanet.SATURN,
                    EPlanet.RAHU,
                    EPlanet.KETU
                };

                foreach (var planet in planets9)
                {
                    if (!baseDay.TransitPack.TryGetValue(planet, out var slicesSorted) ||
                        slicesSorted == null || slicesSorted.Count == 0)
                        continue;

                    var segments = BuildPlanetOverviewSegments(
                        planet,
                        slicesSorted,
                        dayStartUtc,
                        dayEndUtc,
                        dayStartLocal,
                        dayEndLocal,
                        tzInfo,
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
                    seg.Color = null;              
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
            DateTime dayStartLocal,
            DateTime dayEndLocal,
            TimeZoneInfo tzInfo,
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

            var zodiac = GetZodiacName(baseSlice.ZodiacId);
            var label = BuildPlanetLabelText(planet, curRetro, curEx); // Pl / Pl.R / Pl↑ / Pl↓

            // First segment (starts at 00:00 local), with "label on the left"
            var first = new PanchangaSegment
            {
                Start = dayStartLocal,
                End = dayEndLocal, // temporarily, will cut when first event occurs
                Text = string.IsNullOrEmpty(zodiac) ? label : $"{label}, {zodiac}"
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
                var tLocal = TimeZoneInfo.ConvertTimeFromUtc(s.StartUtc, tzInfo); 

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

                if (eventText == null)
                {
                    // Для узлов: ретро-toggle игнорируем как событие.
                    // Тогда просто обновим состояние (если надо) и продолжим без сегмента.
                    curZodiacId = s.ZodiacId;
                    curRetro = s.IsRetrograde;
                    curEx = newEx;
                    continue;
                }

                zodiac = GetZodiacName(s.ZodiacId);
                // new segment begins at event time
                var seg = new PanchangaSegment
                {
                    Start = tLocal,
                    End = dayEndUtc, // will cut by next event
                    Text = string.IsNullOrEmpty(zodiac)
                        ? $"{tLocal:HH:mm} {eventText}"
                        : $"{tLocal:HH:mm} {eventText}, {zodiac}"
                };

                ApplyTransitColorsForSlice(seg, s, transitMode);
                result.Add(seg);

                // advance current state
                curZodiacId = newZodiacId;
                curRetro = newRetro;
                curEx = newEx;
            }

            // ensure last ends at end-of-day
            result[^1].End = dayEndLocal;
            return result;
        }

        private static string BuildPlanetLabelText(EPlanet planet, bool isRetro, EExaltation ex)
        {
            // planet short name (localized, first 2 chars)
            var pl = GetPlanetShortName(planet);

            if (IsNodePlanet(planet))
                return pl;

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

            if (IsNodePlanet(planet))
            {
                // only ingress marker allowed for nodes
                return zodiacChanged ? $"{pl}→" : null;
            }

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

        private static string GetZodiacName(int zodiacId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            string zodiac = DataCache.Instance.ZodiacDescList
                .FirstOrDefault(z => z.LanguageCode == lang && z.ZodiacId == zodiacId)?.Name ?? string.Empty;

            return zodiac;
        }



    }
}
