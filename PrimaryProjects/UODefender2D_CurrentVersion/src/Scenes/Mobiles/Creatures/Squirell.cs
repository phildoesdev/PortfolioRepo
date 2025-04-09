using UODef.Abilities.Attacks;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Mobiles.Enemies
{

    public partial class Squirell : BaseCreature
    {

        public Squirell() : base(sheetId: 278, animationArcheType:AnimationArchetypeEnum.Animal)
        {
			SetStats(hitPoints: 50, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:50, fireResistance: 50, coldResistance: 50, poisonResistance: 50, energyResistance: 75);

			SetMovementInfo(MovementDomainEnum.Ground, 100.0f);
			SetReward(50, 100, 250, 50);
			EquipAttack(new Fists(this));
		}

	}
}