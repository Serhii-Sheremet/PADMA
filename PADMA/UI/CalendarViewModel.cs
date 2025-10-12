using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PADMA.Core.Services;

namespace PADMA.UI
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DayItem> Days { get; } = new ObservableCollection<DayItem>();

        private int _year;
        public int Year
        {
            get => _year;
            private set
            {
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        private int _month;
        public int Month
        {
            get => _month;
            private set
            {
                if (_month != value)
                {
                    _month = value;
                    OnPropertyChanged(nameof(Month));
                }
            }
        }

        // === ★ Добавлено: поддержка CultureCode ===
        private string _cultureCode;
        public string CultureCode
        {
            get => _cultureCode;
            private set
            {
                if (_cultureCode != value)
                {
                    _cultureCode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        public CultureInfo CurrentCulture =>
            !string.IsNullOrWhiteSpace(CultureCode)
                ? new CultureInfo(CultureCode)
                : CultureInfo.CurrentUICulture;

        // === Инициализация культуры из базы ===
        public void InitializeCulture()
        {
            try
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();
                var lang = db.GetCurrentLanguage();
                CultureCode = lang?.CultureCode ?? CultureInfo.CurrentUICulture.Name;
            }
            catch
            {
                CultureCode = CultureInfo.CurrentUICulture.Name;
            }

            OnPropertyChanged(nameof(CurrentCulture));
        }

        public void ReloadCultureAndRefresh()
        {
            try
            {
                var db = ServiceLocator.Services.GetService<DatabaseService>();

                // Принудительно перечитать активный язык (не полагаться на старые поля VM)
                var lang = db.GetCurrentLanguage();
                var newCulture = lang?.CultureCode ?? CultureInfo.CurrentUICulture.Name;

                if (!string.Equals(CultureCode, newCulture, StringComparison.OrdinalIgnoreCase))
                {
                    CultureCode = newCulture;
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
            catch
            {
                CultureCode = CultureInfo.CurrentUICulture.Name;
                OnPropertyChanged(nameof(CurrentCulture));
            }

            // Пересобрать всё, что зависит от культуры
            RefreshCalendar();
        }



        // === Опционально: обновлённый заголовок месяца (если где-то нужен)
        private string _monthTitle;
        public string MonthTitle
        {
            get => _monthTitle;
            private set
            {
                if (_monthTitle != value)
                {
                    _monthTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateMonthTitle()
        {
            try
            {
                MonthTitle = new DateTime(Year, Month, 1).ToString("MMMM yyyy", CurrentCulture);
            }
            catch
            {
                MonthTitle = $"{Year}-{Month:00}";
            }
        }
        // === ★ Конец блока культуры ===


        public CalendarViewModel()
        {
            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;
            GenerateDays(Year, Month);
        }

        /// <summary>
        /// Rebuild current month grid (apply first-day-of-week changes, etc.).
        /// </summary>
        public void RefreshCalendar()
        {
            GenerateDays(Year, Month);
        }

        /// <summary>
        /// Move month by offset (e.g., -1 = previous, +1 = next).
        /// </summary>
        public void MoveMonth(int offset)
        {
            var newDate = new DateTime(Year, Month, 1).AddMonths(offset);
            Year = newDate.Year;
            Month = newDate.Month;
            GenerateDays(Year, Month);
            UpdateMonthTitle(); // ★ добавлено, чтобы заголовок обновлялся по культуре
        }

        /// <summary>
        /// Core: build a 6x7 grid (42 days) based on the selected first day of week.
        /// </summary>
        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            var db = ServiceLocator.Services.GetService<DatabaseService>();
            var firstDay = db.GetFirstDayOfWeekFromDb(); // Sunday или Monday

            var firstOfMonth = new DateTime(Year, Month, 1);
            int shift = ((7 + (int)firstOfMonth.DayOfWeek - (int)firstDay) % 7);
            var startDate = firstOfMonth.AddDays(-shift);

            for (int i = 0; i < 42; i++)
            {
                DateTime date = startDate.AddDays(i);
                bool isCurrentMonth = (date.Month == month && date.Year == year);
                bool isToday = date.Date == DateTime.Today;

                Days.Add(new DayItem
                {
                    Date = date,
                    DayNumber = date.Day,
                    IsCurrentMonth = isCurrentMonth,
                    IsToday = isToday
                });
            }

            OnPropertyChanged(nameof(Days));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
