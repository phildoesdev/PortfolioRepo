class_name EnemyBody
extends CharacterBody3D
"""
The enemy's body, but not its soul. Move, animate, and pass input messages along to the 
    appropriate places.
Attempting to spread the work as much as possible, giving attack and health mgmt their own 
    classes. Who knows exactly how this will end up being orgnaized.

More info on nav meshes:
    https://docs.godotengine.org/en/stable/tutorials/navigation/navigation_different_actor_types.html
"""
# Spawn settings
var nav_path_max_distance: float = 0.75  # This is roughly like a monster can get pushed ~3 tiles away from where it is trying to walk before it'll recalc
var nav_pathdesired_distance_range: Array = [0.50, 0.75]    # Adds some variance so it looks less like all creatures are in a single file line
var nav_target_desired_distance: float = 3.0
var body_min_slide_angle: float = deg_to_rad(1)
var starting_y: float = 0.6
var death_y: float = 0.4
var col_layer_alive:= 0b00000000_00000000_00000000_00000001     # Collision layer for when aliving and walking, gets set in _ready
var col_layer_dead:= 0b00000000_00000000_00000000_00000010      # Collision layer for corpse and w/e else

# Mvmt Settings
var walk_speed: float = 0.2
var ani_speed: float = 1.0

# What it means to be an 'enemy body'
var is_valid: bool = false  # Has this enemy been intialized with a factsheet, is it alive, can it be targeted, attacked, etc?
var fact_sheet: EnemyFactSheet # This unit's 'fact sheet', containing information about health, dmg, resists, loot, etc
var health_obj: EnemyHeart  # This unit's 'heart', or health object

# Attacker, death, and reward information
var last_attacker: Tower

# Keep track of our direction & animations
var last_pos: Vector3 = Vector3.ZERO
var current_direction: Vector3 = Vector3.ZERO
var last_direction: Vector3 = Vector3.ZERO
var current_dir_change_msec: int = 0     # the time our current_direction was set
var check_animation_min_time_msec: int = 150    # To prevent our animation for flipping unwantedly
var check_animation: bool = true    # whether we should update our animation based on our direction 

# We must know the player b/c this is how im doing things like applying dmg and rewarding death
var player: Player
var player_group_name: String = "Player"

@onready var display_sprite_main_2d: Sprite2D = $"2DScreen/SpriteContainer/DisplaySpriteMain2d"
@onready var animation_player: AnimationPlayer = $"2DScreen/SpriteContainer/DisplaySpriteMain2d/AnimationPlayer"
@onready var nav_agent: NavigationAgent3D = $NavigationAgent3D
func _ready() -> void:
    # Connect signal to determine 'safe velocity' for avoidance and what have you
    nav_agent.velocity_computed.connect(set_safe_velocity_lcl)
    nav_agent.target_reached.connect(die)
    current_dir_change_msec = Time.get_ticks_msec()
    set_spawn_settings()
    player = get_tree().get_first_node_in_group(player_group_name)

func _physics_process(_delta: float) -> void:
    calculate_mvmt()
    if check_animation:
        play_correct_mvmt_animation()
        
func get_fs_id() -> int:
    """
    Method to get the ID from the fact sheet of this enemy body... 
    """
    if not fact_sheet:
        return 0
    return fact_sheet.id

func initialize_enemy(fs: EnemyFactSheet) -> void:
    """
    Use the factsheet to populate and create w/e we might need
    """
    # Initialize some variables
    if fs.is_viable():
        # Create new texture and assign it to our sprite container?
        var enemy_texture: Texture2D = load(fs.sprite_sheet_path)
        display_sprite_main_2d.texture = enemy_texture
        health_obj = EnemyHeart.new(fs)
        health_obj.zero_health.connect(sig_zero_health)
        health_obj.dmg_taken.connect(sig_dmg_taken)
        walk_speed = fs.walk_speed
        ani_speed = fs.animation_speed
        fact_sheet = fs
        is_valid = true
        fs.calculate_gold_output()
    
func set_spawn_settings() -> void:
    """
    Default settings and such for the enemy body and its nodes. 
    It is helpful to have one place to view all the changes to the different nodes in one area as much as possible
    """
    wall_min_slide_angle = body_min_slide_angle
    collision_layer = col_layer_alive
    nav_agent.path_desired_distance = randf_range(nav_pathdesired_distance_range[0], nav_pathdesired_distance_range[1])
    nav_agent.path_max_distance = nav_path_max_distance
    nav_agent.target_desired_distance = nav_target_desired_distance 
    
func set_nav_map_rid(map_rid: RID) -> void:
    nav_agent.set_navigation_map(map_rid)

func set_spawn_position(dest: Vector3) -> void:
    global_position = dest
    global_position.y = starting_y
    
func set_destination(dest: Vector3) -> void:
    nav_agent.target_position = dest
    
func is_destination_reachable() -> bool:
    return nav_agent.is_target_reachable()

func current_health() -> float:
    return health_obj.health_c

func calculate_mvmt() -> void:
    # Ask our nav agent where to go and then set a velocity towards that postion
    var next_pos: Vector3 = nav_agent.get_next_path_position()
    last_pos = global_position
    var this_direction: Vector3 = (next_pos-last_pos).normalized()
    if (Time.get_ticks_msec()-current_dir_change_msec > check_animation_min_time_msec) and round(this_direction) != round(current_direction):
        last_direction = current_direction
        current_direction = this_direction
        current_dir_change_msec = Time.get_ticks_msec()
        check_animation = true
    var new_velocity: Vector3 = current_direction*walk_speed
    nav_agent.velocity = new_velocity
    
func play_correct_mvmt_animation() -> void:
    match round(current_direction):
        Vector3.FORWARD:
            # (0,0,-1), -Z
            animation_player.play("EnemyMvmtAnimations/WALK_-Z", -1, ani_speed)
        Vector3.BACK:
            # (0,0,1), +Z
            animation_player.play("EnemyMvmtAnimations/WALK_+Z", -1, ani_speed)
        Vector3.LEFT, Vector3(-1,0,1), Vector3(-1,0,-1):
            # (-1,0,0), -X
            animation_player.play("EnemyMvmtAnimations/WALK_-X", -1, ani_speed)
        Vector3.RIGHT, Vector3(1,0,1), Vector3(1,0,-1):
            # (1,0,0), +X
            animation_player.play("EnemyMvmtAnimations/WALK_+X", -1, ani_speed)
        _:
            pass
    
func set_safe_velocity_lcl(safe_vel: Vector3) -> void:
    """
    Gets called when the nav agent's velocity gets set, it returns us a
        'safe velocity', which avoids other zombies, and such...
    """
    velocity = safe_vel
    move_and_slide()

func take_dmg(amt: float, dmg_types:Dictionary, _src: Tower) -> void:
    if not is_valid:
        return
    last_attacker = _src
    health_obj.take_dmg(amt, dmg_types)
    flash_on_hit()

func flash_on_hit() -> void:
    """
    Simple shader to flash the color of w/e monster we're hitting
    """
    display_sprite_main_2d.material.set_shader_parameter("highlight", true)
    await get_tree().create_timer(0.1).timeout
    display_sprite_main_2d.material.set_shader_parameter("highlight", false)

func die_with_ani() -> void:
    is_valid = false
    collision_layer = col_layer_dead
    nav_agent.avoidance_enabled = false
    nav_agent.velocity = Vector3.ZERO
    set_physics_process(false)
    set_process(false)
    animation_player.play("EnemyMvmtAnimations/DIE_-X", -1, ani_speed)     
    global_position.y = death_y # Readjust y pos so death animation sits better
    
func die() -> void:
    queue_free()

func sig_zero_health() -> void:
    """
    Gets called when our health object emits a signal indicating we've reached 0 health
    
    ... I think that at some point we will give anyone who did dmg to us an equal 
    portion of the xp output... maybe. Idk how last attacker will feel.
    """
    die_with_ani()
    player.deposit_gp(fact_sheet.gold_output)
    last_attacker.register_death(fact_sheet.get_deathsheet())
    
func sig_dmg_taken(dmg_amt: float) -> void:
    """
    Gets called when our health object emits a signal indicating we've taken dmg of some sort
    """
    # Notify the tower marked as 'last attacker' of exactly how much they hurt us :<
    last_attacker.register_dmg(fact_sheet.id, dmg_amt)
