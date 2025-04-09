using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class HeaterShield : BaseEquipment
    {
        public HeaterShield(int hueId) : base(equipmentName: "Heater Shield", sheetID: 582, drawLayer: DrawLayerEnum.RightHand_TwoHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
