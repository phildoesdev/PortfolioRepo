using Godot;
using System.Collections.Generic;
using UODef.MouseInteraction;
using UODef.System;


namespace UODef.TileGridmaps
{
	/// <summary>
	///		Tower tile types, defined in the custom data layer TileID for TowerTileMapLayers
	/// </summary>
	public enum TowerTileType
	{
		None = 99,
		Available = 0,
		Debris = 1,
		Taken = 2,
		Invalid = 3,
		EmptyBlack = 4,
		EmptyWhite = 5
	}

	public partial class TowerTileMapLayer : TileMapLayer, IMouseHoverEffect, IMouseHoverUserInteractionEffect
	{
		#region Signals
		/// <summary>
		///		Gets emitted when the player mouses over a tile and interacts using some action defined in the Input Map
		/// </summary>
		/// <param name="actionName">Input Map Enum that correlates to the action executed</param>
		/// <param name="tileType">The type of tower tile that the action was performed on</param>
		/// <param name="isActionJustPressed">Whether this is 'IsActionJustPressed(...)'</param>
		/// <param name="globalMousePosition">The global position of the mouse on click</param>
		[Signal]
		public delegate void TileInteractionEventHandler(InputMapEnum actionName, TowerTileType tileType, bool isActionJustPressed, Vector2 globalMousePosition);

		/// <summary>
		///		Signal that is emitted when someone is simply hovering over a tile that belong to this TowerTileMapLayer
		/// </summary>
		/// <param name="globalMousePosition">The global position of the mouse on click</param>
		[Signal]
		public delegate void TileMouseHoverEventHandler(Vector2 globalMousePosition);
		#endregion


		/// <summary>
		///		Using the TowerTileType enum, define a dictionary showing each tiles atlascoords in its tilset
		/// </summary>
		public static Dictionary<TowerTileType, Vector2I> TowerTileTypeAtlasCoordsContainer { get; } = new()
		{
			{ TowerTileType.Available, new Vector2I(0, 0) },
			{ TowerTileType.Debris, new Vector2I(1, 0) },
			{ TowerTileType.Taken, new Vector2I(2, 0) },
			{ TowerTileType.Invalid, new Vector2I(3, 0) },
			{ TowerTileType.EmptyBlack, new Vector2I(0, 1) },
			{ TowerTileType.EmptyWhite, new Vector2I(1, 1) },
		};


		public TowerTileMapLayer() : base() 
		{ 
		}

		#region IMouseHoverEffect
		public bool IsMouseHoverEffectActive { get; set; } = false;
		public bool IsChangeCursor { get; set; } = true;

		public CursorType GetMouseHoverCursor(Vector2 globalMousePosition)
		{
			switch (GetTileType(globalMousePosition))
			{
				case TowerTileType.Available:
					return CursorType.Target;
				case TowerTileType.Debris:
					break;
				case TowerTileType.Taken:
					break;
				case TowerTileType.Invalid:
					break;
				default:
					break;
			}
			return CursorType.None;
		}

		public void OnMouseCursorHover(Vector2 globalMousePosition)
		{
			EmitSignal(SignalName.TileMouseHover, globalMousePosition);
			return;
		}
		#endregion

		#region IMouseHoverUserInteraction
		public bool IsMouseHoverUserInteractionEffectActive { get; set; } = true;
		public bool CheckViableActionsBeforeCalling { get; set; } = false;
		public List<InputMapEnum> ViableActionNames { get; } = new() { InputMapEnum.Interact };

		public void OnMouseHoverUserInteraction(InputMapEnum actionName, bool isActionJustPressed, Vector2 globalMousePosition)
		{
			TowerTileType tileType = GetTileType(globalMousePosition);  // Attempt to get the tile type
			if (tileType == TowerTileType.None) return;  // Bad coords, or w/e. Can't act on a nonexistent tile.

			// Emit any signals, do any work
			if (actionName == InputMapEnum.Interact)
			{
				EmitSignal(SignalName.TileInteraction, (int)actionName, (int)tileType, isActionJustPressed, globalMousePosition);
			}
        }
		#endregion

		/// <summary>
		///		Given global coordinates, return the filename
		/// </summary>
		/// <param name="globalPosition">The location of the tile we are looking for</param>
		/// <returns></returns>
		public TowerTileType GetTileType(Vector2 globalPosition)
		{
			// Translate from global to local so we can find which tile this is
			Vector2I translatedPosition = LocalToMap(ToLocal(globalPosition));
			Vector2 tileAtlastCoords = GetCellAtlasCoords(translatedPosition);
			TileData returnTileType = GetCellTileData(translatedPosition);

			if (returnTileType == null) return TowerTileType.None;

			var returnTileTypeID = (int)returnTileType.GetCustomData("TileID");
			return (TowerTileType)returnTileTypeID;
		}

		/// <summary>
		///		Interface into 'SetCell' for external actors. Translates a global position into one relative to the tilemaplayer and changes the tile at that position to the one requested.
		///		There are currently no restrictions on this.
		/// </summary>
		/// <param name="globalPosition">Position of the tile you wish to change</param>
		/// <param name="newTileType">Which tile you wish to change to</param>
		/// <returns></returns>
		public bool ChangeTilemapCell(Vector2 globalPosition, TowerTileType newTileType)
		{
			Vector2I translatedPosition = LocalToMap(ToLocal(globalPosition));
			SetCell(translatedPosition, TileSet.GetSourceId(0), TowerTileTypeAtlasCoordsContainer[newTileType]);
			return true;
		}

		/// <summary>
		///		Given a global position of a tile, return the center of that tile
		/// </summary>
		/// <param name="globalPosition">Global position representing the location of a tile</param>
		/// <returns>The global position representing the center of the tile located at the coords passed in</returns>
		public Vector2 GetTileCenter(Vector2 globalPosition)
		{
			Vector2I translatedPosition = LocalToMap(ToLocal(globalPosition));
			return ToGlobal(MapToLocal(translatedPosition));
		}


	}
}