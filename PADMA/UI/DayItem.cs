using System;

namespace PADMA.UI.Models
{
    /// <summary>
    /// Модель одного дня в календаре.
    /// </summary>
    public class DayItem
    {
        public int DayNumber { get; set; }      // число дня (1–31)
        public DateTime Date { get; set; }      // полная дата
        public bool IsCurrentMonth { get; set; } // принадлежит ли текущему месяцу
        public bool IsToday { get; set; }        // сегодняшний ли день
    }
}
