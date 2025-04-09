using Godot;
using System;
using System.Collections.Generic;
using UODef.Abilities;
using UODef.Abilities.Attacks;
using UODef.Abilities.Targeting;
using UODef.Leveling;
using UODef.Mobiles.Intelligence;
using UODef.Mobiles.Navigation;
using UODef.Player;
using UODef.System;
using UODef.Mobiles.AnimationPlayer;
using UODef.Items.Equipment;


namespace UODef.Mobiles
{
	#region Enums
	/// <summary>
	///     The possible resistance types
	/// </summary>
	public enum ResistanceTypeEnum
	{
		None,
		Physical,
		Fire,
		Cold,
		Poison,
		Energy
	}

	public enum StatTypeEnum
	{
		None,
		Strength,
		Dexterity,
		Intelligence
	}

	/// <summary>
	///     Where this mobile does its movements
	/// </summary>
	public enum MovementDomainEnum
	{
		None,
		Ground,
		Air,
		Water,
	}
	#endregion


	/// <summary>
	///     Base class representing players, npcs, and creatures.
	/// </summary>
	public partial class Mobile : CharacterBody2D, INavigator, IAnimatedCombatant
	{
		private PlayerProfile Player;

		public string MobileName { get; set; } = "A Mobile";
		public Guid MobileGuid { get; } = Guid.NewGuid();
		public bool IsDeleted { get; private set; } = false;
		public MovementDomainEnum MovementDomain { get; set; }

		/// <summary>
		///		The FSM used by this mobile to determine its own behavior
		/// </summary>
		private BaseIntelligence mobileAI;

		#region Healthbar
		/// <summary>
		///		Higher lvl control to enable or disalbe healthbar
		/// </summary>
		public bool ShowHealthbar { get; set; } = true;
		/// <summary>
		///		The actual healthbar scene we loaded in- represented by a TextureProgressbar for now
		/// </summary>
		public TextureProgressBar Healthbar { get; set; }
		#endregion

		#region IAnimated properties
		// We will look south by default
		public DirectionEnum CurrentDirection { get; protected set; } = DirectionEnum.South;
		public Vector2 CurrentDirectionVector { get; protected set; } = Vector2.Zero;
		#endregion

		#region INavigator properties
		/// <summary>
		///		Automatically gets set to true if SetMovementInfo is called
		/// </summary>
		public bool IsMovementEnabled { get; set; } = false;
		public bool NavigationDestinationReached { get; protected set; } = true;
		public NavigationDestination DefaultDestination { get; protected set; }
		public NavigationDestination CurrentDestination { get; protected set; }
		#endregion

		public virtual float WalkSpeed { get; set; }


		#region Collision
		/// All mobiles will share this default Collision Layer 
		private readonly List<PhysicsLayerNameEnum> defaultCollisionLayerList = new() { PhysicsLayerNameEnum.Mobile };
		/// All mobiles will share this default collision mask
		private readonly List<PhysicsLayerNameEnum> defaultCollisionMaskList = new() { PhysicsLayerNameEnum.Impassable, PhysicsLayerNameEnum.Projectile };
		#endregion

		#region ICombatant && Misc. Combat Properties
		/// <summary>
		///		A high level flag that indicates this mobile will look for targets to attack.
		///		If set to false, they will never enter combat mode in the AI.
		///		False by default, gets set to true if an attack has been equiped
		/// </summary>
		public bool IsCombatEnabled { get; set; } = false;
		/// <summary>
		///		Gets set by us when our AI calls OnEnterCombat. 
		///		Used as a flag to indicate that this mobile is attacking a target
		/// </summary>
		public bool IsInCombat { get; set; } = false;

		public IAttack CurrentlyEquippedAttack { get; protected set; }
		public IDamageable CurrentCombatant { get; protected set; } = null;

		/// <summary>
		///		Holds the Targeting strategy that any newly eqiupped attack is expected to respsect
		/// </summary>
		public TargetingStrategy CurrentAttackTargetingStrategy { get; protected set; } = TargetingStrategy.Closest;
		/// <summary>
		///		Holds the AttackTargeting mask list that any newly equipped attack is expected to respect
		/// </summary>
		public List<PhysicsLayerNameEnum> CurrentAttackTargetingMaskLayerList { get; protected set; } = new();

		/// <summary>
		///		Saved as a time (in seconds) relative to our delta clock, used to meter our hand in battle
		/// </summary>
		public double NextAttackTimeMsec { get; protected set; } = 0.0;
		#endregion

		#region IDamageable Properties
		public bool IsAlive { get; protected set; } = true;
		public bool IsTargetable { get; set; } = true;
		public bool IsInvulnerable { get; set; } = false;

		private int _MaxHitPoints = 1;
		public int MaxHitPoints
		{
			get { return _MaxHitPoints; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MaxHitPoints", "MaxHitPoints Cannot be less than or equal to Zero. 8UBQ2XNI");
				}
				_MaxHitPoints = value;
				if (_MaxHitPoints < HitPoints)
				{
					HitPoints = _MaxHitPoints;
				}
			}
		}

		private int _HitPoints = 1;
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

				Healthbar.Value = (100.0 * ((double)_HitPoints / _MaxHitPoints));
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

		private PackedScene DmgTxtAnimationPS = GD.Load<PackedScene>("res://src/Scenes/Mobiles/Misc/DamageText.tscn");
		#endregion

		#region Loot && Reward Properties
		/// <summary>
		///		High lvl override for preventing a default reward from going out.
		///		Defaulted to false, automatically gets set to true if a reward is set.
		/// </summary>
		public bool IsLootable { get; set; } = false;

		public int TowerXPReward { get; set; } = 0;
		public int PlayerXPReward { get; set; } = 0;
		public int GoldReward { get; set; } = 0;
		public int PointsReward { get; set; } = 0;
		/// <summary>
		///		Keeps track of who has done dmg to us and when. Used to distribute rewards on death
		/// </summary>
		private DamageBooklet DmgBooklet = new();
		#endregion

		#region ILeveler && XPManager Properties
		
		#endregion

		#region Various Declarations
		/// <summary>
		///		TargetingAgent specifically related to finding a combat targe
		/// </summary>
		private TargetingAgent Targeting;
		/// <summary>
		///		Default range for the default targeting
		/// </summary>
		public float MobileVisionRange = 0.0f;

		private XPManager XpManager;

		// Animation
		protected MobileAnimationPlayer AniPlayer;

		// Navigation
		private NavigationManager NavigationMngr;

		// Timer
		private DeltaTimeClock DeltaClock;

		// Godot
		private CollisionShape2D CollisionShape;
		private CircleShape2D Shape;
		#endregion

		#region Signals

		[Signal]
		public delegate void DeathEventHandler();
		#endregion

		public Mobile(int sheetID, AnimationArchetypeEnum animationArchetype, IntelligenceTypeEnum intelligenceType, LevelingCurveTypeEnum lvlingCurveType, int hueID=0) : base()
		{
			#region Godot Node Setup
			// Default CharacterBody2D settings to put us at a baseline
			MotionMode = MotionModeEnum.Floating;
			WallMinSlideAngle = Mathf.DegToRad(1.0f);
			SafeMargin = 1.0f;
			#endregion

			// Set up lvling
			if (lvlingCurveType != LevelingCurveTypeEnum.None)
			{
				XpManager = new XPManager(lvlingCurveType);
				XpManager.Leveled += LevelUpCallback;
				XpManager.ReadyToLevel += ReadyToLvlCallback;
				XpManager.AutoLevelUp = true;
				AddChild(XpManager);
			}

			// Force the collision layer and mask that we want
			OverrideCollisionLayer(defaultCollisionLayerList);
			OverrideCollisionMaskLayer(defaultCollisionMaskList);

			switch (intelligenceType)
			{
				case IntelligenceTypeEnum.BaseTower:
					mobileAI = new BaseIntelligence(this);
					break;
				case IntelligenceTypeEnum.BaseCreature:
					mobileAI = new BaseIntelligence(this);
					break;
				default:
					GD.PushError("No 'AI' chosen. SS4UA6PY");
					break;
			}

			// Set up animation player and sprite
			AniPlayer = new MobileAnimationPlayer(animationArchetype, sheetID, hueID);
		}

		#region Godot Overrides and Related Hooks
		public override void _PhysicsProcess(double delta)
		{
			mobileAI.Think();

			// Eventually I assume there will be a lot of these checks for basic things like this that can eventually split out into their own methods with more interesting logic
			// For now, we just show the healthbar if set to do so, adn the hit points gets below max. Seems fair enough
			Healthbar.Visible = (ShowHealthbar && HitPoints < MaxHitPoints && HitPoints > 0) ? true : false;
		}

		/// <summary>
		///		Use Mobile.OnReady() instead of overriding _Ready(). If you override this things will break b/c no godot nodes will get added to the scene tree. 
		/// </summary>
		public override void _Ready()
		{
			InitializeScene();
			FindPlayer();
		}
		#endregion

		#region Mobile Support Methods
		/// <summary>
		///		A handy interface for setting all resists in one method call
		/// </summary>
		/// <param name="physicalResitance"></param>
		/// <param name="fireResistance"></param>
		/// <param name="coldResistance"></param>
		/// <param name="poisonResistance"></param>
		/// <param name="energyResistance"></param>
		protected void SetResists(int physicalResitance = 0, int fireResistance = 0, int coldResistance = 0, int poisonResistance = 0, int energyResistance = 0)
		{
			PhysicalResitance = physicalResitance;
			FireResistance = fireResistance;
			ColdResistance = coldResistance;
			PoisonResistance = poisonResistance;
			EnergyResistance = energyResistance;
		}

		/// <summary>
		///		A handy interface for setting all basic stats in one method call
		/// </summary>
		/// <param name="hitPoints"></param>
		/// <param name="str"></param>
		/// <param name="dex"></param>
		/// <param name="intel"></param>
		protected virtual void SetStats(int hitPoints = 1, int str = 0, int dex = 0, int intel = 0)
		{
			_MaxHitPoints = _HitPoints = hitPoints;
			Strength = str;
			Dexterity = dex;
			Intelligence = intel;
		}

		/// <summary>
		///		Set movement info. Sets the high lvl flag 'IsMovementEnabled' to true
		/// </summary>
		/// <param name="mvmtDomain">Enum indicating how this mobile travels</param>
		/// <param name="mvmtSpeed">Float indicating the mobile's mvmt speed</param>
		protected void SetMovementInfo(MovementDomainEnum mvmtDomain, float mvmtSpeed)
		{
			IsMovementEnabled = true;
			MovementDomain = mvmtDomain;
			WalkSpeed = mvmtSpeed;
		}

		/// <summary>
		///		Sets the reward for killing this creature. 
		///		Currently this means XP and gold, but could evolve to mean items, special lottery style chest spawns, probability, and who knows what.
		/// </summary>
		/// <param name="towerXP">How much XP to reward the tower for the death of this creature</param>
		/// <param name="playerXP">How much XP to reward the player for the death of this creature</param>
		/// <param name="gold">How much gold to reward the player for the death of this creature</param>
		/// <param name="points">The 'score' associated with the death of this creature</param>
		public void SetReward(int towerXP, int playerXP, int gold, int points)
		{
			IsLootable = true;
			TowerXPReward = towerXP;
			PlayerXPReward = playerXP;
			GoldReward = gold;
			PointsReward = points;
		}
		#endregion

		#region INavigator & Related Methods
		public bool CanTravel()
		{
			return IsMovementEnabled && !NavigationDestinationReached && IsAlive && !IsDeleted;
		}

		public void SetVelocityAndMove(Vector2 newVelocity)
		{
			Velocity = newVelocity;
			MoveAndSlide();
		}

		public void SetNewNavigationDestination(NavigationDestination dest, bool resetDefaultDestination = false)
		{
			//Nothing to do
			if (dest.TargetDestination == CurrentDestination?.TargetDestination && dest.NavMapRid == CurrentDestination?.NavMapRid) return;
			if (!IsMovementEnabled)
			{
				GD.PushError("Attempted to set a navigation destination even though movement is disabled. Possibly never initialized. O0D1FYAW");
				return;
			}

			if (resetDefaultDestination)
			{
				DefaultDestination = dest;
			}
			NavigationMngr?.SetNavigationCurrentDestination(dest);
			NavigationDestinationReached = false;
		}

		public void SetDirection(Vector2 direction, bool tryHard = true)
		{
			CurrentDirectionVector = direction;
			CurrentDirection = DirectionUtility.CalculateDirectionEnum(direction, tryHard);
		}

		/// <summary>
		///		Called by our AI 'on travel'. Communicate with our navigation manager to find our new velocity
		/// </summary>
		public void OnTravel()
		{
			// Ask the navigation manager to calculate our next mvmtposition
			Vector2 NextPos = NavigationMngr.GetNextPosition();
			// right now, if nav manager returns vector2.zero, we dont want to do anything- keep velocity
			if (NextPos == Vector2.Zero) return;
			// Stores this dir plus converts the calculated direction into one of our standard directions
			SetDirection((NextPos - GlobalPosition).Normalized(), true);
			// Calculate
			Vector2 tVelocity = (CurrentDirectionVector) * WalkSpeed;
			// If someone were to know that we were not using navigation agent avoidance they could call mobile.SetVelocityAndMove directly.
			NavigationMngr.SetVelocity(tVelocity);

			PlayAnimation(DefaultAnimationGlossaryEnum.DefaultWalkAnimation, CurrentDirection, speedMultiplier: 20.0f/WalkSpeed);
		}

		/// <summary>
		///		Called by our AI 'OnLeaveTravel' - stop the mobile
		/// </summary>
		public void OnLeaveTravel()
		{
			NavigationMngr.SetVelocity(Vector2.Zero);
		}

		/// <summary>
		///		Called by our AI 'OnDestinationReached' - special behavior when a set destination is reached 
		/// </summary>
		public void OnDestinationReached()
		{
			// GD.PushError($"Destination Reached... DefaultDestination: {DefaultDestination} CurrentDestination: {CurrentDestination}");
			NavigationDestinationReached = true;
			// If we reached our current destination and it is not the default destination 
			if (DefaultDestination != null && DefaultDestination != CurrentDestination)
			{
				SetNewNavigationDestination(DefaultDestination);
			}
		}
		#endregion

		#region Equipment
		// This is a pretty brash implementation of this... but just going with it for now		
		public List<IWearableEquipment> EquippedGear { get; private set; } = [];
		
		// This is a pretty brash implementation of this... but just going with it for now
		public void EquipGear(List<IWearableEquipment> newEquipmentList)
		{
			EquippedGear = newEquipmentList;
		}
		#endregion

		#region ICombatant && Related Methods
		/// <summary>
		///		Equips our attack.
		///		Sets IsCombatEnabled to true by default
		/// </summary>
		/// <param name="attack">The attack we're trying to equip</param>
		/// <param name="overrideCombatEnabled">If set to false we will not set the 'IsCombatEnabled' flag to true</param>
		public void EquipAttack(IAttack attack, bool overrideCombatEnabled = true)
		{
			// Release it from the scene
			if (CurrentlyEquippedAttack != null) CurrentlyEquippedAttack.QueueFree();

			// Set combat enabled
			if (overrideCombatEnabled) IsCombatEnabled = true;

			// Set it, add to scene, and enable combat
			CurrentlyEquippedAttack = attack;
			AddChild((Node2D)attack);
		}

		/// <summary>
		///		Available to be caleld by our ai
		/// </summary>
		public void OnEnterCombat()
		{
			// Sanity check
			if (!IsCombatEnabled)
			{
				CurrentCombatant = null;
				return;
			}
			// Passed our checks, set this flag.
			IsInCombat = true;
		}

		/// <summary>
		///		Available to be caleld by our ai - keeps track of the speed at which we request our equipped weapon attacks. 
		/// </summary>
		public virtual void OnCombat()
		{
			// Because of how we are calculating this we will nevr go beyond process frames per second attack speed - not worth the rework atm
			if (NextAttackTimeMsec <= DeltaClock.DeltaTimeSum)
			{
				// Verify our target is still valid, or find a new one if possible. If not, we're essentially bailing out so next tick our intelligence should push us out of combat
				if (!FindCombatTarget()) return;

				// Calculate from last frame when the next attack should occur
				NextAttackTimeMsec = DeltaClock.DeltaTimeSum + (1.0f / GetCalculatedWeaponStats().AttacksPerSecond);
				CurrentlyEquippedAttack.DoAttack(CurrentCombatant);     // This feels pretty lazy b/c surely we'll care if there is no equipped attack?
			}
		}

		/// <summary>
		///		Available to be called by our ai - resets flags and such
		/// </summary>
		public void OnLeaveCombat()
		{
			NextAttackTimeMsec = 0.0;
			CurrentCombatant = null;
			IsInCombat = false;
		}

		/// <summary>
		///		'Find Combat Target' - Consults the currently equipped attack for a valid target
		/// </summary>
		public virtual bool FindCombatTarget()
		{
			// Use our currently equipped attack to find the combat target
			IDamageable tmpTarget = CurrentlyEquippedAttack.FindCombatTarget(CurrentCombatant);
			if (tmpTarget != null && tmpTarget.IsValidCombatTarget())
			{
				// If the target hasn't changed we don't need to do much
				if (CurrentCombatant != tmpTarget)
				{
					tmpTarget?.OnTargeted();
					CurrentCombatant = tmpTarget;
				}
				// FACE YOUR TARGET!
				if (CurrentCombatant != null)
				{
					SetDirection((CurrentCombatant.GlobalPosition - GlobalPosition).Normalized());
					return true;
				}
			}
			// Couldnt find new target and old target is not valid
			CurrentCombatant = null;
			return false;
		}

		/// <summary>
		///		Enters the mobile into its death throes
		///		Marks the mobile as not-targetable and then plays w/e death animation.
		///		Presuming that this death animation handles the call to this.die() or w/e and finishes upthe process of cleaning up the mobile
		/// </summary>
		private void KillSelf()
		{
			// Set a bunch of flags preventing this mobile from interacting or being interacted with
			IsAlive = false;
			IsTargetable = false;
			IsMovementEnabled = false;
			NavigationMngr?.DisableNavigation(); 
			OverrideCollisionLayer(PhysicsLayerNameEnum.DeadMobile);
			DistributeRewards();
			EmitSignal(SignalName.Death);

			// Play the death animation. We are expecting that this animation will call die() at its conclusion
			PlayAnimation(DefaultAnimationGlossaryEnum.DefaultDieAnimation, CurrentDirection, overrideIfPlaying:true);
		}

		/// <summary>
		///		Final step of cleanup, 'poofs' the mobile away with queuefree and w/e flags we may want to set
		///		*Poof*
		/// </summary>
		public void Die()
		{
			IsDeleted = true;
			SetPhysicsProcess(false);
			SetProcess(false);
			QueueFree();
		}

		/// <summary>
		///		Called in killself, organizes all reward related work
		/// </summary>
		public void DistributeRewards()
		{
			if (!DmgBooklet.HasEntries()) return;
			ICombatant RewardRecipient = DmgBooklet.GetMostDmg();
			RewardRecipient.GiveXP(TowerXPReward);
			Player.GiveXP(PlayerXPReward);
			Player.GiveGP(GoldReward);
		}

		/// <summary>
		///		Req'd for ICombatant to receive XP points
		/// </summary>
		/// <param name="xp"></param>
		public void GiveXP(int xp)
		{
			XpManager.GiveXP(xp);
		}


		/// <summary>
		///		Return an object that contains information about the currently equiped weapon after considering
		///			the mobile's stats.
		///			
		///		The current Default looks like:
		///			Int - Magic Spell Inc
		///			Str	- Wp Dmg Inc
		///			Dex - Cast/Attack Speed
		///			
		///			Str+Dex = Melee Range
		///			Int+Dex = Magic Range
		///			
		///		An optional parameter that allows any weapon to be analyzed this way.
		///		If left null, the Mobile's CurrentlyEquippedAttack's wep stats are calculated
		///		If CurrentlyEquipedAttack is also null an empty CalculatedWeaponStats full of zero's is returned.
		/// </summary>
		/// <param name="attack">Optional. If passed in wep stats will be based off this IAttack. Default is the Mobile's CurrentlyEquippedAttack</param>
		/// <returns>CalculatedWeaponStats for the desired weapon</returns>
		public virtual CalculatedWeaponStats GetCalculatedWeaponStats(IAttack attack=null)
		{
			CalculatedWeaponStats wepStats = new();
			IAttack wep = attack;

			// Sanity check -
			if (wep == null && CurrentlyEquippedAttack == null) return wepStats;
			if (wep == null) wep = CurrentlyEquippedAttack;

			// Set base stats from weapon definition
			wepStats.AttacksPerSecond = wep.BaseAttacksPerSecond;
			wepStats.AttackRange = wep.BaseAttackRange;
			wepStats.AttackDamage = wep.BaseAttackDamage;

			// Modulate these stats based on the weapon type and the mobile's stats
			switch (wep.AttackCategory)
			{
				case AttackCategoryEnum.Ranged:
				case AttackCategoryEnum.Melee:
					wepStats.AttackDamage += (Strength / 250.0f);
					wepStats.AttacksPerSecond += (Dexterity / 500.0f);
					wepStats.AttackRange += (Strength + Dexterity) / 100.0f;
					break;
				case AttackCategoryEnum.Magical:
					wepStats.AttackDamage += (Intelligence / 250.0f);
					wepStats.AttacksPerSecond += (Intelligence / 500.0f);
					wepStats.AttackRange += (Intelligence + Dexterity) / 100.0f;
					break;
				default:
					break;
			}

			// Right now damage percentages are one-to-one
			wepStats.PhysicalDamagePercentage = wep.PhysicalDamagePercentage;
			wepStats.FireDamagePercentage = wep.FireDamagePercentage;
			wepStats.ColdDamagePercentage = wep.ColdDamagePercentage;
			wepStats.PoisonDamagePercentage = wep.PoisonDamagePercentage;
			wepStats.EnergyDamagePercentage = wep.EnergyDamagePercentage;
			wepStats.DirectDamagePercentage = wep.DirectDamagePercentage;

			return wepStats;
		}
		#endregion

		#region IAnimatedCombatant related methods
		public void OnAttackAnimationMakeContact()
		{
			// Actively choosing not to re-validate target. May be unwise
			CurrentlyEquippedAttack.OnAttackAnimationMakeContact(CurrentCombatant);
		}

		public void OnAttackAnimationFinished()
		{
			// Actively choosing not to re-validate target. May be unwise
			CurrentlyEquippedAttack.OnAttackAnimationFinished(CurrentCombatant);
		}
		#endregion

		#region Mobile Combat Methods


		#endregion

		#region Scene Initialization
		private void FindPlayer()
		{
			Player = GetNode<PlayerProfile>("/root/CurrentPlayerProfile");
		}
		
		/// <summary>
		///     Creates all nodes and such that are required to have a successful 'enemy scene' created from code
		/// </summary>
		private void InitializeScene()
		{
			InitializeDeltaClock();
			InitializeAnimationPlayer();
			InitializeCollisionShape();	// Should be wrapped up w/ sprite and handeled by animation player? or some other thing.
			InitializeTargetingAgent();
			if (IsMovementEnabled)
			{
				InitializeNavigationManager();
			}
			InitializeHealthbar();
		}

		/// <summary>
		///		Handy little clock node for us... 
		///		Could do ourselves, but seems nice to have an object for this
		/// </summary>
		private void InitializeDeltaClock()
		{
			DeltaClock = new();
			AddChild(DeltaClock);
		}

		/// <summary>
		///     Initializes the collision shape based for this CharacterBody2D and adds it as a child
		/// </summary>
		private void InitializeCollisionShape()
		{
			CollisionShape = new CollisionShape2D();
			Shape = new CircleShape2D();
			Shape.Radius = 15.0f;   // this probably needs to be pulled out somewhere 
			CollisionShape.Shape = Shape;
			AddChild(CollisionShape);
		}

		private void InitializeAnimationPlayer()
		{
			// Add to scene &...
			AddChild(AniPlayer);
			// Equip any gear...
			AniPlayer.SetCurrentEquipment(EquippedGear);
		}
		
		/// <summary>
		///		Initializes a null targeting agent
		/// </summary>
		public void InitializeTargetingAgent()
		{
			Targeting = new TargetingAgent(MobileVisionRange);
			AddChild(Targeting);
		}

		public void InitializeNavigationManager()
		{
			NavigationMngr = new NavigationManager(this);
			AddChild(NavigationMngr);
		}

		public void InitializeHealthbar()
		{
			PackedScene hb = GD.Load<PackedScene>("res://src/Scenes/Mobiles/Misc/healthbar.tscn");
			Healthbar = hb.Instantiate<TextureProgressBar>();
			AddChild(Healthbar);
			Healthbar.Visible = false;
			Healthbar.Scale = new Vector2(0.18f, 0.22f);
			Healthbar.Position = new Vector2(-((Healthbar.Size.X * Healthbar.Scale.X) / 2), -AniPlayer.GetCurrentSpriteSummary().OffsetY);
		}
		#endregion

		#region Animation Player Methods
		public void PlayDefaultAnimation()
		{
			AniPlayer.PlayDefaultAnimation();
		}
		/// <summary>
		///     A pass-thru for our MobileAnimationPlayer using the Animation Glossary Enum
		/// </summary>
		/// <param name="aniID"></param>
		/// <param name="dir"></param>
		/// <param name="speedOverride"></param>
		/// <param name="queueIfCurrentlyPlaying"></param>
		public void PlayAnimation(AnimationGlossaryEnum aniID, DirectionEnum dir = DirectionEnum.None, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false, double playtimeOverrideS = 0.0, float speedMultiplier = 1.0f)
		{
			if (dir == DirectionEnum.None) dir = CurrentDirection;
			AniPlayer.SetCurrentAnimation(aniID, dir, queueIfCurrentlyPlaying, overrideIfPlaying, playtimeOverrideS, speedMultiplier);
		}

		/// <summary>
		///     A pass-thru for our MobileAnimationPlayer- using DEFAULT Animation Glossary enum
		/// </summary>
		/// <param name="aniID"></param>
		/// <param name="dir"></param>
		/// <param name="speedOverride"></param>
		/// <param name="queueIfCurrentlyPlaying"></param>
		public void PlayAnimation(DefaultAnimationGlossaryEnum aniID, DirectionEnum dir = DirectionEnum.None, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false, double playtimeOverrideS = 0.0, float speedMultiplier = 1.0f)
		{
			if (dir == DirectionEnum.None) dir = CurrentDirection;
			AniPlayer.SetCurrentAnimation(aniID, dir, queueIfCurrentlyPlaying, overrideIfPlaying, playtimeOverrideS, speedMultiplier);
		}

		public Vector2 GetCurrentSpriteCenter()
		{
			return AniPlayer.GetCurrentSpriteCenter();
		}

		public Vector2 GetCurrentSpriteTop()
		{
			return AniPlayer.GetCurrentSpriteTop();
		}
		#endregion

		#region IDamageable Related Methods
		/// <summary>
		///		Given a DamagePacket, this IDamagable must decide how to interpret what has be wrought upon it
		/// </summary>
		/// <param name="damagePacket">DamagePacket object describing what damage is coming</param>
		public virtual void OnHit(DamagePacket damagePacket)
		{
			if (!IsValidCombatTarget()) return;
			int totalDmg = CalculateDamage(damagePacket);  // Calculates dmg considering resists, buffs, and debuffs.
			DmgBooklet.Update(damagePacket.DmgSource, totalDmg);	// Keep track of who dmgs us and when so that we can send rewards
			HitPoints -= totalDmg;
			//PlayAnimation(DefaultAnimationGlossaryEnum.DefaultGetHit);
		}

		/// <summary>
		///		Do the work of calculating damage while considering resists, buffs, and debuffs
		/// </summary>
		/// <param name="dp">The damage packet we are tryign to calculate</param>
		public int CalculateDamage(DamagePacket damagePacket)
		{
			float dmgDirect = damagePacket.DirectDamageOutput;
			float dmgPhysicalResitance = (1.0f - PhysicalResitance / 100.0f) * damagePacket.PhysicalDamageOutput;
			float dmgFireResistance = (1.0f - FireResistance / 100.0f) * damagePacket.FireDamageOutput;
			float dmgColdResistance = (1.0f - ColdResistance / 100.0f) * damagePacket.ColdDamageOutput;
			float dmgPoisonResistance = (1.0f - PoisonResistance / 100.0f) * damagePacket.PoisonDamageOutput;
			float dmgEnergyResistance = (1.0f - EnergyResistance / 100.0f) * damagePacket.EnergyDamageOutput;

			/// Stupid way to output this dmg text, but is what it is for now -- https://en.wikipedia.org/wiki/X11_color_names
			if (dmgDirect > 0)
			{
				DamageText dmgTxt = DmgTxtAnimationPS.Instantiate<DamageText>();
				AddChild(dmgTxt);
				dmgTxt.SetAmt((int)dmgDirect, Colors.OrangeRed, GetCurrentSpriteTop());
			}
			if (dmgPhysicalResitance > 0)
			{
				DamageText dmgTxt = DmgTxtAnimationPS.Instantiate<DamageText>();
				AddChild(dmgTxt);
				dmgTxt.SetAmt((int)dmgPhysicalResitance, Colors.Blue, GetCurrentSpriteTop());
			}
			if (dmgFireResistance > 0)
			{
				DamageText dmgTxt = DmgTxtAnimationPS.Instantiate<DamageText>();
				AddChild(dmgTxt);
				dmgTxt.SetAmt((int)dmgFireResistance, Colors.DarkRed, GetCurrentSpriteTop());
			}
			if (dmgColdResistance > 0)
			{
				DamageText dmgTxt = DmgTxtAnimationPS.Instantiate<DamageText>();
				AddChild(dmgTxt);
				dmgTxt.SetAmt((int)dmgColdResistance, Colors.DodgerBlue, GetCurrentSpriteTop());
			}
			if (dmgPoisonResistance > 0)
			{
				DamageText dmgTxt = DmgTxtAnimationPS.Instantiate<DamageText>();
				AddChild(dmgTxt);
				dmgTxt.SetAmt((int)dmgPoisonResistance, Colors.WebGreen, GetCurrentSpriteTop());
			}
			if (dmgEnergyResistance > 0)
			{
				DamageText dmgTxt = DmgTxtAnimationPS.Instantiate<DamageText>();
				AddChild(dmgTxt);
				dmgTxt.SetAmt((int)dmgEnergyResistance, Colors.Indigo, GetCurrentSpriteTop());
			}


			float totalDmgOut = dmgDirect + dmgPhysicalResitance + dmgFireResistance + dmgColdResistance + dmgPoisonResistance + dmgEnergyResistance;
			return (int)totalDmgOut;
		}


		public void OnTargeted()
		{
			// uh-oh
		}

		/// <summary>
		///		Called when targeted for combat to verify that the mobile can be targeted for combat
		/// </summary>
		/// <returns>True if this Mobile can be targeted for combat, false if not</returns>
		public virtual bool IsValidCombatTarget()
		{
			return IsTargetable && IsAlive && !IsInvulnerable && !IsDeleted;
		}
		#endregion

		#region Vision (targeting) Methods
		/// <summary>
		///		Standardize layer mask overrides for the mobile's vision targeting
		///		If no targeting agent is set on the Mobile this method does nothing
		/// </summary>
		/// <param name="targetingStrategy">TargetingStrategy enum value</param>
		/// <param name="maskList">The list of physics layer name enums we want to add or override our targeting agent's </param>
		/// <param name="overrideExistingLayers"></param>
		/// <param name="copyToAttackTargetingOptions">Often desired, gives our attacks the same targets as our vision has</param>
		public void SetVisionTargetingOptions(TargetingStrategy targetingStrategy, List<PhysicsLayerNameEnum> maskLayerList, bool overrideExistingLayers = true, bool copyToAttackTargetingOptions = true)
		{
			Targeting.CurrentTargetingStrategy = targetingStrategy;
			if (overrideExistingLayers)
			{
				Targeting.OverrideTargetingAgentMask(maskLayerList);
			}
			else
			{
				Targeting.AddTargetingAgentMask(maskLayerList);
			}
			// Usually we are wanting to pass the same values to our attack.
			if (copyToAttackTargetingOptions) SetAttackTargetingOptions(targetingStrategy, maskLayerList, overrideExistingLayers);
		}

		/// <summary>
		///		This is the targeting options that any equipped weapon will reference
		/// </summary>
		/// <param name="targetingStrategy"></param>
		/// <param name="maskLayerList"></param>
		/// <param name="overrideExistingLayers"></param>
		public void SetAttackTargetingOptions(TargetingStrategy targetingStrategy, List<PhysicsLayerNameEnum> maskLayerList, bool overrideExistingLayers = true)
		{
			CurrentAttackTargetingStrategy = targetingStrategy;
			if (overrideExistingLayers)
			{
				CurrentAttackTargetingMaskLayerList = maskLayerList;
			}
			else
			{
				CurrentAttackTargetingMaskLayerList.AddRange(maskLayerList);
			}
		}
		#endregion

		#region Physics & Collision Mgmt Methods
		/// <summary>
		///     A human readable interface into our Mobile's collision layers
		///     Appends a collision layer to the Mobile's CharacterBody2D collision layer
		/// </summary>
		/// <param name="newLayer">PhysicsLayerName enum value that we want to add to this mobile</param>
		public void AddCollisionLayer(PhysicsLayerNameEnum newLayer)
		{
			CollisionLayer |= (uint)newLayer;
		}

		/// <summary>
		///     A human readable interface into our Mobile's collision layers
		///     Overrides the mobile's CharacterBody2D collision layer
		/// </summary>
		/// <param name="newLayer">PhysicsLayerName enum value to set this mobile's CollisionLayer to</param>
		public void OverrideCollisionLayer(PhysicsLayerNameEnum newLayer)
		{
			CollisionLayer = (uint)newLayer;
		}

		/// <summary>
		///     A human readable interface into our Mobile's collision layers
		///     Overrides the mobile's CharacterBody2D collision layer
		/// </summary>
		/// <param name="newCollisionLayerList">PhysicsLayerName enum value list to set this mobile's CollisionLayer to</param>
		public void OverrideCollisionLayer(List<PhysicsLayerNameEnum> newCollisionLayerList)
		{
			CollisionLayer = 0;
			foreach (var layer in newCollisionLayerList)
			{
				CollisionLayer |= (uint)layer;
			}
		}

		/// <summary>
		///     A human redable interface into our Mobile's Collision Mask
		///     Appends a collision mask layer to the Mobile's CharacterBody2D Collision Mask
		/// </summary>
		/// <param name="maskLayer"></param>
		public void AddCollisionMaskLayer(PhysicsLayerNameEnum maskLayer)
		{
			CollisionMask |= (uint)maskLayer;
		}

		/// <summary>
		///     A human readable interface into the Mobile's collision mask
		///     Override the mobile's CharacterBody2D CollisionMask
		/// </summary>
		/// <param name="maskLayer">PhysicsLayerName enum value to set this mobile's CollisionmaskLayer to</param>
		public void OverrideCollisionMaskLayer(PhysicsLayerNameEnum maskLayer)
		{
			CollisionMask = (uint)maskLayer;
		}

		/// <summary>
		///     A human readable interface into the Mobile's collision mask
		///     Override the mobile's CharacterBody2D CollisionMask
		/// </summary>
		/// <param name="collisionMaskList">List<PhysicsLayerName> to set the new layer to</param>
		public void OverrideCollisionMaskLayer(List<PhysicsLayerNameEnum> collisionMaskList)
		{
			CollisionMask = 0;
			foreach (var layer in collisionMaskList)
			{
				CollisionMask |= (uint)layer;
			}
		}
		#endregion

		#region AI Specific Related Methods
		public virtual void OnIdle()
		{
			// There is some idle animation stuff in the intelligence ... should get pulled out here
		}
		#endregion

		#region XPManager Methods
		/// <summary>
		///		Callback for signal emitted when XpManager lvls up
		/// </summary>
		public void LevelUpCallback()
		{
			//GD.PushError("We just leveled, dude. GC0LBBRP");
		}

		/// <summary>
		///		Callback for signal emitted when XpManager is ready to lvl up
		/// </summary>
		public void ReadyToLvlCallback()
		{
			//GD.PushError("We are ready to lvl, dude. C4REGFO7");
		}
		#endregion

	}
}
