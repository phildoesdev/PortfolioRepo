using Godot;
using UODef.Abilities;
using UODef.Abilities.Attacks;
using UODef.Abilities.Targeting;

namespace UODef.Abilities.Targeting
{
	/// <summary>
	///		Intended to be built out within the scene, as a child of a structure who will claim to be its parent
	///		This allows for multiple, varied, and unique areas for enemies to target, regardless of what the building looks like.
	///		This class is an implementation of IDamageable that just passes calls up to its parent IDamageable
	///		The structure itself will have no single targetable area, most likely, so this lets us create realistic, and meaningful areas for mobs to attack
	/// </summary>
	public partial class DamageableArea : Area2D, IDamageable
	{
		#region Internal Tracking variables
		public IDamageable ParentDamageable { get; set; }
		#endregion

		#region ITargetable Fields & Props
		public bool IsTargetable
		{
			get 
			{ 
				return ParentDamageable.IsTargetable; 
			}
		}
		#endregion

		#region IDamageable Fields & Props
		public bool IsAlive { get {	return ParentDamageable.IsAlive; } }
		public bool IsInvulnerable
		{
			get { return ParentDamageable.IsInvulnerable; }
			set { ParentDamageable.IsInvulnerable = value; }
		}
		public int MaxHitPoints { get { return ParentDamageable.MaxHitPoints; } }
		public int HitPoints { get { return ParentDamageable.HitPoints; } }
		public int Strength { get { return ParentDamageable.Strength; } }
		public int Intelligence { get { return ParentDamageable.Intelligence; } }
		public int Dexterity { get { return ParentDamageable.Dexterity; } }
		public int PhysicalResitance { get { return ParentDamageable.PhysicalResitance; } }
		public int FireResistance { get { return ParentDamageable.FireResistance; } }
		public int ColdResistance { get { return ParentDamageable.ColdResistance; } }
		public int PoisonResistance { get { return ParentDamageable.PoisonResistance; } }
		public int EnergyResistance { get { return ParentDamageable.EnergyResistance; } }
		#endregion


		#region ITargetable Methods
		public bool IsValidCombatTarget()
		{
			if (ParentDamageable == null) return false;
			return ParentDamageable.IsValidCombatTarget();
		}

		public void OnTargeted()
		{
			GD.PushError("I, A Targetable Area, have been targeted");
			ParentDamageable.OnTargeted();
		}
		#endregion

		#region IDamageable Methods
		public void OnHit(DamagePacket damagePacket)
		{
			ParentDamageable.OnHit(damagePacket);
		}
		#endregion
	}
}