using Godot;
using System;
using System.Collections.Generic;
using UODef.Abilities.Targeting;
using UODef.Mobiles;
using UODef.Mobiles.AnimationPlayer;
using UODef.System;


namespace UODef.Abilities.Attacks
{
	#region enums
	public enum DamageTypeDescriptionEnum
	{
		DOT,
		AOE,
		SpreadEffect,
		SingleTarget
	}
	public enum AttackCategoryEnum
	{
		Melee,
		Ranged,
		Magical
	}
	#endregion

	#region Supporting Interfaces

	public interface IAttack : GodotNode2D
	{
		// Tracking
		public string AttackName { get; set; }
		public IAnimatedCombatant AttackSource { get; }

		// Stats
		public float BaseAttacksPerSecond { get; set; }
		public float BaseAttackRange { get; set; }
		public int BaseAttackDamage { get; set; }
		// Dmg Types
		public float PhysicalDamagePercentage { get; set; }
		public float FireDamagePercentage { get; set; }
		public float ColdDamagePercentage { get; set; }
		public float PoisonDamagePercentage { get; set; }
		public float EnergyDamagePercentage { get; set; }
		public float DirectDamagePercentage { get; set; }
		// Flags && Categorizations
		public List<MovementDomainEnum> AttackTargetDomainList { get; set; }                // Which Mvmt Domains can we target?
		public List<DamageTypeDescriptionEnum> DamageTypeDescriptionList { get; set; }  // Gives a rough description of the damage types supplied by this attack. This will likey change, and hopefully evolve to help categorization and display
		public AttackCategoryEnum AttackCategory { get; set; }                          // Forcing cat of Ranged, Melee, Magical -- might be nice, or annoying. Not sure.

		public bool IsAttacking { get; }    // Is an attack currently happening?

		// Methods
		/// These should be OnAttack, Before Attack, After attack
		public void DoAttack(IDamageable attackTarget);
		public void OnAttackAnimationMakeContact(IDamageable attackTarget);
		public void OnAttackAnimationFinished(IDamageable attackTarget);
		public IDamageable FindCombatTarget(IDamageable attackTarget = null);
	}
	#endregion

	#region Supporting Classes
	#endregion

	public partial class BaseAttack : Node2D, IAttack
	{
		// Tracking
		public string AttackName { get; set; } = "Unnamed Attack";
		public IAnimatedCombatant AttackSource { get; protected set; }
		public bool IsAttacking { get; protected set; }
		/// <summary>
		///		Gets set each time an attack starts so that stats will be the same throughout each part of an attack's animation
		///			but can dynamically update between attacks
		/// </summary>
		public CalculatedWeaponStats CurrentAttackWepStats { get; protected set; }

		/// <summary>
		///		A container for adding children related to this attack such as projectiles, fields, or w/e
		/// </summary>
		protected Node2D AttackContainer { get; set; }

		// Targeting
		public TargetingAgent CombatTargeting { get; protected set; }


		// Stats
		public float BaseAttacksPerSecond { get; set; }
		public float BaseAttackRange { get; set; }
		public int BaseAttackDamage { get; set; }

		#region Dmg Types
		public float PhysicalDamagePercentage { get; set; }
		public float FireDamagePercentage { get; set; }
		public float ColdDamagePercentage { get; set; }
		public float PoisonDamagePercentage { get; set; }
		public float EnergyDamagePercentage { get; set; }
		public float DirectDamagePercentage { get; set; }
		#endregion

		// Categorizations
		public List<MovementDomainEnum> AttackTargetDomainList { get; set; } = new();
		public List<DamageTypeDescriptionEnum> DamageTypeDescriptionList { get; set; } = new();
		public AttackCategoryEnum AttackCategory { get; set; }

		// Defaults
		public AnimationGlossaryEnum DefaultAttackAnimation;

		public BaseAttack(IAnimatedCombatant attackSource, AnimationGlossaryEnum defaultAttackAnimation) : base()
		{
			AttackContainer = new Node2D();
			AddChild(AttackContainer);
			AttackSource = attackSource;
			DefaultAttackAnimation = defaultAttackAnimation;
		}

		public override void _Ready()
		{
			InitializeTargetingAgent();
		}

		#region Helpers Methods
		/// <summary>
		///		Sets the damage distribution. This is a handy way to make it more uniform and easier to init
		///		Forces dmg dist to be set to 100 *shrug*
		/// </summary>
		/// <param name="physicalDamagePercentage">The percent of damage that will now be dealt as physical dmg</param>
		/// <param name="fireDamagePercentage">The percent of damage that will now be dealt as fire dmg</param>
		/// <param name="coldDamagePercentage">The percent of damage that will now be dealt as cold dmg</param>
		/// <param name="poisonDamagePercentage">The percent of damage that will now be dealt as poison dmg</param>
		/// <param name="energyDamagePercentage">The percent of damage that will now be dealt as energy dmg</param>
		/// <param name="directDamagePercentage">The percent of damage that will now be dealt as directdmg</param>
		/// <exception cref="Exception">If damage distributions add up to a value greater than 100 we throw an exception</exception>
		protected void SetDamageDistribution(float physicalDamagePercentage = 0, float fireDamagePercentage = 0, float coldDamagePercentage = 0, float poisonDamagePercentage = 0, float energyDamagePercentage = 0, float directDamagePercentage = 0)
		{
			// More than 100% seems incorrect. I'm not exactly sure what the best course of action is- just throwing this exception for now, whjich will have, who know what side effect
			float totalDmgDist = physicalDamagePercentage + fireDamagePercentage + coldDamagePercentage + poisonDamagePercentage + energyDamagePercentage + directDamagePercentage;
			if (totalDmgDist != 100.0f)
			{
				throw new Exception(message: $"Bad dmg type distribution [physicalDamage: [{physicalDamagePercentage}] fireDamage: [{fireDamagePercentage}] coldDamage: [{coldDamagePercentage}] poisonDamage: [{poisonDamagePercentage}] energyDamage: [{energyDamagePercentage}] directDamage: [{directDamagePercentage}]]. UAWS8FWO");
			}
			PhysicalDamagePercentage = physicalDamagePercentage;
			FireDamagePercentage = fireDamagePercentage;
			ColdDamagePercentage = coldDamagePercentage;
			PoisonDamagePercentage = poisonDamagePercentage;
			EnergyDamagePercentage = energyDamagePercentage;
			DirectDamagePercentage = directDamagePercentage;
		}

		protected void SetBaseStats(float attacksPerSecond, float attackRange, int attackDamage)
		{
			BaseAttacksPerSecond = attacksPerSecond;
			BaseAttackRange = attackRange;
			BaseAttackDamage = attackDamage;
		}
		#endregion

		#region Initialization Methods
		public void InitializeTargetingAgent()
		{
			CombatTargeting = new TargetingAgent(BaseAttackRange, false);
			AddChild(CombatTargeting);
			SetCombatTargetingOptionsFromAttackSource();
		}

		/// <summary>
		///		Observes the attack source's targeting preferences and sets them
		/// </summary>
		/// <returns>Returns true if the combatTargeting's area needs to be reset</returns>
		private bool SetCombatTargetingOptionsFromAttackSource()
		{
			// no reload required for changing tageting strat
			CombatTargeting.CurrentTargetingStrategy = AttackSource.CurrentAttackTargetingStrategy;

			// Must reload
			if (CombatTargeting.CurrentAttackTargetingMaskLayerList != AttackSource.CurrentAttackTargetingMaskLayerList)
			{
				// This call manages CombatTargeting.CurrentAttackTargetingMaskLayerList so the list remains up-to-date
				CombatTargeting.OverrideTargetingAgentMask(AttackSource.CurrentAttackTargetingMaskLayerList);
				return true;
			}
			return false;
		}
		#endregion

		#region Categorization List Mgmt
		public void AddToAttackTargetDomainList(MovementDomainEnum domain)
		{
			if (!AttackTargetDomainList.Contains(domain)) AttackTargetDomainList.Add(domain);
		}
		public void RemoveFromAttackTargetDomainList(MovementDomainEnum domain)
		{
			AttackTargetDomainList.Remove(domain);
		}
		public void AddToDamageTypeDescriptionList(DamageTypeDescriptionEnum dmgType)
		{
			if (!DamageTypeDescriptionList.Contains(dmgType)) DamageTypeDescriptionList.Add(dmgType);
		}
		public void RemoveFromDamageTypeDescriptionList(DamageTypeDescriptionEnum dmgType)
		{
			DamageTypeDescriptionList.Remove(dmgType);
		}
		#endregion

		#region Attack Support Methods
		/// <summary>
		///		Using the
		/// </summary>
		public void PlayAttackAnimation()
		{
			double animationPlayTimeS = 1.0 / CurrentAttackWepStats.AttacksPerSecond;
			//AttackSource.PlayAnimation(DefaultAnimationGlossaryEnum.DefaultAttackAnimation, queueIfCurrentlyPlaying: false, overrideIfPlaying: true, playtimeOverrideS: animationPlayTimeS);
			AttackSource.PlayAnimation(DefaultAttackAnimation, queueIfCurrentlyPlaying: false, overrideIfPlaying: true, playtimeOverrideS: animationPlayTimeS);
		}
		#endregion

		#region IAttack Methods
		/// <summary>
		///		The entry point to comitting an attack, 
		///		Sets 'IsAttacking'. 
		///		Should be proceeded by OnAttackAnimationFinished, so that 'IsAttacking' gets set back to false.
		///		Currently no timeout so if isAttacking gets set, and never reset, attacks will be disabled (until OnAttackAnimationFinished gets called)
		/// </summary>
		/// <param name="attackTarget">The target of this attack</param>
		/// <param name="attacksPerSecond">How many attacks should happen in a second.</param>
		public void DoAttack(IDamageable attackTarget)
		{
			if (IsAttacking) return;
			IsAttacking = true;
			// Calculate current weapon stats using the attack sources bonuses
			CurrentAttackWepStats = AttackSource.GetCalculatedWeaponStats(this);
			PlayAttackAnimation();
			DoAttackHook(attackTarget);
		}

		/// <summary>
		///		This is when the animation 'looks like' it is making contact.
		///		For melee attack animations this is when the weapon makes contact with w/e and before the animation starts to look like it 'pulls away'
		///		For ranged attacks this is when the projectile appears to be being released
		/// </summary>
		/// <param name="attackTarget"></param>
		public void OnAttackAnimationMakeContact(IDamageable attackTarget)
		{
			if (attackTarget is null || !attackTarget.IsAlive) return;
			OnAttackAnimationMakeContactHook(attackTarget);
		}

		/// <summary>
		///		Gets called, by default, after (or durring) all attack animations. 
		///		Can do nothing or something.
		/// </summary>
		/// <param name="attackTarget"></param>
		public void OnAttackAnimationFinished(IDamageable attackTarget)
		{
			IsAttacking = false;
			OnAttackAnimationFinishedHook(attackTarget);
		}

		/// <summary>
		///		Very basic interface into our attack's targeting agent
		/// </summary>
		/// <param name="desiredTarget">If still a valid target, quickly returns this target, otherwise attempt to find a new one</param>
		/// <returns>A valid target for this attack. Returned so that the IAttacker (AttackSource) can do what it will w/ this info</returns>
		public virtual IDamageable FindCombatTarget(IDamageable desiredTarget = null)
		{
			bool resetCombatTargetingRange = false; // Certain changes require a restart

			// Returns true if the area's mask layers need to change. Important b/c it means we have to wipe our pool and recast our area so that we can make sure we have valid targets
			resetCombatTargetingRange = SetCombatTargetingOptionsFromAttackSource();

			CalculatedWeaponStats wepStats = AttackSource.GetCalculatedWeaponStats(this);
			// If the attack range has changed, or something changed when we set options from attack source, we need to recreate our area 
			if (wepStats.AttackRange != CombatTargeting.TargetingRadius || resetCombatTargetingRange)
			{
				CombatTargeting.SetTargetingRange(wepStats.AttackRange);    // Sets if changed, ignores if not
			}

			IDamageable tmpTarget = CombatTargeting.FindTarget(desiredTarget) as IDamageable;
			return tmpTarget;
		}

		/// <summary>
		///		Calculates the base damage, and then how much damage will be dealt to each damage type.
		///		Once these calculations are done, it is packaged into a DamagePacket so that it can be sent around to whoever needs it
		/// </summary>
		/// <returns>A filled out dmg packet</returns>
		public virtual DamagePacket CalculateDamagePacket()
		{
			float baseDamage = CurrentAttackWepStats.AttackDamage;
			int physicalDmgOut = (int)((CurrentAttackWepStats.PhysicalDamagePercentage / 100) * baseDamage);
			int fireDmgOut = (int)((CurrentAttackWepStats.FireDamagePercentage / 100) * baseDamage);
			int coldDmgOut = (int)((CurrentAttackWepStats.ColdDamagePercentage / 100) * baseDamage);
			int poisonDmgOut = (int)((CurrentAttackWepStats.PoisonDamagePercentage / 100) * baseDamage);
			int energyDmgOut = (int)((CurrentAttackWepStats.EnergyDamagePercentage / 100) * baseDamage);
			int directDmgOut = (int)((CurrentAttackWepStats.DirectDamagePercentage / 100) * baseDamage);

			return new DamagePacket((ICombatant)AttackSource, physicalDmgOut, fireDmgOut, coldDmgOut, poisonDmgOut, energyDmgOut, directDmgOut);
		}
		#endregion

		#region IAttack Method Hooks for children

		/// <summary>
		///		A method intended to be overwritten by someone implementing base attack. 
		///		Allows anyone inheriting us to do what they wish w/o having to think too hard.
		///		The method that gets called after base animation checks we can attack, and then begins playing the animation
		/// </summary>
		protected virtual void DoAttackHook(IDamageable attackTarget)
		{
		}
		/// <summary>
		///		A method intended to be overwritten by someone implementing base attack. 
		///		Allows anyone inheriting us to do what they wish w/o having to think too hard.
		///		The method that gets called when it would appear the attack animation should release a projectile or make contact for melee 
		/// </summary>
		protected virtual void OnAttackAnimationMakeContactHook(IDamageable attackTarget)
		{
		}

		/// <summary>
		///		A method intended to be overwritten by someone implementing base attack. 
		///		Allows anyone inheriting us to do what they wish w/o having to think too hard.
		///		The method that gets called after base animation finishes playing.
		/// </summary>
		public virtual void OnAttackAnimationFinishedHook(IDamageable attackTarget)
		{
		}
		#endregion

	}
}
