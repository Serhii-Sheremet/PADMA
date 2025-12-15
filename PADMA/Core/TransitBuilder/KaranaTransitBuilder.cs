using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;

namespace PADMA.Core.TransitBuilder
{
    public static class KaranaTransitBuilder
    {
        public static List<KaranaSlice> BuildKaranaSlices(List<TithiSlice> tithiSlices)
        {
            var result = new List<KaranaSlice>();

            if (tithiSlices == null || tithiSlices.Count == 0)
                return result;

            foreach (var t in tithiSlices)
            {
                var midUtc = t.StartUtc + (t.EndUtc - t.StartUtc) / 2;

                // First karana (POSITION=1)
                var k1 = DataCache.Instance.KaranaList.FirstOrDefault(k =>
                    k.TithiId == t.TithiId && k.Position == 1);

                if (k1 != null)
                {
                    result.Add(new KaranaSlice
                    {
                        StartUtc = t.StartUtc,
                        EndUtc = midUtc,
                        KaranaId = k1.Id,
                        ColorId = KaranaSlice.GetKaranaColorId(k1.Id)
                    });
                }

                // Second karana (POSITION=2)
                var k2 = DataCache.Instance.KaranaList.FirstOrDefault(k =>
                    k.TithiId == t.TithiId && k.Position == 2);

                if (k2 != null)
                {
                    result.Add(new KaranaSlice
                    {
                        StartUtc = midUtc,
                        EndUtc = t.EndUtc,
                        KaranaId = k2.Id,
                        ColorId = KaranaSlice.GetKaranaColorId(k2.Id)
                    });
                }
            }

            return result;
        }
    }
}
