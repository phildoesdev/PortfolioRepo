using Godot;
using UODef.Mobiles;
using UODef.Abilities.Projectiles;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Abilities.Attacks
{
	public partial class TempestsBoltAttack : BaseAttack
	{
		/// <summary>
		///		When we start our attack we locate our target's position and that is where we will spawn bolts and such
		/// </summary>
		private Vector2 AttackLocation;

		public TempestsBoltAttack(IAnimatedCombatant source) : base(source, AnimationGlossaryEnum.H_spellSummon)
		{
			SetBaseStats(attacksPerSecond: 0.1f, attackRange: 1000.0f, attackDamage: 50);
			SetDamageDistribution(fireDamagePercentage: 5.0f, energyDamagePercentage: 85.0f, directDamagePercentage: 10.0f);

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
			// Reference the attack and attack weilder's stats and get an up-to-date damage packet to attach to our lightning bolt
			DamagePacket updatedPacket = CalculateDamagePacket();
			// Fire the projectile
			PackedScene scene = GD.Load<PackedScene>("res://src/Scenes/Abilities/Projectiles/TempestsBoltProjectile.tscn");
			TempestsBoltProjectile tempestBoltProjectile = scene.Instantiate<TempestsBoltProjectile>();
			tempestBoltProjectile.SetupBolt(updatedPacket, effectDurationMS:1000, damageRadius: 50.0f);
			AttackContainer.AddChild(tempestBoltProjectile);
			tempestBoltProjectile.GlobalPosition = AttackLocation;
		}

		public override void OnAttackAnimationFinishedHook(IDamageable attackTarget)
		{
		}
	}
}