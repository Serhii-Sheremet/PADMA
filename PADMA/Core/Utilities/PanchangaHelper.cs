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
        public static List<PanchangaSegment> BuildSegmentsForDay(
            IEnumerable<CalendarSlice> slicesUtc,
            DateTime dayLocal,
            TimeZoneInfo tz,
            DataCache cache,
            Func<CalendarSlice, EColor> getColorCode)
        {
            var result = new List<PanchangaSegment>();

            // начало и конец суток в локальном времени с учётом DST
            var offset = tz.GetUtcOffset(dayLocal);
            var dayStartLocal = new DateTimeOffset(dayLocal.Date, offset);
            var dayEndLocal = dayStartLocal.AddDays(1);

            foreach (var slice in slicesUtc)
            {
                // переводим границы слайса в локальное время
                var startLocal = new DateTimeOffset(slice.StartUtc, TimeSpan.Zero).ToOffset(offset);
                var endLocal   = new DateTimeOffset(slice.EndUtc,   TimeSpan.Zero).ToOffset(offset);

                // пересекаем с текущим днём
                var effStart = startLocal > dayStartLocal ? startLocal : dayStartLocal;
                var effEnd   = endLocal   < dayEndLocal   ? endLocal   : dayEndLocal;

                if (effEnd <= effStart)
                    continue;

                var colorCode = getColorCode(slice);
                var color = cache.GetColor(colorCode); 

                result.Add(new PanchangaSegment
                {
                    Start = effStart.LocalDateTime,
                    End   = effEnd.LocalDateTime,
                    Color = color
                });
            }

            // можно отсортировать по времени начала, чтобы полоска была аккуратной
            return result.OrderBy(s => s.Start).ToList();
        }
    }
}
