using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class OrcLord : BaseCreature
    {

        public OrcLord() : base(sheetId: 138)
        {
			SetStats(hitPoints: 220, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:70, fireResistance: 10, coldResistance: 10, poisonResistance: 10, energyResistance: 10);

			SetMovementInfo(MovementDomainEnum.Ground, 20.0f);
			SetReward(50, 1, 150, 50);
			EquipAttack(new Fists(this));
		}

	}
}