using Godot;
using Godot.Collections;
using System.Collections.Generic;
using UODef.System;

namespace UODef.MouseInteraction
{

	#region Enums
	/// <summary>
	///		The currently available mouse cursors
	/// </summary>
	public enum CursorType
	{
		None,
		Point,
		PointWar,
		ClosedFist,
		ClosedFistWar,
		Pinching,
		PinchingWar,
		PinchingMapPin,
		PinchingMapPinWar,
		PinchingQuill,
		PinchingQuillWar,
		OpenHand,
		OpenHandWar,
		Target,
		TargetWar,
		HourGlass,
		HourGlassWar,
	}
	#endregion

	#region Supporting Interfaces
	/// <summary>
	///		The interface to implement if you want to have a mouse hover effect such as changing the cursor
	/// </summary>
	public interface IMouseHoverEffect
	{
		/// <summary>
		///		Enables or disables the mouse hover effect. Determines if "OnMouseCursorHover" will be called
		/// </summary>
		public bool IsMouseHoverEffectActive { get; set; }

		/// <summary>
		///		Whether we will attempt to change the cursor on hover. Determines if 'GetMouseHoverCursor' will be called
		/// </summary>
		public bool IsChangeCursor { get; set; }

		/// <summary>
		///		Does what ever calculations required and returns what it thinks the mouse cursor should look like
		/// </summary>
		/// <param name="globalMousePos">The global coordinates for the mouse position</param>
		/// <returns>CursorType enum -- the type of cursor we should try to set ours to</returns>
		public CursorType GetMouseHoverCursor(Vector2 globalMousePos);

		/// <summary>
		///		Any action that should be taken on mouse cursor hover
		/// </summary>
		/// <param name="globalMousePos">The global coordinates for the mouse position</param>
		public void OnMouseCursorHover(Vector2 globalMousePos);
	}

	/// <summary>
	///		Interface for classes that want to do something when a player hovers a mouse over it and presess some sort of action such as left clicking or hitting a number.
	/// </summary>
	public interface IMouseHoverUserInteractionEffect
	{
		/// <summary>
		///		Enables or disalbes object interaction on mouse hover. Determines if OnMouseHoverUserInteraction will be called.
		/// </summary>
		public bool IsMouseHoverUserInteractionEffectActive { get; set; }

		/// <summary>
		///		If set to true, indicates that we want to verify the action is in the ViableActionNames list before any call is made
		/// </summary>
		public bool CheckViableActionsBeforeCalling { get; set; }

		/// <summary>
		///		We can check against this list to verify the object cares about this action
		/// </summary>
		public List<InputMapEnum> ViableActionNames { get; }

		/// <summary>
		///		Called when a user is hovering an IMouseHoverUserInteractionEffect object and presses an action. 
		///		If the object cares about this action
		/// </summary>
		/// <param name="actionName">The InputMapEnum of the action that happened on over, allowing for more robust handling of user actions</param>
		/// <param name="isActionJustPressed">True if 'action just pressed', false if 'action pressed'. Allows us to differentiate between to two in our implementation</param>
		/// <param name="globalMousePos">The global coordinates for the mouse position</param>
		public void OnMouseHoverUserInteraction(InputMapEnum actionName, bool isActionJustPressed, Vector2 globalMousePos);
	}
	#endregion

	#region Supporting Classes
	/// <summary>
	///		A helpful container to hold information about mouse collisions
	/// </summary>
	public partial class MouseCollisionContainer : Node
	{
		/// <summary>
		///		The global mouse position where the space state query took place
		/// </summary>
		public Vector2 GlobalMousePosition { get; private set; }

		/// <summary>
		///		An array of dictionaries representing collisions at MousePosition. This is the value returned by spaceState.IntersectPoint(...);
		/// </summary>
		public Array<Dictionary> MouseCollisions { get; private set; }

		/// <summary>
		///		The collision mask used to get these results
		/// </summary>
		public uint CollisionMask { get; private set; }

		public MouseCollisionContainer(Vector2 globalMousePosition, uint collisionMask, Array<Dictionary> mouseCollisions)
		{
			GlobalMousePosition = globalMousePosition;
			CollisionMask = collisionMask;
			MouseCollisions = mouseCollisions;
		}

		/// <summary>
		///		Attempts to find the indicated Rid in the MouseCollisions array
		/// </summary>
		/// <param name="rid">The Rid we are searching for</param>
		/// <returns>Collision query Dictionary</returns>
		public Dictionary GetCollisionFromRid(Rid rid)
		{
			foreach (Dictionary collision in MouseCollisions)
			{
				if ((Rid)collision["rid"] == rid)
				{
					return collision;
				}
			}
			return null;
		}
	}
	#endregion

	/// <summary>
	///		Handles mouse interaction for objects likes, like tilesets, that are more difficult to interact with.
	/// </summary>
	public partial class MouseInteraction2D : Node2D
	{
		#region MouseCursor Mgmt
		/// <summary>
		///		We will attempt to set this path every frame unless instructed otherwise
		/// </summary>
		private string defaultCursorPath = "res://src/Assets/UserInterface/Cursors/Point.png";
		/// <summary>
		///		The current cursor will appear as what ever image is in the path of currentCursorPath at the end of the physics process frame
		///		Currently defaulted to the gloves pointing with one finger
		/// </summary>
		private string currentCursorPath = "";
		private string lastCursorPath = "";
		/// <summary>
		///		Gets set to true if the current cursor image needs to be reloaded
		/// </summary>
		public bool IsRedrawCurrentCursor { get; set; } = true;

		/// <summary>
		///		If set to true it will not be possible to change the mouse cursor
		///		If defaulted to 'true', we will never try to draw a custom mouse cursor
		/// </summary>
		public bool LockCursor { get; set; } = false;
		#endregion

		/// <summary>
		///		The list of input map interactions we'll check for when hovering objects implementing IMouseHoverUserInteractionEffect
		/// </summary>
		private List<InputMapEnum> MouseHoverUserInteractionEffectInputActionList = new() { InputMapEnum.Interact };

		/// <summary>
		///		The default layers that MouseInteraction2D checks for when calling 'DetectCursorCollisions'
		///		Essentially this would be any element that cannot handle mouse interaction on its own, such as tilemap tiles
		/// </summary>
		public uint DefaultMouseCollisionMask = (uint)PhysicsLayerNameEnum.SystemTile & (uint)PhysicsLayerNameEnum.Item & (uint)PhysicsLayerNameEnum.Mobile;

		public MouseInteraction2D() : base()
		{
		}

		public override void _Process(double _delta)
		{
			ProcessMouseCollisions();   // Looks for anything interesting under the mouse
			RedrawMouseCursor();	// Changes the mouse cursor if requested. This call shoud always be at the end of this method.
		}

		/// <summary>
		///		Queries the physicss space state and looks for returns an array of dictionaries representing all collisions currently under the cursor
		/// </summary>
		/// <param name="collisionMask">Override which physics layers we're looking at</param>
		/// <returns>A MouseCollisionContainer object containing the collisions and the mouse position at the time</returns>
		public MouseCollisionContainer DetectCursorCollisions(uint collisionMask=0)
		{
			Vector2 mousePos = GetGlobalMousePosition();

			PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
			PhysicsPointQueryParameters2D pointQuery = new();
			pointQuery.Position = mousePos;
			if (collisionMask == 0) collisionMask = DefaultMouseCollisionMask;
			//pointQuery.CollisionMask = collisionMask;

			Array<Dictionary> mouseCollisions = spaceState.IntersectPoint(pointQuery);
			return new MouseCollisionContainer(mousePos, collisionMask, mouseCollisions);
		}

		/// <summary>
		///		Checks the current mouse position for any collisions. Sends signals, calls methods, and generally
		///			processes the current mouse position collisions.
		///		Particularly useful for less responsive things such as tilemaps
		///		Its frequency is goverend by the 'forceCheck' parameter or the field maxMouseRecheckTimeMS
		/// </summary>
		/// <param name="forceCheck">If True bypasses the maxMouseRecheckTimeMS set and resets 'LastMouseRecheck'</param>
		private void ProcessMouseCollisions(bool forceCheck = false)
		{
			if (!LockCursor) currentCursorPath = defaultCursorPath;  // Maintains our default cursor when not hovering over special tiles

			// Look for collisions under our current cursor position
			MouseCollisionContainer mouseCollisionContainer = DetectCursorCollisions(DefaultMouseCollisionMask);
			if (mouseCollisionContainer.MouseCollisions.Count <= 0) return;

			// Eventually we're going to have to do a test for 'is on top' or 'can see' or something? idk. Buildings might make things confusing. We'll see.
			foreach (Dictionary collisionResult in mouseCollisionContainer.MouseCollisions)
			{
				var collisionObject = collisionResult["collider"].Obj;	// The object we're going to be working with

				// Hnadle mouse-over effects for each collision. The last collision to set the mouse cursor wins
				if (collisionObject is IMouseHoverEffect)
				{
					var mouseHoverEffectObj = collisionObject as IMouseHoverEffect;
					if (mouseHoverEffectObj.IsMouseHoverEffectActive)
					{
						// Request new mouse cursor
						if (!LockCursor && mouseHoverEffectObj.IsChangeCursor) RequestNewMouseCursor(mouseHoverEffectObj.GetMouseHoverCursor(mouseCollisionContainer.GlobalMousePosition));
						// Execute any hover effect code
						mouseHoverEffectObj.OnMouseCursorHover(mouseCollisionContainer.GlobalMousePosition);
					}
				}

				// We already checking what we are hovering over so why not try a little harder and see if there are also any interesting actions pressed
				if (collisionObject is IMouseHoverUserInteractionEffect)
				{
					// Attempt to notify any objects that may care
					foreach (InputMapEnum actionName in MouseHoverUserInteractionEffectInputActionList)
					{
						if (!Input.IsActionPressed(actionName.ToString())) continue;  // If the user is not comitting this action, continue on
						
						var mouseHoverUserActionInteractionObj = collisionObject as IMouseHoverUserInteractionEffect;	// do the conversion at the last possible second
						if (!mouseHoverUserActionInteractionObj.IsMouseHoverUserInteractionEffectActive) continue;
						if (mouseHoverUserActionInteractionObj.CheckViableActionsBeforeCalling && !mouseHoverUserActionInteractionObj.ViableActionNames.Contains(actionName)) continue; // I wonder if doing this check is more work than just calling the method

						bool justPressed = Input.IsActionJustPressed(actionName.ToString()); // we want to pass this info along
						mouseHoverUserActionInteractionObj.OnMouseHoverUserInteraction(actionName, justPressed, mouseCollisionContainer.GlobalMousePosition);
					}
				}
			}
		}

		/// As cursors evolve, it seems more obvious that it could be its own singleton, but I am going to wait until things are more setteled before deciding anything like that
		#region MouseCursor
		/// <summary>
		///		Will request that the cursor looks like this new CursorType. 
		///		... This will grow more interesting as cursorpaths will need to come with coordinates for the 'hot area', and things like this ...
		/// </summary>
		/// <param name="newCursorRequest">The cursor type we are petitioning we change to. This is likely to change.</param>
		public void RequestNewMouseCursor(CursorType newCursorRequest)
		{
			string newCursorPath = "";
			switch (newCursorRequest)
			{
				case CursorType.Point:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/Point.png";
					break;
				case CursorType.PointWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/PointWar.png";
					break;
				case CursorType.ClosedFist:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/ClosedFist.png";
					break;
				case CursorType.ClosedFistWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/ClosedFistWar.png";
					break;
				case CursorType.Pinching:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/Pinching.png";
					break;
				case CursorType.PinchingWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/PinchingWar.png";
					break;
				case CursorType.PinchingMapPin:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/PinchingMapPin.png";
					break;
				case CursorType.PinchingMapPinWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/PinchingMapPinWar.png";
					break;
				case CursorType.PinchingQuill:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/PinchingQuill.png";
					break;
				case CursorType.PinchingQuillWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/PinchingQuillWar.png";
					break;
				case CursorType.OpenHand:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/OpenHand.png";
					break;
				case CursorType.OpenHandWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/OpenHandWar.png";
					break;
				case CursorType.Target:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/Target.png";
					break;
				case CursorType.TargetWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/TargetWar.png";
					break;
				case CursorType.HourGlass:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/HourGlass.png";
					break;
				case CursorType.HourGlassWar:
					newCursorPath = "res://src/Assets/UserInterface/Cursors/HourGlassWar.png";
					break;
				default:
					// Default to pointer
					newCursorPath = "res://src/Assets/UserInterface/Cursors/Point.png";
					break;
			}
			// We found we need to reset our path
			if (newCursorPath != "")
			{
				currentCursorPath = newCursorPath; 
				IsRedrawCurrentCursor = true;
			}
		}

		/// <summary>
		///		If being requested by IsRedrawCurrentCursor, load in a new cursor resource and redraw it
		/// </summary>
		public void RedrawMouseCursor()
		{
			if (!IsRedrawCurrentCursor && currentCursorPath == lastCursorPath) return;
			IsRedrawCurrentCursor = false;  // resetting before work is done becuase getting stuck doing this work every frame could be expensive
			lastCursorPath = currentCursorPath;

			var newCursorResource = ResourceLoader.Load(currentCursorPath);
			Input.SetCustomMouseCursor(newCursorResource);
		}
		#endregion
	}
}