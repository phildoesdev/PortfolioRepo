using Godot;

namespace UODef.GUI;

public partial class MainMenu : CanvasLayer
{

    private TextureButton PlayButton;
    private TextureButton OptionsButton;
    private TextureButton ExitButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		PlayButton = GetNode<TextureButton>("Menu/Buttons/PlayButton");
		OptionsButton = GetNode<TextureButton>("Menu/Buttons/OptionsButton");
		ExitButton = GetNode<TextureButton>("Menu/Buttons/ExitButton");


		PlayButton.Pressed += PlayButtonPressed;
		OptionsButton.Pressed += OptionsbuttonPressed;
		ExitButton.Pressed += ExitbuttonPressed;
	}

    private void PlayButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://src/GUI/Screens/ChooseLevel/ChooseLevel.tscn");
	}

    private void OptionsbuttonPressed()
    {
    }

    private void ExitbuttonPressed()
    {
    }
}
