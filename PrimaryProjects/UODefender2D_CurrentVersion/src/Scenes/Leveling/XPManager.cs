using Godot;
using System;

namespace UODef.Leveling
{
	#region Enums
	public enum LevelingCurveTypeEnum
	{
		None = 0,
		TowerCurve = 1,
		CreatureCurve = 2,
		PlayerCurve = 3
	}
	#endregion

	public partial class XPManager : Node
	{
		/// <summary>
		///		An enum that describes different implementations of the leveling curve
		/// </summary>
		public LevelingCurveTypeEnum LvlCurveType { get; private set; }


		/// <summary>
		///		Total XP ever gained
		/// </summary>
		public int TotalXP { get; private set; } = 0;

		/// <summary>
		///		The amt of XP since our last lvl
		/// </summary>
		public int CurrentXP { get; private set; } = 0;

		/// <summary>
		///		The current lvl we have achieved. We may have enough XP to be a higher lvl, but our current lvl is lower.
		/// </summary>
		public int CurrentLevel { get; private set; } = 1;

		/// <summary>
		///		The amount of awarded but unspent reward points
		/// </summary>
		public int CurrentRewardPoints { get; private set; } = 0;

		/// <summary>
		///		If set to true as XP is added the level is automatically incremented (means we check for lvl up each time xp is awarded)
		/// </summary>
		public bool AutoLevelUp { get; set; } = true;

		#region Signals
		/// <summary>
		///     Signal fired when the XPManager levels up
		/// </summary>
		[Signal]
		public delegate void LeveledEventHandler();

		/// <summary>
		///		Signal indicating that xp could be spent to gain a lvl
		/// </summary>
		[Signal]
		public delegate void ReadyToLevelEventHandler();

		/// <summary>
		///		Fires each time we gain XP
		/// </summary>
		[Signal]
		public delegate void XPGainedEventHandler(int xpGainedAmt, int totalXP);
		#endregion

		public XPManager(LevelingCurveTypeEnum lvlCurveType) : base() 
		{
			LvlCurveType = lvlCurveType;
			// Assuming if this is a creature that we should auto-lvl
			if (lvlCurveType == LevelingCurveTypeEnum.CreatureCurve) AutoLevelUp = true;
		}


		/// <summary>
		///		Returns the XP required to reach a lvl (not cumulatively)
		/// </summary>
		/// <param name="lvl">The lvl we're investigating</param>
		/// <returns>The amt of XP req'd to get to this lvl from 0</returns>
		public int CalculateXPForLvl(int lvl)
		{
			switch (LvlCurveType)
			{
				case LevelingCurveTypeEnum.TowerCurve:
					return (int)Math.Floor((Math.Pow(lvl, 3) * 4) / 5)+125;
				case LevelingCurveTypeEnum.CreatureCurve:
					return (int)Math.Floor((Math.Pow(lvl, 3) * 4) / 5)+125;
				case LevelingCurveTypeEnum.PlayerCurve:
					return (int)Math.Floor((Math.Pow(lvl, 3) * 4) / 5) + 125;
				case LevelingCurveTypeEnum.None:
				default:
					return -1;
			}
		}

		/// <summary>
		///		Curve that determines how many points are 
		/// </summary>
		/// <param name="lvl"></param>
		/// <returns></returns>
		public int CalculateRewardPointsByLvl(int lvl)
		{
			return 2*lvl + 3*(lvl%2);
		}

		/// <summary>
		///		Does the level calculation based on current XP and sees if that is higher than
		///			the current lvl.
		///		Could probably turn this into a flag that gets set by the method that adds XP, but
		///			maybe it's better to not have to manage a flag
		/// </summary>
		/// <returns></returns>
		public bool ReadyToLevelUp()
		{
			if (LvlCurveType == LevelingCurveTypeEnum.PlayerCurve) GD.PushError($"Lvl: {CurrentLevel} XP: {CurrentXP}/{CalculateXPForLvl(CurrentLevel + 1)}");
			return CurrentXP >= CalculateXPForLvl(CurrentLevel + 1);
		}

		/// <summary>
		///		Checks if our XP warrents a lvl up, subtracts the xp cost for this lvl, inc lvl, etc.
		///		Emits a signal indicating that lvl up happened
		/// </summary>
		public void LevelUp()
		{
			if (!ReadyToLevelUp()) return;
			CurrentLevel += 1;
			CurrentXP -= CalculateXPForLvl(CurrentLevel);
			CurrentRewardPoints += CalculateRewardPointsByLvl(CurrentLevel);
			GD.PushError($"Level has increased from {CurrentLevel-1} to {CurrentLevel} for a total of {CurrentRewardPoints} reward points!");
			EmitSignal(SignalName.Leveled);
		}

		/// <summary>
		///		The public interface for adding XP... maybe it's better to just manipulate the property? i thought there would be more work around this, idk
		/// </summary>
		public void GiveXP(int xp)
		{
			CurrentXP += xp;
			TotalXP += xp;

			// Informed any concerned citizens of our newest gainz
			EmitSignal(SignalName.XPGained, xp, TotalXP);

			// Emit signal stating that we are ready to lvl up and attempt to do so if appropriate
			if (ReadyToLevelUp())
			{
				EmitSignal(SignalName.ReadyToLevel);
				if (AutoLevelUp) LevelUp();
			}
		}
	}
}
