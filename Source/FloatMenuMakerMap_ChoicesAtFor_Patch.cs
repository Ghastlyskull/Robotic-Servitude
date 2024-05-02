using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RoboticServitude
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "ChoicesAtFor")]
    public static class FloatMenuMakerMap_ChoicesAtFor_Patch
    {
        public static void Postfix(ref List<FloatMenuOption> __result, Vector3 clickPos, Pawn pawn, bool suppressAutoTakeableGoto = false)
        {
            if (!pawn.RaceProps.Humanlike)
            {
                var compMachine = pawn.GetComp<CompCanEquipWeapons>();
                if (compMachine != null)
                {
                    var c = IntVec3.FromVector3(clickPos);
                    ThingWithComps equipment = null;
                    var thingList2 = c.GetThingList(pawn.Map);
                    for (int i = 0; i < thingList2.Count; i++)
                    {
                        if (thingList2[i].TryGetComp<CompEquippable>() != null)
                        {
                            equipment = (ThingWithComps)thingList2[i];
                            break;
                        }
                    }
                    if (equipment != null)
                    {
                        string labelShort = equipment.LabelShort;
                        FloatMenuOption item6;
                        if (equipment.def.IsWeapon && pawn.WorkTagIsDisabled(WorkTags.Violent))
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "IsIncapableOfViolenceLower".Translate(pawn.LabelShort, pawn), null);
                        }
                        else if (equipment.def.IsRangedWeapon && pawn.WorkTagIsDisabled(WorkTags.Shooting))
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "IsIncapableOfShootingLower".Translate(pawn), null);
                        }
                        else if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly))
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "NoPath".Translate().CapitalizeFirst(), null);
                        }
                        else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "Incapable".Translate(), null);
                        }
                        else if (equipment.IsBurning())
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "BurningLower".Translate(), null);
                        }
                        else if (pawn.IsQuestLodger() && !EquipmentUtility.QuestLodgerCanEquip(equipment, pawn))
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + "QuestRelated".Translate().CapitalizeFirst(), null);
                        }
                        else if (!EquipmentUtility.CanEquip(equipment, pawn, out string cantReason, checkBonded: false))
                        {
                            item6 = new FloatMenuOption("CannotEquip".Translate(labelShort) + ": " + cantReason.CapitalizeFirst(), null);
                        }
                        else
                        {
                            string text4 = "Equip".Translate(labelShort);
                            if (equipment.def.IsRangedWeapon && pawn.story != null && pawn.story.traits.HasTrait(TraitDefOf.Brawler))
                            {
                                text4 += " " + "EquipWarningBrawler".Translate();
                            }
                            if (EquipmentUtility.AlreadyBondedToWeapon(equipment, pawn))
                            {
                                text4 += " " + "BladelinkAlreadyBonded".Translate();
                                var dialogText = "BladelinkAlreadyBondedDialog".Translate(pawn.Named("PAWN"), equipment.Named("WEAPON"), pawn.equipment.bondedWeapon.Named("BONDEDWEAPON"));
                                item6 = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text4, delegate
                                {
                                    Find.WindowStack.Add(new Dialog_MessageBox(dialogText));
                                }, MenuOptionPriority.High), pawn, equipment);
                            }
                            else
                            {
                                item6 = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text4, delegate
                                {
                                    string personaWeaponConfirmationText = EquipmentUtility.GetPersonaWeaponConfirmationText(equipment, pawn);
                                    if (!personaWeaponConfirmationText.NullOrEmpty())
                                    {
                                        Find.WindowStack.Add(new Dialog_MessageBox(personaWeaponConfirmationText, "Yes".Translate(), delegate
                                        {
                                            Equip();
                                        }, "No".Translate()));
                                    }
                                    else
                                    {
                                        Equip();
                                    }
                                }, MenuOptionPriority.High), pawn, equipment);
                            }
                        }
                        __result.Add(item6);
                    }

                    void Equip()
                    {
                        equipment.SetForbidden(value: false);
                        pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.Equip, equipment), JobTag.Misc);
                        FleckMaker.Static(equipment.DrawPos, equipment.MapHeld, FleckDefOf.FeedbackEquip);
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
                    }
                }
            }
        }
    }
}
