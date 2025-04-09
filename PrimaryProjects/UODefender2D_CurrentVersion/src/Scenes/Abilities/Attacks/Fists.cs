using UODef.Mobiles;
using UODef.Mobiles.AnimationPlayer;


namespace UODef.Abilities.Attacks
{
	public partial class Fists : BaseAttack
	{
		public Fists(IAnimatedCombatant source) : base(source, AnimationGlossaryEnum.H_attackBash_1h)
		{
			SetBaseStats(attacksPerSecond: 100.0f, attackRange: 250.0f, attackDamage: 125);
			SetDamageDistribution(physicalDamagePercentage: 90, directDamagePercentage: 10);

			AddToAttackTargetDomainList(MovementDomainEnum.Ground);
			AddToDamageTypeDescriptionList(DamageTypeDescriptionEnum.SingleTarget);
			AttackCategory = AttackCategoryEnum.Melee;
		}

		protected override void DoAttackHook(IDamageable attackTarget)
		{
		}

		protected override void OnAttackAnimationMakeContactHook(IDamageable attackTarget)
		{
			attackTarget.OnHit(CalculateDamagePacket());
		}

		public override void OnAttackAnimationFinishedHook(IDamageable attackTarget)
		{
		}
	}
}