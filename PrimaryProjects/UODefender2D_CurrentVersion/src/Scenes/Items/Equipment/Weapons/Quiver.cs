using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Quiver : BaseEquipment
    {
        public Quiver(int hueId) : base(equipmentName: "Quiver", sheetID: 887, drawLayer: DrawLayerEnum.Back_Cloak, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
