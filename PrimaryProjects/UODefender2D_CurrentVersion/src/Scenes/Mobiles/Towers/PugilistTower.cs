using System;
using Godot;
using UODef.Mobiles.Intelligence;
using UODef.Abilities.Attacks;
using System.Collections.Generic;
using UODef.Items.Equipment;

namespace UODef.Mobiles.Towers
{
    public partial class PugilistTower : BaseTower
    {
		public PugilistTower() : base(sheetId: 401)
		{
			MobileName = "An Unnamed Pugilist Tower";
			TowerTitle = "Pugilist";
			TowerDescription = "A master in hand-to-hand combat, able to focus the forces of the universe to apply energy damage with each hit.";

			CostGP = 250;

			SetResists(physicalResitance: 0, fireResistance: 0, coldResistance: 0, poisonResistance: 0, energyResistance: 0);
			SetStats(hitPoints: 100, str: 100, dex: 100, intel: 10);

			EquipAttack(new PugilistsGloves(this));

			List<IWearableEquipment> StartingGear = [new Boots(1154), new LongHair(1160), new LeatherGloves(1169), new HalfApron(1169)];  
			EquipGear(StartingGear);
		}
	}
}
