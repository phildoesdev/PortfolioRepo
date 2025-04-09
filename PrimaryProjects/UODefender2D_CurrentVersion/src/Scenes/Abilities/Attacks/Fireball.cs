using Godot;
using UODef.Mobiles;
using UODef.Abilities.Projectiles;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Abilities.Attacks
{
	public partial class Fireball : BaseAttack
	{
		/// Animation player todo .......................... why is the animation being defiend this way? must be a better way ............................................
		/// Should... define characteristics of each attack and then each mobile can decide which animation to play base on that? might be nice
		public Fireball(IAnimatedCombatant source) : base(source, AnimationGlossaryEnum.H_spell)
		{
			SetBaseStats(attacksPerSecond: 0.5f, attackRange: 300.0f, attackDamage: 12);
			SetDamageDistribution(physicalDamagePercentage: 0.0f, fireDamagePercentage: 100.0f, directDamagePercentage: 0.0f);

			AttackCategory = AttackCategoryEnum.Magical;
			AddToAttackTargetDomainList(MovementDomainEnum.Ground);
			AddToAttackTargetDomainList(MovementDomainEnum.Air);
			AddToDamageTypeDescriptionList(DamageTypeDescriptionEnum.SingleTarget);
		}

		protected override void DoAttackHook(IDamageable attackTarget)
		{
		}

		protected override void OnAttackAnimationMakeContactHook(IDamageable attackTarget)
		{
			
			// Fire the projectile
			PackedScene scene = GD.Load<PackedScene>("res://src/Scenes/Abilities/Projectiles/FireballProjectile.tscn");
			FireballProjectile fireballProjectile = scene.Instantiate<FireballProjectile>();
			fireballProjectile.InitializeProjectile(attackTarget, CalculateDamagePacket(), ProjectileMovementTypeEnum.FollowTarget);
			AttackContainer.AddChild(fireballProjectile);
			// GD.PushError($".GetCurrentSpriteCenter {AttackSource.GetCurrentSpriteCenter()} ");
			fireballProjectile.Position = AttackSource.GetCurrentSpriteCenter(); // It'd be cool to add variance here so it doesnt always come out in an exact straight line
		}

		public override void OnAttackAnimationFinishedHook(IDamageable attackTarget)
		{
		}
	}
}