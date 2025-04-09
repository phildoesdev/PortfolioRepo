using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using UODef.EnemyMgmt.EnemySpawn;
using UODef.Mobiles.Enemies;

namespace UODef.EnemyMgmt
{
    public partial class EnemyManager : Node
	{
		private int TotalEnemiesSpawned = 0;
		private int TotalEnemiesDied = 0;

		#region Node Maintenance
		/// <summary>
		///     Empty Node that acts as a container for the current spawn wave's timers. 
		///     The intention is to allow us easier control over starting/stopping/pausing spawn waves
		/// </summary>
		private Node2D SpawnTimerContainer;
		#endregion

		/// <summary>
		///     Contains a list of possible enemy navigation regions
		/// </summary>
		public List<EnemyNavigationRegion> EnemyNavigationRegionList = new();

		private EnemySpawnSeries CurrentEnemySpawnSeries;
        private EnemySpawnWave CurrentEnemySpawnWave;

        public bool CanSpawn { get; set; } = false;

        #region Signals
        /// <summary>
        ///     Signal to indicate creature spawned. Currently emit with no additional info b/c I'm just using it to increment a counter
        /// </summary>
        [Signal]
        public delegate void EnemySpawnedEventHandler(int totalEnemiesSpawned);

		/// <summary>
		///		Every time an enemy dies the enemy manager gets a signal, we emit our own w/ the current death count
		/// </summary>
		/// <param name="totalEnemiesDied">The current total enemies died count</param>
		[Signal]
		public delegate void EnemyDiedEventHandler(int totalEnemiesDied);

		[Signal]
		public delegate void WaveChangedEventHandler(int currentWave, int maxWaves);
		#endregion

		#region API
		public int CurrentWaveNumber
		{
			get
			{
				return CurrentEnemySpawnSeries.CurrentWave;
			}

		}

		public int CurrentSeriesTotalWaves
		{
			get
			{
				return CurrentEnemySpawnSeries.TotalWaves;
			}
		}
		#endregion

		#region Godot Overrides and Related Hooks
		public override void _Ready()
		{
            InitializeScene();
            CanSpawn = true;
		}
        
		public override void _PhysicsProcess(double delta)
		{
            if (!CanSpawn) return;

			if (Input.IsActionJustPressed("One"))
			{
				ProcessNextWave();
                // CanSpawn = false;
			}
		}
		#endregion

		#region Spawn Manipulation
        /// <summary>
        ///     Gets the next spawn wave if there is one and creates the appropriate timers for the spawn
        /// </summary>
		public void ProcessNextWave()
		{
			CurrentEnemySpawnWave = CurrentEnemySpawnSeries.GetNextSpawnWave();
			// We need to shoot signal indicating next wave is started
			EmitSignal(SignalName.WaveChanged, CurrentWaveNumber, CurrentSeriesTotalWaves);
			// Sanity check
			if (CurrentEnemySpawnWave == null)
			{
                GD.PushError("No Enemy Spawn Wave To Unspool... EU6YY9P7");
				return;
			}
			// Keep track of when the next timer should go
			float currTime = 0.0f;
            int waveroundcount = 0;
            float seriesDifficultyCurveValue = CurrentEnemySpawnSeries.DifficultyCurve.Sample((float)(CurrentEnemySpawnSeries.CurrentWave+1) / (float)CurrentEnemySpawnSeries.TotalWaves);
            GD.PushError($"seriesDifficultyCurveValue = x = (float)CurrentEnemySpawnSeries.CurrentWave / (float)CurrentEnemySpawnSeries.TotalWaves: {seriesDifficultyCurveValue} = {(float)(CurrentEnemySpawnSeries.CurrentWave+1) / (float)CurrentEnemySpawnSeries.TotalWaves} = {CurrentEnemySpawnSeries.CurrentWave} / {CurrentEnemySpawnSeries.TotalWaves}");
			// Foreach round in the wave create a timer to spawn appropriately
            // for (int i = 0; i < CurrentEnemySpawnWave.WaveRounds.Count; i++)
            foreach (EnemySpawnWaveRound eswr in CurrentEnemySpawnWave.WaveRounds)
			{
                float waveDifficultyCurve = CurrentEnemySpawnWave.DifficultyCurve.Sample((float)waveroundcount / (float)CurrentEnemySpawnWave.WaveRounds.Count);
                GD.PushError($"Sample point = waveroundcount / CurrentEnemySpawnWave.WaveRounds.Count: {(float)waveroundcount / (float)CurrentEnemySpawnWave.WaveRounds.Count} = {waveroundcount} / {CurrentEnemySpawnWave.WaveRounds.Count} for sample {waveDifficultyCurve}");
                waveroundcount++; 
				currTime += eswr.TimeBeforeSpawnMS + CurrentEnemySpawnWave.TimeBetweenWaveRoundsMS;

				Timer tmpTimer = new();
				tmpTimer.WaitTime = currTime;
                tmpTimer.Timeout += () => SpawnWaveCallback(eswr, seriesDifficultyCurveValue, waveDifficultyCurve);
                tmpTimer.Autostart = true;
                tmpTimer.OneShot = true;
				SpawnTimerContainer.AddChild(tmpTimer);
			}
		}

		/// <summary>
		///     Gets called by the wave's timers. Passes a wave round to be placed
		/// </summary>
		/// <param name="spawnWaveRound"></param>
		/// <param name="seriesDifficultyCurveValue"></param>
		/// <param name="waveDifficultyCurveValue"></param>
		public void SpawnWaveCallback(EnemySpawnWaveRound spawnWaveRound, float seriesDifficultyCurveValue, float waveDifficultyCurveValue)
        {
            GD.PushError($"Spawning: {spawnWaveRound.CreatureName}, at {seriesDifficultyCurveValue}:{waveDifficultyCurveValue}");

			/*
             Now I need to 
                1) Create the obj from string name
                2) place on map correctly
                    - path must be assigned
                    - we must place spawns in any formation
             */

			// UODef.Mobiles.Enemies

			// We're just selecting the nav regions in order
			EnemyNavigationRegion navRegion = GetNavigationRegionByName(spawnWaveRound.NavigationRegionName);

			// Create the enmy (assuming we're being told to spawn something that inherits from the baseCreature
			Assembly assembly = Assembly.GetExecutingAssembly();
            for (int i = 0; i < spawnWaveRound.SpawnCount; i++)
			{
				BaseCreature enemySpawn = assembly.CreateInstance("UODef.Mobiles.Enemies." + spawnWaveRound.CreatureName) as BaseCreature;
				enemySpawn.Death += RecordSpawnedEnemyDeath;
				AddChild(enemySpawn);
				enemySpawn.GlobalPosition = navRegion.GetNextSpawnPos();
				enemySpawn.SetNewNavigationDestination(navRegion.GetTargetDest());
				EmitSignal(SignalName.EnemySpawned, ++TotalEnemiesSpawned);
			}
		}

        private EnemyNavigationRegion GetNavigationRegionByName(string name)
        {
            foreach (EnemyNavigationRegion region in EnemyNavigationRegionList)
            {
                if (region.PathName == name)
                {
                    return region;
                }
            }
			// default to the first path in the list
			return EnemyNavigationRegionList[0];
        }
		#endregion

		#region Scene Iniitialization
		/// <summary>
		///     A single call that encapsulates all of our scene intialization calls
		/// </summary>
		private void InitializeScene()
        {
			FindEnemyNavigationRegions();
            FindEnemySpawnSeriesNode();
            SpawnTimerContainer = GetNode<Node2D>("SpawnTimerContainer");
        }

        /// <summary>
        ///     Cycles through the navigation region container and grabs all potential navigation regions for our future reference
        /// </summary>
        private void FindEnemyNavigationRegions()
        {
            Node2D navRegionContainer = GetNode<Node2D>("NavigationRegions");			
            foreach (Node navRegion in navRegionContainer.GetChildren())
			{
				if (navRegion is not EnemyNavigationRegion enrn) continue;
				EnemyNavigationRegionList.Add(enrn);
			}
            if (EnemyNavigationRegionList.Count == 0) throw new Exception("Unable to find any enemy nav regions. GMMB3L5B");
		}

        /// <summary>
        ///     Method to grab the enemy spawn series
        /// </summary>
        public void FindEnemySpawnSeriesNode()
        {
			CurrentEnemySpawnSeries = GetNode<EnemySpawnSeries>("SpawnSeries/EnemySpawnSeries");
            if (CurrentEnemySpawnSeries == null) throw new Exception("Unable to find a valid enemy spawn series. TOD6JGPV");
		}
		#endregion

		#region Signal CallBack Handling
		public void RecordSpawnedEnemyDeath()
		{
			TotalEnemiesDied += 1;
			EmitSignal(SignalName.EnemyDied, TotalEnemiesDied);
		}
		#endregion
	}
}