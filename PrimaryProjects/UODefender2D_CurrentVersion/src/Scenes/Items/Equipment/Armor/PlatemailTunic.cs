using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class PlatemailTunic : BaseEquipment
    {
        public PlatemailTunic(int hueId) : base(equipmentName: "Platemail Tunic", sheetID: 527, drawLayer: DrawLayerEnum.Torso_Inner_ChestArmor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
