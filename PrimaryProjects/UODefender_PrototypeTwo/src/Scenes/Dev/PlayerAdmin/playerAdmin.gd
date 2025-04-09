extends CharacterBody3D
"""
/////////////////////////////////////////////
DECENT Static Camera Settings (no pitch or yaw changes)
    Low angle, long view. Looks great with y-billboard, looks great with regular billboard too...
    Free Mvmt looks mostly good
    A yaw of 90 deg works too, but it kind of makes you brain 'want to look down', and y-billboard does not work with it
        Height: 4m
        FOV: 70%
        Pitch: -10
        Yaw: -45
            # Vector3(deg_to_rad(-10), deg_to_rad(-45), 0)
/////////////////////////////////////////////
Decent Stationary camera
    High angle, slightly stunted view, but could be good to scroll along a single axis in this angle
    Free mvmt doesn't look very good because things float quite a bit. If you turn on collision shapes you can see how far off things go. Ooof.
        Position: (-2,10,7)
        FOV: 65%
        Pitch: -45
        Yaw: -65            
            # Vector3(deg_to_rad(-45), deg_to_rad(-65), 0)
/////////////////////////////////////////////
Interesting "High Up" Camera
    High y, same angle as the other high angle camera, manipulate camera's FOV for interesting zoom-in, zoom-out focus type thing
    It feels pretty nice up super high with a lower FOV. For whatever reason it seems to feel a little different
        Position: (-10,20,11)
        FOV: 35%
        Pitch: -45
        Yaw: -65            
            # Vector3(deg_to_rad(-45), deg_to_rad(-65), 0)
"""


# Move Options
@export var move_speed := 9.0
@export var climb_speed := 175
@export var mouseSensitivity := 0.5
@export var disable_y: bool = false
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
        if not disable_y:
            velocity.y = direction.y * move_speed
    else:
        velocity.x = move_toward(velocity.x, 0, move_speed)
        velocity.z = move_toward(velocity.z, 0, move_speed)
        if not disable_y:
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
    yaw = fmod(yaw  - mouse_pos_rel.x * mouseSensitivity, 360) 
    pitch = clamp(pitch - mouse_pos_rel.y * mouseSensitivity, -90, 90)
    var rotation_vector: Vector3 = Vector3(deg_to_rad(pitch), deg_to_rad(yaw), 0)
    self.set_rotation(rotation_vector)
    mouse_pos_rel = Vector2.ZERO
