extends Path3D

var spawn_path_groups = ["spawn_paths"]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    for _group in spawn_path_groups:
        add_to_group(_group)
