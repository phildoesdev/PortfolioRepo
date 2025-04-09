using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class LizardManSpear : BaseCreature
    {

        public LizardManSpear() : base(sheetId: 35)
        {
			SetStats(hitPoints: 150, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:50, fireResistance: 25, coldResistance: 0, poisonResistance: 90, energyResistance: 50);

			SetMovementInfo(MovementDomainEnum.Ground, 18.0f);
			SetReward(50, 1, 75, 50);
			EquipAttack(new Fists(this));
		}

	}
}