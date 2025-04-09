using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Sandals : BaseEquipment
    {
        public Sandals(int hueId) : base(equipmentName: "Sandals", sheetID: 479, drawLayer: DrawLayerEnum.Feet, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
