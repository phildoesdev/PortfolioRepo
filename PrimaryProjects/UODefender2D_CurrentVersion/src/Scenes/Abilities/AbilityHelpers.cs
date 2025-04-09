using Godot;
using System.Collections.Generic;
using UODef.Abilities.Attacks;
using UODef.Abilities.Targeting;
using UODef.Mobiles.AnimationPlayer;
using UODef.System;

namespace UODef.Abilities
{

	/*
	 * Classes
	 */

	/// <summary>
	///		A Booklet containing tallys of all damage recieved, organized by the ICombatant dmg dealer. Generally against the holder of this book
	/// </summary>
	public class DamageBooklet
	{
		/// <summary>
		///		The booklet
		/// </summary>
		public Dictionary<ICombatant, DamageEntry> DamageEntries { get; set; } = new();

		/// <summary>
		///		A running tally of the total dmg taken
		/// </summary>
		public int TotalDamage { get; private set; } = 0;

		public DamageBooklet()
		{
		}

		/// <summary>
		///		Returns true if the dictionary has entires in it, false if not
		/// </summary>
		/// <returns>Returns true if there are any registered dmg entries, false if not</returns>
		public bool HasEntries()
		{
			return DamageEntries.Count > 0;
		}

		/// <summary>
		///		Loops the damage booklet's entries and finds the ICombatant that dealt the most dmg
		/// </summary>
		/// <returns>The ICombatant that has done the most dmg</returns>
		public ICombatant GetMostDmg()
		{
			DamageEntry highestDmgDealer = null;
			foreach (KeyValuePair<ICombatant, DamageEntry> entry in DamageEntries)
			{
				// If the first loop, or someone has done more dmg set return value
				if (highestDmgDealer == null || entry.Value.TotalDamage > highestDmgDealer.TotalDamage) highestDmgDealer = entry.Value;
			}
			return highestDmgDealer?.Damager;
		}

		/// <summary>
		///		Loops the damage booklet's entries and finds the ICombatant that has done dmg most recently.
		///		This would be useful when trying to determine who executed the kill-shot
		/// </summary>
		/// <returns>The ICombatant that has done dmg most recently</returns>
		public ICombatant GetMostRecentEntry()
		{
			DamageEntry mostRecentDmgDealer = null;
			foreach (KeyValuePair<ICombatant, DamageEntry> entry in DamageEntries)
			{
				// If the first loop, or someone has done dmg more recently, set return value
				if (mostRecentDmgDealer == null || entry.Value.LastDamageTicksMS > mostRecentDmgDealer.LastDamageTicksMS) mostRecentDmgDealer = entry.Value;
			}
			return mostRecentDmgDealer?.Damager;
		}

		/// <summary>
		///		Loops the dmg booklet's entries to calculate total dmg
		/// </summary>
		/// <returns></returns>
		public Dictionary<ICombatant, float> GetDamagePercentByCombatant()
		{
			Dictionary<ICombatant, float> dmgAsAPercentageByCombatant = new();
			foreach (KeyValuePair<ICombatant, DamageEntry> entry in DamageEntries)
			{
				dmgAsAPercentageByCombatant.Add(entry.Key, (entry.Value.TotalDamage / TotalDamage) * 100);
			}
			return dmgAsAPercentageByCombatant;
		}

		/// <summary>
		///		Either adds to the booklet or updates the damge total for this combatant
		/// </summary>
		/// <param name="dmgr">ICombatant that did the dmg</param>
		/// <param name="totalDmg">How much dmg they did</param>
		public void Update(ICombatant dmgr, int totalDmg)
		{
			if (!DamageEntries.ContainsKey(dmgr))
			{
				DamageEntry de = new(dmgr, totalDmg);
				DamageEntries.Add(dmgr, de);
			}
			else
			{
				DamageEntries[dmgr].Update(totalDmg);
			}
			TotalDamage += totalDmg;
		}
	}

	/// <summary>
	///		A Page in the Damagebooklet. 
	///		Meant to be a tally of all the dmg done by an ICombatant against another (the book holder)
	/// </summary>
	public class DamageEntry
	{
		public ICombatant Damager { get; set; }
		public int TotalDamage { get; set; } = 0;
		public int LastDamage { get; set; } = 0;
		public ulong LastDamageTicksMS { get; set; }

		public DamageEntry(ICombatant dmger, int totalDmg)
		{
			Damager = dmger;
			TotalDamage = LastDamage = totalDmg;
			LastDamageTicksMS = Time.GetTicksMsec();
		}

		/// <summary>
		///		Adds to the total dmg, sets last damage amt and last dmg time
		/// </summary>
		public void Update(int dmg)
		{
			TotalDamage += dmg;
			LastDamage = dmg;
			LastDamageTicksMS = Time.GetTicksMsec();
		}
	}

	/// <summary>
	///		Class designed to inform an IDamagable that it's being damaged! 
	///		It could possibly have other uses later
	/// </summary>
	public class DamagePacket
	{
		/// <summary>
		///		The source of the damage packet
		/// </summary>
		public ICombatant DmgSource { get; }

		public int PhysicalDamageOutput { get; }
		public int FireDamageOutput { get; }
		public int ColdDamageOutput { get; }
		public int PoisonDamageOutput { get; }
		public int EnergyDamageOutput { get; }
		public int DirectDamageOutput { get; }

		public DamagePacket(ICombatant dmgSource, int physicalDamageOutput = 0, int fireDamageOutput = 0, int coldDamageOutput = 0, int poisonDamageOutput = 0, int energyDamageOutput = 0, int directDamageOutput = 0)
		{
			DmgSource = dmgSource;
			PhysicalDamageOutput = physicalDamageOutput;
			FireDamageOutput = fireDamageOutput;
			ColdDamageOutput = coldDamageOutput;
			PoisonDamageOutput = poisonDamageOutput;
			EnergyDamageOutput = energyDamageOutput;
			DirectDamageOutput = directDamageOutput;
		}
	}

	/// <summary>
	///		A class that exists to contain a summary of the mobile's currently equiped weapon.
	///		The intention is to give the ability for the modal to inform anyone who asks all their wep stats at that exact moment
	///		A required output for ICombatant
	/// </summary>
	public class CalculatedWeaponStats
	{
		public float AttacksPerSecond { get; set; } = 0;
		public float AttackRange { get; set; } = 0;
		public float AttackDamage { get; set; } = 0;

		public float DirectDamagePercentage { get; set; } = 0;
		public float PhysicalDamagePercentage { get; set; } = 0;
		public float FireDamagePercentage { get; set; } = 0;
		public float ColdDamagePercentage { get; set; } = 0;
		public float PoisonDamagePercentage { get; set; } = 0;
		public float EnergyDamagePercentage { get; set; } = 0;

		public CalculatedWeaponStats()
		{
		}
	}

	/*
	 *	Interfaces
	 */

	public interface ITargetable : GodotNode2D
	{
		/// <summary>
		///		A flag indicating whether the ITargetable object is targetable
		///		Gives us an overarching flag that can override all targeting logic
		/// </summary>
		public bool IsTargetable { get; }

		/// <summary>
		///		A method to allow us to check several flags to determine if this ITargetable can be targetd specifically for combat 
		///		What this means could be very different depending on who is implementing it
		/// </summary>
		/// <returns>Returns true if the ITargetable object can be targeted for combat-related activities</returns>
		public bool IsValidCombatTarget();
		/// <summary>
		///		Gets called when someone chooses us as their target for any reason.
		/// </summary>
		public void OnTargeted();
	}

	public interface IDamageable : ITargetable
	{
		/// <summary>
		///		Flag indicating whether the IDamagable object is 'alive' or not
		///		Alive is generalyl define as Hit Points > 0
		/// </summary>
		public bool IsAlive { get; }
		/// <summary>
		///		A status flag that prevents damage from being done to this object
		/// </summary>
		public bool IsInvulnerable { get; set; }
		// Health
		public int MaxHitPoints { get; }
		public int HitPoints { get; }
		// Stats
		public int Strength { get; }
		public int Intelligence { get; }
		public int Dexterity { get; }
		// Resists
		public int PhysicalResitance { get; }
		public int FireResistance { get; }
		public int ColdResistance { get; }
		public int PoisonResistance { get; }
		public int EnergyResistance { get; }

		/// <summary>
		///		Interface for an IDamageable to interact with damage packets
		///		Sending a damage packet to 'OnHit' doesn't mean it will be applied, IDamagables may have their own defenses or counter attacks
		/// </summary>
		/// <param name="damagePacket">Damage packet for this IDamageable object to process.</param>
		public void OnHit(DamagePacket damagePacket);
	}

	////// THE METHOD SUMMARIES SHOULD BE MOVED DOWN TO MOBILE.
	public interface ICombatant : IDamageable
	{
		public bool IsCombatEnabled { get; }
		public bool IsInCombat { get; }

		public IAttack CurrentlyEquippedAttack { get; }
		public IDamageable CurrentCombatant { get; }

		/// <summary>
		///		Holds the AttackTargeting mask list that any newly equipped attack is expected to respect
		/// </summary>
		public List<PhysicsLayerNameEnum> CurrentAttackTargetingMaskLayerList { get; }
		/// <summary>
		///		Holds the Targeting strategy that any newly eqiupped attack is expected to respsect
		/// </summary>
		public TargetingStrategy CurrentAttackTargetingStrategy { get; }

		/// <summary>
		///		Allows for an outside source to reward the ICombatant with XP
		/// </summary>
		/// <param name="xp">The amt of XP to award</param>
		public void GiveXP(int xp);

		/// <summary>
		///		Gets called by the assigned intelligence when the object enters combat
		/// </summary>
		public void OnEnterCombat();
		/// <summary>
		///		Gets called by the assigned intelligence per loop when in combat
		/// </summary>
		public void OnCombat();
		/// <summary>
		///		Gets called by the assigned intelligence when the object leaves combat
		/// </summary>
		public void OnLeaveCombat();

		/// <summary>
		///		Examines the currently euqipped IAttack and any stats or effects on the ICombatant 
		///			and generates an up to date CalculatedWeaponStats object.
		///		Generated on request, considered stale once it leaves so use it quickly or ask again
		///		
		///		An optional parameter that allows any weapon to be analyzed this way.
		///		If left null, the Mobile's CurrentlyEquippedAttack's wep stats are calculated
		///		If CurrentlyEquipedAttack is also null an empty CalculatedWeaponStats full of zero's is returned.
		/// </summary>
		/// <param name="attack">Optional. If passed in wep stats will be based off this IAttack. Default is the Mobile's CurrentlyEquippedAttack</param>
		/// <returns>CalculatedWeaponStats for the desired weapon</returns>
		public CalculatedWeaponStats GetCalculatedWeaponStats(IAttack attack);
	}

	public interface IAnimatedCombatant : ICombatant, IAnimated
	{


		/// <summary>
		///		Gets called by the animation player when an attack animation finishes - NOT when the last animation frame plays, but when the time runs out.
		/// </summary>
		public void OnAttackAnimationFinished();

		/// <summary>
		///		This gets called by the animation player when an attack animation is 'making contact'.
		///		For melee attacks, this is the apex of the animation's swing, such as the moment before slash begins to retract. This is the visually ideal time to apply damage or w/e.
		///		For ranged attacks, this is the point at which the draw finishes and before the recoil shows. Ideally, this is when you would spawn a projectile, so that it looks like it is produced by the animation.
		///		For magical attacks, this is the point where the incantation appears to be finishing and are visually queued that some energy is being released, such as a projectile from our hands, or the final result of concetrated summoning
		/// </summary>
		public void OnAttackAnimationMakeContact();
	}
}
