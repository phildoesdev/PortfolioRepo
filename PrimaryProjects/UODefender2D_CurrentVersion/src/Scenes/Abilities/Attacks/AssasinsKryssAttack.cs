using UODef.Mobiles;
using UODef.Mobiles.AnimationPlayer;


namespace UODef.Abilities.Attacks
{
	public partial class AssasinsKryssAttack : BaseAttack
	{
		/// Animation player todo .......................... why is the animation being defiend this way? must be a better way ............................................
		/// Should... define characteristics of each attack and then each mobile can decide which animation to play base on that? might be nice
		public AssasinsKryssAttack(IAnimatedCombatant source) : base(source, AnimationGlossaryEnum.H_attackPierce_1h)
		{
			SetBaseStats(attacksPerSecond: 0.75f, attackRange: 75.0f, attackDamage: 24);
			SetDamageDistribution(physicalDamagePercentage: 15, poisonDamagePercentage: 70, directDamagePercentage: 15);

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