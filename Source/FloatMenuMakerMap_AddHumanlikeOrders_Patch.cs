using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RoboticServitude
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
        {
            if (pawn == null || clickPos == null || MechanitorUtility.IsMechanitor(pawn) is false)
                return;
            IntVec3 c = IntVec3.FromVector3(clickPos);
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (Thing thing in c.GetThingList(pawn.Map))
                {
                    if (thing is Pawn mech && mech.RaceProps.IsMechanoid && mech.Faction == Faction.OfPlayer && mech.InMentalState)
                    {
                        if (pawn.CanReserveAndReach(mech, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true))
                        {
                            if (mech.GetOverseer() == pawn)
                            {
                                opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RS.FixMalfunctions".Translate(mech.LabelShort), delegate
                                {
                                    Job job = JobMaker.MakeJob(RS_DefOf.RS_FixMalfunction, mech);
                                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                                }), pawn, new LocalTargetInfo(mech)));
                            }
                        }
                    }
                }
            }
        }
    }
}
