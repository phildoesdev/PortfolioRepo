using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class ArcaneRobe : BaseEquipment
    {
        public ArcaneRobe(int hueId) : base(equipmentName: "Arcane Robe", sheetID: 761, drawLayer: DrawLayerEnum.Torso_Outer_Robe, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
