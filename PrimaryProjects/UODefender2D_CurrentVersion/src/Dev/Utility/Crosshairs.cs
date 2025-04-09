using Godot;
using System;

public partial class Crosshairs : Control
{
    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 4, Colors.DimGray);
        DrawCircle(Vector2.Zero, 3, Colors.White);

        // Right
        DrawLine(new Vector2(16, 0), new Vector2(32, 0), Colors.White, 2);
        // Left
        DrawLine(new Vector2(-16, 0), new Vector2(-32, 0), Colors.White, 2);
        // Top
        DrawLine(new Vector2(0, 16), new Vector2(0, 32), Colors.White, 2);
        // Bottom
        DrawLine(new Vector2(0, -16), new Vector2(0, -32), Colors.White, 2);

    }
}
