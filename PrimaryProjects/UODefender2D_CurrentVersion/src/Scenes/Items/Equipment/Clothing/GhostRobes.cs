using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
    public partial class GhostRobes : BaseEquipment
    {
        public GhostRobes(int hueId) : base(equipmentName: "Ghost Robes", sheetID: 970, drawLayer: DrawLayerEnum.Torso_Outer_Robe, hueID: hueId, animationArchetype: AnimationArchetypeEnum.Human)
        {
        }
	}
}
