using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class CompositeBow : BaseEquipment
    {
        public CompositeBow(int hueId) : base(equipmentName: "Composite Bow", sheetID: 938, drawLayer: DrawLayerEnum.RightHand_TwoHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
