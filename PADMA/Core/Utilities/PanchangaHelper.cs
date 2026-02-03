using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Enums;
using PADMA.UI;

namespace PADMA.Core.Utilities
{
    public static class PanchangaHelper
    {
        /// <summary>
        /// Builds UI segments (local time) for a given local day from UTC slices.
        /// Generic overload allows passing strongly-typed slices and a color selector.
        /// </summary>
        public static List<PanchangaSegment> BuildSegmentsForDay<TSlice>(
            IEnumerable<TSlice> slicesUtc,
            DateTime dayLocal,
            TimeZoneInfo tz,
            DataCache cache,
            Func<TSlice, EColor> getColorCode,
            Func<TSlice, string?>? getText = null,
            Func<TSlice, ETransitKind>? getKind = null,
            Func<TSlice, int>? getId = null)
            where TSlice : CalendarSlice
        {
            if (slicesUtc == null) throw new ArgumentNullException(nameof(slicesUtc));
            if (tz == null) throw new ArgumentNullException(nameof(tz));
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (getColorCode == null) throw new ArgumentNullException(nameof(getColorCode));

            var result = new List<PanchangaSegment>();

            // start/end of day in local time, respecting DST
            var offset = tz.GetUtcOffset(dayLocal);
            var dayStartLocal = new DateTimeOffset(dayLocal.Date, offset);
            var dayEndLocal = dayStartLocal.AddDays(1);

            foreach (var slice in slicesUtc)
            {
                var sliceStartUtc = slice.StartUtc; // или slice.StartDateUtc / Start
                var sliceEndUtc = slice.EndUtc;

                var sliceStartLocal = TimeZoneInfo.ConvertTimeFromUtc(sliceStartUtc, tz);
                var sliceEndLocal = TimeZoneInfo.ConvertTimeFromUtc(sliceEndUtc, tz);

                // convert slice bounds to local time
                var startLocal = new DateTimeOffset(slice.StartUtc, TimeSpan.Zero).ToOffset(offset);
                var endLocal = new DateTimeOffset(slice.EndUtc, TimeSpan.Zero).ToOffset(offset);

                // intersect with current day
                var effStart = startLocal > dayStartLocal ? startLocal : dayStartLocal;
                var effEnd = endLocal < dayEndLocal ? endLocal : dayEndLocal;

                if (effEnd <= effStart)
                    continue;

                var colorCode = getColorCode(slice);
                var color = cache.GetColor(colorCode);

                var core = getText?.Invoke(slice); // например "4.Rohini"
                string? text = core;

                if (!string.IsNullOrWhiteSpace(core))
                {
                    // dayLocal — локальное время, соответствует началу суток (00:00) для строящегося дня
                    dayStartLocal = dayLocal.Date; // 00:00 локально
                    startLocal = effStart.LocalDateTime;

                    // если сегмент начался НЕ в самом начале дня, то добавляем время
                    if (startLocal > dayStartLocal.AddSeconds(1)) // маленький допуск
                    {
                        text = $"{startLocal:HH:mm} {core}";
                    }
                }

                result.Add(new PanchangaSegment
                {
                    TransitStart = sliceStartLocal,
                    TransitEnd = sliceEndLocal,
                    Start = effStart.LocalDateTime,
                    End = effEnd.LocalDateTime,
                    Color = color,
                    Text = text,
                    TransitKind = getKind?.Invoke(slice) ?? ETransitKind.Unknown,
                    TransitId = getId?.Invoke(slice) ?? 0
                });

            }

            // sort by start time for consistent UI rendering
            return result.OrderBy(s => s.Start).ToList();
        }

        /// <summary>
        /// Non-generic wrapper for convenience.
        /// </summary>
        public static List<PanchangaSegment> BuildSegmentsForDay(
            IEnumerable<CalendarSlice> slicesUtc,
            DateTime dayLocal,
            TimeZoneInfo tz,
            DataCache cache,
            Func<CalendarSlice, EColor> getColorCode,
            Func<CalendarSlice, string?>? getText = null,
            Func<CalendarSlice, ETransitKind>? getKind = null,
            Func<CalendarSlice, int>? getId = null)
        {
            return BuildSegmentsForDay<CalendarSlice>(slicesUtc, dayLocal, tz, cache, getColorCode, getText, getKind, getId);
        }

        public static ZodiacDesc GetZodiacDescEntity(int zodiacId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.ZodiacDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.ZodiacId == zodiacId);
        }

        public static PlanetDesc? GetPlanetDescEntity(int planetId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.PlanetDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.PlanetId == planetId);
        }

        public static NakshatraDesc? GetNakshatraDescEntity(int nakshatraId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.NakshatraDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.NakshatraId == nakshatraId);
        }

        public static TaraBalaDesc? GetTaraBalaDescEntity(int tarabalId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.TaraBalaDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.TaraBalaId == tarabalId);
        }

        public static TithiDesc? GetTithiDescEntity(int tithiId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.TithiDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.TithiId == tithiId);
        }

        public static KaranaDesc? GetKaranaDescEntity(int karanaId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.KaranaDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.KaranaId == karanaId);
        }

        public static NityaYoga? GetNityaYogaEntity(int nityaYogaId)
        {
            return DataCache.Instance.NityaYogaList
                .FirstOrDefault(i => i.Id == nityaYogaId);
        }

        public static NityaYogaDesc? GetNityaYogaDescEntity(int nityaYogaId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.NityaYogaDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.NityaYogaId == nityaYogaId);
        }

        public static YogaDesc? GetYogaDescEntity(int yogaId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.YogaDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.YogaId == yogaId);
        }

        public static MuhurtaDesc? GetMuhurtaDescEntity(int muhurtaId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.MuhurtaDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.MuhurtaId == muhurtaId);
        }

        public static TransitDesc? GetTransitDescEntity(int transitId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.TransitDescList
                .FirstOrDefault(i => i.LanguageCode == lang && i.TransitId == transitId);
        }


    }
}
