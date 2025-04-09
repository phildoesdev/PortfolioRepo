using Godot;
using UODef.Abilities;
using UODef.Mobiles.Towers;


public partial class TowerEntryModal : MarginContainer
{
	private BaseTower DisplayTower;
	private TowerTypeEnum DisplayTowerType;
	public CalculatedWeaponStats DisplayTowerWeaponStats;

	// Our parent modal will set these so that our attack, range, and speed ProgressBars can display relative values
	private float MaxAttacksPerSecond = 0.0f;
	private float MaxAttackRange = 0.0f;
	private float MaxAttackDamage = 0.0f;

	#region ModalHeader
	private TextureButton AddBtn;
	private RichTextLabel TowerTitleTxt;
	private RichTextLabel TowerDescriptionTxt;
	private TextureRect TowerIcon;
	private SubViewport TowerIconSubViewport;
	private RichTextLabel CostTxt;
	#endregion

	#region PowerVisualizationPanel
	private TextureProgressBar AttackProgressBar;
	private TextureProgressBar RangeProgressBar;
	private TextureProgressBar SpeedProgressBar;
	#endregion

	#region DamageDistributionPanel
	private RichTextLabel DirectDmgTxt;
	private RichTextLabel PhysDmgTxt;
	private RichTextLabel FireDmgTxt;
	private RichTextLabel ColdDmgTxt;
	private RichTextLabel PoisonDmgTxt;
	private RichTextLabel EnergyDmgTxt;


						
	private TextureRect DamageDistributionProgressBar;
	#endregion

	[Signal]
	public delegate void TowerPlacementRequestEventHandler(TowerEntryModal tem, int twrType, int costGP);

	public TowerEntryModal()
	{

	}

	#region Godot Override Methods
	public override void _Ready()
    {
		FindNodeReferences();
	}
	#endregion


	#region Scene Initialization
	private void FindNodeReferences()
	{
		AddBtn = GetNode<TextureButton>("TowerEntryContainer/TowerDescription/AddBtn");
		AddBtn.Pressed += AddBtnPressed;

		TowerTitleTxt = GetNode<RichTextLabel>("TowerEntryContainer/TowerDescription/TowerTitleTxt");
		TowerDescriptionTxt = GetNode<RichTextLabel>("TowerEntryContainer/TowerDescription/TowerDescriptionTxt");
		TowerIcon = GetNode<TextureRect>("TowerEntryContainer/TowerDescription/TowerIcon");
		TowerIconSubViewport = GetNode<SubViewport>("TowerEntryContainer/TowerDescription/TowerIconDisplayViewport");


		CostTxt = GetNode<RichTextLabel>("TowerEntryContainer/TowerDescription/CostTxt");

		AttackProgressBar = GetNode<TextureProgressBar>("TowerEntryContainer/PowerVisualizationPanel/AttackProgressBar");
		RangeProgressBar = GetNode<TextureProgressBar>("TowerEntryContainer/PowerVisualizationPanel/RangeProgressBar");
		SpeedProgressBar = GetNode<TextureProgressBar>("TowerEntryContainer/PowerVisualizationPanel/SpeedProgressBar");

		DirectDmgTxt = GetNode<RichTextLabel>("TowerEntryContainer/DamgeDistributionPanel/DirectDmgTxt");
		PhysDmgTxt = GetNode<RichTextLabel>("TowerEntryContainer/DamgeDistributionPanel/PhysDmgTxt");
		FireDmgTxt = GetNode<RichTextLabel>("TowerEntryContainer/DamgeDistributionPanel/FireDmgTxt");
		ColdDmgTxt = GetNode<RichTextLabel>("TowerEntryContainer/DamgeDistributionPanel/ColdDmgTxt");
		PoisonDmgTxt = GetNode<RichTextLabel>("TowerEntryContainer/DamgeDistributionPanel/PoisonDmgTxt");
		EnergyDmgTxt = GetNode<RichTextLabel>("TowerEntryContainer/DamgeDistributionPanel/EnergyDmgTxt");

		DamageDistributionProgressBar = GetNode<TextureRect>("TowerEntryContainer/DamgeDistributionPanel/DamageDistributionProgressBar");
	}
	#endregion

	public void SetTowerType(TowerTypeEnum twrType)
	{
		BaseTower twrOutput = null;
		switch (twrType)
		{
			case TowerTypeEnum.Mage:
				twrOutput = new MageTower();
				break;
			case TowerTypeEnum.Tempest:
				twrOutput = new TempestTower();
				break;
			case TowerTypeEnum.Pugilist:
				twrOutput = new PugilistTower();
				break;
			case TowerTypeEnum.Archer:
				twrOutput = new Archer();
				break;
			case TowerTypeEnum.Assasin:
				twrOutput = new AssasinFencer();				
				break;
			default:
				// No tower to spawn. This causes a crash.
				return;
		}

		DisplayTowerType = twrType;
		DisplayTower = twrOutput;

		CalculatedWeaponStats wepStats = twrOutput.GetCalculatedWeaponStats();
		DisplayTowerWeaponStats = wepStats;

		// Default these to us
		MaxAttacksPerSecond = wepStats.AttacksPerSecond;
		MaxAttackRange = wepStats.AttackRange;
		MaxAttackDamage = wepStats.AttackDamage;

		// Right now this is a magic number to place the tower on the subviewport 
		DisplayTower.Position = new Vector2(32.5f, 70.0f);
		TowerIconSubViewport.AddChild(DisplayTower);
		DisplayTower.PlayDefaultAnimation();
		// Set other modal values
		UpdateTowerTitleTxt(DisplayTower.TowerTitle);
		UpdateTowerDescriptionTxt(DisplayTower.TowerDescription);
		UpdateCostTxt(DisplayTower.CostGP.ToString());

		UpdateDirectDmgTxt(wepStats.DirectDamagePercentage.ToString());
		UpdatePhysDmgTxt(wepStats.PhysicalDamagePercentage.ToString());
		UpdateFireDmgTxt(wepStats.FireDamagePercentage.ToString());
		UpdateColdDmgTxt(wepStats.ColdDamagePercentage.ToString());
		UpdatePoisonDmgTxt(wepStats.PoisonDamagePercentage.ToString());
		UpdateEnergyDmgTxt(wepStats.EnergyDamagePercentage.ToString());

		// Set some shader vars so the bar draws correctly
		UpdateDamageDistributionProgressBar();
	}

	public void UpdateStatRangeMaxes(float maxAttacksPerSecond, float maxAttackRange, float maxAttackDamage)
	{
		if (maxAttacksPerSecond > MaxAttacksPerSecond ) MaxAttacksPerSecond = maxAttacksPerSecond;
		if (maxAttackRange > MaxAttackRange ) MaxAttackRange  = maxAttackRange;
		if (maxAttackDamage > MaxAttackDamage) MaxAttackDamage  = maxAttackDamage;

		UpdateSpeedProgressBar((DisplayTowerWeaponStats.AttacksPerSecond / MaxAttacksPerSecond)*100);
		UpdateRangeProgressBar((DisplayTowerWeaponStats.AttackRange / MaxAttackRange)*100);
		UpdateAttackProgressBar((DisplayTowerWeaponStats.AttackDamage / MaxAttackDamage)*100);
	}


	#region btnListeners 
	/// <summary>
	///		When a user clicks 'add' for this entry, notify our parent modal
	/// </summary>
	public void AddBtnPressed()
	{
		EmitSignal(SignalName.TowerPlacementRequest, this, (int)DisplayTowerType, DisplayTower.CostGP);
	}
	#endregion


	#region ModalHeader Manipulation Methods
	private void UpdateTowerTitleTxt(string newVal)
	{
		TowerTitleTxt.Text = newVal;
	}

	private void UpdateTowerDescriptionTxt(string newVal)
	{
		TowerDescriptionTxt.Text = newVal;
	}

	private void UpdateCostTxt(string newVal)
	{
		CostTxt.Text = newVal;
	}

	private void UpdateTowerIcon(Texture2D twrIconTexture)
	{
		TowerIcon.Texture = twrIconTexture;
	}
	#endregion


	#region PowerVisualizationPanel Manipulation Methods
	private void UpdateAttackProgressBar(float newVal)
	{
		AttackProgressBar.Value = newVal;
	}

	private void UpdateRangeProgressBar(float newVal)
	{
		RangeProgressBar.Value = newVal;
	}

	private void UpdateSpeedProgressBar(float newVal)
	{
		SpeedProgressBar.Value = newVal;
	}
	#endregion


	#region DamageDistributionPanel Manipulation Methods
	private void UpdateDirectDmgTxt(string newVal)
	{
		DirectDmgTxt.Text = newVal;
	}

	private void UpdatePhysDmgTxt(string newVal)
	{
		PhysDmgTxt.Text = newVal;
	}

	private void UpdateFireDmgTxt(string newVal)
	{
		FireDmgTxt.Text = newVal;
	}

	private void UpdateColdDmgTxt(string newVal)
	{
		ColdDmgTxt.Text = newVal;
	}

	private void UpdatePoisonDmgTxt(string newVal)
	{
		PoisonDmgTxt.Text = newVal;
	}

	private void UpdateEnergyDmgTxt(string newVal)
	{
		EnergyDmgTxt.Text = newVal;
	}

	private void UpdateDamageDistributionProgressBar()
	{
		ShaderMaterial progressBarShaderMaterial = (ShaderMaterial)DamageDistributionProgressBar.Material;

		progressBarShaderMaterial.SetShaderParameter("direct_dmg_percentage", DisplayTowerWeaponStats.DirectDamagePercentage);
		progressBarShaderMaterial.SetShaderParameter("phys_dmg_percentage", DisplayTowerWeaponStats.PhysicalDamagePercentage);
		progressBarShaderMaterial.SetShaderParameter("fire_dmg_percentage", DisplayTowerWeaponStats.FireDamagePercentage);
		progressBarShaderMaterial.SetShaderParameter("cold_dmg_percentage", DisplayTowerWeaponStats.ColdDamagePercentage);
		progressBarShaderMaterial.SetShaderParameter("poison_dmg_percentage", DisplayTowerWeaponStats.PoisonDamagePercentage);
		progressBarShaderMaterial.SetShaderParameter("energy_dmg_percentage", DisplayTowerWeaponStats.EnergyDamagePercentage);
	}
	#endregion





}
