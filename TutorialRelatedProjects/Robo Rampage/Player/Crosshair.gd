extends Control

# We are going to use code to create the simple shapes we need!!!!!
func _draw() -> void:
    draw_circle(Vector2.ZERO, 4, Color.DIM_GRAY)
    draw_circle(Vector2.ZERO, 3, Color.WHITE)
    
    # Right
    draw_line(Vector2(16, 0), Vector2(32, 0), Color.WHITE, 2)
    # Left
    draw_line(Vector2(-16, 0), Vector2(-32, 0), Color.WHITE, 2)
    # Top 
    draw_line(Vector2(0, 16), Vector2(0, 32), Color.WHITE, 2)
    # Bottom
    draw_line(Vector2(0, -16), Vector2(0, -32), Color.WHITE, 2)
