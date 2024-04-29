using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace RoboticServitude
{
    public class RoboticServitudeMod : Mod
    {
        public RoboticServitudeMod(ModContentPack pack) : base(pack)
        {
            new Harmony("RoboticServitudeMod").PatchAll();
        }
    }

    [HarmonyPatch(typeof(Widgets), "GetIconFor", new Type[] { typeof(ThingDef), typeof(ThingDef), typeof(ThingStyleDef), typeof(int?)})]
    public static class Widgets_GetIconFor_Patch
    {
        public static void Postfix(ref Texture2D __result, ThingDef thingDef)
        {
            if (__result == BaseContent.BadTex)
            {
                if (thingDef.race?.Humanlike ?? false)
                {
                    __result = ContentFinder<Texture2D>.Get("SkeletonIcon");
                }
            }
        }
    }

    [HarmonyPatch(typeof(Building_MechGestator), "Notify_HauledTo")]
    public static class Building_MechGestator_Notify_HauledTo_Patch
    {
        public static void Postfix(Building_MechGestator __instance)
        {
            var allThings = __instance.innerContainer.ToList();
            __instance.innerContainer.Clear();
            var pawnRace = __instance.ActiveBill.recipe.ProducedThingDef;
            var kind = DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(x => x.race == pawnRace);
            var pawn = PawnGenerator.GeneratePawn(kind);
            pawn.Kill(null);
            __instance.innerContainer.TryAdd(pawn.Corpse);
            foreach (var thing in allThings)
            {
                __instance.innerContainer.TryAdd(thing);
            }
        }
    }
}
