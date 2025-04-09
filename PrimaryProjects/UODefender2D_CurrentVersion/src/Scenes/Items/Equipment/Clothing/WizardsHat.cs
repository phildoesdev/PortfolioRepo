using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class WizardsHat : BaseEquipment
    {
        public WizardsHat(int hueId) : base(equipmentName: "Wizard's Hat", sheetID: 409, drawLayer: DrawLayerEnum.HeadCovering, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
