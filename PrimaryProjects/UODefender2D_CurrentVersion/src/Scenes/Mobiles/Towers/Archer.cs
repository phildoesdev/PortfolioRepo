using System;
using Godot;
using UODef.Mobiles.Intelligence;
using UODef.Abilities.Attacks;
using System.Collections.Generic;
using UODef.Items.Equipment;

namespace UODef.Mobiles.Towers
{
	public partial class Archer : BaseTower
    {
		public Archer() : base(sheetId: 401) 
		{
			MobileName = "An Unnamed Archer Tower";
			TowerTitle = "Archer";
			TowerDescription = "An expert with a bow, this hireling knows how to aim for the spaces between armor, dealing a large portion of its attack as Direct Damage. Your armor is no match for my bow!";
			CostGP = 400;

			// Stats & Resists
			SetStats(hitPoints: 100, str: 10, dex: 25, intel: 10);
			SetResists(physicalResitance: 0, 
				fireResistance: 0, 
				coldResistance: 0, 
				poisonResistance: 0, 
				energyResistance: 0);
			// Attack includes special animations, dmg types, etc.
			EquipAttack(new CompositeBowAttack(this));
			List<IWearableEquipment> StartingGear = [ new LeatherLeggings(0), new LeatherGorget(0), new LeatherGloves(0), new LeatherSleeves(0), 
				new LeatherTunic(0), new LongHair(1108), new LeatherCap(0), new CompositeBow(0), new Quiver(1437), new Sandals(0)];
			EquipGear(StartingGear);
		}
	}
}
