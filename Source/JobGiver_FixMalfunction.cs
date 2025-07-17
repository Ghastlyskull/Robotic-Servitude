using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RoboticServitude
{
    internal class JobGiver_FixMalfunction : ThinkNode_JobGiver
    {

        protected override Job TryGiveJob(Pawn pawn)
        {
            //Log.Message("Trying to give job");
            Pawn laborerToFix = GetLaborerToFix(pawn);
            //Log.Message("Found no laborer");
            if (laborerToFix != null)
            {
                Job job = JobMaker.MakeJob(RS_DefOf.RS_FixMalfunction, laborerToFix);
                return job;
            }
            return null;
        }
        private Pawn GetLaborerToFix(Pawn pawn)
        {
            List<Pawn> possiblePawns = [.. pawn.Map.mapPawns.PawnsInFaction(pawn.Faction).Where((c) =>
            {
                if(!c.InMentalState)
                {
                    return false;
                }
                if(!c.HasComp<CompGlitchVolatility>()){
                    return false;
                }
                if(!c.GetComp<CompGlitchVolatility>().autoFix){
                    return false;
                }
                return true;
            })];
            return (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, possiblePawns, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, (Thing t) => pawn.CanReserve(t) && !t.IsForbidden(pawn));
        }
    }
}
