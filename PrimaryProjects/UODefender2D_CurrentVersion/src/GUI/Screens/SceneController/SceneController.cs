using Godot;
using System;

public enum SceneControllerFadeTypeEnum
{
    None = 0,
    FadeInOut = 1
}

public partial class SceneController : Node
{
    // The color rect that we use to fade into and out of black
    private ColorRect BlackFaderColorRect;
    private AnimationPlayer EffectsPlayer;
    

    public override void _Ready()
    {
        BlackFaderColorRect = GetNode<ColorRect>("FadeColor");
		EffectsPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Initialize to not visible to be certain
		BlackFaderColorRect.Visible = false;
	}

}
