using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class PlatemailSleeves : BaseEquipment
    {
        public PlatemailSleeves(int hueId) : base(equipmentName: "Platemail Sleeves", sheetID: 528, drawLayer: DrawLayerEnum.ArmCovering_Armor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
