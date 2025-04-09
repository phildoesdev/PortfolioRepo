using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class WarFork : BaseEquipment
    {
        public WarFork(int hueId) : base(equipmentName: "War Fork", sheetID: 645, drawLayer: DrawLayerEnum.LeftHand_SingleHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
