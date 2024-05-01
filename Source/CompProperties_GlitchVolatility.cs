using System.Collections.Generic;
using Verse;
namespace RoboticServitude
{
    public class CompProperties_GlitchVolatility : CompProperties
    {
        public float mentalBreakRatePerDay;
        public List<MentalStateDef> mentalBreaks;
        public CompProperties_GlitchVolatility()
        {
            this.compClass = typeof(CompGlitchVolatility);
        }
    }
}
