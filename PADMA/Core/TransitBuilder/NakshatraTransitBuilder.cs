using System;
using System.Collections.Generic;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;

namespace PADMA.Core.TransitBuilder
{

    public static class NakshatraTransitBuilder
    {
        public static List<NakshatraSlice> BuildNakshatraSlices(List<PlanetData> mList)
        {
            var result = new List<NakshatraSlice>();
            if (mList == null || mList.Count == 0)
                return result;

            // стартуем с первой точки
            int currentNakId = mList[0].NakshatraId;
            DateTime currentStartUtc = mList[0].DateTimeUtc;

            for (int i = 1; i < mList.Count; i++)
            {
                var item = mList[i];
                var nid = item.NakshatraId;

                // накшатра поменялась - закрываем предыдущий интервал
                if (nid != currentNakId)
                {
                    var slice = new NakshatraSlice
                    {
                        StartUtc = currentStartUtc,
                        EndUtc = item.DateTimeUtc, // до момента смены
                        NakshatraId = currentNakId,
                        ColorId = NakshatraSlice.GetNakshatraColorId(currentNakId),
                        NakshatraCode = (ENakshatra)currentNakId
                    };

                    result.Add(slice);

                    // начинаем новый интервал
                    currentNakId = nid;
                    currentStartUtc = item.DateTimeUtc;
                }
            }

            // закрываем последнюю накшатру до последней точки ряда
            var last = mList[mList.Count - 1];
            var lastSlice = new NakshatraSlice
            {
                StartUtc = currentStartUtc,
                EndUtc = last.DateTimeUtc,
                NakshatraId = currentNakId,
                ColorId = NakshatraSlice.GetNakshatraColorId(currentNakId),
                NakshatraCode = (ENakshatra)currentNakId
            };
            result.Add(lastSlice);

            return result;
        }

    }
}
