

using Godot;

namespace UODef.Abilities.Projectiles
{
	public partial class ArrowProjectile : BaseProjectile
	{
		public ArrowProjectile() : base()
		{
		}

		public override void _PhysicsProcess(double delta)
		{
			if (ProjectileTarget == null || !ProjectileTarget.IsValidCombatTarget())
			{
				// GD.PushError("Projectile created without a target. This is likely an error. NG2OL3R6");
				EndLife();
			}
			CalculateMovement(delta);
			base._PhysicsProcess(delta);
		}

		/// <summary>
		///		Fireball's on hit 
		/// </summary>
		/// <param name="hitTarget"></param>
		protected override void OnHit(IDamageable hitTarget)
		{
			// This is super placeholder, but it's our 'on hit'
			if (hitTarget == ProjectileTarget && Payload != null)
			{
				// GD.PushError($"hitTarget: {hitTarget} == {ProjectileTarget}: [{hitTarget == ProjectileTarget}] -- Payload: {Payload}");
				hitTarget.OnHit(Payload);
				EndLife();
			}
		}
	}
}
