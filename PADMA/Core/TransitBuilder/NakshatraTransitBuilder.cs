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

            for (int i = 0; i < mList.Count; i++)
            {
                var current = mList[i];
                var nextStart = (i < mList.Count - 1)
                    ? mList[i + 1].DateTimeUtc
                    : current.DateTimeUtc.AddDays(1);

                var nid = current.NakshatraId;
                var nCode = (ENakshatra)nid;

                var slice = new NakshatraSlice
                {
                    StartUtc = current.DateTimeUtc,
                    EndUtc = nextStart,
                    NakshatraId = nid,
                    ColorId = NakshatraSlice.GetNakshatraColorId(nid),
                    NakshatraCode = nCode
                };

                result.Add(slice);
            }

            return result;
        }
    }
}
