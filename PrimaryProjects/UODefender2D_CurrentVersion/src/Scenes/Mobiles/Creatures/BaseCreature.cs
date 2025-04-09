using UODef.System;
using System.Collections.Generic;
using UODef.Mobiles.Intelligence;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Mobiles.Enemies
{
	/// <summary>
	///     Each enemy can have several or no tags. These tags are a useful way to generalize enemies and communicate these generalizations to users
	/// </summary>
	public enum EnemyTag
	{
		Horde,
		Speedster,
		Boss,
		MiniBoss,
		Goblin,
		Tank,
		Healer,
		Shaman,
		HordeMaster,
		TrojanHorse,
		Assasin,
	}


	public partial class BaseCreature : Mobile
	{


		public BaseCreature(int sheetId, AnimationArchetypeEnum animationArcheType = AnimationArchetypeEnum.Monster, IntelligenceTypeEnum IntelligenceType = IntelligenceTypeEnum.BaseCreature) : base(sheetID: sheetId, animationArchetype: animationArcheType, IntelligenceType, Leveling.LevelingCurveTypeEnum.CreatureCurve)
		{
		}

		public override void _Ready()
		{
			base._Ready();
			// Set the collisions layers so that we can more easily interact
			AddCollisionLayer(PhysicsLayerNameEnum.Creature);
			AddCollisionLayer(PhysicsLayerNameEnum.TowerTarget);
			AddCollisionMaskLayer(PhysicsLayerNameEnum.Tower);

			SetVisionTargetingOptions(Abilities.Targeting.TargetingStrategy.Closest, new List<PhysicsLayerNameEnum>() { PhysicsLayerNameEnum.CreatureTarget }, true, true);

			IsCombatEnabled = false;
		}

		// Overriding so that I can boost all creature stats more easily
		protected override void SetStats(int hitPoints = 1, int str = 0, int dex = 0, int intel = 0)
		{
			hitPoints += 175;
			base.SetStats(hitPoints, str, dex, intel);
		}
	}
}
