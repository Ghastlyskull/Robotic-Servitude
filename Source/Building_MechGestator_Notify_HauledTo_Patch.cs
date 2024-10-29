using System;
using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
namespace RoboticServitude
{
    // Harmony patch targeting the Notify_HauledTo method of Building_MechGestator
    [HarmonyPatch(typeof(Building_MechGestator), "Notify_HauledTo")]
    public static class Building_MechGestator_Notify_HauledTo_Patch
    {
        // Version identifier for tracking updates in the development log
        private const string VersionIdentifier = "Building_MechGestator_Notify_HauledTo_Patch_v1.1";

        public static void Postfix(Building_MechGestator __instance)
        {
            // Log to confirm that the new version is active
            Log.Message($"[{VersionIdentifier}] - Notify_HauledTo patch is active.");

            // Ensure that ActiveBill and its recipe are not null before proceeding
            if (__instance?.ActiveBill?.recipe != null &&
                (DefDatabase<RecipeDef>.GetNamed("Gha_Laborer") == __instance.ActiveBill.recipe ||
                 DefDatabase<RecipeDef>.GetNamed("Gha_Combat_Laborer") == __instance.ActiveBill.recipe ||
                 DefDatabase<RecipeDef>.GetNamed("Gha_Assassin_Laborer") == __instance.ActiveBill.recipe))
            {
                // Log the active recipe for debugging purposes
                Log.Message($"[{VersionIdentifier}] - Active Recipe Detected: {__instance.ActiveBill.recipe.defName}");

                // Store current contents of the innerContainer before modifications
                var allThings = __instance.innerContainer.ToList();
                Log.Message($"[{VersionIdentifier}] - Items in innerContainer before Clear: {string.Join(", ", allThings.Select(x => x.LabelCap))}");

                // Clear the container to prepare for new contents
                __instance.innerContainer.Clear();

                // Retrieve the ProducedThingDef from the active recipe
                var pawnRace = __instance.ActiveBill.recipe.ProducedThingDef;
                if (pawnRace == null)
                {
                    Log.Error($"[{VersionIdentifier}] - ProducedThingDef is null. Ensure the recipe is configured correctly.");
                    return;
                }

                // Find the PawnKindDef associated with the ProducedThingDef's race
                var kind = DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(x => x.race == pawnRace);
                if (kind == null)
                {
                    Log.Error($"[{VersionIdentifier}] - Could not find PawnKindDef for race: {pawnRace.defName}");
                    return;
                }

                try
                {
                    // Generate a new pawn of the specified kind and immediately kill it to get the corpse
                    var pawn = PawnGenerator.GeneratePawn(kind);
                    pawn.Kill(null);

                    // Check if the pawn's corpse is valid before adding it back to the container
                    if (pawn.Corpse != null)
                    {
                        Log.Message($"[{VersionIdentifier}] - Generated Pawn Corpse: {pawn.Corpse.LabelCap}");
                        __instance.innerContainer.TryAdd(pawn.Corpse);
                    }
                    else
                    {
                        Log.Error($"[{VersionIdentifier}] - Generated pawn does not have a valid corpse!");
                    }
                }
                catch (Exception ex)
                {
                    // Catch any exceptions during the pawn generation process
                    Log.Error($"[{VersionIdentifier}] - An error occurred during pawn generation: {ex.Message}\n{ex.StackTrace}");
                    return;
                }

                // Re-add the original contents back to the innerContainer
                foreach (var thing in allThings)
                {
                    __instance.innerContainer.TryAdd(thing);
                }

                // Log the final state of the innerContainer after modifications
                Log.Message($"[{VersionIdentifier}] - Items in innerContainer after re-adding: {string.Join(", ", __instance.innerContainer.Select(x => x.LabelCap))}");
            }
        }
    }
}
