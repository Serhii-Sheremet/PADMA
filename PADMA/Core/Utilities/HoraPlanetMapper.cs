using PADMA.Core.Enums;

namespace PADMA.Core.Utilities
{
    public static class HoraPlanetMapper
    {
        public static EPlanet ToPlanet(EHoraPlanet horaPlanet)
        {
            return horaPlanet switch
            {
                EHoraPlanet.SUN => EPlanet.SUN,
                EHoraPlanet.MOON => EPlanet.MOON,
                EHoraPlanet.MARS => EPlanet.MARS,
                EHoraPlanet.MERCURY => EPlanet.MERCURY,
                EHoraPlanet.JUPITER => EPlanet.JUPITER,
                EHoraPlanet.VENUS => EPlanet.VENUS,
                EHoraPlanet.SATURN => EPlanet.SATURN,
                _ => EPlanet.SUN
            };
        }
    }
}
