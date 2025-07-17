using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RoboticServitude
{
    public class RoboticServitudeMod : Mod
    {
        public RoboticServitudeSettings settings;
        public RoboticServitudeMod(ModContentPack pack) : base(pack)
        {
            new Harmony("RoboticServitudeMod").PatchAll();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                settings = GetSettings<RoboticServitudeSettings>();
            });
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            Startup.ApplySettings();
            if (Current.Game != null)
            {
                foreach (var pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive)
                {
                    var comp = pawn.GetComp<CompGlitchVolatility>();
                    if (comp != null)
                    {
                        comp.SetNextMentalBreakTick();
                    }
                }
            }
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }

    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            ApplySettings();
        }

        public static void ApplySettings()
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.race != null)
                {
                    var comp = thingDef.GetCompProperties<CompProperties_GlitchVolatility>();
                    if (comp != null)
                    {
                        if (RoboticServitudeSettings.mentalBreakRates.TryGetValue(thingDef, out var rate))
                        {
                            comp.mentalBreakRatePerDay = rate;
                        }
                        else
                        {
                            RoboticServitudeSettings.mentalBreakRates[thingDef] = comp.mentalBreakRatePerDay;
                        }
                    }
                }
            }
        }
    }

    public class RoboticServitudeSettings : ModSettings
    {
        public static Dictionary<ThingDef, float> mentalBreakRates = new Dictionary<ThingDef, float>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mentalBreakRates, "mentalBreakRates", LookMode.Def, LookMode.Value);
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Adjust mental break rates per laborer types. 1 means once per day, 0 means it never occurs.");
            foreach (var data in mentalBreakRates.ToList())
            {
                mentalBreakRates[data.Key] = listing.SliderLabeled(data.Key.LabelCap + ": " + data.Value.ToStringDecimalIfSmall(), data.Value, 0, 10);
            }
            listing.End();
        }
    }
}
