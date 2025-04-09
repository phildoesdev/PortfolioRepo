using Godot;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Items.Equipment
{
	public interface IWearableEquipment
    {
        public int SheetID { get; }
        public int HueID { get; set; }
        public DrawLayerEnum DrawLayer { get; set; }
        public AnimationArchetypeEnum AnimationArchetype { get; }
    }

    /// <summary>
    ///     This class is the base for wearable objects that appear on the avatar. Right now this is a container for us to predefine hues, sheetids, etc in an ordered manner
    ///         for humans (mage hat, mage robe, etc), but could easily be abstracted so that animated gear COULD be created for any animation arche type
    ///     Equipment, as it stands now (to reach MVP) are sort of items, but they are not contributing stats, attacks, or anything of the sort
    /// </summary>
    public partial class BaseEquipment : Sprite2D, IWearableEquipment
	{
        public string EquipmentName { get; set; }

        // What hue we'll apply when animating the sprite sheet
        public int HueID { get; set; } 
        // Sprite sheet id to animate this equipment
        public int SheetID { get; private set; }
        public DrawLayerEnum DrawLayer { get; set; }

		public AnimationArchetypeEnum AnimationArchetype { get; private set; }

        public BaseEquipment(string equipmentName, int sheetID, DrawLayerEnum drawLayer, int hueID=0, AnimationArchetypeEnum animationArchetype=AnimationArchetypeEnum.Human) : base()
        {
            EquipmentName = equipmentName;
			SheetID = sheetID;
            HueID = hueID;
            AnimationArchetype = animationArchetype;
			DrawLayer = drawLayer;
		}

		#region Godot Overrides
		public override void _Ready()
		{
		}
		#endregion
	}
}
