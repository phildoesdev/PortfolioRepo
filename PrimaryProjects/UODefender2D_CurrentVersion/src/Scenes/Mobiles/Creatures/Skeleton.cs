using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Skeleton : BaseCreature
    {

        public Skeleton() : base(sheetId: 50)
        {
			SetStats(hitPoints: 50, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:15, fireResistance: 0, coldResistance: 0, poisonResistance: 15, energyResistance: 0);

			SetMovementInfo(MovementDomainEnum.Ground, 15.0f);
			SetReward(25, 10, 25, 10);
			EquipAttack(new Fists(this));
		}

	}
}