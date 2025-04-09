using System;
using Godot;
using UODef.Mobiles.Intelligence;
using UODef.Abilities.Attacks;
using System.Collections.Generic;
using UODef.Items.Equipment;

namespace UODef.Mobiles.Towers
{
    public partial class MageTower : BaseTower
    {
		public MageTower() : base(sheetId:401)
		{
			MobileName = "An Unnamed Mage Tower";
			TowerTitle = "Magician";
			TowerDescription = "A single target fire magician. Fires at a consistent rate dealing primarily fire damage. A good all-around recruit.";

			/// Animation Player todo... .....................................................................................
			// SetAnimationInformation("res://src/Assets/SpriteSheets/Mobiles/Towers/MageTower.png", 33, 33, AnimationArchetypeEnum.Human, spriteTextureDrawingOffset: new Vector2(0, -22), spriteDimensions: new Vector2(128, 128));
			CostGP = 100;

			SetResists(physicalResitance: 0, fireResistance: 0, coldResistance: 0, poisonResistance: 0, energyResistance: 0);
			SetStats(hitPoints: 100, str: 10, dex: 10, intel: 25);
			EquipAttack(new Fireball(this));

			/*
				787 good dark green
				1173 Blaze
			 */

			List<IWearableEquipment> StartingGear = [new LeatherCap(0), new LeatherGloves(0), new LeatherGorget(0), new LeatherLeggings(0),  new Sandals(1165), new Robe(1357), new Spellbook(0)];
			EquipGear(StartingGear);
		}

	}
}
