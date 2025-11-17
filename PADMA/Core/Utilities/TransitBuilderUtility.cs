using System;
using PADMA.Core.Services;
using PADMA.Core.Models;
using PADMA.Core.TransitBuilder;
using PADMA.Core.Enums;

namespace PADMA.Core.Utilities
{
    public static class TransitBuilderUtility
    {
        public static List<Nakshatra> SwapNakshatras(List<Nakshatra> nList, int birthNakshatraId)
        {
            return nList
                .Where(n => n.Id >= birthNakshatraId)
                .Concat(nList.Where(n => n.Id < birthNakshatraId))
                .ToList();
        }

        public static List<Zodiac> SwapZodiacs(List<Zodiac> zList, int id)
        {
            return zList
                .Where(z => z.Id >= id)
                .Concat(zList.Where(z => z.Id < id))
                .ToList();
        }

        public static int[,] MakeTaraBalaMatrix(List<Nakshatra> swapped)
        {
            int[,] arr = new int[9, 3];
            int index = 0;

            for (int col = 0; col < 3; col++)
                for (int row = 0; row < 9; row++)
                    arr[row, col] = swapped[index++].Id;

            return arr;
        }

        /// <summary>
        /// ¬озвращает (TaraBalaId 1..9, Percent 100/50/25) из уже предсобранной матрицы.
        /// </summary>
        public static (int taraBalaId, int percent) ComputeTaraBalaFromMatrix(int nakshatraId, int[,] matrix)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (matrix[row, col] == nakshatraId)
                    {
                        int id = row + 1;
                        int pct = (col == 0) ? 100 : (col == 1 ? 50 : 25);
                        return (id, pct);
                    }
                }
            }
            return (0, 0);
        }

        public static bool Overlap(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            return aStart < bEnd && bStart < aEnd;
        }

        public static DateTime Max(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }

        public static DateTime Min(DateTime a, DateTime b)
        {
            return a < b ? a : b;
        }


    }
}