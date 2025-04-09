using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Boots : BaseEquipment
    {
        public Boots(int hueId) : base(equipmentName: "Boots", sheetID: 477, drawLayer: DrawLayerEnum.Feet, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
