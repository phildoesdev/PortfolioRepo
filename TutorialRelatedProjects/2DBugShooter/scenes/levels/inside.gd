extends LevelParent

func _on_exit_gate_area_body_entered(_body):
    var tween = get_tree().create_tween()
    tween.tween_property($player, "speed", 1, 0.1)
    # get_tree().change_scene_to_file("res://scenes/levels/outside.tscn")
    TransitionLayer.change_scene("res://scenes/levels/outside.tscn")
