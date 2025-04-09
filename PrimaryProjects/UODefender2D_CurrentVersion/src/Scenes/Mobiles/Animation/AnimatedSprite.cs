using Godot;
using System.Collections.Generic;
using UODef.System.ResourceUtility;
using UODef.Mobiles.AnimationPlayer;

namespace UODef.Mobiles.AnimationPlayer;

public partial class AnimatedSprite : Sprite2D
{
	public int SheetID { get; set; }
	public AnimationArchetypeEnum AnimationArchetype { get; set; }

	// Current hue related data
	public int HueID { get; set; }
	public bool IsHued { get; set; } = false;
	public bool OnlyHueGreyPixels { get; set; } = false;
	public DrawLayerEnum DrawLayer { get; set; }
	public List<Vector3> ColorGradient;
	private string HueShaderPath = "res://src/Assets/Shaders/HueShader.gdshader";	// Maybe not the perfect place for this path, but it'll do for now

	// Our copy so we dont have to keep pinging
	public SpriteSheetMeta SpriteSheetMetaInfo { get; private set; }

	/// <summary>
	///		Holds information on the current animation for this specific sprite sheet.
	///		Gets set when the animation gets set
	/// </summary>
	public AnimationSummary CurrentSpriteSheetAnimationSummary { get; private set; }

	public AnimatedSprite(int sheetID, int hueID, AnimationArchetypeEnum animationArchetype, bool onlyHueGreyPixels = false, DrawLayerEnum drawLayer = DrawLayerEnum.None) : base()
	{
		SheetID = sheetID;
		HueID = hueID;
		OnlyHueGreyPixels = onlyHueGreyPixels;
		DrawLayer = drawLayer;
		if (HueID != 0) IsHued = true;	// Not everything will have a hue, so no reason to do that work
		AnimationArchetype = animationArchetype;
	}

	#region Godot Overrides
	public override void _Ready()
	{
		// Grab sprite sheet meta dta so that we know how to animate our sprite (correlates animation IDs to frame #s and offsets)
		ResourceUtility ResourceUtilityRef = GetNode<ResourceUtility>("/root/ResourceUtility");
		SpriteSheetMetaInfo = ResourceUtilityRef.GetSpriteSheetMeta(SheetID);

		// Grab our texture
		string SpriteTexturePath = ResourceUtilityRef.SpriteSheetMetaDirPath + SpriteSheetMetaInfo.SheetName;
		Texture = GD.Load<Texture2D>(SpriteTexturePath);

		TextureFilter = TextureFilterEnum.Nearest;
		TextureRepeat = TextureRepeatEnum.Disabled;

		YSortEnabled = true;	
		Vframes = SpriteSheetMetaInfo.Rows;
		Hframes = SpriteSheetMetaInfo.Columns;
		Centered = false;

		// Prevents us from being drawn initially w/ no offset
		Position = SpriteSheetMetaInfo.GetFrameOffset(0, false);

		// Hue's the gear/sprites
		InitializeHueShader(ResourceUtilityRef.GetHueMeta(HueID));
	}
	#endregion

	#region Intialization Methods
	/// <summary>
	///		Loads the sahder resoruce and sets the params it requires to work
	/// </summary>
	/// <param name="hue"></param>
	private void InitializeHueShader(HueMeta hue)
	{
		// This is a little dumb maybe, but for now just attach no shader and do no work if there is no hueid
		if (!IsHued) return;
		ShaderMaterial MyMaterial = new ShaderMaterial();
		MyMaterial.Shader = GD.Load<Shader>(HueShaderPath);
		MyMaterial.SetShaderParameter("OnlyHueGreyPixels", OnlyHueGreyPixels);
		MyMaterial.SetShaderParameter("IsActive", IsHued);
		// Deep copy seems to be req'd because the shader somehow manipulates and clears the passed in array after some time
		MyMaterial.SetShaderParameter("ColorGradient", hue.ColorGradient.Duplicate(deep: true));
		Material = MyMaterial;
	}
	#endregion

	#region Utility Methods
	/// <summary>
	///		Given an animation ID (relevant to the sprite sheets animation arche type, not glossary enum id) return
	///			the number of frames per direction.
	/// </summary>
	/// <param name="animationID">Animation ID related to this sprite sheets animation id enum</param>
	/// <returns></returns>
	public int GetFramesPerDirection(int animationID)
	{
		return SpriteSheetMetaInfo.GetAnimationSummaryByID(animationID).FramesPerDirection;
	}

	/// <summary>
	///		Get the frame summary (w/h, etc) for the current playing animation
	/// </summary>
	/// <returns>Frame summary object</returns>
	public FrameSummary GetCurrentFrameSummary()
	{
		return SpriteSheetMetaInfo.GetFrameSummary(Frame);
	}
	#endregion

	/// <summary>
	///		Given the frame # and the direction passed in, calculate where in our sprite sheet we should be at,
	///			and set offets and such
	/// </summary>
	/// <param name="currentAnimationFrame"></param>
	/// <param name="currDir"></param>
	public void PlayAnimationFrame(int currentAnimationFrame, AnimationDirectionDescription currDir)
	{
		// Calculate frame # for this direction and considering our current progression into the animation
		int currSSFrame = CurrentSpriteSheetAnimationSummary.StartingFrame + currDir.RelativeStartingPosition * CurrentSpriteSheetAnimationSummary.FramesPerDirection + currentAnimationFrame;
		Frame = currSSFrame;
		// Certain directions are just flips of other directions. AnimationPlayer defines this more concretely
		FlipH = currDir.IsFlipH;
		// The sprite sheet meta calculates the current frame's center and places it at this node's 0,0 
		Position = SpriteSheetMetaInfo.GetFrameOffset(currSSFrame, currDir.IsFlipH);
	}

	/// <summary>
	///		Do the work of changing the animation.
	///		This incldues resetting frame counts and the such
	/// </summary>
	/// <param name="animationID"> Integer value that correlates to the animation id for this animation archetype</param>
	public void SetCurrentAnimation(int animationID)
	{
		CurrentSpriteSheetAnimationSummary = SpriteSheetMetaInfo.GetAnimationSummaryByID(animationID);
	}
}
