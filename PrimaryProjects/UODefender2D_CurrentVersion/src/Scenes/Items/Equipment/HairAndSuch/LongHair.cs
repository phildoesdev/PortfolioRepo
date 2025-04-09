using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LongHair : BaseEquipment
    {
        public LongHair(int hueId) : base(equipmentName: "Long Hair", sheetID: 701, drawLayer: DrawLayerEnum.Hair, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
