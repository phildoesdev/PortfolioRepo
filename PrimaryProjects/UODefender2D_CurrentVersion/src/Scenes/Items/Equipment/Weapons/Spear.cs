using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Spear : BaseEquipment
    {
        public Spear(int hueId) : base(equipmentName: "Spear", sheetID: 641, drawLayer: DrawLayerEnum.RightHand_TwoHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
