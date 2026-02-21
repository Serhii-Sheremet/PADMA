using System;
using System.Collections.Generic;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;

namespace PADMA.Core.TransitBuilder
{
    public static class TithiTransitBuilder
    {
        public static List<TithiSlice> BuildTithiSlices(List<TithiData> list, DateTime endUtc)
        {
            var result = new List<TithiSlice>();

            if (list == null || list.Count == 0)
                return result;

            var cache = DataCache.Instance;

            for (int i = 0; i < list.Count; i++)
            {
                var current = list[i];
                var nextStart = (i < list.Count - 1)
                    ? list[i + 1].DateTimeUtc
                    : endUtc;

                // защита на всякий случай
                if (nextStart <= current.DateTimeUtc)
                    continue;

                var tithiId = current.TithiId;

                var slice = new TithiSlice
                {
                    StartUtc = current.DateTimeUtc,
                    EndUtc = nextStart,
                    TithiId = tithiId,
                    ColorId = TithiSlice.GetTithiColorId(tithiId)
                };

                result.Add(slice);
            }

            return result;
        }


    }
}