extends StaticBody2D
class_name ItemContainer

var item_name: String = "Base"
var is_open: bool = false
@onready var current_direction: Vector2 = Vector2.DOWN.rotated(rotation)

signal spawn_items(pos_out, current_direction)

func hit(modifier=1):
    if not is_open:
        $AudioStreamPlayer2D.play()
        is_open = true
        $LidSprite.hide()
        for x in range(modifier):
            var pos_out = $SpawnPositions.get_child(randi()% $SpawnPositions.get_child_count()).global_position
            spawn_items.emit(pos_out, current_direction)
