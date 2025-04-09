using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Cloak : BaseEquipment
    {
        public Cloak(int hueId) : base(equipmentName: "Cloak", sheetID: 468, drawLayer: DrawLayerEnum.Back_Cloak, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
