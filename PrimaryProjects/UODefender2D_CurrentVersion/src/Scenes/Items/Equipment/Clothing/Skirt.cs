using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Skirt : BaseEquipment
    {
        public Skirt(int hueId) : base(equipmentName: "Skirt", sheetID: 449, drawLayer: DrawLayerEnum.Legs_Outer_Skirt, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
