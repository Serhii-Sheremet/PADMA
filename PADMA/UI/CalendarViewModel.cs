using PADMA.Core.TransitBuilder;
using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // === Добавлено: поддержка CultureCode ===
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

        // === Инициализация культуры ===
        public void InitializeCulture()
        {
            ReloadCultureAndRefresh();
        }

        public void ReloadCultureAndRefresh()
        {
            try
            {
                var code = DataCache.Instance.CurrentLanguageCode;
                string newCulture = code switch
                {
                    "en" => "en-US",
                    "uk" => "uk-UA",
                    "pl" => "pl-PL",
                    "ru" => "ru-RU",
                    _ => CultureInfo.CurrentUICulture.Name
                };

                if (!string.Equals(CultureCode, newCulture, StringComparison.OrdinalIgnoreCase))
                {
                    CultureCode = newCulture;
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"[PADMA] ReloadCultureAndRefresh failed: {ex.Message}");
                CultureCode = CultureInfo.CurrentUICulture.Name;
                OnPropertyChanged(nameof(CurrentCulture));
            }

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
        // === Конец блока культуры ===


        public CalendarViewModel()
        {
            DataCache.Instance.ProfileContextUpdated += () => RefreshCalendar();

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
            UpdateMonthTitle(); // добавлено, чтобы заголовок обновлялся по культуре
        }

        /// <summary>
        /// Core: build a 6x7 grid (42 days) based on the selected first day of week.
        /// </summary>

        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            TimeZoneInfo? tzInfo = null;
            int birthNakshatraMoonId = 0, birthZodiacMoonId = 0, birthLagnaId = 0;
            List<PlanetSlice>? moonSlices = null;
            List<NakshatraSlice>? nakshatraSlices = null;
            List<TaraBalaSlice>? taraBalaSlices = null;
            List<TithiSlice>? tithiSlices = null;
            List<KaranaSlice>? karanaSlices = null;
            List<NityaYogaSlice>? nityaYogaSlices = null;
            List<ChandraBalaSlice>? chandraBalaSlices = null;
            Profile? profile = DataCache.Instance.ActiveProfile;
            var ctx = DataCache.Instance.ProfileContextService.Current;

            IReadOnlyList<DateOnly> visibleDays;
            if (profile != null && ctx != null)
            {
                tzInfo = ctx.TimeZoneInfo;
                birthNakshatraMoonId = ctx.BirthNakshatraMoonId;
                birthZodiacMoonId = ctx.BirthZodiacMoonId;
                birthLagnaId = ctx.BirthLagnaId;

                // Считаем окно календаря (если есть таймзона)
                DateTimeOffset visibleStart, visibleEnd, bufferStart, bufferEnd;

                (visibleStart, visibleEnd, bufferStart, bufferEnd, visibleDays) =
                    CalendarWindowService.BuildWindow(year, month, DataCache.Instance.DayOfWeek, tzInfo);

                var bufferStartUtc = bufferStart.UtcDateTime;
                var bufferEndUtc = bufferEnd.UtcDateTime;

                // ---- Swiss + Moon Slices + Nakshatra (Луна) + TaraBala ----
                var moonData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.MOON, bufferStartUtc, bufferEndUtc);
                moonSlices = PlanetTransitBuilder.BuildPlanetSlices(moonData, birthNakshatraMoonId, birthZodiacMoonId, birthLagnaId, ctx.NodeType);
                nakshatraSlices = NakshatraTransitBuilder.BuildNakshatraSlices(moonData);
                if (birthNakshatraMoonId > 0)
                    taraBalaSlices = TaraBalaTransitBuilder.BuildTaraBalaSlices(nakshatraSlices, birthNakshatraMoonId);

                // ---- Swiss + Tithi —---
                var tithiData = SwissAnalysis.CalculateTithiDataList_London(bufferStartUtc, bufferEndUtc);
                tithiSlices = TithiTransitBuilder.BuildTithiSlices(tithiData);

                // ---- Karana ----
                karanaSlices = KaranaTransitBuilder.BuildKaranaSlices(tithiSlices);

                // ---- Swiss + Nitya Yoga ----
                var nityaYogaData = SwissAnalysis.CalculateNityaYogaDataList_London(bufferStartUtc, bufferEndUtc);
                nityaYogaSlices = NityaYogaTransitBuilder.BuildNityaYogaSlices(nityaYogaData);

                // ---- Chandra Bala ----
                chandraBalaSlices = ChandraBalaTransitBuilder.BuildChandraBalaSlices(moonSlices, birthZodiacMoonId);
            }
            else
            {
                // если профиля/локации/таймзоны нет — строим видимые дни по старой логике
                var firstOfMonth = new DateTime(year, month, 1);
                int shift = ((7 + (int)firstOfMonth.DayOfWeek - (int)DataCache.Instance.DayOfWeek) % 7);
                var startDate = firstOfMonth.AddDays(-shift);

                var tmp = new List<DateOnly>();
                for (int i = 0; i < 42; i++)
                    tmp.Add(DateOnly.FromDateTime(startDate.AddDays(i)));

                visibleDays = tmp;
            }

            // Создаём 42 DayItem строго по visibleDays
            foreach (var d in visibleDays)
            {
                var date = d.ToDateTime(TimeOnly.MinValue);
                bool isCurrentMonth = (date.Month == month && date.Year == year);
                bool isToday = date.Date == DateTime.Today;

                IList<PanchangaSegment> nakshatraSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> taraBalaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> tithiSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> karanaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> nityaYogaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> chandraBalaSegments = new List<PanchangaSegment>();

                if (profile != null && tzInfo != null)
                {
                    if (nakshatraSlices != null)
                    {
                        nakshatraSegments = PanchangaHelper.BuildSegmentsForDay(
                            nakshatraSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId);
                    }

                    if (taraBalaSlices != null)
                    {
                        taraBalaSegments = PanchangaHelper.BuildSegmentsForDay(
                            taraBalaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId);
                    }

                    if (tithiSlices != null)
                    {
                        tithiSegments = PanchangaHelper.BuildSegmentsForDay(
                            tithiSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId);
                    }

                    if(karanaSlices != null)
                    {
                         karanaSegments = PanchangaHelper.BuildSegmentsForDay(
                             karanaSlices, date, tzInfo, DataCache.Instance,
                             slice => (EColor)slice.ColorId);
                    }   

                    if(nityaYogaSlices != null)
                    {
                        nityaYogaSegments = PanchangaHelper.BuildSegmentsForDay(
                            nityaYogaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId);
                    }

                    if(chandraBalaSlices != null)
                    {
                        chandraBalaSegments = PanchangaHelper.BuildSegmentsForDay(
                            chandraBalaSlices, date, tzInfo, DataCache.Instance,
                            slice => (EColor)slice.ColorId);
                    }
                }

                Days.Add(new DayItem
                {
                    Date = date,
                    DayNumber = date.Day,
                    IsCurrentMonth = isCurrentMonth,
                    IsToday = isToday,
                    NakshatraSegments = nakshatraSegments,
                    TaraBalaSegments = taraBalaSegments,
                    TithiSegments = tithiSegments,
                    KaranaSegments = karanaSegments,
                    NityaYogaSegments = nityaYogaSegments,
                    ChandraBalaSegments = chandraBalaSegments
                });
            }
            
            OnPropertyChanged(nameof(Days));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
