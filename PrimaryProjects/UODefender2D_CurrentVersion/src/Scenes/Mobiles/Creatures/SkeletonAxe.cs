using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class SkeletonAxe : BaseCreature
    {

        public SkeletonAxe() : base(sheetId: 56)
        {
			SetStats(hitPoints: 75, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:15, fireResistance: 15, coldResistance: 50, poisonResistance: 50, energyResistance: 0);

			SetMovementInfo(MovementDomainEnum.Ground, 15.0f);
			SetReward(50, 10, 50, 50);
			EquipAttack(new Fists(this));
		}

	}
}