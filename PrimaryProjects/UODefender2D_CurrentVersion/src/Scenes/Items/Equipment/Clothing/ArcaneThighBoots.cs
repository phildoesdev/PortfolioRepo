using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class ArcaneThighBoots : BaseEquipment
    {
        public ArcaneThighBoots(int hueId) : base(equipmentName: "Arcane Thigh Boots", sheetID: 762, drawLayer: DrawLayerEnum.Feet, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
