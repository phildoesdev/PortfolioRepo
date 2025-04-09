using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class FancyShirt : BaseEquipment
    {
        public FancyShirt(int hueId) : base(equipmentName: "Fancy Shirt", sheetID: 435, drawLayer: DrawLayerEnum.Torso_Clothing_FemaleArmor, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
