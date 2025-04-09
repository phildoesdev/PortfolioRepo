using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Robe : BaseEquipment
    {
        public Robe(int hueId) : base(equipmentName: "Robe", sheetID: 469, drawLayer: DrawLayerEnum.Torso_Outer_Robe, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
