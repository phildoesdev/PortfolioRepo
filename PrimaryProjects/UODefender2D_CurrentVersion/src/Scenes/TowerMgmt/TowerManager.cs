using Godot;
using System;
using System.Collections.Generic;
using UODef.Mobiles.Towers;
using UODef.MouseInteraction;
using UODef.Player;
using UODef.System;
using UODef.TileGridmaps;


namespace UODef.Towers
{
	public partial class TowerManager : Node2D
	{
		#region NodeReferences
		private PlayerProfile Player;	// Autoload singleton
		private MouseInteraction2D mouseInteraction;    // Autoload singleton


		private TowerTileMapLayer towerTileMapLayer;	// Part of the TowerManager scene
		private Node2D towerContainerNode;              // Part of the TowerManager scene

		/// <summary>
		///		PackedScene for our place tower modal
		/// </summary>
		private PackedScene PlaceTowerModalPS = GD.Load<PackedScene>("res://src/GUI/Modals/PlaceTower/PlaceTowerModal.tscn");
		#endregion

		#region Place Tower Modal Members
		/// <summary>
		///		If there is a place tower modal currently being displayed it will be stored here.
		///		The intention is that we dont want to display multiple place tower modals at once.
		///		Having a reference to thoe modal that we can easily dismissed seems okay
		/// </summary>
		private PlaceTowerModal CurrentPlaceTowerModal;

		private List<TowerTypeEnum> AvailableTowerTypeSpawns = new();

		#endregion

		#region Godot Override Methods
		public override void _Ready()
		{
			FindNodeReferences();
			InitializePlaceTowerOptions();
		}

		public override void _ExitTree()
		{
			// cleanup events
			if (towerTileMapLayer is not null) towerTileMapLayer.TileInteraction -= TowerTileInteractionCallback;
		}
		#endregion

		#region Initialization Methods
		private void FindNodeReferences()
		{
			// Find Nodes
			mouseInteraction = GetNode<MouseInteraction2D>("/root/MouseInteraction");
			Player = GetNode<PlayerProfile>("/root/CurrentPlayerProfile");	// should this request get moved to UODef.System in some fashion?
			towerTileMapLayer = GetNode<TowerTileMapLayer>("TowerTileMapLayer");
			towerContainerNode = GetNode<Node2D>("TowerContainerNode");

			// Verify nodes
			if (mouseInteraction == null) throw new Exception(message: "Unable to find mouse interaction node. 9DXL9F3J");
			if (Player.Bank == null) throw new Exception(message: "Unable to find playerbank node. G3I2UH6J");
			if (towerTileMapLayer == null) throw new Exception(message: "Unable to find Tower Tile Map Layer node. KJV2BP05");
			if (towerContainerNode == null) throw new Exception(message: "Unable to find Tower Container Node node. 63A41Q8Z");

			// Add listeners
			towerTileMapLayer.TileInteraction += TowerTileInteractionCallback;
		}

		/// <summary>
		///		This will eventually be more complicated as each lvl, and different progression markers from outside sources
		///			are all likely to dictate which towers can and cannot be placed.
		///		Just going with mage tower for now.
		/// </summary>
		private void InitializePlaceTowerOptions()
		{
			AvailableTowerTypeSpawns.Add(TowerTypeEnum.Mage);
			AvailableTowerTypeSpawns.Add(TowerTypeEnum.Assasin);
			AvailableTowerTypeSpawns.Add(TowerTypeEnum.Archer);
			AvailableTowerTypeSpawns.Add(TowerTypeEnum.Tempest);
		}
		#endregion

		#region Place Tower Related Methods
		/// <summary>
		///		Gets called when TowerTileMapLayer.TileInteraction signal is emitted. 
		///		When someone commits an action w/ their mouse over one of our tiles, we get notified if the TowerTileMapLayer deems it appropraite
		/// </summary>
		private void TowerTileInteractionCallback(InputMapEnum actionName, TowerTileType tileType, bool isActionJustPressed, Vector2 globalMousePosition)
		{
			if (!isActionJustPressed) return;

			switch (tileType)
			{
				case TowerTileType.Available:
					/**/

					// We only want one of these to exist at a time
					if (CurrentPlaceTowerModal != null) CurrentPlaceTowerModal.DismissModal();

					PlaceTowerModal placeTwrMdl = PlaceTowerModalPS.Instantiate<PlaceTowerModal>();
					placeTwrMdl.CurrentTowerTypeSpawns = AvailableTowerTypeSpawns;
					placeTwrMdl.ModalDismissed += PlaceTowerModalDismissed;
					placeTwrMdl.TowerPlacementTarget = globalMousePosition;
					CurrentPlaceTowerModal = placeTwrMdl;
					placeTwrMdl.AddTowerRequest += AddTower;
					Player.DisplayModal(placeTwrMdl);
					break;
				case TowerTileType.Debris:
					GD.PushError("It is not currently possible to clear debris");
					break;
				case TowerTileType.Taken:
					GD.PushError("This tower tile is taken, you absolute dolt");
					break;
				default:
					break;
			}
		}
				
		/// <summary>
		///		Callback from the PlaceTowerModal that clears our reference when it gets dismissed. Allows us to dismiss it manually, or for someone else to 
		///			dismiss it and have us be notified
		/// </summary>
		private void PlaceTowerModalDismissed()
		{
			CurrentPlaceTowerModal.AddTowerRequest -= AddTower;
			CurrentPlaceTowerModal = null;
		}

		public void AddTower(int twrType, int costGP, Vector2 towerPlacementTarget)
		{
			towerTileMapLayer.ChangeTilemapCell(towerPlacementTarget, TowerTileType.Taken);              // change tile
			Vector2 tileCenter = towerTileMapLayer.GetTileCenter(towerPlacementTarget);                  // Tower placement pos

			BaseTower outputTower = null;

			switch ((TowerTypeEnum)twrType)
			{
				case TowerTypeEnum.Mage:
					outputTower = new MageTower();
					break;
				case TowerTypeEnum.Tempest:
					outputTower = new TempestTower();
					break;
				case TowerTypeEnum.Pugilist:
					outputTower = new PugilistTower();
					break;
				case TowerTypeEnum.Archer:
					outputTower = new Archer();
					break;
				case TowerTypeEnum.Assasin:
					outputTower = new AssasinFencer();
					break;
				default:
					GD.PushError("Unable to create this tower type. MP2MGF0X"); 
					return;
			}
			towerContainerNode.AddChild(outputTower);
			outputTower.PlaceTower(tileCenter);
			CurrentPlaceTowerModal.DismissModal();
		}
		#endregion




		/*		/// <summary>
				///		Listens for and processes any user input that the Tower Manager might care about
				/// </summary>
				public void HandleUserInput()
				{
					if (Input.IsActionJustPressed("Action"))
					{
						// This can get pulled out if anyone else ends up caring about it
						Array<Dictionary> towerTilesClicked = mouseInteraction.DetectCursorCollisions((uint)PhysicsLayerNameEnum.TowerTile);
						if (towerTilesClicked.Count <= 0) return;

						var twrTile = towerTilesClicked[0];

						GD.PushError(twrTile);


						// Now I could detect tile type, flip it, place a tower, etc
					}
				}*/

	}
}