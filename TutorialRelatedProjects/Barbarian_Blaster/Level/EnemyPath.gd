extends Path3D

@export var difficult_manger: Node
@export var enemy_scene: PackedScene
@export var victory_layer: CanvasLayer

@onready var spawn_timer: Timer = $SpawnTimer

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
    pass

func spawn_enemy() -> void:
    var new_enemy = enemy_scene.instantiate()
    new_enemy.current_health = difficult_manger.get_enemy_health()
    add_child(new_enemy)    
    spawn_timer.wait_time = difficult_manger.get_spawn_time()
    new_enemy.tree_exited.connect(enemy_defeated)

func _on_difficulty_manager_stop_spawning_enemies() -> void:
    spawn_timer.stop()

func enemy_defeated() -> void:
    if spawn_timer.is_stopped():
        for child in get_children():
            if child is PathFollow3D:
                return
        victory_layer.victory()
