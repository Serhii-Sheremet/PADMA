using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;

namespace PADMA.Core.TransitBuilder
{
    public static class LagnaTransitBuilder
    {
        public static List<LagnaSlice> BuildLagnaSlices(List<LagnaData> list)
        {
            var result = new List<LagnaSlice>();
            if (list.Count == 0) return result;

            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];

                var slice = new LagnaSlice
                {
                    StartUtc = d.DateTimeUtc,
                    EndUtc = (i < list.Count - 1) ? list[i + 1].DateTimeUtc : d.DateTimeUtc,

                    ZodiacId = d.ZodiacId,
                    NakshatraId = d.NakshatraId,
                    PadaId = d.PadaId,
                    NavamsaZodiacId = d.NavamsaZodiacId
                };

                result.Add(slice);
            }

            return result;
        }
    }
}
