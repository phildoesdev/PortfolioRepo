using System;
using Godot;
using UODef.Mobiles.Intelligence;
using UODef.Abilities.Attacks;
using System.Collections.Generic;
using UODef.Items.Equipment;

namespace UODef.Mobiles.Towers
{
    public partial class TempestTower : BaseTower
    {
		public TempestTower() : base(sheetId: 401)
		{
			MobileName = "An Unnamed Tempest Tower";
			TowerTitle = "Tempest";
			TowerDescription = "A weather-making magician who can summon lightning strikes. Excels vs large groups, weak against quick and single target enemies.";


			/// Animatoin player todoo ...............................................................................................................................................................................
			/// SetAnimationInformation("res://src/Assets/SpriteSheets/Mobiles/Towers/MageTower.png", 33, 33, AnimationArchetypeEnum.Human, spriteTextureDrawingOffset: new Vector2(0, -22), spriteDimensions: new Vector2(128, 128));
			CostGP = 525;

			SetResists(physicalResitance: 0, fireResistance: 0, coldResistance: 0, poisonResistance: 0, energyResistance: 0);
			SetStats(hitPoints: 100, str: 10, dex: 10, intel: 25);

			EquipAttack(new TempestsBoltAttack(this));

			List<IWearableEquipment> StartingGear = [new ArcaneThighBoots(1273), new ArcaneGloves(0), new Robe(1274), new LongHair(1142), new Cloak(1272), new BlackStaff(0)];
			EquipGear(StartingGear);
		}
	}
}
