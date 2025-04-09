using System.Text.Json;
using System.Collections.Generic;
using Godot;
using System;
using System.Reflection.PortableExecutable;

namespace UODef.System.ResourceUtility
{
	/// <summary>
	///		The container class designed to be used to load in our sprite sheet meta data
	/// </summary>
	public class SpriteSheetMeta
	{
		public int SheetID { get; set; }
		public string SheetName { get; set; }
		public int CreateTimeEpoch { get; set; }
		public int TileWidth { get; set; }
		public int TileHeight { get; set; }
		public Vector2 TileSize
		{
			get
			{
				return new Vector2(TileWidth, TileHeight);
			}
		}
		public int TotalUsedTiles { get; set; }
		public int Rows { get; set; }
		public int Columns { get; set; }
		public Vector2 RowColSize
		{
			get
			{
				return new Vector2(Rows, Columns);
			}
		}
		public int SpriteOffsetOriginX { get; set; }
		public int SpriteOffsetOriginY { get; set; }
		public Vector2 SpriteOffsetOriginVector
		{
			get
			{
				return new Vector2(SpriteOffsetOriginX, SpriteOffsetOriginY);
			}
		}
		// Could be useful for sizing, placing, and animating things around us like healthbars or whath ave you
		public int SmallestSpriteUsedWidth { get; set; }
		public int SmallestSpriteUsedHeight { get; set; }
		public Vector2 SmallestSpriteDimensions
		{
			get
			{
				return new Vector2(SmallestSpriteUsedWidth, SmallestSpriteUsedHeight);
			}
		}
		public int LargestSpriteUsedWidth { get; set; }
		public int LargestSpriteUsedHeight { get; set; }
		public Vector2 LargestSpriteDimensions
		{
			get
			{
				return new Vector2(LargestSpriteUsedWidth, LargestSpriteUsedHeight);
			}
		}
		public Dictionary<int, AnimationSummary> AnimationSummaries { get; set; }  // Not sure that I can actually load this from file as dict, or if list will be req'd
		public Dictionary<int, FrameSummary> FrameSummaryMap { get; set; }  // Not sure that I can actually load this from file as dict, or if list will be req'd

		public SpriteSheetMeta()
		{
		}

		public AnimationSummary GetAnimationSummaryByID(int animationID)
		{
			return AnimationSummaries[animationID];
		}

		/// <summary>
		///		Useful for someone curious about how many frames this animations has in it.
		///		Part of calculating animation time.
		/// </summary>
		/// <param name="animationID">AnimationID in reference to the animationArcheType</param>
		/// <returns>The number of frames for a single direction of the requested animationID</returns>
		public float GetAnimationFrameCount(int animationID)
		{
			return AnimationSummaries[animationID].FramesPerDirection;
		}

		public Vector2 GetFrameOffset(int frameID, bool isFlipH = false)
		{
			// B/c our sprite tiles are square to be the same size, but the sprite is aligned to the bot left corner,
			//	if we flipH, we drastically flip around a bunch of empty space and the sprite apperas to move distnace
			//	on the screen, this is how I've chosen to address that issue
			float offsetX = 0.0f;
			if (isFlipH)
			{
				offsetX = -TileWidth + FrameSummaryMap[frameID].OffsetX;
			}
			else
			{
				offsetX = -FrameSummaryMap[frameID].OffsetX;
			}				
			float offsetY = (-TileHeight) - FrameSummaryMap[frameID].OffsetY;  // -tileheight aligns our bot left corner at 0,0

			return new Vector2(offsetX, offsetY);
		}

		public Vector2 GetFrameOffsetRaw(int frameID)
		{
			float offsetX = FrameSummaryMap[frameID].OffsetX;
			float offsetY = FrameSummaryMap[frameID].OffsetY;
			return new Vector2(offsetX, offsetY);
		}

		/// <summary>
		///		Queries the Frame Summary amp for this item's details
		/// </summary>
		/// <param name="frameID"></param>
		/// <returns></returns>
		public FrameSummary GetFrameSummary(int frameID)
		{
			return FrameSummaryMap[frameID];
		}

		public void PrintSummary()
		{
			GD.PushError($"SheetID: [{SheetID}]");
			GD.PushError($"SheetName: [{SheetName}]");
			GD.PushError($"CreateTimeEpoch: [{CreateTimeEpoch}]");
			GD.PushError($"TileWidth: [{TileWidth}]");
			GD.PushError($"TileHeight: [{TileHeight}]");
			GD.PushError($"TotalUsedTiles: [{TotalUsedTiles}]");
			GD.PushError($"Rows: [{Rows}]");
			GD.PushError($"Columns: [{Columns}]");
			GD.PushError($"SpriteOffsetOriginX: [{SpriteOffsetOriginX}]");
			GD.PushError($"SpriteOffsetOriginY: [{SpriteOffsetOriginY}]");

			foreach (int aniID in AnimationSummaries.Keys)
			{
				GD.PushError($"aniID: [{aniID}]");
				GD.PushError($"AnimationSummaries[aniID].StartingFrame: [{AnimationSummaries[aniID].StartingFrame}]");
				GD.PushError($"AnimationSummaries[aniID].TotalDirections: [{AnimationSummaries[aniID].TotalDirections}]");
				GD.PushError($"AnimationSummaries[aniID].FramesPerDirection: [{AnimationSummaries[aniID].FramesPerDirection}]");
			}

			foreach (int frameID in FrameSummaryMap.Keys)
			{
				GD.PushError($"frameID: [{frameID}]");
				GD.PushError($"FrameSummaryMap[frameID].OffsetX: [{FrameSummaryMap[frameID].OffsetX}]");
				GD.PushError($"FrameSummaryMap[frameID].OffsetY: [{FrameSummaryMap[frameID].OffsetY}]");
			}
		}
	}

	/// <summary>
	///		Holds information about each animation defined by the sprite sheet
	/// </summary>
	public class AnimationSummary
	{
		// public int AnimationID;
		public int StartingFrame { get; set; }
		public int TotalDirections { get; set; }
		public int FramesPerDirection { get; set; }

		public AnimationSummary()
		{
		}
	}

	/// <summary>
	///		Holds information about each frame in the spirte sheet
	/// </summary>
	public class FrameSummary
	{
		// public int FrameID;
		public int OffsetX { get; set; }
		public int OffsetY { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public FrameSummary()
		{
		}
	}


	/// <summary>
	///		Does the work of reading the sprite sheet meta files
	/// </summary>
	public class SpriteSheetLoader
    {

		/// <summary>
		///		The file name pattern we are expecting to see for sprite sheet meta files
		/// </summary>
		private string MetaFileNamePattern = "_meta.json";

        public SpriteSheetLoader()
        {
		}

		public Dictionary<int, SpriteSheetMeta> LoadSpriteSheetMetaFiles(string spriteSheetMetaDirPath)
		{
			// Create out list
			Dictionary<int, SpriteSheetMeta> spritesheetMetaDictOut = new();
			// Grab contents of folder
			using DirAccess dirList = DirAccess.Open(spriteSheetMetaDirPath);

			// Load files
			foreach (string filePath in dirList.GetFiles())
			{
				// Only care about very specific files
				if (!filePath.EndsWith(MetaFileNamePattern)) continue;
				try
				{
					// Load files and attempt to read the json in as a list of objects
					List<SpriteSheetMeta> fileOutput = null;
					using var file = FileAccess.Open(spriteSheetMetaDirPath + filePath, FileAccess.ModeFlags.Read);

					// The json deserializer wants a list of structs, but our flow is for each file to have one description in it so we accomidate the deserializer.
					string fileContents = "[" + file.GetAsText() + "]";
					fileOutput = JsonSerializer.Deserialize<List<SpriteSheetMeta>>(fileContents);
					// We expect one description per file currently.
					if (fileOutput.Count != 1)
					{
						throw new Exception($"Unable to Parse File: [{filePath}] K3O63Q4R");
					}
					spritesheetMetaDictOut[fileOutput[0].SheetID] = fileOutput[0];
					//GD.PushError($"Parsed meta sheetid: [{fileOutput[0].SheetID}] - [{fileOutput[0].SheetName}]");
				}
				catch (Exception e)
				{
					// Silly to just push this error again, but idk, thought I'd do something with it
					GD.PushError(e);
				}
			}

			if (spritesheetMetaDictOut.Count == 0)
			{
				GD.PushError($"Unable to locate Sprite Sheet Meta Files In Requested Directory: [{spriteSheetMetaDirPath}]. 1XP4QGF6");
			}
			return spritesheetMetaDictOut;
		}

    }
}
