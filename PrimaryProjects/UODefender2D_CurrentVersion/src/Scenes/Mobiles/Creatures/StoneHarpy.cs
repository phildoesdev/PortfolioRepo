using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class StoneHarpy : BaseCreature
    {

        public StoneHarpy() : base(sheetId: 73)
        {
			SetStats(hitPoints: 150, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:45, fireResistance: 80, coldResistance: 0, poisonResistance: 80, energyResistance: 0);

			SetMovementInfo(MovementDomainEnum.Ground, 18.0f);
			SetReward(50, 10, 75, 50);
			EquipAttack(new Fists(this));
		}

	}
}