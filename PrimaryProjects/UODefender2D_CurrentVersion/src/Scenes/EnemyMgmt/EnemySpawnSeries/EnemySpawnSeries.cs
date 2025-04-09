using Godot;
using System.Collections.Generic;

namespace UODef.EnemyMgmt.EnemySpawn
{
	#region enums
	/// <summary>
	///		Intended as an abstract way to describe how to organize the 'spawn' on the field
	///		Set in the WaveRound, utilized by w/e tool spawns these rounds to place enemies correctly on the field
	/// </summary>
	public enum WaveRoundSpawnShape
	{
		None = 0,
		Line = 1,
		Diagonal = 2,
		Circle = 3,
	}
	#endregion

	/// <summary>
	///		A level will contain a series of waves that make up the enemy spawn across this lvl
	///		Once a series has been completed the lvl is effectively won -- there are no other objectives in terms of enemy spawn
	/// </summary>
	public partial class EnemySpawnSeries : Node
	{
		#region Signals
		#endregion

		/// <summary>
		///		The difficulty curve describing the levels overall difficult ramp.
		///		Each wave defines a difficulty curve to provide variance w/in each, but uses this curve
		///			as its base
		/// </summary>
		[Export]
		public Curve DifficultyCurve;

		/// <summary>
		///		A Queue of waves, constructed programatically, gleaned from a predictable node structure
		/// </summary>
		public Queue<EnemySpawnWave> Waves = new();

		/// <summary>
		///		Indicates how many waves in this series have spawned.
		///		Gets incremented when GetNextSpawnWave is called
		/// </summary>
		public int CurrentWave { get; set; } = 0;

		/// <summary>
		///		The number of waves in this series on scene initialization.
		/// </summary>
		public int TotalWaves { get; set; } = 0;

		public EnemySpawnSeries()
		{	
		}

		public override void _Ready()
		{
			InitializeScene();
		}

		#region Scene Initialization
		private void InitializeScene()
		{
			FindWaveNodes();
		}

		/// <summary>
		///		Method to look at the scene tree and store all child waves in our queue to be processed
		/// </summary>
		private void FindWaveNodes()
		{
			Node2D wavesContainer = GetNode<Node2D>("Waves");
			foreach (Node node in wavesContainer.GetChildren())
			{
				if (node is not EnemySpawnWave eswn) continue;
				Waves.Enqueue(eswn);
			}
			TotalWaves = Waves.Count;
		}
		#endregion

		#region API
		public EnemySpawnWave GetNextSpawnWave()
		{
			if (Waves.Count == 0) return null;
			CurrentWave += 1;
			return Waves.Dequeue();
		}
		#endregion
	}
}
