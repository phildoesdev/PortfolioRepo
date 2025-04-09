using UODef.Abilities.Attacks;

namespace UODef.Mobiles.Enemies
{

    public partial class Ogre : BaseCreature
    {

        public Ogre() : base(sheetId: 1)
        {
			SetStats(hitPoints: 250, str: 10, dex: 10, intel: 10);
			SetResists(physicalResitance:80, fireResistance: 0, coldResistance: 80, poisonResistance: 50, energyResistance: 75);

			SetMovementInfo(MovementDomainEnum.Ground, 12.0f);
			SetReward(50, 1, 250, 50);
			EquipAttack(new Fists(this));
		}

	}
}