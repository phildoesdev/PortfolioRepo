using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class BodySash : BaseEquipment
    {
        public BodySash(int hueId) : base(equipmentName: "Body Sash", sheetID: 490, drawLayer: DrawLayerEnum.Torso_Middle_SashTunic, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
