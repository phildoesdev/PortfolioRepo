using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class PlatemailLegs : BaseEquipment
    {
        public PlatemailLegs(int hueId) : base(equipmentName: "Platemail Legs", sheetID: 529, drawLayer: DrawLayerEnum.Legs_Inner_Armor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
