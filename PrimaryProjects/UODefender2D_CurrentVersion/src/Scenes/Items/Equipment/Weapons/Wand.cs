using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Wand : BaseEquipment
    {
        public Wand(int hueId) : base(equipmentName: "Wand", sheetID: 982, drawLayer: DrawLayerEnum.LeftHand_SingleHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
