using Godot;
using System.Collections.Generic;
using UODef.Mobiles;
using System;
using System.Text.Json;

namespace UODef.EnemyMgmt.EnemySpawn
{
	#region Supporting Classes
	/// <summary>
	///		Describes a wave's possibly many rounds
	/// </summary>
	public partial class EnemySpawnWaveRound : Node
	{
		/// <summary>
		///		The mobile that we are going to spawn for a round of a parent wave
		///		Intended to exist as a way to describe each element of our wave
		/// </summary>
		public string CreatureName { get; set; }

		/// <summary>
		///		The number of mobiles to spawn
		/// </summary>
		public int SpawnCount { get; set; }

		/// <summary>
		///		How should these spawn be placed on the map?
		/// </summary>
		public string SpawnShape { get; set; }

		/// <summary>
		///		Integer value for spawn shape enum
		/// </summary>
		public WaveRoundSpawnShape SpawnShapeEnum { get; set; }

		/// <summary>
		///		Time in seconds that we will pause before spawning this EnemySpawnWaveRound.
		///		This allows us to buffer time between rounds strategically.
		///		This time always is a modifier on top of EnemySpawnWAve.DefaultTimeBetweenWaveRoundsMS
		/// </summary>
		public float TimeBeforeSpawnMS { get; set; }

		/// <summary>
		///		Description for any humans who may editing the json file
		///		Bringing it into the class b/c why not
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		Attempts to match on the name of a path on screen. If not match is found nothing is spawned
		///		Based on UODef.EnemyMgmt.EnemyNavigationRegion's PathName property
		/// </summary>
		public string NavigationRegionName { get; set; }

		public EnemySpawnWaveRound() : base()
		{
		}
	}
	#endregion


	/// <summary>
	///		Describes the possibly multiple waves contained within a Series
	/// </summary>
	public partial class EnemySpawnWave : Node
	{

		/// <summary>
		///		The difficulty curve for this specific wave, based off the Spawn Series' dificulty curve, but providing dynamic 
		///			experiences within
		/// </summary>
		[Export]
		public Curve DifficultyCurve { get; set; }

		/// <summary>
		///		The default amt of time between wave rounds. 
		///		Individual wave rounds can alter this value negatively or positively, likely w/ some sort of minimum above 0.0f
		/// </summary>
		[Export]	
		public float TimeBetweenWaveRoundsMS { get; set; } = 8.0f;

		/// <summary>
		///		The location of the json file that describes all the EnemySpawnWaveRounds that this EnemySpawnWave should spawn
		/// </summary>
		[Export(PropertyHint.File, "*.json")]
		public string WaveDetailsFile { get; set; }

		/// <summary>
		///		The List of EnemySpawnWaveRounds that that make up this EnemySpawnWave
		/// </summary>
		public List<EnemySpawnWaveRound> WaveRounds { get; set; }

		public EnemySpawnWave() 
		{
			
		}

		public override void _Ready()
		{
			ProcessWaveDetailsFile();
		}

		#region Scene Initialization
		/// <summary>
		///		Reads the wave details file and sets up our queue
		/// </summary>
		private void ProcessWaveDetailsFile()
		{
			if (WaveDetailsFile == null) throw new Exception("A wave nas no Wave Detail File assigned to it. ZANWOSD8");
			using FileAccess waveRoundFile = FileAccess.Open(WaveDetailsFile, FileAccess.ModeFlags.Read);
			string fileContent = waveRoundFile.GetAsText();
			WaveRounds = JsonSerializer.Deserialize<List<EnemySpawnWaveRound>>(fileContent);

			//foreach (EnemySpawnWaveRound wr in WaveRounds)
			//{
			//	GD.PushError(wr.CreatureName);
			//	GD.PushError(wr.SpawnCount);
			//	GD.PushError(wr.SpawnShape);
			//	GD.PushError(wr.SpawnShapeEnum);
			//	GD.PushError(wr.TimeBeforeSpawnMS);
			//}
		}
		#endregion
	}
}
