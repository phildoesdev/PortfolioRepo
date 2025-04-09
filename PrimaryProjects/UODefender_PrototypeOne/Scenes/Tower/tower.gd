class_name Tower
extends StaticBody3D

var placement_cost: int = 125
var refund_percentage: float = 0.93  # probably refunds less $ as time goes on? Idk, sometimes this feels bad and just stops me from doing w/e. so If I reduce the refund amt there should be a good reason for it

## Attacks
@export var sticky_attack: bool = true  # if true, we will hold on a target until it leaves range and then find our new target (implement later? ... there will be a lot more to this, and will have to feel out options)
@export var can_attack: bool = true

@export var dmg_out: int = 25

@export var attack_range: float = 10.0
@export var attacks_per_s: float = 1
@export var attack_timer: Timer

@export var projectile: PackedScene = load("res://Scenes/enemies/projectile.tscn")

## Targeting
var current_target: Node3D


@onready var attack_spawn_marker: Marker3D = $AttackSpawnMarker
func _ready() -> void:
    if attacks_per_s == 0:
        print("Attacks per second cannot be zero. Disabling attack abilities QX7EXYCL.")
        attacks_per_s = 1
        can_attack = false
    create_timers()
    
func _physics_process(_delta: float) -> void:
    pass

func create_timers() -> void:
    # Attack timer- if not set externally create it
    if attack_timer == null:
        attack_timer = Timer.new()
        add_child(attack_timer)
        attack_timer.wait_time = 1/attacks_per_s
        attack_timer.start()
    attack_timer.timeout.connect(attack_timer_timeout)

func get_refund_amt() -> int:
    """ Returns the refund amt for this tower- some percentage of the original cost"""
    prints("Estimated Refund: ",placement_cost * refund_percentage)
    return floor(placement_cost * refund_percentage)

func attack_timer_timeout() -> void:
    # If we dont have an active target, try to find one
    if not current_target or not is_target_in_tower_range(current_target):
        find_target()

    if can_attack and current_target != null:
        var proj: Projectile = projectile.instantiate()
        add_child(proj)
        proj.global_position = attack_spawn_marker.global_position
        proj.direction = global_transform.basis.z
        proj.dmg = dmg_out
        proj.target = current_target

func find_target() -> void:
    var enemy_paths = get_tree().get_first_node_in_group("enemy_paths").get_children()
    
    var curr_target: Node3D
    var curr_distance: float = 0.0
    var curr_dist_set: bool = false
    
    for _path in enemy_paths:
        # Sanity Check
        if not _path is Path3D:
            continue
        var enemies: Array = _path.get_children()
        for _enemy in enemies:
            var enemy_target: EnemyBody = _enemy.return_targetable_body()
            var enemy_dist: float = global_position.distance_to(enemy_target.global_position)
            if not curr_dist_set:
                curr_distance = enemy_dist
                curr_dist_set = true
            # Sanity check
            if not (_enemy is EnemyPathFollow3D and enemy_dist <= attack_range and enemy_dist <= curr_distance):
                continue
            curr_target = enemy_target
            curr_distance = enemy_dist
    # this can be null
    current_target = curr_target

func is_target_in_tower_range(targ: Node3D) -> bool:
    """ Is the target provided in range? 
    A more generalized version of 'is my current target still in range'
    """
    # Sanity check b/c targ will frequently be empty
    if targ == null or not is_instance_valid(targ):
        return false

    var targ_dist: float = global_position.distance_to(targ.global_position)
    if targ_dist <= attack_range:
        return true
    return false

func put_something_in_place(pos: Vector3) -> void:
    var scene: PackedScene = load("res://Scenes/Levels/Nature/bush_1.tscn")
    var my_scene = scene.instantiate()
    add_child(my_scene)
    my_scene.top_level = true
    my_scene.global_position = pos
    
    
            
            
