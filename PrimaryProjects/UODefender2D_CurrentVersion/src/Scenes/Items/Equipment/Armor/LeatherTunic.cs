using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LeatherTunic : BaseEquipment
    {
        public LeatherTunic(int hueId) : base(equipmentName: "Leather Tunic", sheetID: 542, drawLayer: DrawLayerEnum.Torso_Inner_ChestArmor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
