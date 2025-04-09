using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Orc : BaseCreature
    {

        public Orc() : base(sheetId: 17)
        {
			SetStats(hitPoints: 100, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:25, fireResistance: 25, coldResistance: 20, poisonResistance: 10, energyResistance: 20);

			SetMovementInfo(MovementDomainEnum.Ground, 18.0f);
			SetReward(50, 1, 25, 50);
			EquipAttack(new Fists(this));
		}

	}
}