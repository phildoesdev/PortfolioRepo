using Godot;
using System.Collections.Generic;
using UODef.System;


namespace UODef
{
	/// <summary>
	///		A wrapper for TileMapLayer that sets all of our default setings for any future tilemaps we'll want to create
	/// </summary>
	public partial class EnemyNavTileMapLayer : TileMapLayer
	{
		/// All mobiles will sahre this default Collision Layer 
		private List<PhysicsLayerNameEnum> defaultCollisionLayerList = new() {  };

		public override void _Ready()
		{
		}


	}

}

