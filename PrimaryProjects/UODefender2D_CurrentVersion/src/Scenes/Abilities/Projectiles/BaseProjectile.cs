using Godot;
using UODef.System;
using UODef.Abilities.Attacks;
using System.Reflection.Metadata.Ecma335;

namespace UODef.Abilities.Projectiles
{

    #region Enums
    public enum ProjectileMovementTypeEnum
    {
        None = 0,
		FollowTarget = 1,
        ToFixedPoint = 2
    }
	#endregion

    /// <summary>
    ///     Base Projectile. I chose to implement this like this by forcing anyone who wants to inherit me to create a new scene and attach themselves to it
    ///     So, Fireball has a scene that is an Area2d, an animated sprite, and a collision shape with the 'fireball.cs' being attached to it. 
    ///     I thought future me would enjoy the robustness of creating cool proljectiles in the scene editor
    /// </summary>
	public partial class BaseProjectile : Area2D
    {
		#region Projectile Mvmt
		/// <summary>
		///     If set, the projectile will pursue the target!
		/// </summary>
		protected IDamageable ProjectileTarget = null;
		/// <summary>
		///		If fixed target is set, we want to grab that position
		/// </summary>
		protected Vector2 ProjectileFixedTarget;
		/// <summary>
		///		Some cases we want to preserve our original direction and keep that forever. Stored here.
		/// </summary>
		private Vector2 DirectionToTarget = Vector2.Zero;

		/// <summary>
		///		A flag used for projectile navigation - gives us an extra frame of mvmt, specifically so
		///			'to fixed' point looks nice for arrowProjectiles
		/// </summary>
		private bool SlowlyStopProjectile = false;

		/// <summary>
		///     Depending on how this is set will determine how the projectile moves after being released
		/// </summary>
		protected ProjectileMovementTypeEnum ProjectileMovementType = ProjectileMovementTypeEnum.FollowTarget;

		protected float ProjectileVelocity = 350.0f;
        /// <summary>
        ///     The direction the projectile is facing by default. 
        ///     Lets us rotate to face our enemy!
        /// </summary>
        protected Vector2 BaseDirection = Vector2.Left;
		#endregion

		// lifespan
		private ulong SpawnTimeTicksMSec = 0;
        protected ulong ProjectileLifespanMS = 10000;
        // Mvmt throttling
        private ulong LastMvmtCalcTime = 0;
        private ulong MSecBetweenMovementCalcs = 5000;

        //
        protected DamagePacket Payload;

        public BaseProjectile(uint collisionLayers= (uint)PhysicsLayerNameEnum.Projectile, uint collisionMask= (uint)PhysicsLayerNameEnum.TowerTarget | (uint)PhysicsLayerNameEnum.CreatureTarget) : base() 
        {
            // The actual collision shape is created in teh scene for the specific projectile
			BodyEntered += BodyEnteredProjectileTargetArea;
            AreaEntered += AreaEnteredProjectileTargetArea;
			// Set our collision layer and mask. Could write robust way of setting this for children (like mobile has), but doesnt seem necesary atm
            CollisionLayer = collisionLayers;
            // This should be passed in from the attack or something, set a smarter way. I am sure it'll come up again...
			CollisionMask = collisionMask;
			// Today we were born
            SpawnTimeTicksMSec = Time.GetTicksMsec();
		}

		public override void _ExitTree()
		{
			BodyEntered -= BodyEnteredProjectileTargetArea;
			AreaEntered -= AreaEnteredProjectileTargetArea;
		}

		public override void _Ready()
		{
		}

		/// <summary>
		///     Get's called by the Area2D's 'BodyEntered' signal
		///     We check to see if the body we've made contact with is our 'ProjectileTarget'- if it is we'll call 'OnHit' and then 'EndLife'
		///     OnHit is intended to by overridden by whoever, and then endLife cleans everything up
		/// </summary>
		/// <param name="body"></param>
		protected void BodyEnteredProjectileTargetArea(Node2D body)
		{
            // Pass the on hit call down the chain
			OnHit((IDamageable)body);
		}

        /// <summary>
        ///     Gets called by Area2D's 'AreaEntered' signal
        ///     Calls the same code as body enetered for now
        /// </summary>
        /// <param name="area"></param>
        protected void AreaEnteredProjectileTargetArea(Area2D area)
		{
            OnHit((IDamageable)area);
		}

		#region Godot Overrides
		/// <summary>
		///     If you overwrite this you will break projectiles. Createa a hook if you want access.
		/// </summary>
		/// <param name="delta"></param>
		public override void _PhysicsProcess(double delta)
        {
			// We will kill ourselves if we last too long
			TrackLifeSpan();
		}
		#endregion

		#region Lifespan Maintenance
		private void TrackLifeSpan()
		{
			// Who wants to live forever?
			if ((Time.GetTicksMsec() - SpawnTimeTicksMSec) > ProjectileLifespanMS) EndLife();
		}

		/// <summary>
		///     Clean ourselves up and do w/e might need to be done?
		/// </summary>
		protected void EndLife()
		{
			QueueFree();
		}
		#endregion


		/// <summary>
		///     The default 'On Hit', just applies the damage payload if there is one
		/// </summary>
		/// <param name="hitTarget">Implies the body entering is the originally intended target. Some projectiles only hit their targets</param>
		protected virtual void OnHit(IDamageable hitTarget)
		{
		}

		/// <summary>
		///     We need a way to attach the damage payload *shrug* feels sort of bad, but roughly something like this neesd to happen
		/// </summary>
		/// <param name="damagePacket"></param>
		public void AttachDamagePayload(DamagePacket damagePacket)
		{
			Payload = damagePacket;
		}

		#region Movement and Navigation
		public void InitializeProjectile(IDamageable damageable, DamagePacket damagePacket, ProjectileMovementTypeEnum projectileMovementType = ProjectileMovementTypeEnum.None, float projectileVelocity = 350.0f)
		{
			ProjectileTarget = damageable;
			ProjectileVelocity = projectileVelocity;

			switch (projectileMovementType)
			{
				case ProjectileMovementTypeEnum.FollowTarget:
					break;
				case ProjectileMovementTypeEnum.ToFixedPoint:
					ProjectileFixedTarget = ProjectileTarget.GlobalPosition;
					DirectionToTarget = Vector2.Zero;
					break;
				case ProjectileMovementTypeEnum.None:
					break;
				default:
					break;
			}

			Payload = damagePacket;
			ProjectileMovementType = projectileMovementType;
		}

		/// <summary>
		///     Sets the target that we want to pursue.
		///     The default behavior of a projectile is 'target-seeking', meaning it will change directiosn to pursue its target
		/// </summary>
		/// <param name="damageable"></param>

		



		/// <summary>
		///     Our custom 'look at' incase we want to flip it to something less difficult to calculate in the future
		/// </summary>
		/// <param name="direction"></param>
		private void FaceDirection(Vector2 direction)
        {
            Rotation = BaseDirection.AngleTo(direction);
        }

		/// <summary>
		///     Pretty lazy implementation of movemnt, keeping it kind of stupid becuase Idk exactly what I want to do with this right now
		/// </summary>
		/// <param name="delta">Pass in delta so we can move nice and cheap</param>
		protected void CalculateMovement(double delta)
		{
			if (ProjectileMovementType == ProjectileMovementTypeEnum.None) return;
			// Not time to recalculate our trajectory yet
			if ((Time.GetTicksMsec() - LastMvmtCalcTime) <= MSecBetweenMovementCalcs) return;

			Vector2 NewMvmtVector = Vector2.Zero;

			switch (ProjectileMovementType)
			{
				case ProjectileMovementTypeEnum.None:
					break;
				case ProjectileMovementTypeEnum.FollowTarget:
					DirectionToTarget = (ProjectileTarget.GlobalPosition - GlobalPosition).Normalized();
					break;
				case ProjectileMovementTypeEnum.ToFixedPoint:
					// We set the original path to target, then
					if (DirectionToTarget == Vector2.Zero) DirectionToTarget = (ProjectileFixedTarget - GlobalPosition).Normalized();
					// We passed our target, missed, or otherwise, shorten lifespan
					if (ProjectileFixedTarget.DistanceTo(GlobalPosition) < 10.0f)
					{
						SlowlyStopProjectile = true;
						//SpawnTimeTicksMSec = Time.GetTicksMsec();
						//ProjectileLifespanMS = 100;
					}
					else if (SlowlyStopProjectile)
					{
						ProjectileMovementType = ProjectileMovementTypeEnum.None;
					}
					break;
				default:
					break;
			}
			NewMvmtVector = DirectionToTarget * ProjectileVelocity * (float)delta;
			FaceDirection(DirectionToTarget);
			GlobalPosition += NewMvmtVector;
		}
		#endregion


	}
}