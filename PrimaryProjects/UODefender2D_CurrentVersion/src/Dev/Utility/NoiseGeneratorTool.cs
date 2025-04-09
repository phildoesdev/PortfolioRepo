using Godot;


public partial class NoiseGeneratorTool : CanvasLayer
{

    private Button SaveButton;
    private ViewportTexture ViewportTxt;

    [Export]
    public string SaveLoc { get; set; } = "res://src/Dev/Utility/NoiseTextureSnapshotFolder/";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ViewportTxt = GetNode<SubViewport>("Container/NoiseViewport").GetTexture();

		SaveButton = GetNode<Button>("Container/Buttons/Save");
        SaveButton.Pressed += ExportViewportAsImage;
	}

    private void ExportViewportAsImage()
    {
        string fileName = "NoiseShot_"+Time.GetDatetimeStringFromSystem() + ".png";
		fileName = fileName.Replace(":", "_");
		Image img = ViewportTxt.GetImage();
        GD.PushError($"Save To: {SaveLoc + fileName}");
        img.SavePng(SaveLoc + fileName);

	}
}
