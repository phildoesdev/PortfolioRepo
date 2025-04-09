class_name EnemyPathFollow3D
extends PathFollow3D

@export var walk_speed: float = 3.0
@export var dmg_out: int = 1
@export var point_value: int = 10

signal dmg_base_signal(amt)
signal enemy_killed(points)

@onready var skeleton_body: EnemyBody = $Skeleton

func _process(delta: float) -> void:
    progress += walk_speed*delta
    if progress_ratio >= .99:
        dmg_base_signal.emit(dmg_out)
        die()

func return_targetable_body() -> EnemyBody:
    return skeleton_body

func die(give_points: bool = false) -> void:
    if give_points:
        enemy_killed.emit(point_value)
    queue_free()
