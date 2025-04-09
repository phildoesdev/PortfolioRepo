using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class LizardMan : BaseCreature
    {

        public LizardMan() : base(sheetId: 33)
        {
			SetStats(hitPoints: 125, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance: 50, fireResistance: 25, coldResistance: 0, poisonResistance: 90, energyResistance: 50);

			SetMovementInfo(MovementDomainEnum.Ground, 21.0f);
			SetReward(50, 10, 75, 50);
			EquipAttack(new Fists(this));
		}

	}
}