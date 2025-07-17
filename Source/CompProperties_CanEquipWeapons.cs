using System.Collections.Generic;
using Verse;
namespace RoboticServitude
{
    public class CompProperties_CanEquipWeapons : CompProperties
    {
        public bool canEquipHeavyWeapons = false;

        public CompProperties_CanEquipWeapons()
        {
            this.compClass = typeof(CompCanEquipWeapons);
        }
    }
}
