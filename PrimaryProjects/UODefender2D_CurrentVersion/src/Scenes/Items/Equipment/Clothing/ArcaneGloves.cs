using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class ArcaneGloves : BaseEquipment
    {
        public ArcaneGloves(int hueId) : base(equipmentName: "Arcane Gloves", sheetID: 760, drawLayer: DrawLayerEnum.Gloves, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
