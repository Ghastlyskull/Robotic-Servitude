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
        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddFinishAction((JobCondition cond) =>
            {

                if (Mech != null)
                {
                    if (Mech.jobs?.curJob != null)
                    {
                        Mech.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }

            });
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnForbidden(TargetIndex.A);
            int ticks = 2 * GenDate.TicksPerHour;
            Toil goToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return goToil;
            Toil toil = Toils_General.WaitWith(TargetIndex.A, ticks, useProgressBar: true, maintainPosture: false, maintainSleep: true, TargetIndex.A);
            toil.WithEffect(EffecterDefOf.MechRepairing, TargetIndex.A);
            toil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch);
            toil.AddEndCondition(() => Mech.InMentalState ? JobCondition.Ongoing : JobCondition.Succeeded);
            yield return toil;
            yield return Toils_General.Do(() =>
            {
                if (Mech != null)
                {
                    Mech.mindState.mentalStateHandler.CurState.RecoverFromState();
                }
            });
        }

    }
}
