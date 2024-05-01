using HarmonyLib;
using RimWorld;
using Verse;
namespace RoboticServitude
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "CanControlColonist", MethodType.Getter)]
    public static class ITab_Pawn_Gear_CanControlColonist
    {
        public static void Postfix(ITab_Pawn_Gear __instance, ref bool __result)
        {
            Pawn pawn = __instance.SelPawnForGear;
            var comp = pawn.GetComp<CompCanEquipWeapons>();
            if (comp != null)
            {
                __result = true;
            }
        }
    }

}
