using HarmonyLib;
using System;
using UnityEngine;
using Verse;
namespace RoboticServitude
{
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
}
