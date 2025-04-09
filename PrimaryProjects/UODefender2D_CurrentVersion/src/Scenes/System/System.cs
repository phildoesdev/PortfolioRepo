using Godot;
using System.Collections.Generic;

namespace UODef.System
{

	#region Enums

	/// <summary>
	///		All our input map commands in an enum so that I don't have to pass strings around like a fool.
	/// </summary>
	public enum InputMapEnum
	{
		MoveLeft,
		MoveRight,
		MoveUp,
		MoveDown,
		ScrollUp,
		ScrollDown,
		One,
		Two,
		Three,
		Four,
		Five,
		Six,
		Seven,
		Eight,
		Nine,
		Zero,
		Interact,
	}

	/// <summary>
	///		
	/// </summary>
	public enum PhysicsLayerNameEnum
	{
		Structures = 1 << 6,
		DeadMobile = 1 << 7,
		Mobile = 1 << 8,
		Tower = 1 << 9,
		Creature = 1 << 10,
		TowerTarget = 1 << 11,
		CreatureTarget = 1 << 12,
		Item = 1 << 14,
		Projectile = 1 << 15,
		AOE = 1 << 16,
		SystemTile = 1 << 18,
		TowerTile = 1 << 19,
		Impassable = 1 << 27,
	}

	/// <summary>
	///     All official directions
	/// </summary>
	public enum DirectionEnum
	{
		None,
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest,
	}
	#endregion

	#region Interfaces

	/// <summary>
	///		A helpful interface for Godot node related fields
	///		Useful if your classes inherit from godot's and you don't want to write 'Vector2 GlobalPosition' in your new interface 
	/// </summary>
	public interface GodotNode2D
	{
		// Godot carry-over (if inheriting from a godot node, no need to implement GlobalPosition)
		public Vector2 GlobalPosition { get; set; }

		// Perhaps this is getting dangerous, but it is quite nice.
		public void AddChild(Node node, bool forceReadableName = false, Node.InternalMode @internal = (Node.InternalMode)(0));


		public void QueueFree();

	}

	public interface GodotCharacterBody2D: GodotNode2D
	{
		public Vector2 Velocity { get; set; }

	}


	/// <summary>
	///		A generic interface for sometihng that utilizes the DirectionEnum and generally has a direction.
	///		This was split into its own interface b/c direction, and its required fields are shared across Animations nad navigation
	/// </summary>
	public interface IDirectional
	{
		/// <summary>
		///		The current direction of the obj - which dir are we facing - referenced by a value from the DirectionEnum enum.
		/// </summary>
		public DirectionEnum CurrentDirection { get; }
		/// <summary>
		///		The current direction of the obj - which dir are we facing - referenced by a value from the CurrentDirectionVector dictionary
		/// </summary>
		public Vector2 CurrentDirectionVector { get; }

		/// <summary>
		///		A method that will take a vector2 and attempt to convert it into one of our valid directions
		/// </summary>
		/// <param name="direction">The vector2 direction meant to be translated</param>
		/// <param name="tryHard">True to 'try hard', which can be expensive</param>
		public void SetDirection(Vector2 direction, bool tryHard);
	}


	#endregion


	#region Utility & System Classes
	/// <summary>
	///		A very simple class to keep track of delta time - it's something we do in a few places
	/// </summary>
	public partial class DeltaTimeClock : Node
	{
		public double DeltaTimeSum { get; set; } = 0.0;

		public DeltaTimeClock() : base()
		{
		}

		public override void _Process(double delta)
		{
			DeltaTimeSum += delta;
		}
	}

	/// <summary>
	///		This has turned to be a utility class related to direction, mostly converting values to words and vice-versa
	/// </summary>
	public static class DirectionUtility
	{
		/// <summary>
		///     Gices a Vector2 definition for each Direction in Direction Enum
		/// </summary>
		public static Dictionary<DirectionEnum, Vector2> DirectionVectorsDict = new Dictionary<DirectionEnum, Vector2>
		{
			{DirectionEnum.None, new Vector2(0, 0)},
			{DirectionEnum.North, new Vector2(0, -1)},
			{DirectionEnum.NorthEast, new Vector2(0.5f, -0.5f)},
			{DirectionEnum.East, new Vector2(1, 0)},
			{DirectionEnum.SouthEast, new Vector2(0.5f, 0.5f)},
			{DirectionEnum.South, new Vector2(0, 1)},
			{DirectionEnum.SouthWest, new Vector2(-0.5f, 0.5f)},
			{DirectionEnum.West, new Vector2(-1, 0)},
			{DirectionEnum.NorthWest, new Vector2(-0.5f, -0.5f)}
		};

		/// <summary>
		///		Calculates the direction ENUM the mobile is currently facing. Required 
		/// </summary>
		/// <param name="tryHard">Whether we do extra math if the cheap way of finding the direction didnt work.</param>
		/// <returns>Direction enum, returns 'Direction.None' if it does not fit in to the paradigm of the enum</returns>
		public static DirectionEnum CalculateDirectionEnum(Vector2 direction, bool tryHard = false)
		{
			DirectionEnum returnValue = DirectionEnum.None;
			Vector2 RoundedDir = new Vector2(Mathf.Round(direction.X * 2) / 2, Mathf.Round(direction.Y * 2) / 2);
			foreach (var dir in DirectionVectorsDict)
			{
				if (RoundedDir == dir.Value)
				{
					returnValue = dir.Key;
					break;
				}
			}

			// We will frequently fail to find a direction w/ the attempt above, so we need to try a little harder so we can play the correct animation.
			// This is worth doing, but can be expensive if it is always the first choice
			// It is also worth noting that this is the most expensive thing we do for a mobile limits us via lag (a physics frame takes too long) on how many enemies we can spawn
			//		I will really have to come back to this. There is almost certainly a smart solution. It seems like there must be a smarter way for me to try to make these judgements
			//		I am getting like (-1, 0.5), which doesnt exist in my set of directions, but could probably be mapped one way or the other
			//		The work will be to to map out a stationary mobile so a key makes them face in each of the directions and see if there is a more reasonable way to map all this, even if it is mostly guessing, or some sort of comparison... time will tell.
			//		... I think that mapping the in-between points would go a long way 
			//				pp 09.28.24

			// if (returnValue == DirectionEnum.None)
			// {
			// 	GD.PushError($"Dir: [{direction}] - RoundedDir: [{RoundedDir}]");
			// }
			if (tryHard && returnValue == DirectionEnum.None /*&& false*/)
			{
				// Loop the directions and compare our current... see which we are 'closets to'.
				// This can be expensive.
				float lowestDiff = 0.0f;
				foreach (KeyValuePair<DirectionEnum, Vector2> dir in DirectionVectorsDict)
				{
					var thisDiff = (direction - dir.Value).LengthSquared();
					if (thisDiff <= lowestDiff || lowestDiff == 0.0f)
					{
						lowestDiff = thisDiff;
						returnValue = dir.Key;
					}
				}
			}
			return returnValue;
		}

		/// <summary>
		///		Given a DirectionEnum value convert it to a string.
		///		This is our 'cached implementation of DirectionEnum.ToString()'
		///		https://learn.microsoft.com/en-us/visualstudio/profiling/performance-insights-enum-tostring?view=vs-2022
		/// </summary>
		/// <param name="direction">The direction enum you want the string name of</param>
		/// <returns></returns>
		public static string DirectionEnumToString(DirectionEnum direction)
		{
			switch (direction)
			{
				case DirectionEnum.None:
					return "None";
				case DirectionEnum.North:
					return "North";
				case DirectionEnum.NorthEast:
					return "NorthEast";
				case DirectionEnum.East:
					return "East";
				case DirectionEnum.SouthEast:
					return "SouthEast";
				case DirectionEnum.South:
					return "South";
				case DirectionEnum.SouthWest:
					return "SouthWest";
				case DirectionEnum.West:
					return "West";
				case DirectionEnum.NorthWest:
					return "NorthWest";
				default:
					return "None";
			}
		}
	}

	#endregion
}
