using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Spellbook : BaseEquipment
    {
        public Spellbook(int hueId) : base(equipmentName: "Spellbook", sheetID: 984, drawLayer: DrawLayerEnum.RightHand_TwoHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
