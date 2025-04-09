using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class PlatemailHelm : BaseEquipment
    {
        public PlatemailHelm(int hueId) : base(equipmentName: "Platemail Helm", sheetID: 563, drawLayer: DrawLayerEnum.HeadCovering, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
