using Godot;
using UODef.Mobiles;
using UODef.Abilities.Projectiles;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Abilities.Attacks
{
	public partial class CompositeBowAttack : BaseAttack
	{
		/// <summary>
		///		When we start our attack we locate our target's position and that is where we will spawn bolts and such
		/// </summary>
		private Vector2 AttackLocation;

		/// Animation player todo .......................... why is the animation being defiend this way? must be a better way ............................................
		/// Should... define characteristics of each attack and then each mobile can decide which animation to play base on that? might be nice
		public CompositeBowAttack(IAnimatedCombatant source) : base(source, AnimationGlossaryEnum.H_attackBow)
		{
			SetBaseStats(attacksPerSecond: 0.4f, attackRange: 500.0f, attackDamage: 50);
			SetDamageDistribution(physicalDamagePercentage: 60.0f, directDamagePercentage: 40.0f);

			AttackCategory = AttackCategoryEnum.Magical;
			AddToAttackTargetDomainList(MovementDomainEnum.Ground);
			AddToAttackTargetDomainList(MovementDomainEnum.Air);
			AddToDamageTypeDescriptionList(DamageTypeDescriptionEnum.AOE);
		}

		protected override void DoAttackHook(IDamageable attackTarget)
		{
			AttackLocation = attackTarget.GlobalPosition;
		}

		protected override void OnAttackAnimationMakeContactHook(IDamageable attackTarget)
		{
			DamagePacket updatedPacket = CalculateDamagePacket();
			// Fire the projectile
			PackedScene scene = GD.Load<PackedScene>("res://src/Scenes/Abilities/Projectiles/ArrowProjectile.tscn");
			ArrowProjectile arrowProjectile = scene.Instantiate<ArrowProjectile>();
			arrowProjectile.InitializeProjectile(attackTarget, CalculateDamagePacket(), ProjectileMovementTypeEnum.ToFixedPoint, projectileVelocity:750.0f);
			AttackContainer.AddChild(arrowProjectile);
			arrowProjectile.Position = AttackSource.GetCurrentSpriteCenter(); // It'd be cool to add variance here so it doesnt always come out in an exact straight line
		}

		public override void OnAttackAnimationFinishedHook(IDamageable attackTarget)
		{
		}
	}
}