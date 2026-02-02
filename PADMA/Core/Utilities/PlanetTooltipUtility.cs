using System.Globalization;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;

namespace PADMA.Core.Utilities
{
    public static class PlanetTooltipUtility
    {
        public static string FormatDt(DateTime dt) => dt.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

        private static string TrimCommaSpace(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            s = s.Trim();
            if (s.EndsWith(","))
                s = s.Substring(0, s.Length - 1).TrimEnd();
            return s;
        }

        /// <summary>
        /// јналог PAD DateTimeUtils.GetIntervalIntersection.
        /// ¬озвращает пустой интервал (From=default, To=default), если пересечени€ нет.
        /// </summary>
        public static (DateTime From, DateTime To) GetIntersection(DateTime from1, DateTime to1, DateTime from2, DateTime to2)
        {
            // вне слева / вне справа (как в PAD: строгие < / >)
            if ((from1 < from2 && to1 < from2) || (from1 > to2 && to1 > to2))
                return (default, default);

            var from = from1 < from2 ? from2 : from1;
            var to = to1 > to2 ? to2 : to1;
            return (from, to);
        }

        /// <summary>
        /// —двигает список 108 пад так, чтобы стартовой стала birthPadaId (Pada.Id).
        /// Ёто пр€мой аналог твоего SortingPadaListByBirthMoonOrLagna, но уже без поиска по nakshatra+padaNumber.
        /// </summary>
        private static List<Pada> SwapPadaListByStartPadaId(IReadOnlyList<Pada> source, int birthPadaId)
        {
            var list = source.ToList();
            if (list.Count == 0) return new List<Pada>();
            if (birthPadaId <= 0) birthPadaId = list[0].Id;

            int idx = list.FindIndex(p => p.Id == birthPadaId);
            if (idx < 0) idx = 0;

            var res = new List<Pada>(list.Count);
            res.AddRange(list.Skip(idx));
            res.AddRange(list.Take(idx));
            return res;
        }

        /// <summary>
        /// PAD: GetSpecNavamsha(Pada sPada, lang)
        /// PADMA: берЄм имена из DataCache.SpecialNavamsaDescList.
        /// ¬озвращает строку вида ", xxx, yyy" или пусто.
        /// </summary>
        public static string GetSpecNavamsha(Pada sPada)
        {
            
            try
            {
                if (sPada == null) return string.Empty;
                if (string.IsNullOrWhiteSpace(sPada.SpecialNavamsa)) return string.Empty;

                var ids = sPada.SpecialNavamsa
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x.Trim()))
                    .ToArray();

                if (ids.Length == 0) return string.Empty;

                var parts = new List<string>();
                foreach (var id in ids)
                {
                    var text = GetSpecialNavamsaName(id);
                    if (!string.IsNullOrWhiteSpace(text))
                        parts.Add(text);
                }
                if (parts.Count == 0) return string.Empty;
                return ", " + string.Join(", ", parts);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetSpecialNavamsaName(int specialNavamsaId)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            return DataCache.Instance.SpecialNavamsaDescList
                .FirstOrDefault(r => r.SpecialNavamsaId == specialNavamsaId && r.LanguageCode.Equals(lang.ToString(), StringComparison.OrdinalIgnoreCase))
                ?.Name ?? string.Empty;
        }

        /// <summary>
        /// PAD: GetBadNavamsha(pId, lang)
        /// «десь pId = Pada.Id (1..108).
        /// ¬озвращает строку вида "36 Navamsa from Natal Moon, 55 Navamsa from Lagna, " (как в PAD).
        /// </summary>
        public static string GetBadNavamsha(int padaId, int birthPadaMoonId, int birthPadaLagnaId, Func<string, string> L)
        {
            var lang = DataCache.Instance.CurrentLanguageCode;
            string badNavamsha = string.Empty;
            int[] badNavamshaArray = new[] { 36, 55, 64, 72, 81, 88, 96 };

            var dc = DataCache.Instance;
            var swappedMoon = SwapPadaListByStartPadaId(dc.PadaList, birthPadaMoonId);
            var swappedLagna = SwapPadaListByStartPadaId(dc.PadaList, birthPadaLagnaId);

            int indexMoon = swappedMoon.FindIndex(p => p.Id == padaId);
            int indexLagna = swappedLagna.FindIndex(p => p.Id == padaId);

            for (int i = 0; i < badNavamshaArray.Length; i++)
            {
                int badPos = badNavamshaArray[i];

                if ((indexMoon + 1) == badPos)
                    badNavamsha += badPos + " " + L("Navamsa from Natal Moon") + ", ";

                if ((indexLagna + 1) == badPos)
                    badNavamsha += badPos + " " + L("Navamsa from Lagna") + ", ";
            }

            return badNavamsha;
        }

        /// <summary>
        /// PAD: GetBadDrekkanaList(padaId)
        /// padaId = Pada.Id (1..108)
        /// </summary>
        public static List<DrekkanaEntity> GetBadDrekkanaList(int padaId, int birthPadaMoonId, int birthPadaLagnaId)
        {
            var result = new List<DrekkanaEntity>();
            var dc = DataCache.Instance;

            var swappedMoon = SwapPadaListByStartPadaId(dc.PadaList, birthPadaMoonId);
            var swappedLagna = SwapPadaListByStartPadaId(dc.PadaList, birthPadaLagnaId);

            if (swappedMoon.Count == 0 || swappedLagna.Count == 0)
                return result;

            int birthMoonDrekkana = swappedMoon[0].Drekkana;
            int birthLagnaDrekkana = swappedLagna[0].Drekkana;

            int currentMoonD = 0;
            for (int i = 0; i < swappedMoon.Count; i++)
            {
                var p = swappedMoon[i];

                if ((currentMoonD + p.Drekkana) == (birthMoonDrekkana + 15) && p.Id == padaId)
                {
                    result.Add(new DrekkanaEntity { Drekkana = 16, NakshatraId = p.NakshatraId, PadaId = p.Id, IsLagna = false });
                }
                if ((currentMoonD + p.Drekkana) == (birthMoonDrekkana + 21) && p.Id == padaId)
                {
                    result.Add(new DrekkanaEntity { Drekkana = 22, NakshatraId = p.NakshatraId, PadaId = p.Id, IsLagna = false });
                }
                if (i > 2 && swappedMoon[i - 2].Drekkana == 36)
                    currentMoonD = 36;
            }

            int currentLagnaD = 0;
            for (int i = 0; i < swappedLagna.Count; i++)
            {
                var p = swappedLagna[i];

                if ((currentLagnaD + p.Drekkana) == (birthLagnaDrekkana + 15) && p.Id == padaId)
                {
                    result.Add(new DrekkanaEntity { Drekkana = 16, NakshatraId = p.NakshatraId, PadaId = p.Id, IsLagna = true });
                }
                if ((currentLagnaD + p.Drekkana) == (birthLagnaDrekkana + 21) && p.Id == padaId)
                {
                    result.Add(new DrekkanaEntity { Drekkana = 22, NakshatraId = p.NakshatraId, PadaId = p.Id, IsLagna = true });
                }
                if (i > 2 && swappedLagna[i - 2].Drekkana == 36)
                    currentLagnaD = 36;
            }

            return result;
        }

        /// <summary>
        /// PAD: CheckIfVedhaExistforPlanetCalendarTimeFrame
        /// </summary>
        public static bool CheckIfVedhaExist(EPlanet target, EPlanet vedhaPlanet)
        {
            if (target == EPlanet.SUN && vedhaPlanet == EPlanet.SATURN) return false;
            if (target == EPlanet.SATURN && vedhaPlanet == EPlanet.SUN) return false;
            if (target == EPlanet.MOON && vedhaPlanet == EPlanet.MERCURY) return false;
            if (target == EPlanet.MERCURY && vedhaPlanet == EPlanet.MOON) return false;
            return true;
        }

        /// <summary>
        /// PAD: PrepareVedhaPlanetList(...) но на PlanetSlice.
        /// »щет у каждой планеты слайсы в vedhaDom и пересекает врем€ с target.
        /// </summary>
        public static List<VedhaEntity> PrepareVedhaPlanetList(
            PlanetSlice targetSlice,
            IReadOnlyDictionary<EPlanet, IReadOnlyList<PlanetSlice>> transitPack,
            int vedhaDom,
            bool isLagna,
            EAppSetting nodeType)
        {
            var vList = new List<VedhaEntity>();
            var targetPlanet = (EPlanet)targetSlice.PlanetId;

            foreach (var kvp in transitPack)
            {
                var vedhaPlanet = kvp.Key;
                if (vedhaPlanet == targetPlanet) continue;
                if (!CheckIfVedhaExist(targetPlanet, vedhaPlanet)) continue;

                var slices = kvp.Value;
                if (slices == null || slices.Count == 0) continue;

                foreach (var s in slices)
                {
                    int dom = isLagna ? s.HouseFromLagna : s.HouseFromMoon;
                    if (dom != vedhaDom) continue;

                    // пересечение в UTC (как безопаснее)
                    var (from, to) = GetIntersection(targetSlice.StartUtc, targetSlice.EndUtc, s.StartUtc, s.EndUtc);
                    if (from == default && to == default) continue;

                    vList.Add(new VedhaEntity
                    {
                        PlanetCode = vedhaPlanet,
                        DateStart = from,
                        DateEnd = to
                    });
                }
            }

            return vList;
        }
    }
}