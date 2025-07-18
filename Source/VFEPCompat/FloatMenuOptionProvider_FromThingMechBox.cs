using RimWorld;
using RoboticServitude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using VFEPirates;

namespace VFEPCompat
{
    public class FloatMenuOptionProvider_FromThingMechBox : FloatMenuOptionProvider_FromThing
    {
        public override bool SelectedPawnValid(Pawn pawn, FloatMenuContext context)
        {
            return pawn.HasComp<CompCanEquipWeapons>();
        }
        protected override bool MechanoidCanDo => true;
        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            return thing.HasComp<CompWeaponBox>();
        }
        public override IEnumerable<FloatMenuOption> GetOptionsFor(Pawn clickedPawn, FloatMenuContext context)
        {
            Log.Message("test");

            return base.GetOptionsFor(clickedPawn, context);
        }
    }
}
