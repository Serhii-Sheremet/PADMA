using System.Collections.Generic;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Utilities;
using PADMA.Core.Services;

namespace PADMA.Core.TransitBuilder
{
    public static class TaraBalaTransitBuilder
    {
        public static List<TaraBalaSlice> BuildTaraBalaSlices(
            List<NakshatraSlice> nsList,
            int birthNakshatraId)
        {
            var result = new List<TaraBalaSlice>();

            if (nsList == null || nsList.Count == 0)
                return result;

            // Prepare data for Tara-Bala calculation
            var swapped = TransitBuilderUtility.SwapNakshatras(
                DataCache.Instance.NakshatraList.ToList(),
                birthNakshatraId);

            var matrix = TransitBuilderUtility.MakeTaraBalaMatrix(swapped);

            // Build slices
            foreach (var ns in nsList)
            {
                (var tbId, var tbPct) =
                    TransitBuilderUtility.ComputeTaraBalaFromMatrix(
                        ns.NakshatraId, matrix);

                var slice = new TaraBalaSlice
                {
                    StartUtc = ns.StartUtc,
                    EndUtc = ns.EndUtc,
                    TaraBalaId = tbId,
                    TaraBalaPercent = tbPct,
                    ColorId = TaraBalaSlice.GetTaraBalaColorId(tbId, tbPct)
                };

                result.Add(slice);
            }

            return result;
        }
    }
}
