using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Scythe : BaseEquipment
    {
        public Scythe(int hueId) : base(equipmentName: "Scythe", sheetID: 930, drawLayer: DrawLayerEnum.RightHand_TwoHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
