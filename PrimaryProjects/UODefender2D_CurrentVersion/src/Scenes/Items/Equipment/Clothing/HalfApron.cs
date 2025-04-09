using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class HalfApron : BaseEquipment
    {
        public HalfApron(int hueId) : base(equipmentName: "Half Apron", sheetID: 466, drawLayer: DrawLayerEnum.Waist_HalfApron, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
