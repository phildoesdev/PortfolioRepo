class_name Player
extends CharacterBody3D

"""
The camera, and a container for things like a bank, stats, etc
"""
# Player settings
var starting_gold: int = 1250

# Move Options
var move_speed: float = 5.0
var climb_speed: float = 150
var mouseSensitivity: float= 0.5
var starting_y: float= 7.0
@export var disable_camera_rotation: bool = true
@export var disable_y: bool = true
@export var disable_z: bool = true

var player_groups: Array = ["Player", "PlayerBank"]   # How most other things will reference us 

# Internal Mgmt
var bank: PlayerBank
signal balance_changed(gold_pieces:int)  # We bubble up the balance changed signal

func _ready() -> void:
    init_bank()
    set_spawn_settings()
    for _group in player_groups:
        add_to_group(_group)

func init_bank() -> void:
    """
    There are a couple of tasks including singal connections that go along with creating player's bank
    """    
    bank = PlayerBank.new(starting_gold)
    bank.balance_changed.connect(balance_changed_signal_transponder)

func set_spawn_settings() -> void:
    """
    Sets the camera rotation, starting position, etc.
    Created to keep our _ready method easier to read
    """
    process_camera_rotation()
    global_position.y = starting_y

func _physics_process(_delta: float) -> void:
    set_dir_and_velocity()
    move_and_slide()
    
func set_dir_and_velocity() -> void:
    # Get the input direction and handle the movement/deceleration.
    var input_dir := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
    var dir_y := 0
    if Input.is_action_pressed("move_up"):
        dir_y = -1
    elif Input.is_action_pressed("move_down"):
        dir_y = 1

    var direction := (transform.basis * Vector3(input_dir.x, dir_y, input_dir.y)).normalized()
    if direction:
        velocity.x = direction.x * move_speed
        if not disable_z:
            velocity.z = direction.z * move_speed
        if not disable_y:
            velocity.y = direction.y * move_speed
    else:
        velocity.x = move_toward(velocity.x, 0, move_speed)
        if not disable_z:
            velocity.z = move_toward(velocity.z, 0, move_speed)
        if not disable_y:
            velocity.y = move_toward(velocity.y, 0, move_speed)

func process_camera_rotation() -> void:
    """
    Sets the camera rotation.
    Pretty bland for the time being because we lock it to one value, but could be set to move
        with mouse and called in physics process if desired (as in PlayerAdmin)
    """
    # var rotation_vector: Vector3 = Vector3(deg_to_rad(-45), deg_to_rad(-120), 0) # Old default 04.10.24
    # var rotation_vector: Vector3 = Vector3(deg_to_rad(-45), deg_to_rad(-40), 0) # Old default 04.10.24
    # var rotation_vector: Vector3 = Vector3(deg_to_rad(-30), deg_to_rad(-90), 0)  #  I Think i like this a lot better
    # var rotation_vector: Vector3 = Vector3(deg_to_rad(-40), deg_to_rad(0), 0) ## When I start doing new art this is what we will actually be using so that Vector3.FORWARD is actually forward for us. No reason to fight this
    var rotation_vector: Vector3 = Vector3(deg_to_rad(-50), deg_to_rad(-50), 0)  #  I Think i like this a lot better
    # var rotation_vector: Vector3 = Vector3(deg_to_rad(-50), deg_to_rad(-45), 0)  #  more isomorphic test
    # global_position.y = 15
    self.set_rotation(rotation_vector)

func deposit_gp(amt: int) -> bool:
    """
    Wrapper to make our bank easier to interface with
    """
    return bank.deposit_gp(amt)

func withdraw_gp(amt: int) -> bool:
    """
    Wrapper to make our bank easier to interface with
    """
    return bank.withdraw_gp(amt)

func balance_gp() -> int:
    """
    Wrapper to make our bank easier to interface with
    """
    return bank.balance_gp()

func balance_changed_signal_transponder(gold_peices:int) -> void:
    """
    We bubble up the balance changed signal received from our bank for the sake
        of gui and such
    """
    balance_changed.emit(gold_peices)
