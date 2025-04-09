using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using UODef.System;

namespace UODef.Abilities.Targeting
{
	#region enums
	public enum TargetingStrategy
	{
		Closest,
		Furthest,
		MostRecentlyEnteredRange,		// This is the most recent enemy to enter your targeting range
		LeastRecentlyEnteredRange,
		Rand,							// Random target from your list of available targets
		LowestHealth,
		HighestHealth
	}
	#endregion


	///		I think that in the future, we just make this take in a generic type and w/ that type
	///			is will be what we are targeting.. It seems annoying to have to only be targeting oen type, though
	///			... not sure. Just random thoughts	
	
	public partial class TargetingAgent : Node2D
	{
		#region Godot Nodes
		private Area2D Area;
		private CollisionShape2D CollisionShape;
		private CircleShape2D Shape;
		#endregion

		#region Settings & Internal Mgmt
		private Random RandGen = new((int)Time.GetTicksMsec());
		/// <summary>
		///		How big the area2d we generate and keep track of
		/// </summary>
		public float TargetingRadius { get; set; }
		public List<ITargetable> TargetWhiteList = new();       // List of mobiles that cannot be targeted- generally includes the owner of this agent
		protected List<ITargetable> ActiveTargetPool = new();	//
		#endregion

		/// <summary>
		///		The strategy that we are currently choosing our target from the list of possible targets
		/// </summary>
		public TargetingStrategy CurrentTargetingStrategy = TargetingStrategy.Rand;
		public List<PhysicsLayerNameEnum> CurrentAttackTargetingMaskLayerList { get; set; } = new();

		public bool DamageableTargetsOnly { get; set; } = false;


		/// <summary>
		///		Constructor for the base targeting agent
		/// </summary>
		/// <param name="body">The obj that we're doing the targeting for</param>
		/// <param name="damageableTargetsOnly">A bool indicating that the potential target list will only ever be filled with IDamageables (as opposed to ITargetables)</param>
		public TargetingAgent(float defaultRadius = 0.0f, bool damageableTargetsOnly=false) : base() 
		{
			TargetingRadius = defaultRadius;
			DamageableTargetsOnly = damageableTargetsOnly;
		}

		#region Godot Method Overrides 
		public override void _Ready()
		{
			InitializeTargetingRange();
		}

		public override void _ExitTree()
		{
			DestroyTragetingRangeArea();
		}
		#endregion

		#region Targeting Area Mgmt
		/// <summary>
		///		Allows owners to change the area we're looking in.
		///		Resets any targeting information by destroying the old area2d and recreating 
		///			it as well as wiping the current targeting pool
		///		If the newTargetingRadius is the same as the old we return without doing anything
		/// </summary>
		/// <param name="newTargetingRadius">The new target radius we want to check</param>
		public void SetTargetingRange(float newTargetingRadius)
		{
			// Destory the connected Area2D if it already exists
			DestroyTragetingRangeArea();
			ActiveTargetPool.Clear();
			// reset target radius, clear pools, and re-init 
			TargetingRadius = newTargetingRadius;
			InitializeTargetingRange();
		}

		/// <summary>
		///		Destorys the current area if there is one set, does nothing if it is null
		/// </summary>
		private void DestroyTragetingRangeArea()
		{
			if (Area == null) return;
			Area.BodyEntered -= TargetingAreaOnBodyEntered;
			Area.BodyExited -= TargetingAreaOnBodyExited;
			Area.AreaEntered -= TargetingAreaOnAreaEntered;
			Area.AreaExited -= TargetingAreaOnAreaExited;
			RemoveChild(Area);
			Area.QueueFree();
			Area = null;
		}

		/// <summary>
		///		Creates the required nodes for a targeting area
		///		Initializes the nodes with a default range of TargetingRadius
		/// </summary>
		private void InitializeTargetingRange()
		{
			// Create notes
			Area = new Area2D();
			// Create the area and wipe any default collision layer/masking to prep it for intialization elsewhere
			Area.CollisionLayer = 0x0;
			Area.CollisionMask = 0x0;
			CollisionShape = new CollisionShape2D();
			Shape = new CircleShape2D();
			// Set properties and connect them
			Shape.Radius = TargetingRadius;
			CollisionShape.Shape = Shape;
			// Add to the scene tree in the appropriate manner
			AddChild(Area);
			Area.AddChild(CollisionShape);
			// Attach appropriate listeners
			Area.BodyEntered += TargetingAreaOnBodyEntered;
			Area.BodyExited += TargetingAreaOnBodyExited;
			Area.AreaEntered += TargetingAreaOnAreaEntered;
			Area.AreaExited += TargetingAreaOnAreaExited;
			ResetTargetingAgentMaskLayers();
		}

		/// <summary>
		///		Resets the targeting agent's mask layers (on the area) based on the 
		///			last set value
		///		Useful if we reset the area for some reason or what have you
		/// </summary>
		public void ResetTargetingAgentMaskLayers()
		{
			OverrideTargetingAgentMask(CurrentAttackTargetingMaskLayerList);
		}

		#endregion

		#region Find targets

		/// <summary>
		///		Attempts to find a target from the avaialbe target pool according to w/e current targeting strategy is set.
		///		If an ITargetable object is passed in we will first determine if this object is still a valid target (in the targeting pool), 
		///			if it is, we return it, if it isn't we try to find a new one.
		/// </summary>
		/// <param name="target">An optional parameter- If 'target' is still valid in the active targeting pool we will return it w/o doing any work</param>
		/// <returns>A Mobile chosen from a pool of possible targets in range, returns null if nothing found or targeting pool is empty</returns>
		public ITargetable FindTarget(ITargetable targ=null)
		{
			if (ActiveTargetPool.Count == 0) return null;
			if (VerifyTargetable(targ))
			{
				return targ;    // If a target was passed in we check to see if it is still valid w/in our targeting pool or not
			}

			ITargetable tmpCurrentTarget = null;
			ITargetable rtnValueTarget = null;
			switch (CurrentTargetingStrategy)
			{
				case TargetingStrategy.Closest:
					float max_dist = 0.0f;
					tmpCurrentTarget = null;
					foreach (ITargetable target in ActiveTargetPool)
					{
						float distToTarg = GlobalPosition.DistanceSquaredTo(target.GlobalPosition);
						if (distToTarg > max_dist)
						{
							max_dist = distToTarg;
							tmpCurrentTarget = target;
						}
					}
					if (tmpCurrentTarget != null)
					{
						rtnValueTarget = tmpCurrentTarget;
					}
					break;
				case TargetingStrategy.Furthest:
					float min_dist = 0.0f;
					tmpCurrentTarget = null;
					foreach (ITargetable mob in ActiveTargetPool)
					{
						float distToTarg = GlobalPosition.DistanceSquaredTo(mob.GlobalPosition);
						if (distToTarg < min_dist || min_dist == 0.0f)
						{
							min_dist = distToTarg;
							tmpCurrentTarget = mob;
						}
					}
					if (tmpCurrentTarget != null)
					{
						rtnValueTarget = tmpCurrentTarget;
					}
					break;
				case TargetingStrategy.MostRecentlyEnteredRange:
					if (ActiveTargetPool.Count > 0) rtnValueTarget = ActiveTargetPool.First();
					break;
				case TargetingStrategy.LeastRecentlyEnteredRange:
					if (ActiveTargetPool.Count > 0) rtnValueTarget = ActiveTargetPool.Last();
					break;
				case TargetingStrategy.Rand:
					if (ActiveTargetPool.Count > 0) rtnValueTarget = ActiveTargetPool[RandGen.Next(ActiveTargetPool.Count)];
					break;
				case TargetingStrategy.LowestHealth:
					GD.PushError("NOT IMPLEMENTED YOU FOOL!");
					GD.PushError("NOT IMPLEMENTED YOU FOOL!");
					GD.PushError("NOT IMPLEMENTED YOU FOOL!");
					break;
				case TargetingStrategy.HighestHealth:
					GD.PushError("NOT IMPLEMENTED YOU FOOL!");
					GD.PushError("NOT IMPLEMENTED YOU FOOL!");
					GD.PushError("NOT IMPLEMENTED YOU FOOL!");
					break;
				default:
					GD.PushError("NOT IMPLEMENTED YOU FOOL!!");
					GD.PushError("NOT IMPLEMENTED YOU FOOL!!");
					GD.PushError("NOT IMPLEMENTED YOU FOOL!!");
					break;
			}
			// So... Deciding to do a recursive call here. The other option is to check these loops as we try to pull from them, but that is effectively what I am doing. OR the other other option is to loop at the start of this thru all targs
			// if (rtnValueTarget != null && !VerifyTargetable(rtnValueTarget)) FindTarget(targ);
			// /I need to see what this is that is stuck in my target list by removing it, or more helpfully highlighting it or something
			return rtnValueTarget;
		}

		#endregion

		#region Target List Mgmt Methods
		/// <summary>
		///		Is this targetable from the targeting agent's point of view?
		///		Currently, this means that we are checking if the obj is null, if ITargetable.IsTargetable is set to true, and whether it is already in our targeting pool.
		///		If it is not in our targeting pool, it cannot be targeted (b/c we presume it is out of range or invalid in some other manner)
		/// </summary>
		/// <param name="target"></param>
		public bool VerifyTargetable(ITargetable target)
		{
			bool inPool = TargetInTargetPool(target);

			if (target is null || !target.IsTargetable || !inPool || TargetIsWhiteListed(target))
			{
				if (inPool)
				{
					RemoveFromActiveTargetPool(target);
				}
				return false;
			}
			return true;
		}

		/// <summary>
		///		Publicly available test to see if the target is still in our active target pool
		/// </summary>
		/// <param name="target">The ITargetable object your are curious about</param>
		/// <returns>True if the target is still in our active target pool and false if not</returns>
		public bool TargetInTargetPool(ITargetable target)
		{
			return ActiveTargetPool.Contains(target);
		}

		/// <summary>
		///		Returns whether this target is currently in the Targeting Agent's white list, and therefore shouldn't be targeted
		/// </summary>
		/// <param name="target"></param>
		/// <returns>Whether the passed-in target is in this targeting agent's white list</returns>
		public bool TargetIsWhiteListed(ITargetable target)
		{
			return TargetWhiteList.Contains(target);
		}

		/// <summary>
		///		Primarily so we don't target ourselves, but could be used to some effect
		/// </summary>
		/// <param name="target">The target that you no longer wish to target</param>
		public void AddToTargetWhiteList(ITargetable target)
		{
			if (!TargetWhiteList.Contains(target))
			{
				TargetWhiteList.Add(target);
			} 
		}

		/// <summary>
		///		Adds a ITargetable to the list of possible targets (if they are not whitelisted or already in the list)
		/// </summary>
		/// <param name="target">The target to add to our target pool</param>
		/// <returns>true if the ITargetable was successfully added to the target list, false otherwise. Failure to add to list could mean it was already in the list</returns>
		private void AddToActiveTargetPool(ITargetable target)
		{
			if (!ActiveTargetPool.Contains(target))
			{
				// If the targeting agent is set to only care about damagable targets, lets never add them to our targeting pool
				if (DamageableTargetsOnly && !target.IsValidCombatTarget()) return;
				ActiveTargetPool.Add(target);
			}
		}

		/// <summary>
		///		Removes a target from the list of possible targets
		///		There is no guarentee they will not re-enter the pool at some point
		/// </summary>
		/// <param name="target">The target we want to manually remove from our pool</param>
		private void RemoveFromActiveTargetPool(ITargetable target)
		{
			ActiveTargetPool.Remove(target);
		}
		#endregion

		#region Area2D Listeners
		public void TargetingAreaOnBodyEntered(Node2D body)
		{
			if (body is ITargetable targetable)
			{
				if (TargetIsWhiteListed(targetable)) return;
				AddToActiveTargetPool(targetable);
			}
		}

		public void TargetingAreaOnBodyExited(Node2D body)
		{
			if (body is ITargetable targetable) RemoveFromActiveTargetPool(targetable);
		}

		public void TargetingAreaOnAreaEntered(Area2D area)
		{
			if (area is DamageableArea dArea)
			{
				if (TargetIsWhiteListed(dArea)) return;
				AddToActiveTargetPool(dArea);
			}
		}

		public void TargetingAreaOnAreaExited(Area2D area)
		{
			if (area is DamageableArea dArea)
			{
				RemoveFromActiveTargetPool(dArea);
			}
		}
		#endregion

		#region TargetingMask Layer Methods

		/// <summary>
		///		Overrides the collision mask for the Area2D used for targeting
		///		Keeps track of CurrentAttackTargetingMaskLayerList
		/// </summary>
		/// <param name="maskList">A list of values from the PhysicsLayerName enum</param>
		public void OverrideTargetingAgentMask(List<PhysicsLayerNameEnum> maskList)
		{
			CurrentAttackTargetingMaskLayerList = maskList;
			Area.CollisionMask = 0;
			foreach (var mask in CurrentAttackTargetingMaskLayerList)
			{
				Area.CollisionMask |= (uint)mask;
			}
		}

		/// <summary>
		///		Adds to the collision mask for the Area2D used for tageting without overriding it
		///		Keeps track of CurrentAttackTargetingMaskLayerList
		/// </summary>
		/// <param name="maskList">A list of values from the PhysicsLayerNameEnum enum</param>
		public void AddTargetingAgentMask(List<PhysicsLayerNameEnum> maskList)
		{
			CurrentAttackTargetingMaskLayerList.AddRange(maskList);
			foreach (var mask in CurrentAttackTargetingMaskLayerList)
			{
				Area.CollisionMask |= (uint)mask;
			}
		}

		#endregion

	}
}
