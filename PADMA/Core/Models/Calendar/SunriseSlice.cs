namespace PADMA.Core.TransitBuilder
{
    public class SunriseSlice
    {
        /// <summary>
        /// Восход предыдущего дня (UTC)
        /// </summary>
        public DateTime PreviousSunriseUtc { get; set; }

        /// <summary>
        /// Восход текущего дня (UTC)
        /// </summary>
        public DateTime SunriseUtc { get; set; }

        /// <summary>
        /// Закат текущего дня (UTC)
        /// </summary>
        public DateTime SunsetUtc { get; set; }

        /// <summary>
        /// Восход следующего дня (UTC)
        /// </summary>
        public DateTime NextSunriseUtc { get; set; }
    }
}
