class_name EnemyManager
extends Node3D

## Timer Settings
var spawn_timer: Timer 
var spawn_interval_default: float = 1.0

var enemy_scene: PackedScene = load("res://Scenes/enemies/air_elemental.tscn")

@onready var enemy_path_gridmap: EnemyPathGridMap = $"../EnemyPathGridmap"
@onready var player: Node3D = $"../../Player"

func _ready() -> void:
    print(enemy_path_gridmap)
    # init_timers()
    spawn_enemy()
    
func init_timers() -> void:
    # only create the spawn timer if it hasn't been overwritten
    if spawn_timer == null:
        spawn_timer = Timer.new()
        add_child(spawn_timer)
        spawn_timer.wait_time = spawn_interval_default
        spawn_timer.start()
    spawn_timer.timeout.connect(spawn_enemy)

func spawn_enemy() -> void:
    var rand_spawn_pos: Vector3 = enemy_path_gridmap.get_rand_spawn_point()
    var rand_dest_pos: Vector3 = enemy_path_gridmap.get_rand_exit_point()

    printt(rand_spawn_pos, rand_dest_pos)

    # Sanity check - make sure the lvl is set up correctly    
    if rand_spawn_pos == Vector3.ZERO or rand_dest_pos == Vector3.ZERO:
        print("Failed to spawn enemies. Either missing a spawn or exit position (EnemyPathGridmap.TileType.ENEMY_PATH_EXIT). WQD92KCP")
        return
    var new_enemy = enemy_scene.instantiate()
    add_child(new_enemy)
    new_enemy.dmg_base_signal.connect(do_dmg)
    new_enemy.global_position = rand_spawn_pos
    new_enemy.destination_point = rand_dest_pos

func do_dmg(amt: int) -> void:
    player.get_hit(amt)
