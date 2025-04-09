using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Ratman : BaseCreature
    {

        public Ratman() : base(sheetId: 42)
        {
			SetStats(hitPoints: 75, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:25, fireResistance: 10, coldResistance: 10, poisonResistance: 25, energyResistance: 0);

			SetMovementInfo(MovementDomainEnum.Ground, 18.0f);
			SetReward(50, 10, 60, 50);
			EquipAttack(new Fists(this));
		}

	}
}