class_name Enemy
extends CharacterBody3D

var walk_speed: float = 1.0
var dmg_out: int = 1

## Destination and flag indicating whether we have set a real (not Vector3.ZERO) destination in our nav agent
var destination_point: Vector3 = Vector3.ZERO
var has_set_nav_dest: bool = false

signal dmg_base_signal(amt)

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity: float = ProjectSettings.get_setting("physics/3d/default_gravity")
@onready var enemy_nav_agent: NavigationAgent3D = $EnemyNavAgent


func _physics_process(delta: float) -> void:
    if destination_point == Vector3.ZERO:
        print("here")
        return
    if not has_set_nav_dest:
        print("here2")
        enemy_nav_agent.target_position = destination_point
        has_set_nav_dest = true
    ## prob remove htis later
    enemy_nav_agent.target_position = destination_point
    # Add the gravity.
    if not is_on_floor():
        velocity.y -= gravity * delta
    
    var next_position = enemy_nav_agent.get_next_path_position()
    var direction := global_position.direction_to(next_position)
    printt("destination_point, global_pos", destination_point, global_position)
    
    ## 'Look At' stuff. Later on, this is going to be more complicated b/c we will always face the same way, but change our animations
    #var adjusted_direction := direction
    #adjusted_direction.y = 0
    #look_at(global_position + adjusted_direction, Vector3.UP, true)
    
    
    if direction:
        var adjusted_direction := direction
        adjusted_direction.y = 0
        look_at(global_position + adjusted_direction, Vector3.UP, true)
        velocity.x = direction.x * walk_speed
        velocity.z = direction.z * walk_speed
    else:
        velocity.x = move_toward(velocity.x, 0, walk_speed)
        velocity.z = move_toward(velocity.z, 0, walk_speed) 
    move_and_slide()

"""

func _process(_delta: float) -> void:
    if provoked:
        navigation_agent_3d.target_position = player.global_position

func _physics_process(delta: float) -> void:
    # Add the gravity.
    if not is_on_floor():
        velocity.y -= gravity * delta

    var distance_to_player := global_position.distance_to(player.global_position)
    provoked = distance_to_player <= aggro_range
    
    if provoked and distance_to_player <= attack_range:
        playback.travel("attack")

    var next_position = navigation_agent_3d.get_next_path_position()
    var direction := global_position.direction_to(next_position)

    if direction:
        look_at_target(direction)
        velocity.x = direction.x * SPEED
        velocity.z = direction.z * SPEED
    else:
        velocity.x = move_toward(velocity.x, 0, SPEED)
        velocity.z = move_toward(velocity.z, 0, SPEED) 
    move_and_slide()

func look_at_target(direction: Vector3) -> void:
    var adjusted_direction := direction
    adjusted_direction.y = 0
    look_at(global_position + adjusted_direction, Vector3.UP, true)

func attack() -> void:
    player.hit(attack_out)

"""
