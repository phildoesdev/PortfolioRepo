using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LeatherGorget : BaseEquipment
    {
        public LeatherGorget(int hueId) : base(equipmentName: "Leather Gorget", sheetID: 546, drawLayer: DrawLayerEnum.NeckCovering, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
