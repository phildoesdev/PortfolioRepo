using System;
using Godot;


namespace UODef.Player.Viewport
{
	/// <summary>
	///		This class will require a lot of rework, but I am making it work as is, but with a new, cleaner structure
	/// </summary>
	internal partial class PlayerViewport : Node2D
	{

		#region GUI Related props
		private CanvasLayer PlayerCanvasLayer;
		#endregion

		public Camera2D Camera { get; set; }

		//public Vector2 StartingPosition { get; set; } = new Vector2(4823, -5622);
		public Vector2 StartingPosition { get; set; } = new Vector2(334, -5063);

		public float MoveSpeed { get; set; } = 0.5f * 750.0f;
		public float ZoomStep { get; set; } = 0.05f;
		public float ZoomMax { get; set; } = 2.0f;
		public float ZoomMin { get; set; } = 0.1f;


		public PlayerViewport() : base()
		{
		}

		#region Godot Overrdie Methods
		public override void _Ready()
		{
			InitializeCamera();
			InitializePlayerCanvasLayer();
		}

		public override void _Process(double delta)
		{
			ProcessViewportMovement(delta);
		}
		#endregion

		#region Initialization Methods
		public void InitializeCamera()
		{
			Camera = new Camera2D();
			Camera.Position = StartingPosition;
			AddChild(Camera);
		}

		public void InitializePlayerCanvasLayer()
		{
			PlayerCanvasLayer = new CanvasLayer();
			AddChild(PlayerCanvasLayer);
		}
		#endregion

		#region GUI # CanvasLayer Related Methods
		/// <summary>
		///		Set default settings for our control nodes and add them to our player's canvas layer
		/// </summary>
		/// <param name="cntrlNode"></param>
		public void DisplayModal(Control cntrlNode)
		{
			cntrlNode.TextureFilter = TextureFilterEnum.Nearest;
			PlayerCanvasLayer.AddChild(cntrlNode);
		}
		#endregion

		#region Camera Methods
		/// <summary>
		///		Processes user input and translates that into camera mvmt
		///		We actually move the entire viewport so that we can drag things like GUI with us. I think this is necessary
		/// </summary>
		public void ProcessViewportMovement(double delta)
		{
			// Calculates changes in camera motion based player interaction
			Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
			if (inputDir != Vector2.Zero)
			{
				Vector2 dir = inputDir.Normalized();
				Position += new Vector2(dir.X * MoveSpeed * (float)delta, dir.Y * MoveSpeed * (float)delta);
			}
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			// Camera Zoom... Will eventually do the math to make this smoother. The exponential nature of the zoom is annoying as a user and lerp doesnt quite solve it.
			if (@event.IsActionPressed("ScrollUp"))
			{
				if (Camera != null)
				{
					Camera.Zoom = Camera.Zoom with { X = Mathf.Lerp(Camera.Zoom.X, ZoomMax, ZoomStep), Y = Mathf.Lerp(Camera.Zoom.Y, ZoomMax, ZoomStep) };
				}
			}
			if (@event.IsActionPressed("ScrollDown"))
			{
				if (Camera != null)
				{
					Camera.Zoom = Camera.Zoom with { X = Mathf.Lerp(Camera.Zoom.X, ZoomMin, ZoomStep), Y = Mathf.Lerp(Camera.Zoom.Y, ZoomMin, ZoomStep) };
				}
			}
		}
		#endregion
	}
}
