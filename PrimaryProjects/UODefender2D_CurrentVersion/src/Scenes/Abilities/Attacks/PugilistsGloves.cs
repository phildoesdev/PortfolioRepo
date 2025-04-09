using UODef.Mobiles;
using UODef.Mobiles.AnimationPlayer;


namespace UODef.Abilities.Attacks
{
	public partial class PugilistsGloves : BaseAttack
	{
		public PugilistsGloves(IAnimatedCombatant source) : base(source, AnimationGlossaryEnum.H_attackBash_1h)
		{
			SetBaseStats(attacksPerSecond: 1.5f, attackRange: 250.0f, attackDamage: 75);
			SetDamageDistribution(physicalDamagePercentage: 25, energyDamagePercentage: 50, directDamagePercentage: 25);

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