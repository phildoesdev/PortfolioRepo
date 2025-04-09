using Godot;
using System;
using UODef.Mobiles;


namespace UODef.Mobiles.Intelligence
{

	#region enums
	public enum IntelligenceTypeEnum
	{
		BaseTower,
		BaseCreature
	}

	public enum IntelligenceActionStateEnum
	{
		Idle,
		Combat,
		Travel,
		Interact
	}
	#endregion


	/// <summary>
	///		Me trying my best to not use the phrase "Artificial Intelligence"
	/// </summary>
	public class BaseIntelligence
	{
		/// <summary>
		///		The body who is being provided intelligence to- maybe need to abstract this our one day
		/// </summary>
		private Mobile body;
		private Random RandGen = new((int)Time.GetTicksUsec());

		private IntelligenceActionStateEnum currentAction = IntelligenceActionStateEnum.Idle;
		/// <summary>
		///		The action we are currently thinking on.
		///		Also handles switching actions
		/// </summary>
		private IntelligenceActionStateEnum CurrentAction
		{
			get
			{
				return currentAction;
			}
			set
			{
				OnActionChanged(CurrentAction, value);
				currentAction = value;
			}
		}

		public BaseIntelligence(Mobile m) : base()
		{
			body = m;
			CurrentAction = IntelligenceActionStateEnum.Idle;
		}

		/// <summary>
		///		Gets called when an action is updated
		/// </summary>
		/// <param name="oldAction"></param>
		/// <param name="newAction"></param>
		public virtual void OnActionChanged(IntelligenceActionStateEnum oldAction, IntelligenceActionStateEnum newAction)
		{
			switch (oldAction)
			{
				case IntelligenceActionStateEnum.Idle:
					// GD.PushError("LEAVING IDLE!");
					break;
				case IntelligenceActionStateEnum.Combat:
					// GD.PushError("LEAVING COMBAT!");
					body.OnLeaveCombat();
					break;
				case IntelligenceActionStateEnum.Travel:
					body.OnLeaveTravel();
					break;
				case IntelligenceActionStateEnum.Interact:
					// GD.PushError("LEAVING INTERACT!");
					break;
				default:
					break;
			}
			switch (newAction)
			{
				case IntelligenceActionStateEnum.Idle:
					// GD.PushError("ENTERING IDLE!");
					break;
				case IntelligenceActionStateEnum.Combat:
					// GD.PushError("ENTERING COMBAT!");
					body.OnEnterCombat();
					break;
				case IntelligenceActionStateEnum.Travel:
					// GD.PushError("ENTERING TRAVEL!");
					break;
				case IntelligenceActionStateEnum.Interact:
					// GD.PushError("ENTERING INTERACT!");
					break;
				default:
					break;
			}
		}

		#region Think
		/// <summary>
		///		This is the main entry point - what gets called by the mobile utilizing us
		/// </summary>
		public virtual void Think()
		{
			// Sanity check 
			if (body is null || body.IsDeleted || !body.IsAlive) return;

			DoThink();
		}

		public virtual void BeforeThink()
		{
		}

		public virtual void DoThink()
		{
			if (body.IsDeleted) return;		// Some early bail-out qualifiers "Can't think if you dont exist"

			switch (CurrentAction)
			{
				case IntelligenceActionStateEnum.Idle:
					//BeforeActionIdle();
					OnActionIdle();
					//AfterActionIdle();
					break;
				case IntelligenceActionStateEnum.Combat:
					OnActionCombat();
					break;
				case IntelligenceActionStateEnum.Travel:
					OnActionTravel();
					break;
				case IntelligenceActionStateEnum.Interact:
					OnActionInteract();
					break;
				default:
					break;
			}
		}

		public virtual void AfterThink()
		{
		}
		#endregion

		#region IDLE
		public virtual void BeforeActionIdle()
		{
		}

		public virtual void OnActionIdle()
		{
			// Check battle conditions!
			if (body.IsCombatEnabled && body.IsAlive)
			{				
				// Search for target, and if one is found, enter combat
				if (body.FindCombatTarget()) // This is an ultra aggressive call on FindCombatTarget, could be costly.
				{
					CurrentAction = IntelligenceActionStateEnum.Combat;
					return;
				}
			}
			// Check Travel Conditions
			if (body.CanTravel())
			{
				CurrentAction = IntelligenceActionStateEnum.Travel;
			}
		}

		public virtual void AfterActionIdle()
		{
		}
		#endregion

		#region COMBAT
		public virtual void BeforeActionCombat()
		{
		}

		public virtual void OnActionCombat()
		{
			// Verify that we should be in combat and w/e else. 
			if (!body.IsCombatEnabled || body.CurrentCombatant is null || !body.IsInCombat)
			{
				CurrentAction = IntelligenceActionStateEnum.Idle;
				return;
			}

			body.OnCombat();
		}

		public virtual void AfterActionCombat()
		{
		}
		#endregion

		#region TRAVEL
		public virtual void BeforeActionTravel()
		{
		}

		public virtual void OnActionTravel()
		{
			// ABC (ALWAYS BE CHASING (enemies)!)
			if (body.IsCombatEnabled && body.IsAlive)
			{
				// Search for target, and if one is found, enter combat
				if (body.FindCombatTarget()) // This is an ultra aggressive call on FindCombatTarget, could be costly.
				{
					CurrentAction = IntelligenceActionStateEnum.Combat;
					return;
				}
			}
			// If we reached our destination, or something prevents us from traveling, go into IDLE and think on it
			if (!body.CanTravel())
			{
				CurrentAction = IntelligenceActionStateEnum.Idle;
			}
			// This will have to be replaced with an 'on travel' or some siht, but alright. works. dope.
			body.OnTravel();
		}

		public virtual void AfterActionTravel()
		{
		}
		#endregion

		public virtual void BeforeActionInteract()
		{
		}

		public virtual void OnActionInteract()
		{
		}

		public virtual void AfterActionInteract()
		{
		}
	}
}
