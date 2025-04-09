using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Shoes : BaseEquipment
    {
        public Shoes(int hueId) : base(equipmentName: "Shoes", sheetID: 480, drawLayer: DrawLayerEnum.Feet, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
