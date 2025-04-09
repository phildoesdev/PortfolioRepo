using Godot;
using System;
using System.Collections.Generic;
using UODef;
using UODef.Mobiles.Navigation;

namespace UODef.EnemyMgmt
{
	public partial class EnemyNavigationRegion : Node2D
	{
		#region NavRegionSettings
		private EnemyNavTileMapLayer EnemyNavTilemap;
		private List<Marker2D> SpawnPoints = new();
		private Marker2D DestinationPoint = new();
		private int SpawnPointIdx = 0;

		// Tilemaplayer constants, settings, etc
		private Vector2I NavCellOK = new(0, 0);     // The location of the 'OK' enemy nav cell

		/// <summary>
		/// Path name is used as a way to reference a specific path from within a config file of some sort
		/// </summary>
		[Export]
		public string PathName = "GenericPath";

		/// <summary>
		///     tilemapNavPolygon.AgentRadius
		/// </summary>
		[Export]
		public float NavPolygonAgentRadius = 30.0f;
		/// <summary>
		///     tilemapNavPolygon.CellSize 
		/// </summary>
		[Export]
		public float NavPolyCellSize = 0.25f;
		#endregion

		#region Godot Overrides and Realted Nodes
		public override void _Ready()
		{
			InitializeScene();

		}
		#endregion

		#region Scene Initialization
		/// <summary>
		///     Method containing all the initialization calls that should happen in _ready
		/// </summary>
		private void InitializeScene()
		{
			FindNodes();
			GenerateNewNavigationMap();
		}

		/// <summary>
		///     Grabs all appropriate nodes. 
		///     Warns if fails to find something
		/// </summary>
		private void FindNodes()
		{
			// Enemy tilemap to draw paths and such
			EnemyNavTilemap = GetNode<EnemyNavTileMapLayer>("EnemyNavTileMapLayer");
			if (EnemyNavTilemap == null) throw new Exception("Unable to find Enemy Nav TileMap Layer. UB6NR46Q");

			// We only expect one destination point to be listed
			DestinationPoint = GetNode<Marker2D>("DestinationPoint/Marker2D");
			if (DestinationPoint == null) throw new Exception("Unable to find a set Destination Point. 37LWQKPQ");

			// We cycle through the spawn points as we lay out enemies 
			foreach (Marker2D marker in GetNode<Node2D>("SpawnPoints").GetChildren())
			{
				SpawnPoints.Add((Marker2D)marker);
			}
			if (SpawnPoints.Count == 0) throw new Exception("Unable to find a set Spawn Point. TM4Z66WM");
		}


		/// <summary>
		///     Calls the NavigationServer to create a new map and then sets the tilemap to belong to it
		/// </summary>
		private void GenerateNewNavigationMap()
		{
			// Create new map and activate it
			Rid newNavMapRid = NavigationServer2D.MapCreate();
			NavigationServer2D.MapSetActive(newNavMapRid, true);

			// Apply new map to the enemy tilemaplayer and to this nav region
			EnemyNavTilemap.SetNavigationMap(newNavMapRid);

			// Get the list of navigatable cells so that we may choose one randomly in an attempt to access its NavigationPolygon
			Godot.Collections.Array<Vector2I> NavCellOKArray = EnemyNavTilemap.GetUsedCellsById(-1, NavCellOK);
			// We fail hard if there are no navigation layers to choose from, and we should presume if there is a navigation region we should have navigatable regions? Maybe this will change in the future.
			if (NavCellOKArray.Count <= 0) throw new Exception(message: "Unable to locate any valid navigation layers. MAKE SURE YOU HAVE DRAWN THE APPROPRIATE TILES ON THE MAP! PK6PGYUF");

			// Possible that these should go out to be fields but no reason for this yet
			NavigationPolygon tilemapNavPolygon = EnemyNavTilemap.GetCellTileData(NavCellOKArray[0]).GetNavigationPolygon(0);
			NavigationMesh tilemapNavMesh = tilemapNavPolygon.GetNavigationMesh();

			tilemapNavPolygon.AgentRadius = NavPolygonAgentRadius;
			tilemapNavPolygon.CellSize = NavPolyCellSize;
		}
		#endregion

		#region External Access
		/// <summary>
		/// A way for outside sources to access our navigation map RID
		/// </summary>
		/// <returns>Rid of this tilemap's current navigation mamp</returns>
		public Rid GetNavMapRID()
		{
			return EnemyNavTilemap.GetNavigationMap();
		}

		/// <summary>
		/// Grabs a destination point from the list of possible destination points.
		/// Currently just returns the first element
		/// </summary>
		/// <returns>Returns the target destination as a NavigationDestination obj. containing the destination and mapRID</returns>
		public NavigationDestination GetTargetDest()
		{
			return new NavigationDestination(DestinationPoint.GlobalPosition, GetNavMapRID());
		}

		/// <summary>
		///		Cycles through the posisble spawn points
		/// </summary>
		/// <returns></returns>
		public Vector2 GetNextSpawnPos()
		{
			SpawnPointIdx = SpawnPointIdx >= (SpawnPoints.Count - 1) ? 0 : SpawnPointIdx + 1;
			return SpawnPoints[SpawnPointIdx].GlobalPosition;
		}

		public void ResetSpawnPointIndex()
		{
			SpawnPointIdx = 0;
		}
		#endregion
	}
}
