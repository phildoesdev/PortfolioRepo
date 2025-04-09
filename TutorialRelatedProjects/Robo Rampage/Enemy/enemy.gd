class_name Enemy
extends CharacterBody3D

const SPEED = 7

@export var attack_out := 20


# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity: float = ProjectSettings.get_setting("physics/3d/default_gravity")

var player: Player 
var provoked := false
var aggro_range := 25.0
@export var attack_range := 1.5

@export var max_hitpoints := 250.0
var current_hitpoints: float = max_hitpoints:
    set(val):
        current_hitpoints = val
        if current_hitpoints <= 0:
            queue_free()
        provoked = true
    get:
        return current_hitpoints

@onready var navigation_agent_3d: NavigationAgent3D = $NavigationAgent3D

# We got rid of our reference to the animation player in favor of an animation tree. 
@onready var animation_tree: AnimationTree = $AnimationTree
# This playback property is what we use to transition between our different playback states. It's the thing that creates our state machine we created in the bottom panel
# Also, this is a bit of a unique syntax b/c there are several types that this property could be. 
# So, now we have this playback and we can tell it to play specific animations
@onready var playback: AnimationNodeStateMachinePlayback = animation_tree["parameters/playback"]

func _ready() -> void:
    player = get_tree().get_first_node_in_group("player")
    
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
