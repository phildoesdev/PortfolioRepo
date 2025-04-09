using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class RatmanWarriorSword : BaseCreature
    {

        public RatmanWarriorSword() : base(sheetId: 45)
        {
			SetStats(hitPoints: 80, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:25, fireResistance: 25, coldResistance: 25, poisonResistance: 25, energyResistance: 25);

			SetMovementInfo(MovementDomainEnum.Ground, 22.0f);
			SetReward(50, 1, 100, 50);
			EquipAttack(new Fists(this));
		}

	}
}