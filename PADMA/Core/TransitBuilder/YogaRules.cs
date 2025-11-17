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
    }



}
