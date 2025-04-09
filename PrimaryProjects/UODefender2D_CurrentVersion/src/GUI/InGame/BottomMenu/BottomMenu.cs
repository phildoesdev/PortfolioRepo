using Godot;
using System;
using UODef.EnemyMgmt;
using UODef.Player;

namespace UODef.GUI;

public partial class BottomMenu : CanvasLayer
{
	private PlayerProfile PlayerProfileNode;
	private EnemyManager EnemyManagerNode;


	private RichTextLabel HealthText;
	private RichTextLabel GoldText;
	private RichTextLabel SpawnText;
	private RichTextLabel KillsText;
	private RichTextLabel ScoresText;
	private RichTextLabel WavesText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializeNodes();
	}

	#region Scene Initialization
	public void InitializeNodes()
	{
		HealthText = GetNode<RichTextLabel>("BottomMenu/Background/BotMenuText/HealthText");
		GoldText = GetNode<RichTextLabel>("BottomMenu/Background/BotMenuText/GoldText");
		SpawnText = GetNode<RichTextLabel>("BottomMenu/Background/BotMenuText/SpawnText");
		KillsText = GetNode<RichTextLabel>("BottomMenu/Background/BotMenuText/KillsText");
		ScoresText = GetNode<RichTextLabel>("BottomMenu/Background/BotMenuText/ScoresText");
		WavesText = GetNode<RichTextLabel>("BottomMenu/Background/BotMenuText/WavesText");

		PlayerProfileNode = GetNode<PlayerProfile>("/root/CurrentPlayerProfile");
		EnemyManagerNode = GetNode<EnemyManager>("/root/CastleCourtYard/EnemyManager"); // This path is idiotic

		if (PlayerProfileNode != null)
		{
			// Setup listeners to update things
			PlayerProfileNode.Bank.BalanceChanged += UpdateGoldText;
		}
		if (EnemyManagerNode != null)
		{
			// Setup listeners to update things
			EnemyManagerNode.EnemySpawned += UpdateSpawnText;
			EnemyManagerNode.EnemyDied += UpdateKillsText;
			EnemyManagerNode.WaveChanged += UpdateWavesText;
		}

		UpdateGoldText(PlayerProfileNode.Bank.GoldPieces);
		UpdateWavesText(EnemyManagerNode.CurrentWaveNumber, EnemyManagerNode.CurrentSeriesTotalWaves);
	}
	#endregion

	#region Callbacks to update the menu
	public void UpdateHealthText(string txt)
	{
		HealthText.Text = txt;
	}

	public void UpdateGoldText(int newBalance)
	{
		GoldText.Text = newBalance.ToString();
	}

	public void UpdateSpawnText(int spawnedEnemies)
	{
		SpawnText.Text = spawnedEnemies.ToString();
	}

	public void UpdateKillsText(int totalKills)
	{
		KillsText.Text = totalKills.ToString();
	}

	public void UpdateScoresText(string txt)
	{
		ScoresText.Text = txt;
	}

	public void UpdateWavesText(int current, int max)
	{
		WavesText.Text = current.ToString() + "/" + max.ToString();
	}
	#endregion
}
