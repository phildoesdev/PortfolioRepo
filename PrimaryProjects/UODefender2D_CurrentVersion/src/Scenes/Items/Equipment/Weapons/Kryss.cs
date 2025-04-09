using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Kryss : BaseEquipment
    {
        public Kryss(int hueId) : base(equipmentName: "Kryss", sheetID: 630, drawLayer: DrawLayerEnum.LeftHand_SingleHanded, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
