using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RoboticServitude
{
    public class JobDriver_FixMalfunction : JobDriver
    {
        protected Pawn Mech => (Pawn)job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Mech, job, 1, -1, null, errorOnFailed);
        }
        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnForbidden(TargetIndex.A);
            this.FailOn(() => Mech.IsAttacking());
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil toil = Toils_General.WaitWith(TargetIndex.A, 4 * GenDate.TicksPerHour, useProgressBar: true, maintainPosture: true, maintainSleep: true);
            toil.WithEffect(EffecterDefOf.MechRepairing, TargetIndex.A);
            toil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch);
            toil.AddFinishAction(delegate
            {
                Mech.mindState.mentalStateHandler.CurState.RecoverFromState();
                if (Mech.jobs?.curJob != null)
                {
                    Mech.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
            });
            toil.AddEndCondition(() => Mech.InMentalState ? JobCondition.Ongoing : JobCondition.Succeeded);
            yield return toil;
        }
    }
}
