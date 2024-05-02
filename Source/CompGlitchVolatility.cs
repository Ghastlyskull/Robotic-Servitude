using RimWorld;
using Verse;
namespace RoboticServitude
{
    public class CompGlitchVolatility : ThingComp
    {
        public CompProperties_GlitchVolatility Props => props as CompProperties_GlitchVolatility;
        public int? nextMentalBreakTick;
        public Effecter effecter;
        public override void CompTick()
        {
            base.CompTick();
            var pawn = parent as Pawn;
            if (pawn.InMentalState)
            {
                effecter ??= EffecterDefOf.ControlMech.SpawnAttached(pawn, pawn.Map);
                effecter.EffectTick(pawn, pawn);
            }
            else
            {
                effecter?.Cleanup();
                effecter = null;
                if (nextMentalBreakTick is null || nextMentalBreakTick == 0)
                {
                    SetNextMentalBreakTick();
                }
                if (nextMentalBreakTick != null && Find.TickManager.TicksGame >= nextMentalBreakTick)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(Props.mentalBreaks.RandomElement());
                    nextMentalBreakTick = null;
                }
            }
        }

        public void SetNextMentalBreakTick()
        {
            if (Props.mentalBreakRatePerDay > 0)
            {
                nextMentalBreakTick = (int)(Find.TickManager.TicksGame + (GenDate.TicksPerDay / Props.mentalBreakRatePerDay));
            }
            else
            {
                nextMentalBreakTick = null;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextMentalBreakTick, "nextMentalBreakTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (nextMentalBreakTick == 0)
                {
                    SetNextMentalBreakTick();
                }
            }
        }
    }
}
