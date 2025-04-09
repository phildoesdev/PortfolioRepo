using System.Collections.Generic;
using Godot;
using UODef.Mobiles.Intelligence;
using UODef.System;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Mobiles.Towers
{
	#region Supporting Enums
	public enum TowerTypeEnum
	{
		None = 0,
		Tempest,
		Mage,
		Magus,
		Spearman,
		Archer,
		Pugilist,
		Assasin
	}
	#endregion


	//
	public partial class BaseTower : Mobile
	{
		#region Tower Cost && Placement Vars
		private int _CostGP = 0;
		/// <summary>
		///		The cost, in gold pieces, to place this tower
		/// </summary>
		public int CostGP
		{
			get { return _CostGP; }
			set
			{
				if (value < 0) value = 0;
				_CostGP = value;
			}
		}

		public string TowerTitle { get; set; } = "Default Tower";
		public string TowerDescription { get; set; } = "A tower without a description.";

		#endregion

		
		#region Initialization Methods 
		public BaseTower(int sheetId, AnimationArchetypeEnum animationArcheType = AnimationArchetypeEnum.Human, IntelligenceTypeEnum IntelligenceType =IntelligenceTypeEnum.BaseTower) : base(sheetID: sheetId, animationArchetype: animationArcheType, intelligenceType: IntelligenceType, lvlingCurveType: Leveling.LevelingCurveTypeEnum.TowerCurve, hueID: 1032)
		{
			// Add the collision layer of tower
			AddCollisionLayer(PhysicsLayerNameEnum.Tower);

		}

		public override void _Ready()
		{
			base._Ready();

			// Set the targeting agent to look after 'TowerTarget's...
			List<PhysicsLayerNameEnum> TargetingLayers = new() { PhysicsLayerNameEnum.TowerTarget };
			SetVisionTargetingOptions(Abilities.Targeting.TargetingStrategy.Closest, TargetingLayers, true, true);
		}
		#endregion

		#region Tower Cost && Placement Methods
		public void PlaceTower(Vector2 globalPosition)
		{
			GlobalPosition = globalPosition;
			PlayDefaultAnimation();
		}
		#endregion
	}
}
