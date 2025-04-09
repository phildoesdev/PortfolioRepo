using Godot;
using System;
using System.Collections.Generic;
using UODef.Items.Equipment;
using UODef.System;
using UODef.System.ResourceUtility;

namespace UODef.Mobiles.AnimationPlayer
{

	//// WE WILL CLEARLY HAVE TO RE-WRITE THIS IANIMATED INTERFACE TO MAKE SENSE..............................................!
	#region Animation related interfaces
	/// <summary>
	///    An interface to indicate that w/e class is implementing our animation player class
	/// </summary>
	public interface IAnimated : IDirectional
	{
		public void PlayAnimation(AnimationGlossaryEnum aniID, DirectionEnum dir = DirectionEnum.None, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false, double playtimeOverrideS = 0.0, float speedMultiplier = 1.0f);
		public void PlayAnimation(DefaultAnimationGlossaryEnum aniID, DirectionEnum dir = DirectionEnum.None, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false, double playtimeOverrideS = 0.0, float speedMultiplier = 1.0f);
		public Vector2 GetCurrentSpriteCenter();
	}
	#endregion


	#region Enums
	/// <summary>
	///		Options for our new animation player
	/// </summary>
	public enum AnimationLoopModeEnum
	{
		None,
		Repeat,
		HoldOnLastFrame
	}

	/// <summary>
	///		Currently only makes sense for the human animation. 
	///		Helps to define the order of the sprites we draw on the screen when animating a 
	///			sprite with many layers (such as a human with clothing and weapons)
	///		If you add to draw layer, you probably also want to add to 
	/// </summary>
	public enum DrawLayerEnum
	{
		Background,
		LeftHand_SingleHanded,
		RightHand_TwoHanded,
		Feet,
		Pants,
		Torso_Clothing_FemaleArmor,
		HeadCovering,
		Gloves,
		Ring,
		Talisman,
		NeckCovering,
		Hair,
		Waist_HalfApron,
		Torso_Inner_ChestArmor,
		Bracelet,
		FacialHair,
		Torso_Middle_SashTunic,
		Earrings,
		ArmCovering_Armor,
		Back_Cloak,
		BackPack,
		Torso_Outer_Robe,
		Legs_Outer_Skirt,
		Legs_Inner_Armor,
		None
	}
	#endregion

	public partial class MobileAnimationPlayer : Node2D
	{

		/// <summary>
		///		The order in which to draw the layers.
		///		The length of this informs the # of layers we can have animated psrites equipped to 
		///			(Sets the length of the AnimatedEquipmentSprites array)
		/// </summary>
		public static readonly int[] DrawOrder = {
			0x00,// - Background
            0x14,// - Back (Cloak)
            0x05,// - Chest Clothing/Female Chest Armor
            0x04,// - Pants
            0x03,// - Foot Covering/Armor
            0x18,// - Legs (inner)(Leg Armor)
            0x13,// - Arm Covering/Armor
            0x0D,// - Torso (inner)(Chest Armor)
            0x11,// - Torso (Middle)(Surcoat, Tunic, Full Apron, Sash)
            0x08,// - Ring
            0x09,// - Talisman
            0x0E,// - Bracelet
            0x07,// - Gloves
            0x17,// - Legs (outer)(Skirt/Kilt)
            0x0A,// - Neck Covering/Armor
            0x0B,// - Hair
            0x0C,// - Waist (Half-Apron)
            0x16,// - Torso (outer)(Robe)
            0x10,// - Facial Hair
            0x12,// - Earrings
            0x06,// - Head Covering/Armor
            0x01,// - Single-Hand item/weapon
            0x02,// - Two-Hand item/weapon (including Shield)
            0x15,// - BackPack
			0X19 // - None
        };

		/*private static readonly int[] DrawOrder = {
			0x00,// - Background
            0x05,// - Chest Clothing/Female Chest Armor
            0x04,// - Pants
            0x03,// - Foot Covering/Armor
            0x18,// - Legs (inner)(Leg Armor)
            0x13,// - Arm Covering/Armor
            0x0D,// - Torso (inner)(Chest Armor)
            0x11,// - Torso (Middle)(Surcoat, Tunic, Full Apron, Sash)
            0x08,// - Ring
            0x09,// - Talisman
            0x0E,// - Bracelet
            0x07,// - Gloves
            0x17,// - Legs (outer)(Skirt/Kilt)
            0x0A,// - Neck Covering/Armor
            0x0B,// - Hair
            0x0C,// - Waist (Half-Apron)
            0x16,// - Torso (outer)(Robe)
            0x10,// - Facial Hair
            0x12,// - Earrings
            0x06,// - Head Covering/Armor
            0x01,// - Single-Hand item/weapon
            0x02,// - Two-Hand item/weapon (including Shield)
            0x14,// - Back (Cloak)
            0x15,// - BackPack
			0X19 // - None
        };
		
		private static readonly Dictionary<DrawLayerEnum, List<DrawLayerEnum>> HiddenLayers = new();
		*/

		// The order in which we lay the nodes out to be drawn
		private int[] CurrentDrawOrder = DrawOrder;

		/// <summary>
		///		A reference to the animation utility reference node
		/// </summary>
		public ResourceUtility ResourceUtilityRef;

		/// <summary>
		///		Which animation archetype must this animation player conform to?
		/// </summary>
		public AnimationArchetypeEnum AnimationArchetype { get; set; }

		/// <summary>
		///		Created in the constructor, this is the main sprite animation for this player. 
		///		In opposition to equipment, hair, etc.
		/// </summary>
		public AnimatedSprite PrimaryAnimatedSprite { get; set; }
		/// <summary>
		///		The array of equipment that should be animated along with our primary animated sprite
		///		
		/// </summary>
		public AnimatedSprite[] AnimatedEquipmentSprites { get; set; }

		public Node2D AnimatedEquipmentNodeContainer { get; set; }

		#region Animation information
		private AnimationGlossaryEnum DefaultAnimation = AnimationGlossaryEnum.None;
		private DirectionEnum DefaultDirection = DirectionEnum.SouthWest;

		/// There are several 'default animations' for each arche type that we will fill out in our constructor, or w/e
		private AnimationGlossaryEnum DefaultIdleStillAnimation;
		private AnimationGlossaryEnum DefaultIdleAnimation;
		private AnimationGlossaryEnum DefaultWalkAnimation;
		private AnimationGlossaryEnum DefaultAttackAnimation;
		private AnimationGlossaryEnum DefaultDieAnimation;
		private AnimationGlossaryEnum DefaultGetHit;

		/// <summary>
		///		Holds information on how to construct the animation track such as loop, time b/w frames, etc
		/// </summary>
		private AnimationDescription CurrentAnimationDescription;
		/// <summary>
		///		Holds a single animation to be queued. Will be played if not empty when current animation finishes
		/// </summary>
		private AnimationDescription QueuedAnimationDescription;

		// Other internal tracking
		private int CurrentAnimationFrame = 0;
		private double NextFrameTickTimeS = 0.0;
		private DirectionEnum CurrentDirection;

		private DeltaTimeClock DeltaClock;
		#endregion

		public MobileAnimationPlayer(AnimationArchetypeEnum aniArchetype, int sheetID, int hueID=0) : base()
        {
			AnimationArchetype = aniArchetype;
			// Always 'only hue grey pixels for body
			PrimaryAnimatedSprite = new AnimatedSprite(sheetID, hueID, aniArchetype, onlyHueGreyPixels:true);
			// Initialize an array to hold our sprites per layer
			AnimatedEquipmentSprites = new AnimatedSprite[DrawOrder.Length];

			// Set defaults for our current and queued animations
			CurrentAnimationDescription = AnimationDefinitionLibrary.GetAnimationDescription(AnimationGlossaryEnum.None);
			QueuedAnimationDescription = AnimationDefinitionLibrary.GetAnimationDescription(AnimationGlossaryEnum.None);
			CurrentDirection = DefaultDirection;

			// Set default animations per arche type. 
			switch (aniArchetype)
			{
				case AnimationArchetypeEnum.Animal:
					DefaultAnimation = AnimationGlossaryEnum.A_idleStill;
					DefaultIdleStillAnimation = AnimationGlossaryEnum.A_idleStill;
					DefaultIdleAnimation = AnimationGlossaryEnum.A_idle;
					DefaultWalkAnimation = AnimationGlossaryEnum.A_walk;
					DefaultAttackAnimation = AnimationGlossaryEnum.A_attack1;
					DefaultDieAnimation = AnimationGlossaryEnum.A_die1;
					DefaultGetHit = AnimationGlossaryEnum.A_getHit;
					break;
				case AnimationArchetypeEnum.Monster:
					DefaultAnimation = AnimationGlossaryEnum.M_idleStill;
					DefaultIdleStillAnimation = AnimationGlossaryEnum.M_idleStill;
					DefaultIdleAnimation = AnimationGlossaryEnum.M_idle;
					DefaultWalkAnimation = AnimationGlossaryEnum.M_walk;
					DefaultAttackAnimation = AnimationGlossaryEnum.M_attack1;
					DefaultDieAnimation = AnimationGlossaryEnum.M_die1;
					DefaultGetHit = AnimationGlossaryEnum.M_getHit;
					break;
				case AnimationArchetypeEnum.Human:
					DefaultAnimation = AnimationGlossaryEnum.H_idleStill;
					DefaultIdleStillAnimation = AnimationGlossaryEnum.H_idleStill;
					DefaultIdleAnimation = AnimationGlossaryEnum.H_idle;
					DefaultWalkAnimation = AnimationGlossaryEnum.H_walk;
					DefaultAttackAnimation = AnimationGlossaryEnum.H_attackBash_1h;
					DefaultDieAnimation = AnimationGlossaryEnum.H_dieHard_back;
					DefaultGetHit = AnimationGlossaryEnum.H_getHit_fr_hi;
					break;
				default:
					break;
			}
		}

		#region Godot Overrides and Related
		public override void _Ready()
		{
			// Validate that the sheet id we set exists
			ResourceUtilityRef = GetNode<ResourceUtility>("/root/ResourceUtility");
			if (!ResourceUtilityRef.IsValidSheetID(PrimaryAnimatedSprite.SheetID))
			{
				throw new Exception($"Invalid Primary Body Sheet ID Used: [{PrimaryAnimatedSprite.SheetID}]. C6M28MZO");
			}
			InitializeDeltaClock();
			InitializeAnimatedSprites();
		}

		public override void _Process(double delta)
		{
			ProcessCurrentAnimation();
		}
		#endregion

		/// <summary>
		///		Nice way to keep track of time
		/// </summary>
		private void InitializeDeltaClock()
		{
			DeltaClock = new();
			AddChild(DeltaClock);
		}

		/// <summary>
		///		Adds the primary aniamted sprite to the scene - Gets initialized in our constructor and then added here.
		/// </summary>
		private void InitializeAnimatedSprites()
		{
			// Create node container for future equipment
			AnimatedEquipmentNodeContainer = new Node2D();

			// Add the primary animated spprite as a child, then the container
			AddChild(PrimaryAnimatedSprite);
			AddChild(AnimatedEquipmentNodeContainer);
		}

		#region Equipment Animation Detials
		/// <summary>
		///		Pretty blunt atm-- loops given list and replaces animations with that, cleaning up the old to make room for the new.
		/// </summary>
		/// <param name="newEquipment"></param>
		public void SetCurrentEquipment(List<IWearableEquipment> newEquipment)
		{
			// Not-so-smart override just wipes out all old equip and replaces it with the new
			if (newEquipment.Count > 0) RemoveCurrentEquipment();

			foreach (IWearableEquipment equip in newEquipment)
			{
				AnimatedSprite newAnimatedSprite = new(equip.SheetID, equip.HueID, equip.AnimationArchetype, drawLayer: equip.DrawLayer);

				if (AnimatedEquipmentSprites[(int)equip.DrawLayer] != null) continue; // Stops us from equiping in a slot that already has an item in it

				AnimatedEquipmentSprites[(int)equip.DrawLayer] = newAnimatedSprite;
				AnimatedEquipmentNodeContainer.AddChild(newAnimatedSprite);
			}
			ChangeDirection(DefaultDirection, true);
		}

		/// <summary>
		///		This will get more complicated in the future if we try to implement forcing the 
		///			correct draw order
		///		Doing the draw order correctly will take more time than I have and it's easy to just 
		///			equip items 'in the right order', so that the user can't tell.
		/// </summary>
		/// <param name="dir">New direction we want to switch to</param>
		private void ChangeDirection(DirectionEnum dir, bool bypassCheck = false)
		{
			if (dir == CurrentDirection || bypassCheck) return;
			//GD.PushError("Changing order");
			bool NewDirFlippedH = AnimationDefinitionLibrary.AnimationDirectionDefinitionDict[dir].IsFlipH;

			CurrentDirection = dir;
		}

		/// <summary>
		///		Wipes all draw equipment nodes, and clears our internal list.
		///		Effectively removes all items for the animation, preserving the primary animated sprite
		/// </summary>
		public void RemoveCurrentEquipment()
		{
			// cleanup list and nodes 
			Array.Clear(AnimatedEquipmentSprites);
			foreach (Node equip in AnimatedEquipmentNodeContainer.GetChildren())
			{
				RemoveChild(equip);
				equip.QueueFree();
			}
		}
		#endregion


		/// <summary>
		///		Given all current settings, process the current 'animation frame', set the time for the next, etc
		/// </summary>
		private void ProcessCurrentAnimation()
		{
			// No work to do
			if (NextFrameTickTimeS > DeltaClock.DeltaTimeSum || CurrentAnimationDescription.AnimationGlossaryID == AnimationGlossaryEnum.None) return;
			AnimationDirectionDescription currDir = AnimationDefinitionLibrary.AnimationDirectionDefinitionDict[CurrentDirection];

			while (NextFrameTickTimeS < DeltaClock.DeltaTimeSum)
			{
				// Play animation on primary, and then again on equipment
				PrimaryAnimatedSprite.PlayAnimationFrame(CurrentAnimationFrame, currDir);
				foreach (AnimatedSprite equipment in AnimatedEquipmentSprites)
				{
					equipment?.PlayAnimationFrame(CurrentAnimationFrame, currDir);
				}
				// Check for method calls
				ProcessMethodCalls();

				// Attempt to calculate the next frame time if there are altercations to it, otherwise do the default calculation
				int framesPerDir = PrimaryAnimatedSprite.GetFramesPerDirection(CurrentAnimationDescription.AnimationID);
				if (CurrentAnimationDescription.PlaytimeOverrideS > 0 && framesPerDir > 0)
				{
					double diff = (CurrentAnimationDescription.PlaytimeOverrideS / framesPerDir) * CurrentAnimationDescription.SpeedMultiplier;
					if (diff > CurrentAnimationDescription.TimeBetweenFramesS) diff = CurrentAnimationDescription.TimeBetweenFramesS;
					NextFrameTickTimeS += diff;
				}
				else
				{
					NextFrameTickTimeS += CurrentAnimationDescription.TimeBetweenFramesS * CurrentAnimationDescription.SpeedMultiplier;
				}

				CurrentAnimationFrame += 1;
				// If we are at our last frame we need to decide, loop, play next in queue, etc.
				CheckForLastFrame();
			}
		}

		/// <summary>
		///		There is some thought that has to go into how we handle the last frame, and it is easier to pull that work out
		///			into its own method
		///		Decides if we should loop, play the queued animation, etc
		/// </summary>
		public void CheckForLastFrame()
		{
			// if not the last frame, no work to do
			if (CurrentAnimationFrame <= PrimaryAnimatedSprite.CurrentSpriteSheetAnimationSummary.FramesPerDirection - 1) return;

			// We have to decide what to do when we come to the end of the current animation. 
			if (CurrentAnimationDescription.AnimationLoopType == AnimationLoopModeEnum.HoldOnLastFrame)
			{
				CurrentAnimationFrame -= 1;
			}
			else if (QueuedAnimationDescription.AnimationGlossaryID != AnimationGlossaryEnum.None)
			{
				DequeueAnimation();
			}
			else if (CurrentAnimationDescription.AnimationLoopType == AnimationLoopModeEnum.Repeat)
			{
				CurrentAnimationFrame = 0;
			}
			else
			{
				PlayDefaultAnimation();
			}
		}

		/// <summary>
		///		Looks at the current frame of the current animation and tests to see if there any method calls that need to happen
		///		If there is something like this, try to execute it
		/// </summary>
		public void ProcessMethodCalls()
		{
			// frameCountfromEnd is an index starting at the end. -1 being the last element, -2 being the second from last, etc. 
			int frameCountfromEnd = CurrentAnimationFrame - PrimaryAnimatedSprite.CurrentSpriteSheetAnimationSummary.FramesPerDirection;
			foreach (AnimationMethodCallDescription methodCall in CurrentAnimationDescription.MethodCallDescriptions)
			{
				if (CurrentAnimationFrame == methodCall.KeyFrameNumber || frameCountfromEnd == methodCall.KeyFrameNumber)
				{
					try
					{
						if (methodCall.AdditionalSBeforeCall > 0.0)
						{
							// If requested, we should wait to do this call
							Timer tmpTimer = new();
							tmpTimer.WaitTime = methodCall.AdditionalSBeforeCall;
							tmpTimer.Timeout += () => GetNode(methodCall.NodePathToActOn).Call(methodCall.MethodName);
							tmpTimer.Autostart = true;
							tmpTimer.OneShot = true;
							AddChild(tmpTimer);
						}
						else
						{
							// Do the call
							GetNode(methodCall.NodePathToActOn).Call(methodCall.MethodName);
						}
					}
					catch (Exception e)
					{
						GD.PushError($"Unable to call method: [{methodCall.MethodName}] on node: [{methodCall.NodePathToActOn}] -- BWQ2GA0F");
						throw;
					}
				}
			}
		}

		/// <summary>
		///		Sets the current animation to be played.
		///		All actual animation/sprite management is done on _process
		/// </summary>
		/// <param name="aniID">Animation to play</param>
		/// <param name="dir">Direction we should play for</param>
		/// <param name="playtimeOverrideS">How long the overall animation will take to play (Before speed multiplier is applied. Will not expand time beyond default time, but can shrink forever</param>
		/// <param name="speedMultiplier">The time b/w each frame is multiplied by this factor</param>
		/// <param name="queueIfCurrentlyPlaying">If true we will queue the animation when something besides default ani is playing, if false we will begin playing this animation on the next animation frame </param>
		public void SetCurrentAnimation(AnimationGlossaryEnum aniID, DirectionEnum dir, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false, double playtimeOverrideS=0.0, float speedMultiplier = 1.0f)
		{
			// We must be initialized before setting any animation
			if (ResourceUtilityRef is null) return;
			// Nothing to do w/o a real direction. Sanity check. Also avoids us playing the first frame as the default in the default dir, which often appears random
			if (dir == DirectionEnum.None) return;

			// Verify the animation exists for this animation archetype
			if (!AnimationDefinitionLibrary.IsValidAnimationForArchetype(aniID, AnimationArchetype))
			{
				// Likely to do more or less than throw an exception here in the future
				throw new Exception($"Invalid Animation: [{aniID}] for the set archetype: [{AnimationArchetype}].  W61EUE8B");
			}

			// Verify this sheet ID has this animation
			if (!IsValidAnimationForSheet(PrimaryAnimatedSprite.SheetID, aniID))
			{
				// Likely to do more or less than throw an exception here in the future
				throw new Exception($"Invalid Animation: [{aniID}] for the SheetID: [{PrimaryAnimatedSprite.SheetID}]  VPMM1UF2");
			}

			// There is something currently playing besides the default animation
			if (CurrentAnimationDescription.AnimationGlossaryID != AnimationGlossaryEnum.None && aniID != DefaultAnimation)
			{
				// We are already playing this animation, so update direction w/o restting frame #s creates a nice seamless direction change
				if (aniID == CurrentAnimationDescription.AnimationGlossaryID) ChangeDirection(dir);

				// It was requested that we queue this animation if something is currently playing
				if (queueIfCurrentlyPlaying)
				{
					QueueAnimation(aniID, playtimeOverrideS, speedMultiplier);
					// If we're not supposed to also override, we can leave now
					if (!overrideIfPlaying) return;
				}
				// We were not asked to queue, so if we can interrupt, we will
				if (!CurrentAnimationDescription.Interruptable) return;
			}
			
			// Forces the next frame to happen immediately instead of waiting for the last one to finish
			if (overrideIfPlaying && aniID != CurrentAnimationDescription.AnimationGlossaryID) NextFrameTickTimeS = DeltaClock.DeltaTimeSum;

			// Grab current animation and get ready to play it
			AnimationDescription tmp = AnimationDefinitionLibrary.GetAnimationDescription(aniID);
			CurrentAnimationDescription = new AnimationDescription(tmp.AnimationGlossaryID, tmp.AnimationArchetype, tmp.AnimationID, tmp.AnimationLoopType, tmp.MethodCallDescriptions, tmp.TimeBetweenFramesS);
			CurrentAnimationDescription.PlaytimeOverrideS = playtimeOverrideS;
			CurrentAnimationDescription.SpeedMultiplier = speedMultiplier;
			ChangeDirection(dir);
			CurrentAnimationFrame = 0;

			// Set current aniamtion for all AnimatedSprites
			PrimaryAnimatedSprite.SetCurrentAnimation(CurrentAnimationDescription.AnimationID);
			foreach (AnimatedSprite equipment in AnimatedEquipmentSprites)
			{
				equipment?.SetCurrentAnimation(CurrentAnimationDescription.AnimationID);
			}
		}

		/// <summary>
		///		Using the default animation glossary enum, set the current animation.
		///		Allows us to set the animation such as walk, die, or get hit very genericly in the Mobile class while
		///			giving the flexibility to implement more complex logic for its children
		/// </summary>
		/// <param name="aniID"></param>
		/// <param name="dir"></param>
		/// <param name="speedMultiplier"></param>
		/// <param name="queueIfCurrentlyPlaying"></param>
		public void SetCurrentAnimation(DefaultAnimationGlossaryEnum aniID, DirectionEnum dir, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false, double playtimeOverrideS = 0.0, float speedMultiplier = 1.0f)
		{
			AnimationGlossaryEnum aniOut = AnimationGlossaryEnum.None;
			switch (aniID)
			{
				case DefaultAnimationGlossaryEnum.DefaultIdleStillAnimation:
					aniOut = DefaultIdleStillAnimation;
					break;
				case DefaultAnimationGlossaryEnum.DefaultIdleAnimation:
					aniOut = DefaultIdleAnimation;
					break;
				case DefaultAnimationGlossaryEnum.DefaultWalkAnimation:
					aniOut = DefaultWalkAnimation;
					break;
				case DefaultAnimationGlossaryEnum.DefaultAttackAnimation:
					aniOut = DefaultAttackAnimation;
					break;
				case DefaultAnimationGlossaryEnum.DefaultDieAnimation:
					aniOut = DefaultDieAnimation;
					break;
				case DefaultAnimationGlossaryEnum.DefaultGetHit:
					aniOut = DefaultGetHit;
					break;
				default:
					break;
			}
			SetCurrentAnimation(aniOut, dir, queueIfCurrentlyPlaying, overrideIfPlaying, playtimeOverrideS, speedMultiplier);
		}

		/// <summary>
		///		Every animation archetype has a 'default animation' to play on creation, usually idle of some sort. This is defined in our constructor
		/// </summary>
		public void PlayDefaultAnimation()
		{
			SetCurrentAnimation(DefaultAnimation, CurrentDirection);
		}

		/// <summary>
		///		Another Set Current Animation overload intended to make it easy for us to dequeue queued animation descriptions
		///		Because of this intent, it is missing many fields that will be assumed or pulled from the animation description itself
		/// </summary>
		/// <param name="aniDesc">The animation description you want to play</param>
		public void SetCurrentAnimation(AnimationDescription aniDesc, bool queueIfCurrentlyPlaying = true, bool overrideIfPlaying = false)
		{
			SetCurrentAnimation(aniDesc.AnimationGlossaryID, CurrentDirection, queueIfCurrentlyPlaying, overrideIfPlaying, aniDesc.PlaytimeOverrideS, aniDesc.SpeedMultiplier);
		}

		/// <summary>
		///		An interface for easily queuing an animation w/ speed overrides set
		/// </summary>
		/// <param name="aniID">Animation ID To Queue</param>
		/// <param name="PlaytimeOverrideS">How long the overall animation will take to play (Before speed multiplier is applied</param>
		/// <param name="speedMultiplier">The time b/w each frame is multiplied by this factor</param>
		public void QueueAnimation(AnimationGlossaryEnum aniID, double playtimeOverrideS = 0.0, float speedMultiplier = 1.0f)
		{
			AnimationDescription tmp = AnimationDefinitionLibrary.GetAnimationDescription(aniID);
			QueuedAnimationDescription = new AnimationDescription(tmp.AnimationGlossaryID, tmp.AnimationArchetype, tmp.AnimationID, tmp.AnimationLoopType, tmp.MethodCallDescriptions, tmp.TimeBetweenFramesS);
			QueuedAnimationDescription.PlaytimeOverrideS = playtimeOverrideS;
			QueuedAnimationDescription.SpeedMultiplier = speedMultiplier;
		}

		/// <summary>
		///		Sets the current animation to the queued animation and clears the queued animation
		/// </summary>
		public void DequeueAnimation()
		{
			if (QueuedAnimationDescription.AnimationGlossaryID == AnimationGlossaryEnum.None) return;   // no queued animation
			AnimationDescription tmpAniDesc = QueuedAnimationDescription;
			QueuedAnimationDescription = AnimationDefinitionLibrary.GetAnimationDescription(AnimationGlossaryEnum.None);
			SetCurrentAnimation(tmpAniDesc, queueIfCurrentlyPlaying: false, overrideIfPlaying:true);
		}

		/// <summary>
		///		Searches the sprite dictionary to see if the requested sprite sheet contains the requested animation
		/// </summary>
		/// <param name="sheetID">The sprite sheet ID to test</param>
		/// <param name="animationID">The Animation to look for</param>
		/// <returns>true if this animation exists for this sprite sheet </returns>
		public bool IsValidAnimationForSheet(int sheetID, AnimationGlossaryEnum animationID)
		{
			return ResourceUtilityRef.SpriteSheetMetaDataDict[sheetID].AnimationSummaries.ContainsKey(AnimationDefinitionLibrary.GetAnimationDescription(animationID).AnimationID);
		}

		#region Animation Details API
		public Vector2 GetCurrentSpriteCenter()
		{
			FrameSummary frameSummary = GetCurrentSpriteSummary();
			return new Vector2(0.0f, -(frameSummary.Height - frameSummary.OffsetY) / 2.0f);
		}

		public Vector2 GetCurrentSpriteTop()
		{
			FrameSummary frameSummary = GetCurrentSpriteSummary();
			return new Vector2(0, -(frameSummary.Height - frameSummary.OffsetY) / 2.0f);
		}

		public FrameSummary GetCurrentSpriteSummary()
		{
			return PrimaryAnimatedSprite.GetCurrentFrameSummary();
		}
		#endregion

	}
}
