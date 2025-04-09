using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Troll : BaseCreature
    {

        public Troll() : base(sheetId: 53)
        {
			SetStats(hitPoints: 250, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:90, fireResistance: 50, coldResistance: 25, poisonResistance: 25, energyResistance: 0);

			SetMovementInfo(MovementDomainEnum.Ground, 12.0f);
			SetReward(50, 250, 250, 50);
			EquipAttack(new Fists(this));
		}

	}
}