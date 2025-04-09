using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LeatherCap : BaseEquipment
    {
        public LeatherCap(int hueId) : base(equipmentName: "Leather Cap", sheetID: 560, drawLayer: DrawLayerEnum.HeadCovering, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
