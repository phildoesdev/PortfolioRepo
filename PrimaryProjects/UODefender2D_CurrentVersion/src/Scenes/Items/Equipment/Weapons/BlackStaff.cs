using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class BlackStaff : BaseEquipment
    {
        public BlackStaff(int hueId) : base(equipmentName: "Black Staff", sheetID: 617, drawLayer: DrawLayerEnum.RightHand_TwoHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
