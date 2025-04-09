using Godot;
using System.Collections.Generic;

namespace UODef.System.ResourceUtility
{
	public partial class ResourceUtility : Node
	{

		/// <summary>
		///		Directory containing all our sprite sheet meta definitions
		/// </summary>
		public string SpriteSheetMetaDirPath { get; } = "res://src/Assets/MobileSpriteSheetDefinitions/";
		/// <summary>
		///		File containing hue descriptions
		/// </summary>
		public string HueMetaDataFilePath { get; } = "res://src/Assets/Hues/HueMeta.json";

		/// <summary>
		///		Dictionary containing information about each sprite sheet. Holds info like tile size,
		///			frame offsets, and avaialable animations
		/// </summary>
		public Dictionary<int, SpriteSheetMeta> SpriteSheetMetaDataDict { get; private set; }

		/// <summary>
		///		Dictionary containing infomration about each hue. Holds info like the list of 32 ints that define a hue
		/// </summary>
		public Dictionary<int, HueMeta> HueMetaDataDict { get; private set; }

		public ResourceUtility() : base()
		{
			LoadAnimations();
			LoadHues();
		}

		/// <summary>
		///     Load all of the meta data from any sprite sheets in the SpriteSheetMetaDirPath folder
		/// </summary>
		public void LoadAnimations()
		{
			var ani_loader = new SpriteSheetLoader();
			SpriteSheetMetaDataDict = ani_loader.LoadSpriteSheetMetaFiles(SpriteSheetMetaDirPath);
		}

		/// <summary>
		///		Load all the meta data from the hue meta dta file 
		/// </summary>
		public void LoadHues()
		{
			var hue_loader = new HueLoader();
			HueMetaDataDict = hue_loader.LoadHueMetaFile(HueMetaDataFilePath);
		}

		public SpriteSheetMeta GetSpriteSheetMeta(int id)
		{
			return SpriteSheetMetaDataDict[id];
		}

		public HueMeta GetHueMeta(int id)
		{
			return HueMetaDataDict[id];
		}

		/// <summary>
		/// Tests whether this sheet ID exists w/in our dictionary of loaded sheets.
		/// Req'd to animate most things
		/// </summary>
		/// <param name="sheetID">The ID in question</param>
		/// <returns>bool</returns>
		public bool IsValidSheetID(int sheetID)
		{
			return SpriteSheetMetaDataDict.ContainsKey(sheetID);
		}

	}
}
