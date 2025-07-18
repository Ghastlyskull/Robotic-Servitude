using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RoboticServitude
{
    public class ThinkNode_ConditionalCanFix : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            //Log.Message(pawn.Label);
            if (pawn.kindDef != RS_DefOf.Gha_Skull_Laborer)
            {
                //Log.Message("Is not skull");
                return false;
            }
            //Log.Message("Is skull");
            return true;
        }
    }
}
