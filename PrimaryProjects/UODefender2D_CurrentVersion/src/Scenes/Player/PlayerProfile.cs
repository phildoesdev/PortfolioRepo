using System;
using System.Collections.Generic;
using Godot;
using UODef.Leveling;
using UODef.Player.Viewport;

namespace UODef.Player
{
	internal partial class PlayerProfile : Node2D
	{
		public PlayerBank Bank { get; private set; }
		public XPManager XpManager { get; private set; }
		public PlayerViewport PViewport { get; set; }

		public int CurrentScore { get; private set; } = 0;


		public PlayerProfile() : base()
		{
			//  In the future the gui should be attached to the player profile in some sort of way that they can be attached/detatched, b/c if they must stay with the viewport
			//	This will probably mean like some sort of class that we can easily interact with, loop over, etc.
			//	But they will for the most part have to be constructed scenes. Interesting.
			//	For now, I am just attaching the bottom menu on the scene tree.
		}

		public override void _Ready()
		{
			InitializeBank();
			InitializeXpManager();
			InitializePlayerViewport();
		}

		#region Scene Initialization
		public void InitializeXpManager()
		{
			XpManager = new(LevelingCurveTypeEnum.PlayerCurve);
		}

		public void InitializeBank()
		{
			Bank = new PlayerBank();
		}

		public void InitializePlayerViewport()
		{
			PViewport = new PlayerViewport();
			AddChild(PViewport);
		}
		#endregion

		#region XP Manager Related Methods
		public void GiveXP(int xp)
		{
			XpManager.GiveXP(xp);
		}
		#endregion

		#region PlayerBank Related Methods
		public void GiveGP(int gp)
		{
			Bank.DepositGP(gp);
		}

		public bool TakeGP(int gp)
		{
			return Bank.WithdrawGP(gp);
		}

		public int CurrentGPBalance()
		{
			return Bank.BalanceInquery();
		}
		#endregion

		#region GUI # CanvasLayer Related Methods
		/// <summary>
		///		A pass-thru that allows us to display a modal.
		///		Very simple minded, any controls such as distinctness of the modal being displayed is expected to be handeled outside of the player profile
		/// </summary>
		/// <param name="cntrlNode"></param>
		public void DisplayModal(Control cntrlNode)
		{
			PViewport.DisplayModal(cntrlNode);
		}

		/*
			Could have things like
				-HideGUI
				-Dismiss GUI
			Could split things into types of guis to display like modals vs hud
				-Hide HUD
				-Hide Modals
				-Dismiss HUD
				-Dismiss Modals
		 */
		#endregion
	}
}
