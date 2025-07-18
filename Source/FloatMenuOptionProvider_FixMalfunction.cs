using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static RimWorld.MechClusterSketch;
using Verse.AI;

namespace RoboticServitude
{
    public class FloatMenuOptionProvider_FixMalfunction : FloatMenuOptionProvider
    {
        private bool isMechanitor = false;
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;
        protected override bool RequiresManipulation => true;
        protected override bool MechanoidCanDo => true;
        public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
        {
           //Log.Message("testing if target is valid");
            if (!base.TargetPawnValid(pawn, context))
            {
                //Log.Message("Base invalid");
                return false;
            }
            if (!pawn.RaceProps.IsMechanoid)
            {
                //Log.Message("Not mech");
                return false;
            }
            if (pawn.Faction != Faction.OfPlayer)
            {
                //Log.Message("Not player mech");
                return false;
            }
            if(!pawn.HasComp<CompGlitchVolatility>())
            {
                return false;
            }
            if (!pawn.InMentalState)
            {
                //Log.Message("Not in mental state");
                return false;
            }
            if (isMechanitor && pawn.GetOverseer() != context.FirstSelectedPawn)
            {
                //Log.Message("Mechanitor present but target not controlled by them");
                return false;
            }
            return true;
        }
        public override bool SelectedPawnValid(Pawn pawn, FloatMenuContext context)
        {
            if (!base.SelectedPawnValid(pawn, context))
            {
                //Log.Message("Base selected invalid");
                return false;
            }
            if (MechanitorUtility.IsMechanitor(pawn))
            {
                //Log.Message("Is mechanitor");
                isMechanitor = true;
            }
            else
            {
                isMechanitor = false;
                if (!(pawn.IsColonyMech && pawn.kindDef == RS_DefOf.Gha_Skull_Laborer))
                {
                    //Log.Message("Is not skull");
                    return false;
                }
               //Log.Message("Is skull");
            }
            return true;
        }
        
        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (!context.FirstSelectedPawn.CanReserve(clickedPawn))
            {
                return new FloatMenuOption("RS.CannotFix".Translate() + ": " + "Reserved".Translate().CapitalizeFirst(), null);
            }
            if (!context.FirstSelectedPawn.CanReach(clickedPawn, PathEndMode.ClosestTouch, Danger.Deadly))
            {
                return new FloatMenuOption("RS.CannotFix".Translate() + ": " + "NoPath".Translate().CapitalizeFirst(), null);
            }
            return new FloatMenuOption("RS.FixMalfunctions".Translate(clickedPawn.LabelShort), delegate
            {
                Job job = JobMaker.MakeJob(RS_DefOf.RS_FixMalfunction, clickedPawn);
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
        }

    }
}
