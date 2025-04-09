extends CharacterBody3D

# Move Options
@export var move_speed := 15.0
@export var climb_speed := 150
@export var mouseSensitivity := 0.5
var yaw := 0.0
var pitch := 0.0

# Gravity Options
var gravity_on: bool = false
var gravity: float = ProjectSettings.get_setting("physics/3d/default_gravity")
var gravity_fall_multiplier: float = 1.4

# Input Options
var mouse_pos_rel: Vector2 = Vector2.ZERO
var mouse_pos: Vector2 = Vector2.ZERO
var mouse_vel: Vector2 = Vector2.ZERO

@onready var player_admin_camera: Camera3D = $PlayerAdminCamera
@onready var crosshair: CenterContainer = $Crosshair

func _ready() -> void:
    if not player_admin_camera.current:
        crosshair.visible = false

func _physics_process(delta: float) -> void:
    """ *** MVMT HANDLE *** """ 
    # Get the input direction and handle the movement/deceleration.
    # As good practice, you should replace UI actions with custom gameplay actions.
    var input_dir := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
    var dir_y := 0
    if Input.is_action_pressed("move_up"):
        # print(input_dir)
        dir_y = -1
    elif Input.is_action_pressed("move_down"):
        dir_y = 1
    
    var direction := (transform.basis * Vector3(input_dir.x, dir_y, input_dir.y)).normalized()
    # print(transform)
    if direction:
        velocity.x = direction.x * move_speed
        velocity.z = direction.z * move_speed
        velocity.y = direction.y * move_speed
    else:
        velocity.x = move_toward(velocity.x, 0, move_speed)
        velocity.z = move_toward(velocity.z, 0, move_speed)
        velocity.y = move_toward(velocity.y, 0, move_speed)

    # We are going to have the option to turn gravity on/off so we'll touch it here at the end *shrug*
    if gravity_on:
        # Apply gravity as normal when we are jumping (going up), and then apply MORE gravity if they are falling
        if velocity.y >= 0:
            velocity.y -= gravity * delta
        else:
            velocity.y -= gravity * delta * gravity_fall_multiplier
            
    process_camera_rotation(delta)
    move_and_slide()

func _input(event: InputEvent) -> void:
    if event.is_action_pressed("ui_cancel"):
        if Input.mouse_mode == Input.MOUSE_MODE_VISIBLE:
            Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
        else:
            Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

    if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
        # I am getting the mouse motion relative to the last position of the screen adn recording that. I can use it for mvmt direction
        mouse_pos_rel = event.relative
        # Taking more params from this event than I need but I am curious so holding onto it
        mouse_pos = event.position
        mouse_vel = event.velocity

func process_camera_rotation(_delta: float) -> void:
    # fmod is a nice way of allowing us to cycle w/o having our yaw potentially be a HGUE #. just mod by 360 essentially...
    # other than that, we are taking the previous yaw we got and adding the relative amount we moved,'mouse sensitivity' keeps these #s adjustable/realistic
    # this is essentially keeping track of my rotation and adding/removing from it
    # Both pitch and yaw are generally working the same
    yaw = fmod(yaw  - mouse_pos_rel.x * mouseSensitivity, 360) 
    pitch = clamp(pitch - mouse_pos_rel.y * mouseSensitivity, -90, 90)
    self.set_rotation(Vector3(deg_to_rad(pitch), deg_to_rad(yaw), 0))    
    mouse_pos_rel = Vector2.ZERO
