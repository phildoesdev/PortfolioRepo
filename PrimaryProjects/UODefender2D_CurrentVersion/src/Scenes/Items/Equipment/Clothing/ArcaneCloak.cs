using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class ArcaneCloak : BaseEquipment
    {
        public ArcaneCloak(int hueId) : base(equipmentName: "Arcane Cloak", sheetID: 759, drawLayer: DrawLayerEnum.Back_Cloak, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
