using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace RoboticServitude
{
    public class WorkGiver_FixMalfunction : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
        }

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !MechanitorUtility.IsMechanitor(pawn);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!ModLister.CheckBiotech("Fix malfunction"))
            {
                return false;
            }
            Pawn pawn2 = (Pawn)t;
            CompGlitchVolatility comp = t.TryGetComp<CompGlitchVolatility>();
            if (comp == null)
            {
                return false;
            }
            if (!pawn2.InMentalState)
            {
                return false;
            }
            if (!pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            if (!forced)
            {
                return comp.autoFix;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(RS_DefOf.RS_FixMalfunction, t);
        }
    }
}
