
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class LeatherSleeves : BaseEquipment
    {
        public LeatherSleeves(int hueId) : base(equipmentName: "Leather Sleeves", sheetID: 544, drawLayer: DrawLayerEnum.ArmCovering_Armor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
