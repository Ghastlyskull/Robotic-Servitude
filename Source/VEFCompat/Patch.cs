using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using VEF.Apparels;
using Verse;
namespace VEFCompat
{
    [StaticConstructorOnStartup]
    public static class HarmonyStarter
    {
        static HarmonyStarter()
        {
            Log.Message("test");
            Harmony harmony = new Harmony("RoboticServitude.VEFCompat");
            harmony.PatchAll();
        }
    }
    [HarmonyPatch]
    public static class Patch_CanEquipHeavyWeapon
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(VEF.Weapons.VanillaExpandedFramework_EquipmentUtility_CanEquip_Patch), nameof(VEF.Weapons.VanillaExpandedFramework_EquipmentUtility_CanEquip_Patch.CanEquip));
        }
        public static void Postfix(ref bool __result, Pawn pawn)
        {
            Log.Message("Rah");
            if (!__result && pawn.HasComp<RoboticServitude.CompCanEquipWeapons>())
            {
                if (pawn.GetComp<RoboticServitude.CompCanEquipWeapons>().Props.canEquipHeavyWeapons)
                {
                    __result = true;
                }
            }
        }
    }
}
