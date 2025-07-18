using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RoboticServitude
{
    public class JobGiver_LaborerRepairMech : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            //Log.Message("Trying to give job");
            Pawn laborerToFix = GetMechToRepair(pawn);
            //Log.Message("Found no laborer");
            if (laborerToFix != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.RepairMech, laborerToFix);
                return job;
            }
            return null;
        }
        public Pawn GetMechToRepair(Pawn pawn)
        {
            List<Pawn> potential = [..pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Where((c) =>
            {
                CompMechRepairable compMechRepairable = c.TryGetComp<CompMechRepairable>();
                if (compMechRepairable == null)
                {
                    return false;
                }
                if (!c.RaceProps.IsMechanoid)
                {
                    return false;
                }
                if (c.InAggroMentalState || c.HostileTo(pawn))
                {
                    return false;
                }
                if (!pawn.CanReserve(c, 1, -1, null))
                {
                    return false;
                }
                if (c.IsBurning())
                {
                    return false;
                }
                if (c.IsAttacking())
                {
                    return false;
                }
                if (!MechRepairUtility.CanRepair(c))
                {
                    return false;
                }
                return compMechRepairable.autoRepair;
            })];
            return (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, potential, PathEndMode.Touch, TraverseParms.For(pawn), 9999f, (Thing t) => pawn.CanReserve(t) && !t.IsForbidden(pawn));
        }
    }
}