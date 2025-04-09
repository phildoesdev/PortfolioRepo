using Godot;
using System;

public partial class ChooseLevel : CanvasLayer
{
    private TextureRect LevelSnapshotImg;
	private TextureButton PreviousLevelButton;
    private TextureButton NextLevelButton;
    private TextureButton PlayButton;
    private TextureButton BackButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LevelSnapshotImg = GetNode<TextureRect>("Content/ChooseALevel/Headstone/LevelSnapshotImg");
        PreviousLevelButton = GetNode<TextureButton>("Content/ChooseALevel/Headstone/HeadstoneDisplaySection/BackBtn");
        NextLevelButton = GetNode<TextureButton>("Content/ChooseALevel/Headstone/HeadstoneDisplaySection/ForwardBtn");
        PlayButton = GetNode<TextureButton>("Content/ChooseALevel/Buttons/Play");
		BackButton = GetNode<TextureButton>("Content/ChooseALevel/Buttons/Back");

		PreviousLevelButton.Pressed += PreviousLevelButtonPressed;
		NextLevelButton.Pressed += NextLevelButtonPressed;
		PlayButton.Pressed += PlayButtonPressed;
        BackButton.Pressed += BackButtonPressed;
	}

    private void PreviousLevelButtonPressed()
    {
    }

    private void NextLevelButtonPressed()
    {
    }

    private void PlayButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://src/Scenes/Levels/CastleCourtYard/CastleCourtYard.tscn");
	}

    private void BackButtonPressed()
    {
    }



}
