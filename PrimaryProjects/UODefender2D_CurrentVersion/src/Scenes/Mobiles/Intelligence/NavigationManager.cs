using Godot;
using System;
using UODef.System;

namespace UODef.Mobiles.Navigation
{

	#region Interfaces
	public interface INavigator : IDirectional, GodotCharacterBody2D
	{
		/// <summary>
		///		Bool that indicates if mvmt is enabled at the highest lvl.
		///		There are many things that will prevent us from moving, but this flag disabled it at the root.
		/// </summary>
		public bool IsMovementEnabled { get; }
		public bool NavigationDestinationReached { get; }

		/// <summary>
		///		We allow our Mobile to hold a destination, but get distracted with things like battle.
		///		More directly, the DefaultDestination will be defaulted to if the CurrentDestination is reached.
		///		This allows for the current destination to be changed for less important things without forgetting about our primary purpose
		/// </summary>
		public NavigationDestination DefaultDestination { get; }

		/// <summary>
		///		The Destination that the INavigator is currently pursuing
		/// </summary>
		public NavigationDestination CurrentDestination { get; }









		public void OnDestinationReached();








		/// <summary>
		///		Method that can be called to determine if we can currently travel, w/e that might mean to w/e obj implementing us
		/// </summary>
		/// <returns>True if the INavigator can travel, false if not</returns>
		public bool CanTravel();


		/// <summary>
		///		Either gets called by the nav agent's callback, or called by us when we want to set velocity AND move
		///		We also count on it to play its correct animation if there is one, etc
		/// </summary>
		/// <param name="newVelocity">Velocity vector2</param>
		public void SetVelocityAndMove(Vector2 newVelocity);

	}
	#endregion

	#region Supporting Classes
	public class NavigationDestination
	{
		public Rid NavMapRid { get; set; }
		public Vector2 TargetDestination { get; set; }

		public NavigationDestination(Vector2 targetDestination, Rid navMapRid)
		{
			NavMapRid = navMapRid;
			TargetDestination = targetDestination;
		}
	}
	#endregion

	public partial class NavigationManager : Node
	{
		/// <summary>
		///		Who this navigation manager is tied to
		/// </summary>
		private INavigator Navigator;

		public NavigationDestination CurrentDestination { get; private set; }

		/// <summary>
		///		Godot's implementation of the navigation agent 
		/// </summary>
		private NavigationAgent2D NavigationAgent;

		// Settings && Utility
		private bool NavAgentAvoidanceEnabled = true;
		private Random RandGen; // Used to make pathing look more natural and... random
		private int PathOffsetAmt = 0;							// will get set after some randomness is added
		private Vector2 LastPathPosition = Vector2.Zero;
		private Vector2 NextPathPosition = Vector2.Zero;

		public NavigationManager(INavigator navigator) : base()
		{
			RandGen = new((int)Time.GetTicksMsec());
			Navigator = navigator;
		}

		#region Godot Overrides and Related Hooks
		public override void _Ready()
		{
			InitializeNavigationAgent();
		}

		/// <summary>
		///		Do not override. Required to clean up listeners and what have you.
		/// </summary>
		public override void _ExitTree()
		{
			// dont think I need to do this b/c it seems like they already dont exist?
			if (NavigationAgent is not null && Navigator is not null)
			{
				NavigationAgent.VelocityComputed -= NavigationAgentVelocityComputed;
				NavigationAgent.NavigationFinished -= NavigationAgentTargetReached;
				NavigationAgent.PathChanged -= NavigationAgentPathChanged;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		///     Creates the navigation agent and adds it as a child to the 'Navigator' (the obj that owns this). 
		///     Sets several navigation agent settings
		/// </summary>
		private void InitializeNavigationAgent()
		{
			// Navigation Randomization Initialization
			PathOffsetAmt = RandGen.Next(-75, 75);    // * Later on we can overridethis to create packs of monsters. Some setting 'use custom value?' to turn on/off. *** Would also be cool to have some percentage change to change the sign of htis value, or to re-roll it. Could make enimies look more interesting

			NavigationAgent = new NavigationAgent2D();
			Navigator.AddChild(NavigationAgent);

			// Navigation Agent Settings
/*			NavigationAgent.PathMaxDistance = 200.0f;
			NavigationAgent.PathDesiredDistance = 128.0f;
			NavigationAgent.TargetDesiredDistance = 75.0f;
			*/
			// Duping these b/c I want to see if it helps w/ lag && monsters getting stuck (I am getting stuck on this non-problem :p)
			NavigationAgent.PathMaxDistance = 200.0f;
			NavigationAgent.PathDesiredDistance = 128.0f;
			NavigationAgent.TargetDesiredDistance = 75.0f;

			NavigationAgent.PathPostprocessing = NavigationPathQueryParameters2D.PathPostProcessing.Edgecentered;
			NavigationAgent.AvoidanceEnabled = NavAgentAvoidanceEnabled;
			NavigationAgent.DebugEnabled = false;

			// Settings that only apply to navigation agent avoidance
			if (NavAgentAvoidanceEnabled)
			{
				NavigationAgent.PathMetadataFlags = NavigationPathQueryParameters2D.PathMetadataFlags.None; // Not necessary, but, fine for now
				NavigationAgent.Radius = 25.0f;
				NavigationAgent.NeighborDistance = 5.01f; // distance in pixels(?)
				NavigationAgent.TimeHorizonAgents = 0.01f; // time in seconds
				NavigationAgent.MaxSpeed = 250; // px/s
				NavigationAgent.MaxNeighbors = 1;
			}

			NavigationAgent.VelocityComputed += NavigationAgentVelocityComputed;
			NavigationAgent.NavigationFinished += NavigationAgentTargetReached;
			NavigationAgent.PathChanged += NavigationAgentPathChanged;

			// Seeding this position to help with navigation
			LastPathPosition = Navigator.GlobalPosition;
		}
		#endregion


		#region API
		/// <summary>
		///		This is changing the target we are currently pursuing.
		///		Upon reaching this location we will look to our DefaultDestination
		/// </summary>
		public void SetNavigationCurrentDestination(NavigationDestination currentDestination)
		{
			// Nothing to do
			if (currentDestination.TargetDestination == CurrentDestination?.TargetDestination && currentDestination.NavMapRid == CurrentDestination?.NavMapRid) return;

			CurrentDestination = currentDestination;

			// Set the navigation agent to the new target
			NavigationAgent.TargetPosition = CurrentDestination.TargetDestination;
			NavigationAgent.SetNavigationMap(CurrentDestination.NavMapRid);
		}

		/// <summary>
		///		The INavigator is requesting our navigation services.
		///		Give them a velocity to continue their movement down a calculated path
		///			towards the CurrentDestination
		///		*If NavAgent Avoidance is enabled (NavAgentAvoidanceEnabled) this method sets nav agents velocity and 
		///			any veloicty manipulation is intended to happen through that signal callback
		/// </summary>
		/// <param name="movementSpeed">Effects the magnitude of the returned velocity</param>
		/// <returns></returns>
		public Vector2 GetNextPosition()
		{
			//* Eventually od a check on navigator.canmove + destination reached stuff

			// This is a probably an expensive way to wait for next path position to be reached
			Vector2 unverifiedNextPathPosition = NavigationAgent.GetNextPathPosition();
			// B/c we want a path offset. if we do not do this we'll stutter all around trying to constantly recalc things based off of our offset global position
			if (unverifiedNextPathPosition == NextPathPosition) return (Vector2.Zero);

			// We want to know the direction of the path so that we can attempt to go X degress offset to it
			LastPathPosition = (NextPathPosition == Vector2.Zero) ? Navigator.GlobalPosition : NextPathPosition;
			NextPathPosition = unverifiedNextPathPosition;

			// Calculate the next position but offset from it a bit	the path so we can create the illusion of natual group mvmt
			Vector2 pathDirection = (NextPathPosition - LastPathPosition).Normalized();
			Vector2 pathDirectionRotated = pathDirection.Rotated(Mathf.DegToRad(125));
			Vector2 projectedNextPathPosition = NextPathPosition + (pathDirectionRotated * PathOffsetAmt);
			return projectedNextPathPosition;
		}

		/// <summary>
		///		A interface that exists so we can turn our navigation agent avoidance on and off and have things still work
		/// </summary>
		/// <param name="newVelocity"></param>
		public void SetVelocity(Vector2 newVelocity)
		{
			if (NavAgentAvoidanceEnabled)
			{
				// Sets the velocity on the nav agent, counting on its 'computed velocity' callback
				NavigationAgent.Velocity = newVelocity;
			}
			else
			{
				Navigator.SetVelocityAndMove(newVelocity);
			}
		}
		#endregion



		/// <summary>
		///		Sets the velocity on our navigation agent to 0, disables avoidance, etc.
		/// </summary>
		public void DisableNavigation()
		{
			SetVelocity(Vector2.Zero);
			NavigationAgent.AvoidanceEnabled = false;
		}

		#region Navigation Agent Signal Callbacks
		/// <summary>
		///     Called by a signal from the navigation agent that the target has been reached
		/// </summary>
		private void NavigationAgentTargetReached()
		{
			SetVelocity(Vector2.Zero);
			Navigator.OnDestinationReached();
		}

		/// <summary>
		///		Gets called as the callback for NavigationAgent if avoidance is enabled.
		///		Could be called directly to manipulate our Navigators movement.
		/// </summary>
		/// <param name="calculatedVelocity"></param>
		private void NavigationAgentVelocityComputed(Vector2 calculatedVelocity)
		{
			Navigator.SetVelocityAndMove(calculatedVelocity);
		}

		/// <summary>
		///		This is an attempt to help fix any bodies that get blown too far off course by forcing us to 
		///			move in a new direction instead of holding
		/// </summary>
		public void NavigationAgentPathChanged()
		{
			NextPathPosition = Vector2.Zero;
		}

		#endregion

	}
}
