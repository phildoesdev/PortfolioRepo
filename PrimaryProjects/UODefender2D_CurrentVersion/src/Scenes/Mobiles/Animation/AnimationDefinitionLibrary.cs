
using System.Collections.Generic;
using UODef.System;

namespace UODef.Mobiles.AnimationPlayer
{
	#region Animation Archtype Enums
	/// <summary>
	/// Defines the greater animation type for this mobile- determines which AnimationResource we load in
	/// </summary>
	public enum AnimationArchetypeEnum
	{
		None,
		Monster,
		Human,
		Animal,
	}

	/// <summary>
	///		Some animations that are common across all current animation arche types.
	///		This allows us to broadly set the base cases for things like death, get hit, and walk while allowing
	///			allowing the freedome of distinct animation arche types...
	/// </summary>
	public enum DefaultAnimationGlossaryEnum
	{
		DefaultIdleStillAnimation,
		DefaultIdleAnimation,
		DefaultWalkAnimation,
		DefaultAttackAnimation,
		DefaultDieAnimation,
		DefaultGetHit,
	}

	/// <summary>
	///     A comprehnesive list of all animation names for sake of being able to consistently pass some kind of enum around. 
	///     The order of the entries should match the order of the animationArchetypeEnum
	/// </summary>
	public enum AnimationGlossaryEnum
	{
		None,
		M_walk,
		M_idleStill,
		M_die1,
		M_die2,
		M_attack1,
		M_attack2,
		M_attack3,
		M_attackBow,
		M_attackCrossbow,
		M_attackThrow,
		M_getHit,
		M_pillage,
		M_stomp,
		M_cast2,
		M_cast3,
		M_blockRight,
		M_blockLeft,
		M_idle,
		M_fidget,
		M_fly,
		M_takeOff,
		M_getHitInAir,
		H_walk,
		H_walkStaff,
		H_run,
		H_runStaff,
		H_idleStill,
		H_idle,
		H_fidget,
		H_combatIdle_1h,
		H_combatIdleAlert_1h,
		H_attackSlash_1h,
		H_attackPierce_1h,
		H_attackBash_1h,
		H_attackBash_2h,
		H_attackSlash_2h,
		H_attackPierce_2h,
		H_combatAdvance_1h,
		H_spell,
		H_spellSummon,
		H_attackBow,
		H_attackCrossbow,
		H_getHit_fr_hi,
		H_dieHard_fwd,
		H_dieHard_back,
		H_mountWalk,
		H_mountRun,
		H_mountIdleStill,
		H_mountAttackSlashRight_1h,
		H_mountAttackBow,
		H_mountAttackCrossbow,
		H_mountAttackSlashRight_2h,
		H_blockShieldHard,
		H_punch,
		H_bowDown,
		H_saluteArmed,
		H_ingestEat,
		A_walk,
		A_run,
		A_idleStill,
		A_eat,
		A_alert,
		A_attack1,
		A_attack2,
		A_getHit,
		A_die1,
		A_idle,
		A_fidget,
		A_lieDown,
		A_die2,
	}

	// These ID enums give us the animationID for each animation w/in the archetype. Essentially what maps to the outside animationid reference 
	public enum MonsterAnimationIDEnum
	{
		walk,
		idleStill,
		die1,
		die2,
		attack1,
		attack2,
		attack3,
		attackBow,
		attackCrossbow,
		attackThrow,
		getHit,
		pillage,
		stomp,
		cast2,
		cast3,
		blockRight,
		blockLeft,
		idle,
		fidget,
		fly,
		takeOff,
		getHitInAir,
	}

	public enum HumanAnimationIDEnum
	{
		walk,
		walkStaff,
		run,
		runStaff,
		idleStill,
		idle,
		fidget,
		combatIdle_1h,
		combatIdleAlert_1h,
		attackSlash_1h,
		attackPierce_1h,
		attackBash_1h,
		attackBash_2h,
		attackSlash_2h,
		attackPierce_2h,
		combatAdvance_1h,
		spell,
		spellSummon,
		attackBow,
		attackCrossbow,
		getHit_fr_hi,
		dieHard_fwd,
		dieHard_back,
		mountWalk,
		mountRun,
		mountIdleStill,
		mountAttackSlashRight_1h,
		mountAttackBow,
		mountAttackCrossbow,
		mountAttackSlashRight_2h,
		blockShieldHard,
		punch,
		bowDown,
		saluteArmed,
		ingestEat,
	}

	public enum AnimalAnimationIDEnum
	{
		walk,
		run,
		idleStill,
		eat,
		alert,
		attack1,
		attack2,
		getHit,
		die1,
		idle,
		fidget,
		lieDown,
		die2,
	}

	#endregion

	#region Supporting Classes

	/// <summary>
	///		Used to map the cardinal directions relative to each other for purposes of animating sprite sheets
	/// </summary>
	public class AnimationDirectionDescription
	{
		public string Name { get; set; }
		/// <summary>
		///		The relatively starting position of this direction in the sprite sheets. 
		///		Allows us to do necessary math
		/// </summary>
		public int RelativeStartingPosition { get; set; }
		/// <summary>
		///		Some directions are just other directions flipped and need to know this for necessary math
		/// </summary>
		public bool IsFlipH { get; set; }

		public AnimationDirectionDescription(string name, int relStartingPos, bool flipH = false)
		{
			Name = name;
			RelativeStartingPosition = relStartingPos;
			IsFlipH = flipH;
		}
	}

	/// <summary>
	///		Allows us to call methods as the animation plays- useful for things like contact frames of attacks
	/// </summary>
	public class AnimationMethodCallDescription
	{
		public string MethodName { get; set; }
		public string NodePathToActOn { get; set; }
		/// <summary>
		///     The frame number (relative to the animations) at which we'll place this keyframe
		///     Frames numbers go like: 0 (start) and -1 (end).
		/// </summary>
		public int KeyFrameNumber { get; set; }
		/// <summary>
		///		Allows us to add extra time before our method call
		/// </summary>
		public double AdditionalSBeforeCall { get; set; }

		public AnimationMethodCallDescription(string methodName, string nodePathToActOn, int keyFrameNumber, double additionalSBeforeCall=0.0)
		{
			MethodName = methodName;
			NodePathToActOn = nodePathToActOn;
			KeyFrameNumber = keyFrameNumber;
			AdditionalSBeforeCall = additionalSBeforeCall;
		}
	}

	/// <summary>
	///		Used as an overarching description for all animations, when created will contain all req'd information to animate a generic
	///			sprite sheet of this animation arche type, for this animation ID
	/// </summary>
	public class AnimationDescription
	{
		public AnimationGlossaryEnum AnimationGlossaryID { get; set; }
		public AnimationArchetypeEnum AnimationArchetype { get; set; }
		/// <summary>
		///		The animation id for the animation in reference to the specific arche type
		/// </summary>
		public int AnimationID { get; set; }
		public AnimationLoopModeEnum AnimationLoopType { get; set; }
		public double TimeBetweenFramesS { get; set; } = 0.25;
		public List<AnimationMethodCallDescription> MethodCallDescriptions { get; set; }

		/// <summary>
		///		TimeBetweenFramesMS will be multiplied by 'SpeedMultiplier' when animating
		/// </summary>
		public float SpeedMultiplier { get; set; } = 1.0f;
		/// <summary>
		///		Alters the time between frame ms on calculation and forces the animations total playtime to be this value. 
		///		0 means no Playtime Override
		///		The SpeedMultiplier will be applied to the calculated TimeBetweenFramesMS 
		///			(SpriteSheetMeta.TotalFrames/PlaytimeOverrideS)*SpeedMultiplier == Time b/w frames
		/// </summary>
		public double PlaytimeOverrideS { get; set; } = 0.0;
		/// <summary>
		///		Some animations, like death, should be uninteruptable because of their method calls and meaning 
		/// </summary>
		public bool Interruptable { get; set; } = true;

		public AnimationDescription(AnimationGlossaryEnum animationGlossaryID, AnimationArchetypeEnum animationArchetype, int animationID, AnimationLoopModeEnum animationLoopType, List<AnimationMethodCallDescription> methodCallDescriptions, double timeBetweenFramesS = 0.25)
		{
			AnimationGlossaryID = animationGlossaryID;
			AnimationArchetype = animationArchetype;
			AnimationID = animationID;
			AnimationLoopType = animationLoopType;
			TimeBetweenFramesS = timeBetweenFramesS;
			MethodCallDescriptions = methodCallDescriptions;
		}
	}
	#endregion


	/// <summary>
	///		The static class referenced by my animation player. This class holds the definitions of all animations that are possible to play
	///			through my animation player
	/// </summary>
	public static class AnimationDefinitionLibrary
	{

		public static readonly Dictionary<DirectionEnum, AnimationDirectionDescription> AnimationDirectionDefinitionDict;
		public static readonly Dictionary<AnimationGlossaryEnum, AnimationDescription> AnimationDefinitionDict;

		static AnimationDefinitionLibrary()
		{
			AnimationDirectionDefinitionDict = new();
			AnimationDefinitionDict = new();

			#region Animation Direction Definitions
			AnimationDirectionDefinitionDict[DirectionEnum.South] = new AnimationDirectionDescription(name: "South", relStartingPos: 0, flipH: false);
			AnimationDirectionDefinitionDict[DirectionEnum.SouthWest] = new AnimationDirectionDescription(name: "SouthWest", relStartingPos: 1, flipH: false);
			AnimationDirectionDefinitionDict[DirectionEnum.West] = new AnimationDirectionDescription(name: "West", relStartingPos: 2, flipH: false);
			AnimationDirectionDefinitionDict[DirectionEnum.NorthWest] = new AnimationDirectionDescription(name: "NorthWest", relStartingPos: 3, flipH: false);
			AnimationDirectionDefinitionDict[DirectionEnum.North] = new AnimationDirectionDescription(name: "North", relStartingPos: 4, flipH: false);
			AnimationDirectionDefinitionDict[DirectionEnum.NorthEast] = new AnimationDirectionDescription(name: "NorthEast", relStartingPos: 3, flipH: true);
			AnimationDirectionDefinitionDict[DirectionEnum.East] = new AnimationDirectionDescription(name: "East", relStartingPos: 2, flipH: true);
			AnimationDirectionDefinitionDict[DirectionEnum.SouthEast] = new AnimationDirectionDescription(name: "SouthEast", relStartingPos: 1, flipH: true);
			#endregion

			// A placeholder animation definition useful for defaults
			AnimationDefinitionDict[AnimationGlossaryEnum.None] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.None,
				animationArchetype: AnimationArchetypeEnum.None,
				animationID: -1,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);

			#region Monster Animation Definitions
			AnimationDefinitionDict[AnimationGlossaryEnum.M_walk] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_walk,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.walk,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_idleStill] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_idleStill,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.idleStill,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_die1] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_die1,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.die1,
				animationLoopType: AnimationLoopModeEnum.HoldOnLastFrame,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "Die",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 10.25
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_die2] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_die2,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.die2,
				animationLoopType: AnimationLoopModeEnum.HoldOnLastFrame,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "Die",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 10.25
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_attack1] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_attack1,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.attack1,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_attack2] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_attack2,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.attack2,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_attack3] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_attack3,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.attack3,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_attackBow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_attackBow,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.attackBow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_attackCrossbow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_attackCrossbow,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.attackCrossbow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_attackThrow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_attackThrow,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.attackThrow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_getHit] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_getHit,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.getHit,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_pillage] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_pillage,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.pillage,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_stomp] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_stomp,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.stomp,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_cast2] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_cast2,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.cast2,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_cast3] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_cast3,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.cast3,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_blockRight] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_blockRight,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.blockRight,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_blockLeft] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_blockLeft,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.blockLeft,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_idle] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_idle,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.idle,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_fidget] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_fidget,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.fidget,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_fly] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_fly,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.fly,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_takeOff] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_takeOff,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.takeOff,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.M_getHitInAir] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.M_getHitInAir,
				animationArchetype: AnimationArchetypeEnum.Monster,
				animationID: (int)MonsterAnimationIDEnum.getHitInAir,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			#endregion

			#region Human Animation Definitions
			AnimationDefinitionDict[AnimationGlossaryEnum.H_walk] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_walk,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.walk,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_walkStaff] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_walkStaff,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.walkStaff,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_run] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_run,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.run,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_runStaff] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_runStaff,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.runStaff,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_idleStill] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_idleStill,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.idleStill,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_idle] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_idle,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.idle,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_fidget] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_fidget,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.fidget,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_combatIdle_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_combatIdle_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.combatIdle_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_combatIdleAlert_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_combatIdleAlert_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.combatIdleAlert_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackSlash_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackSlash_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackSlash_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackPierce_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackPierce_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackPierce_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackBash_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackBash_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackBash_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackBash_2h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackBash_2h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackBash_2h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackSlash_2h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackSlash_2h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackSlash_2h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackPierce_2h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackPierce_2h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackPierce_2h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_combatAdvance_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_combatAdvance_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.combatAdvance_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_spell] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_spell,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.spell,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_spellSummon] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_spellSummon,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.spellSummon,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackBow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackBow,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackBow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_attackCrossbow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_attackCrossbow,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.attackCrossbow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_getHit_fr_hi] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_getHit_fr_hi,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.getHit_fr_hi,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_dieHard_fwd] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_dieHard_fwd,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.dieHard_fwd,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "Die",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 10.25
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_dieHard_back] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_dieHard_back,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.dieHard_back,
				animationLoopType: AnimationLoopModeEnum.HoldOnLastFrame,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "Die",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 10.25
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountWalk] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountWalk,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountWalk,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountRun] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountRun,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountRun,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountIdleStill] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountIdleStill,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountIdleStill,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountAttackSlashRight_1h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountAttackSlashRight_1h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountAttackSlashRight_1h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountAttackBow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountAttackBow,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountAttackBow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountAttackCrossbow] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountAttackCrossbow,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountAttackCrossbow,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_mountAttackSlashRight_2h] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_mountAttackSlashRight_2h,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.mountAttackSlashRight_2h,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_blockShieldHard] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_blockShieldHard,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.blockShieldHard,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_punch] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_punch,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.punch,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_bowDown] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_bowDown,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.bowDown,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_saluteArmed] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_saluteArmed,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.saluteArmed,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.H_ingestEat] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.H_ingestEat,
				animationArchetype: AnimationArchetypeEnum.Human,
				animationID: (int)HumanAnimationIDEnum.ingestEat,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			#endregion

			#region Animal Animation Defintions
			AnimationDefinitionDict[AnimationGlossaryEnum.A_walk] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_walk,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.walk,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_run] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_run,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.run,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_idleStill] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_idleStill,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.idleStill,
				animationLoopType: AnimationLoopModeEnum.Repeat,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_eat] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_eat,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.eat,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_alert] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_alert,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.alert,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_attack1] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_attack1,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.attack1,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_attack2] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_attack2,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.attack2,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationFinished",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 0.25
						),
					new AnimationMethodCallDescription(
						methodName: "OnAttackAnimationMakeContact",
						nodePathToActOn: "..",
						keyFrameNumber: -3
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_getHit] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_getHit,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.getHit,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_die1] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_die1,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.die1,
				animationLoopType: AnimationLoopModeEnum.HoldOnLastFrame,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "Die",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 10.25
						)
				}
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_idle] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_idle,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.idle,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_fidget] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_fidget,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.fidget,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_lieDown] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_lieDown,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.lieDown,
				animationLoopType: AnimationLoopModeEnum.None,
				methodCallDescriptions: new List<AnimationMethodCallDescription> { }
				);
			AnimationDefinitionDict[AnimationGlossaryEnum.A_die2] = new AnimationDescription(
				animationGlossaryID: AnimationGlossaryEnum.A_die2,
				animationArchetype: AnimationArchetypeEnum.Animal,
				animationID: (int)AnimalAnimationIDEnum.die2,
				animationLoopType: AnimationLoopModeEnum.HoldOnLastFrame,
				methodCallDescriptions: new List<AnimationMethodCallDescription> {
					new AnimationMethodCallDescription(
						methodName: "Die",
						nodePathToActOn: "..",
						keyFrameNumber: -1,
						additionalSBeforeCall: 10.25
						)
				}
				);
			#endregion
		}

		public static AnimationDescription GetAnimationDescription(AnimationGlossaryEnum animationID)
		{
			return AnimationDefinitionDict[animationID];
		}

		/// <summary>
		///		Tests to see if the requested animation is within the animation archetype
		/// </summary>
		/// <param name="aniID"></param>
		/// <param name="aniArchetype"></param>
		/// <returns></returns>
		public static bool IsValidAnimationForArchetype(AnimationGlossaryEnum aniID, AnimationArchetypeEnum aniArchetype)
		{
			if (AnimationDefinitionDict.TryGetValue(aniID, out AnimationDescription animationDescrpition))
			{
				return animationDescrpition.AnimationArchetype == aniArchetype;
			}
			return false;
		}

	}
}
