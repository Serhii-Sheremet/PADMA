using System.Collections.Generic;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;

namespace PADMA.Core.TransitBuilder
{
    public static class NityaYogaTransitBuilder
    {
        public static List<NityaYogaSlice> BuildNityaYogaSlices(List<NityaYogaData> dataList)
        {
            var result = new List<NityaYogaSlice>();

            if (dataList == null || dataList.Count == 0)
                return result;

            for (int i = 0; i < dataList.Count; i++)
            {
                var current = dataList[i];
                var nextStart = (i < dataList.Count - 1)
                    ? dataList[i + 1].DateTimeUtc
                    : current.DateTimeUtc.AddDays(1); // last fallback

                var id = current.NityaYogaId;

                var slice = new NityaYogaSlice
                {
                    StartUtc = current.DateTimeUtc,
                    EndUtc = nextStart,
                    NityaYogaId = id,
                    ColorId = NityaYogaSlice.GetYogaColorId(id)
                };

                result.Add(slice);
            }

            return result;
        }
    }
}
