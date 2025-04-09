using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Harpy : BaseCreature
    {

        public Harpy() : base(sheetId: 30)
        {
			SetStats(hitPoints: 50, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:30, fireResistance: 10, coldResistance: 30, poisonResistance: 10, energyResistance: 10);

			SetMovementInfo(MovementDomainEnum.Ground, 30.0f);
			SetReward(50, 1, 75, 50);
			EquipAttack(new Fists(this));
		}

	}
}