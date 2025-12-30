using PADMA.Core.Enums;

namespace PADMA.Core.Utilities
{
    public static class ExaltationUtility
    {
        public static EExaltation GetPlanetExaltation(EPlanet planet, EZodiac zodiac) =>
            planet switch
            {
                EPlanet.MOON => zodiac == EZodiac.TAU ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.SCO ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                EPlanet.SUN => zodiac == EZodiac.ARI ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.LIB ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                EPlanet.VENUS => zodiac == EZodiac.PSC ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.VIR ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                EPlanet.JUPITER => zodiac == EZodiac.CNC ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.CAP ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                EPlanet.MERCURY => zodiac == EZodiac.VIR ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.PSC ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                EPlanet.MARS => zodiac == EZodiac.CAP ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.CNC ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                EPlanet.SATURN => zodiac == EZodiac.LIB ? EExaltation.EXALTATION :
                                   zodiac == EZodiac.ARI ? EExaltation.DEBILITATION : EExaltation.NOEXALTATION,

                _ => EExaltation.NOEXALTATION
            };

        public static bool IsExalted(EPlanet planet, EZodiac zodiac) =>
            GetPlanetExaltation(planet, zodiac) == EExaltation.EXALTATION;

        public static bool IsDebilitated(EPlanet planet, EZodiac zodiac) =>
            GetPlanetExaltation(planet, zodiac) == EExaltation.DEBILITATION;

    }
}