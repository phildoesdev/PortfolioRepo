using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class PlatemailGloves : BaseEquipment
    {
        public PlatemailGloves(int hueId) : base(equipmentName: "Platemail Gloves", sheetID: 530, drawLayer: DrawLayerEnum.Gloves, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
