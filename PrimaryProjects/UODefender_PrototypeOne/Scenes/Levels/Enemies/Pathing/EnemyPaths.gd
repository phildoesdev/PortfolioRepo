class_name EnemyPaths
extends Node3D


var groups = ["enemy_paths"]

# Spawn Timer Settings
var timer_spawn_interval: Timer
var default_wait_time_s := 1.0

@export var enemy_scene: PackedScene = load("res://Scenes/Levels/Enemies/skeleton.tscn")

@onready var player: Node3D = $"../Player"
func _ready() -> void:
    # Create our groups
    for _group in groups:
        add_to_group(_group)
    create_spawn_timer()


func create_spawn_timer() -> void:
    # only create the spawn timer if it hasn't been overwritten
    if timer_spawn_interval == null:
        timer_spawn_interval = Timer.new()
        add_child(timer_spawn_interval)
        timer_spawn_interval.wait_time = default_wait_time_s
        timer_spawn_interval.start()
    timer_spawn_interval.timeout.connect(spawn_enemy)
    
func do_dmg(amt: int) -> void:
    player.take_dmg(amt)

func spawn_enemy() -> void:
    var paths := get_tree().get_nodes_in_group("spawn_paths")
    for path in paths:
        # print("Spawn enemy")
        var new_enemy = enemy_scene.instantiate()
        path.add_child(new_enemy)
        new_enemy.dmg_base_signal.connect(do_dmg)
        new_enemy.enemy_killed.connect(player.update_player_score)
        break
