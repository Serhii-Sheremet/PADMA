using System;
using PADMA.Core.Enums;

namespace PADMA.Core.Models
{
    public class MrityuBhagaData
    {
        public int PlanetId { get; set; }          // ID планеты (EPlanet)
        public int ZodiacId { get; set; }          // ID знака (EZodiac)
        public double Degree { get; set; }         // абсолютный градус 0–360 (из MRITYUBHAGA)
        public EAppSetting MrityuBhagaSetting { get; set; } // активный режим расчёта (Equal/Less/More/Ernst)

        public double LongitudeFrom { get; set; }  // начало диапазона попадания (°)
        public double LongitudeTo { get; set; }    // конец диапазона попадания (°)

        public DateTime DateFromUtc { get; set; }  // момент входа в диапазон
        public DateTime DateToUtc { get; set; }    // момент выхода

        public override string ToString() =>
            $"{((EPlanet)PlanetId),-8} | Zod={ZodiacId,2} | Deg={Degree,7:F3} | " +
            $"Lon[{LongitudeFrom:F2}–{LongitudeTo:F2}] | {DateFromUtc:yyyy-MM-dd HH:mm} → {DateToUtc:yyyy-MM-dd HH:mm} | Mode={MrityuBhagaSetting}";
    }
}
