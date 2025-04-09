extends LevelParent


func _on_gate_player_entered_gate(_body):
    var tween = get_tree().create_tween()
    tween.tween_property($player, "speed", 0, 0.1)
    TransitionLayer.change_scene("res://scenes/levels/inside.tscn")

func _on_house_player_entered():
    var tween = get_tree().create_tween()
    # tween.set_parallel(true)
    tween.tween_property($player/Camera2D, "zoom", Vector2(1, 1), 1 )
    # tween.tween_property($player, "modulate:a", 0, 3).from(0.5)

func _on_house_player_departed():
    var tween = get_tree().create_tween()
    tween.tween_property($player/Camera2D, "zoom", Vector2(0.6, 0.6), 1 )
    # tween.tween_property($player, "modulate:a", 1, 3 )
