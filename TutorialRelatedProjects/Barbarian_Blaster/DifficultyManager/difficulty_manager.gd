extends Node

signal stop_spawning_enemies

@export var game_length_s := 180.0
@export var spawn_time_curve: Curve
@export var health_curve: Curve

@onready var timer: Timer = $Timer


func _ready() -> void:
    timer.start(game_length_s)
    
func _process(_delta: float) -> void:
    pass
    
func get_enemy_health() -> int:
    var health = int(health_curve.sample(game_progress_ratio()))
    return health

func game_progress_ratio() -> float:
    return 1.0 - (timer.time_left/game_length_s)

func get_spawn_time() -> float:
    return spawn_time_curve.sample(game_progress_ratio())

func _on_timer_timeout() -> void:
    stop_spawning_enemies.emit()
