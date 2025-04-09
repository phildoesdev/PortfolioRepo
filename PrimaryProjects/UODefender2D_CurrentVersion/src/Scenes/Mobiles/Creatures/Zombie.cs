using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Zombie : BaseCreature
    {

        public Zombie() : base(sheetId: 3)
		{
			SetStats(hitPoints: 25, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance: 15, fireResistance: 0, coldResistance: 0, poisonResistance: 15, energyResistance: 0);

			SetMovementInfo(MovementDomainEnum.Ground, 10.0f);
			SetReward(25, 10, 25, 10);
			EquipAttack(new Fists(this));
		}

	}
}