

using Godot;
using UODef.System;

namespace UODef.Abilities.Projectiles
{
	public partial class TempestsBoltProjectile : BaseProjectile
	{
		private CollisionShape2D CollisionShape;
		private CircleShape2D Shape;

		private Sprite2D AoeIndicator;

		/// <summary>
		///		The size of our collision shape that we use to deal dmg
		/// </summary>
		private float DamageRadius;

		public TempestsBoltProjectile() : base( 
			collisionLayers:((uint)PhysicsLayerNameEnum.AOE | (uint)PhysicsLayerNameEnum.Projectile), 
			collisionMask: (uint)PhysicsLayerNameEnum.TowerTarget)
		{
			ProjectileVelocity = 0.0f;
			ProjectileMovementType = ProjectileMovementTypeEnum.None;	
		}

		public void SetupBolt(DamagePacket dmgPayload, float damageRadius, ulong effectDurationMS)
		{
			DamageRadius = damageRadius;
			ProjectileLifespanMS = effectDurationMS;

			AttachDamagePayload(dmgPayload);
		}

		#region Godot Overrides
		public override void _Ready()
		{
			base._Ready();
			InitializeCollision();
			InitializeAOEIndicator();
		}

		public override void _Draw()
		{
			//Font font = ResourceLoader.Load<FontFile>("res://src/Assets/UserInterface/Fonts/ouClassic.ttf");
			//DrawString(font, Position, "Test");

			// DrawCircle(Vector2.Zero, DamageRadius, color: Colors.Purple, filled: true);
		}
		#endregion

		#region Initialization
		private void InitializeCollision()
		{
			CollisionShape = new CollisionShape2D();
			Shape = new CircleShape2D();
			Shape.Radius = DamageRadius;
			CollisionShape.Shape = Shape;
			AddChild(CollisionShape);
		}

		private void InitializeAOEIndicator()
		{
			AoeIndicator = GetNode<Sprite2D>("AOE_Indicator");
			((ShaderMaterial)AoeIndicator.Material).SetShaderParameter("aoe_radius", DamageRadius);
		}
		#endregion

		#region BaseProjectile overrides
		protected override void OnHit(IDamageable hitTarget)
		{
			// This is super placeholder, but it's our 'on hit'
			if (Payload != null)
			{
				hitTarget.OnHit(Payload);
			}

			// We wont end our life until the timer kicks
		}
		#endregion
	}
}


