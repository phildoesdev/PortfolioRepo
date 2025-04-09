extends Camera3D

@export var speed := 44.0
@onready var camera_pivot: Node3D = $".."

func _physics_process(delta: float) -> void:
    var weight = clamp(delta * speed, 0.0, 1.0)
    global_transform = global_transform.interpolate_with(camera_pivot.global_transform, weight)
    global_position = camera_pivot.global_position
