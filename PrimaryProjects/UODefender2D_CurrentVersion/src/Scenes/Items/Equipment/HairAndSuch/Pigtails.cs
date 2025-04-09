using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class Pigtails : BaseEquipment
    {
        public Pigtails(int hueId) : base(equipmentName: "Pigtails", sheetID: 902, drawLayer: DrawLayerEnum.Gloves, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
