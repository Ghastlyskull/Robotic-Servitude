using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace RoboticServitude
{
    public class CompGlitchVolatility : ThingComp
    {
        public bool autoFix = false;
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
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.Faction == Faction.OfPlayer)
            {
                Command_Toggle command_Toggle = new Command_Toggle();
                command_Toggle.defaultLabel = "RS.AutoFix".Translate();
                command_Toggle.defaultDesc = "RS.AutoFixDesc".Translate();
                command_Toggle.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AutoRepair");
                command_Toggle.isActive = () => autoFix;
                command_Toggle.toggleAction = (Action)Delegate.Combine(command_Toggle.toggleAction, (Action)delegate
                {
                    autoFix = !autoFix;
                });
                yield return command_Toggle;
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
            Scribe_Values.Look(ref autoFix, "autoFix", false);
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
