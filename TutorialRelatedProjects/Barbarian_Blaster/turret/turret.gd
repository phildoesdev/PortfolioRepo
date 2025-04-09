extends Node3D

@export var projectile: PackedScene
@export var turret_range := 25.0

var enemy_path: Path3D
var target: PathFollow3D

@onready var turret_base: Node3D = $TurretBase
@onready var cannon: Node3D = $TurretBase/TurretTop/Cannon
@onready var animation_player: AnimationPlayer = $AnimationPlayer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(_delta: float) -> void:
    target = find_best_target()
    if target != null:
        turret_base.look_at(target.global_position, Vector3.UP, true)

func _on_projectile_timer_timeout() -> void:
    if target != null:
        animation_player.play("FireCannon")
        var shot = projectile.instantiate()
        add_child(shot)
        shot.global_position = cannon.global_position
        shot.direction = turret_base.global_transform.basis.z

func find_best_target() -> PathFollow3D:
    var best_target = null
    var best_progress = 0
    
    for enemy in enemy_path.get_children():
        if enemy is PathFollow3D and enemy.progress > best_progress:
            if global_position.distance_to(enemy.global_position) <= turret_range:
                best_target = enemy
                best_progress = enemy.progress            
        
    return best_target
