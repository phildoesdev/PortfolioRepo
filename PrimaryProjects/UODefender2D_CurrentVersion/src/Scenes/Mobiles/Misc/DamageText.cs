using Godot;
using System;

/// <summary>
///		Above the head dmg labels to give visual feedback to users
/// </summary>
public partial class DamageText : Marker2D
{
	private Label DmgLabel;

	public DamageText() : base()
	{
	}

	public override void _Ready()
	{
		foreach (Label child in GetChildren())
		{
			DmgLabel = child;
		}
		ulong curtime = Time.GetTicksUsec();
		float curSign = -1.0f;

		if (curtime % 2 == 0)
		{
			curSign = 1.0f;
		}

		// Varied position for multiple numbers to show
		Position += new Vector2(curSign * (curtime % 25.0f), -(curtime % 25.0f));
	}

	public void SetAmt(float dmgAmt, Color fontColor, Vector2 startingPos)
	{
		DmgLabel.Text = "-" + dmgAmt.ToString();
		DmgLabel.AddThemeColorOverride("font_color", fontColor);

		Position += startingPos;
		Vector2 finalPos = Position + new Vector2(0.0f, (float)Math.Clamp((Time.GetTicksUsec() % 155.0), 50.0, 125.0));

		Tween labelAnimationTween = CreateTween();
		labelAnimationTween.SetTrans(Tween.TransitionType.Sine);
		labelAnimationTween.TweenProperty(this, "position", finalPos, 1.25f);
		labelAnimationTween.TweenCallback(Callable.From(QueueFree));

	}
}
