using PADMA.Core.Enums;
using PADMA.Core.Services;

namespace PADMA.Core.Models.Calendar
{
    public class MuhurtaSlice : CalendarSlice
    {
        /// <summary>
        /// Тип мухурты (Abhijit, Rahu, Brahma, Gulika, Yamaganda).
        /// Значения совпадают с MUHURTA.ID в БД.
        /// </summary>
        public EMuhurta MuhurtaCode { get; set; }

        /// <summary>
        /// Используется только для UI, чтобы показать конфликт двух мухурт.
        /// (Например, Rahu перекрывает Abhijit)
        /// </summary>
        public EMuhurta OverlappedMuhurtaCode { get; set; } = EMuhurta.NOMUHURTA;

        /// <summary>
        /// Читает MUHURTA.ID (совпадает с enum).
        /// </summary>
        public int MuhurtaId => (int)MuhurtaCode;

        /// <summary>
        /// Читает цвет из таблицы MUHURTA по ID.
        /// </summary>
        public int ColorId =>
            DataCache.Instance.MuhurtaList
                .FirstOrDefault(m => m.Id == (int)MuhurtaCode)
                ?.ColorId ?? 0;

        public MuhurtaSlice()
        {
            Kind = ETransitKind.Muhurta;
        }
    }
}
