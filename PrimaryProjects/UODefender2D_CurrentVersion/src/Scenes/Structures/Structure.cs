using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using UODef.Abilities;
using UODef.Abilities.Attacks;
using UODef.Abilities.Targeting;
using UODef.Mobiles;
using UODef.System;

namespace UODef.Structures
{
	public partial class Structure : Node2D, IDamageable
	{


		public string StructureName { get; set; } = "A Structure";
		public Guid StructureGuid { get; } = Guid.NewGuid();
		public bool IsDeleted { get; private set; } = false;

		#region Collision
		/// All mobiles will share this default Collision Layer 
		private readonly List<PhysicsLayerNameEnum> defaultCollisionLayerList = new() { PhysicsLayerNameEnum.Structures, PhysicsLayerNameEnum.CreatureTarget };
		/// All mobiles will share this default collision mask
		private readonly List<PhysicsLayerNameEnum> defaultCollisionMaskList = new() { PhysicsLayerNameEnum.Projectile};
		#endregion

		#region IDamageable && ITargetable Properties
		public bool IsAlive { get; protected set; } = true;
		public bool IsTargetable { get; set; } = true;
		public bool IsInvulnerable { get; set; } = false;

		private int _MaxHitPoints;
		public int MaxHitPoints
		{
			get { return _MaxHitPoints; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MaxHitPoints", "MaxHitPoints Cannot be less than or equal to Zero. N58NO925");
				}
				_MaxHitPoints = value;
				if (_MaxHitPoints < HitPoints)
				{
					HitPoints = _MaxHitPoints;
				}
			}
		}

		private int _HitPoints;
		public int HitPoints
		{
			get { return _HitPoints; }
			set
			{
				int tmpHitPoints = value;

				if (tmpHitPoints < 0) tmpHitPoints = 0;
				if (tmpHitPoints > MaxHitPoints) tmpHitPoints = MaxHitPoints;
				bool valChanged = (tmpHitPoints != HitPoints);

				_HitPoints = tmpHitPoints;
				// If the hitpoint value has changed 
				if (valChanged && tmpHitPoints == 0)
				{
					// Rest in pieces-- sets flags and calls kill to intiate our death
					IsAlive = false;
					IsTargetable = false;
					KillSelf();
				}
			}
		}

		private int _Strength;
		public int Strength
		{
			get { return _Strength; }
			set { _Strength = value; }
		}

		private int _Intelligence;
		public int Intelligence
		{
			get { return _Intelligence; }
			set { _Intelligence = value; }
		}

		private int _Dexterity;
		public int Dexterity
		{
			get { return _Dexterity; }
			set { _Dexterity = value; }
		}

		private int _PhysicalResitance;
		public int PhysicalResitance
		{
			get { return _PhysicalResitance; }
			set { _PhysicalResitance = value; }
		}

		private int _FireResistance;
		public int FireResistance
		{
			get { return _FireResistance; }
			set { _FireResistance = value; }
		}

		private int _ColdResistance;
		public int ColdResistance
		{
			get { return _ColdResistance; }
			set { _ColdResistance = value; }
		}

		private int _PoisonResistance;
		public int PoisonResistance
		{
			get { return _PoisonResistance; }
			set { _PoisonResistance = value; }
		}

		private int _EnergyResistance;
		public int EnergyResistance
		{
			get { return _EnergyResistance; }
			set { _EnergyResistance = value; }
		}
		#endregion

		#region Internal Tracking
		/// <summary>
		///		Reference to our targetable areas so we can do things like change collision layers 
		/// </summary>
		private List<DamageableArea> StructureDamageableAreas;
		#endregion


		public Structure() : base()
		{
			SetResists(physicalResitance: 0, fireResistance: 0, coldResistance: 0, poisonResistance: 0, energyResistance: 0);
			SetStats(hitPoints: 150, str: 10, dex: 10, intel: 10);

			/// More hapepns once this node enters the scene tree in the _Ready method
		}

		public override void _Ready()
		{
			// Initialize targetable areas 
			InitializeDamageableAreas();
		}

		/// <summary>
		///		Find all targetable areas under this structure node, make record of them, and set their collision layer/masks
		/// </summary>
		private void InitializeDamageableAreas()
		{
			StructureDamageableAreas = new List<DamageableArea>();
			foreach (DamageableArea targetableArea in FindChildren(pattern: "DamageableArea_*"))
			{
				targetableArea.ParentDamageable = this;
				StructureDamageableAreas.Add(targetableArea);
			}
			// Force the collision layer and mask that we want (This sets the collision layers for all targetable areas that we've found)
			OverrideCollisionLayer(defaultCollisionLayerList);
			OverrideCollisionMaskLayer(defaultCollisionMaskList);
		}

		/// <summary>
		///		Do not override. Required to clean up listeners and what have you.
		/// </summary>
		public override void _ExitTree()
		{
			base._ExitTree();
		}

		#region Structure Support Methods
		protected void SetResists(int physicalResitance = 0, int fireResistance = 0, int coldResistance = 0, int poisonResistance = 0, int energyResistance = 0)
		{
			PhysicalResitance = physicalResitance;
			FireResistance = fireResistance;
			ColdResistance = coldResistance;
			PoisonResistance = poisonResistance;
			EnergyResistance = energyResistance;
		}
		protected void SetStats(int hitPoints = 1, int str = 0, int dex = 0, int intel = 0)
		{
			_MaxHitPoints = _HitPoints = hitPoints;
			Strength = str;
			Dexterity = dex;
			Intelligence = intel;
		}
		#endregion


		#region IDamageable && ITargetable Methods
		/// <summary>
		///		Given a DamagePacket, this IDamagable must decide how to interpret what has be wrought upon it
		/// </summary>
		/// <param name="damagePacket">DamagePacket object describing what damage is coming</param>
		public virtual void OnHit(DamagePacket damagePacket)
		{
			// Calculate damage, if it is a fatal blow set flags and report it to the dmg source. Finally 
			int totalDmg = CalculateDamage(damagePacket);
			HitPoints -= totalDmg;
		}

		/// <summary>
		///		Do the work
		/// </summary>
		/// <param name="dp"></param>
		public int CalculateDamage(DamagePacket damagePacket)
		{
			float dmgDirect = damagePacket.DirectDamageOutput;
			float dmgPhysicalResitance = (1.0f - (float)PhysicalResitance) * damagePacket.PhysicalDamageOutput;
			float dmgFireResistance = (1.0f - (float)FireResistance) * damagePacket.FireDamageOutput;
			float dmgColdResistance = (1.0f - (float)ColdResistance) * damagePacket.ColdDamageOutput;
			float dmgPoisonResistance = (1.0f - (float)PoisonResistance) * damagePacket.PoisonDamageOutput;
			float dmgEnergyResistance = (1.0f - (float)EnergyResistance) * damagePacket.EnergyDamageOutput;

			float totalDmgOut = dmgDirect + dmgPhysicalResitance + dmgFireResistance + dmgColdResistance + dmgPoisonResistance + dmgEnergyResistance;
			return (int)totalDmgOut;
		}

		public bool IsValidCombatTarget()
		{
			return IsTargetable && IsAlive && !IsDeleted;
		}
		
		public void OnTargeted()
		{
			GD.PushError("I, A structure, Have been targeted");
			// uh-oh
		}
		#endregion

		/// <summary>
		///		Enters the structure into its death throes
		///		Marks the structure as not-targetable and then does w/e death stuff a structure might do
		/// </summary>
		private void KillSelf()
		{
			// Set a bunch of flags preventing this mobile from interacting or being interacted with
			IsAlive = false;
			IsTargetable = false;

			// Unlikely that this will be called here in the future
			Die();
		}

		/// <summary>
		///		Does some of the actaul cleanup process like calling godot's QueuFree()
		/// </summary>
		public void Die()
		{
			IsDeleted = true;
			SetPhysicsProcess(false);
			SetProcess(false);
			QueueFree();
		}

		#region Physics & Collision Mgmt Methods
		/// <summary>
		///     A human readable interface into our Mobile's collision layers
		///     Appends a collision layer to the Mobile's CharacterBody2D collision layer
		/// </summary>
		/// <param name="newLayer">PhysicsLayerName enum value that we want to add to this mobile</param>
		public void AddCollisionLayer(PhysicsLayerNameEnum newLayer)
		{
			foreach (DamageableArea damageableArea in StructureDamageableAreas)
			{
				damageableArea.CollisionLayer |= (uint)newLayer;
			}
		}

		/// <summary>
		///     A human readable interface into our Mobile's collision layers
		///     Overrides the mobile's CharacterBody2D collision layer
		/// </summary>
		/// <param name="newLayer">PhysicsLayerName enum value to set this mobile's CollisionLayer to</param>
		public void OverrideCollisionLayer(PhysicsLayerNameEnum newLayer)
		{
			foreach (DamageableArea damageableArea in StructureDamageableAreas)
			{
				damageableArea.CollisionLayer = (uint)newLayer;
			}
		}

		/// <summary>
		///     A human readable interface into our Mobile's collision layers
		///     Overrides the mobile's CharacterBody2D collision layer
		/// </summary>
		/// <param name="newCollisionLayerList">PhysicsLayerName enum value list to set this mobile's CollisionLayer to</param>
		public void OverrideCollisionLayer(List<PhysicsLayerNameEnum> newCollisionLayerList)
		{
			foreach (DamageableArea damageableArea in StructureDamageableAreas)
			{
				damageableArea.CollisionLayer = 0;
				foreach (var layer in newCollisionLayerList)
				{
					damageableArea.CollisionLayer |= (uint)layer;
				}
			}
		}

		/// <summary>
		///     A human redable interface into our Mobile's Collision Mask
		///     Appends a collision mask layer to the Mobile's CharacterBody2D Collision Mask
		/// </summary>
		/// <param name="maskLayer"></param>
		public void AddCollisionMaskLayer(PhysicsLayerNameEnum maskLayer)
		{
			foreach (DamageableArea damageableArea in StructureDamageableAreas)
			{
				damageableArea.CollisionMask |= (uint)maskLayer;
			}
		}

		/// <summary>
		///     A human readable interface into the Mobile's collision mask
		///     Override the mobile's CharacterBody2D CollisionMask
		/// </summary>
		/// <param name="maskLayer">PhysicsLayerName enum value to set this mobile's CollisionmaskLayer to</param>
		public void OverrideCollisionMaskLayer(PhysicsLayerNameEnum maskLayer)
		{
			foreach (DamageableArea damageableArea in StructureDamageableAreas)
			{
				damageableArea.CollisionMask = (uint)maskLayer;
			}
		}

		/// <summary>
		///     A human readable interface into the Mobile's collision mask
		///     Override the mobile's CharacterBody2D CollisionMask
		/// </summary>
		/// <param name="collisionMaskList">List<PhysicsLayerName> to set the new layer to</param>
		public void OverrideCollisionMaskLayer(List<PhysicsLayerNameEnum> collisionMaskList)
		{
			foreach (DamageableArea damageableArea in StructureDamageableAreas)
			{
				damageableArea.CollisionMask = 0;
				foreach (var layer in collisionMaskList)
				{
					damageableArea.CollisionMask |= (uint)layer;
				}
			}
		}
		#endregion

	}
}
