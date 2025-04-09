class_name EnemyBody
extends StaticBody3D


@export var starting_health: int = 50
var current_health: int = 0

@onready var skeleton_path_3d: EnemyPathFollow3D = $".."

func _ready() -> void:
    current_health = starting_health
    set_process(false)

func take_dmg(amt: int) -> void:
    var new_health: int = current_health - amt
    if new_health < 0:
        new_health = 0
    current_health = new_health
    if new_health <= 0:
        skeleton_path_3d.die(true)
