using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RoboticServitude
{
    public class FloatMenuOptionProvider_EquipMech : FloatMenuOptionProvider_Equip
    {
        protected override bool MechanoidCanDo => true;
        public override bool SelectedPawnValid(Pawn pawn, FloatMenuContext context)
        {
            if(!base.SelectedPawnValid(pawn, context))
            {
                return false;
            }
            if (!pawn.HasComp<CompCanEquipWeapons>())
            {
                return false;
            }
            return true;
        }

    }
}
