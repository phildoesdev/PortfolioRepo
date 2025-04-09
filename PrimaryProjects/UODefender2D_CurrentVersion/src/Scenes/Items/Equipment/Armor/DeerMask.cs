using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class DeerMask : BaseEquipment
    {
        public DeerMask(int hueId) : base(equipmentName: "Deer Mask", sheetID: 415, drawLayer: DrawLayerEnum.HeadCovering, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
