using Godot;
using System;


namespace UODef.Player
{
	public enum BankTransactionType
	{
		None = 0,
		Withdrawl = 1,
		Deposit = 2,
		BalanceInquery = 3,
	};
	/*
		- Starting gold values will be set by the lvl
		- bank accessible can go
		- this needs to exist in a different context
		- the pieces are fine but they should exist within some sort of "Player" profile
		- The class that I have labeled as 'Player' right now is actually just the camera & controls for its mvmt

		- So, some sort of grander 'PlayerProfile' class should likely exist
		- It would probably contain a reference to the bank
		- It would probably not contain a reference to camera mvmt?
			-- I think it's just the name that is confusing me. If I think of it as the player's viewport maybe that is better? 
				- Each lvl would have its own viewport, with its own, flexible starting position
				- Maybe the player profile updates things like mvmt speed and default zoom, but there is no need for much else.

		Player
		=Bank=XpManager=Stats=Settings?
	 */
	
	public partial class PlayerBank : Node
	{
		/// <summary>
		///		If this is set to false any request to the bank will be denied.
		///		Intended as a safeguard for disabling purchasing at certain periods of time
		/// </summary>
		public bool BankAccessible { get; set; } = true;

		/// <summary>
		///		The current number of gold pieces that this player has for this round
		/// </summary>
		public int GoldPieces { get; private set; } = 125*25;

		#region Signals
		/// <summary>
		///		Signal emitted when the bank's balance changes. Emits the current balance.
		/// </summary>
		/// <param name="gp">The GoldPieces in the bank</param>
		[Signal]
		public delegate void BalanceChangedEventHandler(int gp);

		/// <summary>
		///		Signal emitted when a transaction happens
		/// </summary>
		/// <param name="transMsg">String containing the transaction msg - whether balance inquery, withdraw, etc.</param>
		[Signal]
		public delegate void BankTransactionMsgEventHandler(string transMsg);
		#endregion

		public PlayerBank() : base()
		{
		}

		public override void _Ready()
		{
			// We'll take advantage of the built in _Ready method to emit a signal informing the world of our current (starting) balance
			EmitSignal(SignalName.BalanceChanged, GoldPieces);
		}

		/// <summary>
		///		Requests a withdrawl from the bank. The input amount can be positive or negative, we will always try to withdraw the absolute value of the amt entered.
		/// </summary>
		/// <param name="amt">The amount of gold pieces that you wish to withdraw</param>
		/// <returns>Returns whether this withdrawl request was sucessful or not</returns>
		public bool WithdrawGP(int amt)
		{
			// Force negative
			if (amt > 0) amt *= -1;
			GetTransactionMsg(amt, BankTransactionType.Withdrawl);
			return ProcessTransaction(amt);
		}

		/// <summary>
		///		Requests a deposit to the bank. The input amount can be positive or negative, we will always try to deposit the absolute value of the amt entered. 
		/// </summary>
		/// <param name="amt">The amount of gold pieces that you wish to deposit</param>
		/// <returns>Returns whether this deposit was successful or not</returns>
		public bool DepositGP(int amt)
		{
			// Force positive 
			amt = Math.Abs(amt);
			GetTransactionMsg(amt, BankTransactionType.Deposit);
			return ProcessTransaction(amt);
		}
		
		/// <summary>
		///		Request teh current GoldPieces count
		/// </summary>
		/// <returns>The current number of gold pieces</returns>
		public int BalanceInquery()
		{
			GetTransactionMsg(0, BankTransactionType.BalanceInquery);
			return GoldPieces;
		}

		/// <summary>
		///		Does the work of actually effecting the balance. Does not allow for a balance less than zero. 
		///		Only capable of adding the passed 'amt' to the current GP balance
		/// </summary>
		/// <param name="amt">The amt to which effect the GP balance by</param>
		/// <returns>Whether the transaction succesfully occured or not</returns>
		private bool ProcessTransaction(int amt)
		{
			if (!BankAccessible) return false;
			int newBalance = GoldPieces + amt;
			if (newBalance < 0)
			{
				return false;
			}
			GoldPieces = newBalance;
			EmitSignal(SignalName.BalanceChanged, GoldPieces);
			return true;
		}

		/// <summary>
		///		Return a nice human-readable message describing the given GP amt and BankTransactionType
		/// </summary>
		/// <param name="amt">How many GP the transaction represents</param>
		/// <param name="bankTransactionType">Enum value describing the event</param>
		/// <param name="printMsg">Whether we will try to print this message to console or not</param>
		private void GetTransactionMsg(int amt, BankTransactionType bankTransactionType, bool printMsg = false)
		{
			// Force positive so that our messages will always read nicely
			amt = Math.Abs(amt);

			string outMsg = "";
			switch (bankTransactionType)
			{
				case BankTransactionType.Withdrawl:
					outMsg = "Withdrawing: " + amt.ToString() + " gold pieces from your bank";
					break;
				case BankTransactionType.Deposit:
					outMsg = "Depositing: " + amt.ToString() + " gold pieces into your bank";
					break;
				case BankTransactionType.BalanceInquery:
					outMsg = "Your current balance is: " + GoldPieces.ToString() + " gold pieces";
					break;
			}
			if (printMsg)
			{
				GD.PushError(outMsg);
			}
			EmitSignal(SignalName.BankTransactionMsg, outMsg);
		}

	}

}
