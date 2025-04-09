using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UODef.Mobiles.Towers;
using UODef.Player;

public partial class PlaceTowerModal : Control
{
	#region Node References
	private PlayerProfile Player;   // Autoload singleton
	private TextureButton CloseBtn;
    private VBoxContainer TowerEntryList;


	#endregion

	private PackedScene NewTowerEntry = GD.Load<PackedScene>("res://src/GUI/Modals/PlaceTower/TowerEntryModal.tscn");

    /// <summary>
    ///     The list of tower types we will display
    /// </summary>
    public List<TowerTypeEnum> CurrentTowerTypeSpawns { get; set; }

	private List<TowerEntryModal> TowerEntryModalList = new();

	/// <summary>
	///		The global pos that was clicked on to pop this modal
	/// </summary>
	public Vector2 TowerPlacementTarget;

	#region Signals
	[Signal]
    public delegate void ModalDismissedEventHandler();

	[Signal]
	public delegate void AddTowerRequestEventHandler(int twrType, int costGP, Vector2 placementLocation);

	#endregion


	#region Godot Override Methods
	public override void _Ready()
    {
        FindReleveantNodes();

		// We expect CurrentTowerTypeSpawns to have been initialized before this. Otherwise we will display an empty modal
		DisplayTowerEntries();
	}
	#endregion


	#region Scene Initialization
	private void FindReleveantNodes()
	{
		Player = GetNode<PlayerProfile>("/root/CurrentPlayerProfile");  // should this request get moved to UODef.System in some fashion?
		CloseBtn = GetNode<TextureButton>("ModalPlacement/Modal/ModalHeader/CloseBtn");
		TowerEntryList = GetNode<VBoxContainer>("ModalPlacement/Modal/ScrollContainer/TowerEntryList");
		CloseBtn.Pressed += DismissModal;
	}

	public void DisplayTowerEntries()
	{
		float maxAttacksPerSecond = 0.0f;
		float maxAttackRange = 0.0f;
		float maxAttackDamage= 0.0f;

		// Create and keep track of each tower entry modal
		foreach (TowerTypeEnum twrType in CurrentTowerTypeSpawns)
        {
			TowerEntryModal twrEntry = NewTowerEntry.Instantiate<TowerEntryModal>();
			TowerEntryList.AddChild(twrEntry);
            twrEntry.SetTowerType(twrType);

			// Keep track of max stats so we can loop again and update the relative settings guages
			if (twrEntry.DisplayTowerWeaponStats.AttacksPerSecond > maxAttacksPerSecond) maxAttacksPerSecond = twrEntry.DisplayTowerWeaponStats.AttacksPerSecond;
			if (twrEntry.DisplayTowerWeaponStats.AttackRange > maxAttackRange) maxAttackRange = twrEntry.DisplayTowerWeaponStats.AttackRange;
			if (twrEntry.DisplayTowerWeaponStats.AttackDamage > maxAttackDamage) maxAttackDamage = twrEntry.DisplayTowerWeaponStats.AttackDamage;
			// Attach signals
			twrEntry.TowerPlacementRequest += AddTowerRequestCB;
			TowerEntryModalList.Add(twrEntry);
		}
		// We've created all the appropriate entries so now we must update them to display info relative to eachother
		foreach (TowerEntryModal tem in TowerEntryModalList)
		{
			tem.UpdateStatRangeMaxes(maxAttacksPerSecond, maxAttackRange, maxAttackDamage);
		}
    }


    public void AddTowerRequestCB(TowerEntryModal tem, int twrType, int costGP)
    {
		/*
			HERE I will request the money from the player bank.. 
				then I have a ton of access to change displays, set warning or success msgs, etc if it fails or succeeds
				even have access to the tower entry modal that made this request
		 */
		if (!Player.TakeGP(costGP))
		{
			GD.PushError("Can't afford it, dingus");
			return;
		}

		EmitSignal(SignalName.AddTowerRequest, (int)twrType, costGP, TowerPlacementTarget);
    }

    public void DismissModal()
    {
        EmitSignal(SignalName.ModalDismissed);
        QueueFree();
    }
	#endregion
}
