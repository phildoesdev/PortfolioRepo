using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LeatherLeggings : BaseEquipment
    {
        public LeatherLeggings(int hueId) : base(equipmentName: "Leather Leggings", sheetID: 543, drawLayer: DrawLayerEnum.Legs_Inner_Armor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
