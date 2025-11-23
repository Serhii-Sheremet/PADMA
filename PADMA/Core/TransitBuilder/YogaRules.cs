using System;
using System.Collections.Generic;
using PADMA.Core.Enums;

namespace PADMA.Core.TransitBuilder
{
    public static partial class YogaRules
    {
        public static readonly Dictionary<EYoga, YogaRule> Rules =
            new Dictionary<EYoga, YogaRule>
            {
                {
                    EYoga.DWIPUSHKAR,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Tuesday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                        TithiIds = new[] { 2,7,12,17,22,27 },
                        Nakshatra = new[]
                        {
                            ENakshatra.MRIGASHIRA,
                            ENakshatra.CHITRA,
                            ENakshatra.DHANISHTA
                        }
                    }
                },

                {
                    EYoga.TRIPUSHKAR,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Tuesday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                        TithiIds = new[] { 2,7,12,17,22,27 },
                        Nakshatra = new[]
                        {
                            ENakshatra.KRITTIKA,
                            ENakshatra.PUNARVASU,
                            ENakshatra.UTTARAPHALGUNI,
                            ENakshatra.VISAKHA,
                            ENakshatra.UTTARAASHADHA,
                            ENakshatra.PURVABHADRAPADA
                        }
                    }
                },

                {
                    EYoga.AMRITASIDDHA,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                        // Особая логика в билдере
                        ForbiddenTithis = null,
                        TithiIds = null,
                        Nakshatra = null
                    }
                },

                {
                    EYoga.SARVARTHA,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },

                        // Логика целиком в BuildSarvartha()
                        TithiIds = null,
                        ForbiddenTithis = null,
                        Nakshatra = null,
                        MultiResultAllowed = true,
                        NeedsOverlap = false
                    }
                },

                {
                    EYoga.SIDDHA,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday
                        },
                
                        // Логика в билдере
                        TithiIds = null,
                        ForbiddenTithis = null,
                        Nakshatra = null,
                        MultiResultAllowed = true
                    }
                },

                {
                    EYoga.MRITYU,
                    new YogaRule
                    {
                        // Йога может формироваться в любой день недели,
                        // а конкретные титьи задаём в отдельной таблице MrityuTithisByVara.
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },

                        TithiIds = null,          
                        ForbiddenTithis = null,
                        Nakshatra = null,
                        MultiResultAllowed = true
                    }
                },

                {
                    EYoga.ADHAM,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                
                        TithiIds = null,
                        ForbiddenTithis = null,
                        Nakshatra = null,
                        MultiResultAllowed = true
                    }
                },

                {
                    EYoga.YAMAGHATA,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                        
                        TithiIds = null, // TITTHI не участвуют
                        ForbiddenTithis = null,
                        Nakshatra = null,  // Накшатра зависит от дня недели
                        MultiResultAllowed = true
                    }
                },

                {
                    EYoga.DAGDHA,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                
                        TithiIds = null,           // TITTHI не участвуют
                        ForbiddenTithis = null,
                        Nakshatra = null,          // накшатра зависит от дня недели
                        MultiResultAllowed = true
                    }
                },

                {
                    EYoga.UNFAVORABLE,
                    new YogaRule
                    {
                        Vara = new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                            DayOfWeek.Saturday,
                            DayOfWeek.Sunday
                        },
                
                        TithiIds = null,           // титхи выбираются из таблицы
                        ForbiddenTithis = null,
                        Nakshatra = null,          // накшатра не участвует
                        MultiResultAllowed = true
                    }
                }

            };
    }

    public static partial class YogaRules
    {
        public class YogaLargeRule
        {
            public HashSet<ENakshatra> AllowedNakshatras { get; init; } = new();
            public HashSet<int>? AllowedTithis { get; init; } = null; // null => ignore tithis
        }

        /// <summary>
        /// Правила Large Siddha по дням недели
        /// </summary>
        public static readonly Dictionary<DayOfWeek, YogaLargeRule> LargeSiddha =
            new Dictionary<DayOfWeek, YogaLargeRule>
            {
                {
                    DayOfWeek.Monday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.ROHINI,
                            ENakshatra.MRIGASHIRA,
                            ENakshatra.PUNARVASU,
                            ENakshatra.CHITRA,
                            ENakshatra.SHRAVANA,
                            ENakshatra.DHANISHTA,
                            ENakshatra.SHATABHISHA,
                            ENakshatra.PURVABHADRAPADA
                        },
                        AllowedTithis = new()
                        {
                            2, 7, 12, 17, 22, 27
                        }
                    }
                },

                {
                    DayOfWeek.Tuesday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.ASHWINI,
                            ENakshatra.MRIGASHIRA,
                            ENakshatra.UTTARAPHALGUNI,
                            ENakshatra.CHITRA,
                            ENakshatra.ANURADHA,
                            ENakshatra.MULA,
                            ENakshatra.DHANISHTA,
                            ENakshatra.PURVABHADRAPADA
                        },
                        AllowedTithis = null // Tuesday special case: ignore tithis
                    }
                },

                {
                    DayOfWeek.Wednesday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.ROHINI,
                            ENakshatra.MRIGASHIRA,
                            ENakshatra.ARDRA,
                            ENakshatra.UTTARAPHALGUNI,
                            ENakshatra.ANURADHA,
                            ENakshatra.UTTARAASHADHA
                        },
                        AllowedTithis = new()
                        {
                            2, 3, 7, 8, 12, 13, 17, 18, 22, 23, 27, 28
                        }
                    }
                },

                {
                    DayOfWeek.Thursday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.ASHWINI,
                            ENakshatra.PUNARVASU,
                            ENakshatra.PUSHYA,
                            ENakshatra.MAGHA,
                            ENakshatra.SWATI,
                            ENakshatra.PURVAASHADHA,
                            ENakshatra.PURVABHADRAPADA,
                            ENakshatra.REVATI
                        },
                        AllowedTithis = new()
                        {
                            4, 5, 7, 9, 13, 14, 19, 20, 22, 24, 28, 29
                        }
                    }
                },

                {
                    DayOfWeek.Friday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.ASHWINI,
                            ENakshatra.BHARANI,
                            ENakshatra.ARDRA,
                            ENakshatra.UTTARAPHALGUNI,
                            ENakshatra.CHITRA,
                            ENakshatra.SWATI,
                            ENakshatra.PURVAASHADHA,
                            ENakshatra.REVATI
                        },
                        AllowedTithis = new()
                        {
                            1, 2, 6, 7, 11, 12, 16, 17, 21, 22, 26, 27
                        }
                    }
                },

                {
                    DayOfWeek.Saturday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.ROHINI,
                            ENakshatra.SWATI,
                            ENakshatra.VISAKHA,
                            ENakshatra.ANURADHA,
                            ENakshatra.DHANISHTA,
                            ENakshatra.SHATABHISHA
                        },
                        AllowedTithis = new()
                        {
                            2, 4, 7, 9, 12, 14, 17, 19, 22, 24, 27, 29
                        }
                    }
                },

                {
                    DayOfWeek.Sunday,
                    new YogaLargeRule
                    {
                        AllowedNakshatras = new()
                        {
                            ENakshatra.PUSHYA,
                            ENakshatra.UTTARAPHALGUNI,
                            ENakshatra.HASTA,
                            ENakshatra.MULA,
                            ENakshatra.UTTARAASHADHA,
                            ENakshatra.SHRAVANA,
                            ENakshatra.UTTARABHADRAPADA
                        },
                        AllowedTithis = new()
                        {
                            1, 4, 6, 7, 12, 16, 19, 21, 22, 27
                        }
                    }
                }
            };

        /// <summary>
        /// Mrityu-yoga: разрешённые титхи по дням недели.
        /// </summary>
        public static readonly Dictionary<DayOfWeek, HashSet<int>> MrityuTithisByVara =
            new()
            {
                { DayOfWeek.Monday,    new HashSet<int> { 2, 7, 12, 17, 22, 27 } },
                { DayOfWeek.Tuesday,   new HashSet<int> { 1, 6, 11, 16, 21, 26 } },
                { DayOfWeek.Wednesday, new HashSet<int> { 3, 8, 13, 18, 23, 28 } },
                { DayOfWeek.Thursday,  new HashSet<int> { 4, 9, 14, 19, 24, 29 } },
                { DayOfWeek.Friday,    new HashSet<int> { 2, 7, 12, 17, 22, 27 } },
                { DayOfWeek.Saturday,  new HashSet<int> { 5, 10, 15, 20, 25, 30 } },
                { DayOfWeek.Sunday,    new HashSet<int> { 1, 6, 11, 16, 21, 26 } }
            };

        /// <summary>
        /// ADHAM-yoga: разрешённые тихти по дням недели.
        /// </summary>
        public static readonly Dictionary<DayOfWeek, HashSet<int>> AdhamTithisByVara =
            new()
            {
                { DayOfWeek.Monday,    new HashSet<int> { 11, 21 } },
                { DayOfWeek.Tuesday,   new HashSet<int> { 10, 25 } },
                { DayOfWeek.Wednesday, new HashSet<int> { 1,  9, 16, 24 } },
                { DayOfWeek.Thursday,  new HashSet<int> { 8, 23 } },
                { DayOfWeek.Friday,    new HashSet<int> { 7, 22 } },
                { DayOfWeek.Saturday,  new HashSet<int> { 6, 21 } },
                { DayOfWeek.Sunday,    new HashSet<int> { 7, 12, 22, 27 } }
            };

        /// <summary>
        /// YAMAGHATA-yoga: разрешённые накшатры по дням недели.
        /// </summary>
        public static readonly Dictionary<DayOfWeek, ENakshatra> YamaghataNakshatraByVara =
            new()
            {
                { DayOfWeek.Monday,    ENakshatra.VISAKHA },
                { DayOfWeek.Tuesday,   ENakshatra.ARDRA },
                { DayOfWeek.Wednesday, ENakshatra.MULA },
                { DayOfWeek.Thursday,  ENakshatra.KRITTIKA },
                { DayOfWeek.Friday,    ENakshatra.ROHINI },
                { DayOfWeek.Saturday,  ENakshatra.HASTA },
                { DayOfWeek.Sunday,    ENakshatra.MAGHA }
            };

        /// <summary>
        /// DAGDHA-yoga: разрешённые накшатры по дням недели.
        /// </summary>
        public static readonly Dictionary<DayOfWeek, ENakshatra> DagdhaNakshatraByVara =
            new()
            {
                { DayOfWeek.Monday,    ENakshatra.CHITRA },
                { DayOfWeek.Tuesday,   ENakshatra.UTTARAASHADHA },
                { DayOfWeek.Wednesday, ENakshatra.DHANISHTA },
                { DayOfWeek.Thursday,  ENakshatra.UTTARAPHALGUNI },
                { DayOfWeek.Friday,    ENakshatra.JYESHTHA },
                { DayOfWeek.Saturday,  ENakshatra.REVATI },
                { DayOfWeek.Sunday,    ENakshatra.BHARANI }
            };

        /// <summary>
        /// UNFAVORABLE-yoga: разрешённые титхи по дням недели.
        /// </summary>
        public static readonly Dictionary<DayOfWeek, HashSet<int>> UnfavorableTithisByVara =
            new()
            {
                { DayOfWeek.Monday,    new HashSet<int> { 6, 26 } },
                { DayOfWeek.Tuesday,   new HashSet<int> { 5, 7, 20, 22 } },
                { DayOfWeek.Wednesday, new HashSet<int> { 3, 8, 18, 23 } },
                { DayOfWeek.Thursday,  new HashSet<int> { 6, 9, 21, 24 } },
                { DayOfWeek.Friday,    new HashSet<int> { 8, 9, 10, 23, 24, 25 } },
                { DayOfWeek.Saturday,  new HashSet<int> { 7, 9, 11, 22, 24, 26 } },
                { DayOfWeek.Sunday,    new HashSet<int> { 4, 9 } }
            };
    }
}
