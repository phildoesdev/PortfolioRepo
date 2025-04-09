using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LeatherGloves : BaseEquipment
    {
        public LeatherGloves(int hueId) : base(equipmentName: "Leather Gloves", sheetID: 545, drawLayer: DrawLayerEnum.Gloves, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
