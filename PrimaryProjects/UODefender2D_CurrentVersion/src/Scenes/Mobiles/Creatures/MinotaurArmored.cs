using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class MinotaurArmored : BaseCreature
    {

        public MinotaurArmored() : base(sheetId: 280)
        {
			SetStats(hitPoints: 5000, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:98, fireResistance: 80, coldResistance: 80, poisonResistance: 80, energyResistance: 25);

			SetMovementInfo(MovementDomainEnum.Ground, 18.0f);
			SetReward(300, 1, 75, 50);
			EquipAttack(new Fists(this));
		}

	}
}