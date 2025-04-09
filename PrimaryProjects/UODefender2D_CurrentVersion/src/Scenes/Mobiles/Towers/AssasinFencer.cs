using System;
using Godot;
using UODef.Mobiles.Intelligence;
using UODef.Abilities.Attacks;
using System.Collections.Generic;
using UODef.Items.Equipment;

namespace UODef.Mobiles.Towers
{
    public partial class AssasinFencer : BaseTower
    {
		public AssasinFencer() : base(sheetId: 401)
		{
			MobileName = "An Unnamed Assasin Tower";
			TowerTitle = "Assasin";
			TowerDescription = "An expert with a kryss, this shady hireling applies poison damage with each strike. The blood of my enemies flows discretely, tainted with my poisons!";

			/// Animatoin player todoo ...............................................................................................................................................................................
			/// SetAnimationInformation("res://src/Assets/SpriteSheets/Mobiles/Towers/MageTower.png", 33, 33, AnimationArchetypeEnum.Human, spriteTextureDrawingOffset: new Vector2(0, -22), spriteDimensions: new Vector2(128, 128));
			CostGP = 250;

			SetResists(physicalResitance: 0, fireResistance: 0, coldResistance: 0, poisonResistance: 0, energyResistance: 0);
			SetStats(hitPoints: 100, str: 10, dex: 25, intel: 10);

			EquipAttack(new AssasinsKryssAttack(this));

			List<IWearableEquipment> StartingGear = [new LeatherLeggings(1269), new LeatherGorget(1269), new LeatherGloves(1269), new LeatherSleeves(1269), 
									new LeatherTunic(1269), new LongHair(1107), new LeatherCap(1269), new Kryss(1371), new Sandals(1269)];
			EquipGear(StartingGear);
		}
	}
}
