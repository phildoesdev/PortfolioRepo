class_name Tower
extends CharacterBody3D

enum TOWER_TYPES {
    NONE=0,
    MAGE=1,
    WARRIOR=2
}

enum TARGETING_STRATEGY {
    NONE=0, 
    CLOSEST=1, 
    LIFO=2, 
    RAND=3,
    LOWEST_HEALTH=4,
    HIGHEST_HEALTH=5
}

enum ATTACK_TYPE {
    NONE=0,
    PROJECTILE=1,
    MELEE=2,
    FIELD=3,
    SUMMON=4
}

enum DMG_TYPES {
    NONE=0,
    PHYSICAL=1,
    FIRE=2,
    POISON=3,
    COLD=4,
    ENERGY=5,
    CHOAS=6,
    DIRECT=7
}

enum DMG_MODES {
    NONE=0,
    MELEE=1,
    MAGICAL=2,
    RANGED=3,
}

## This hsould be renamed to something less generic, and documented where it is used...
var is_valid: bool = false

# Used to place towers at the correct y
var tower_height: float = 0.6
var current_equipment_ani_players: Array = []   # List of all the armor and stuff we might be wearing to simplify playing animations

# Targeting
var targeting_strat: TARGETING_STRATEGY = TARGETING_STRATEGY.CLOSEST
var tower_range_radius: float = 5.5
var tower_range_height: float = 2

# Attacking
var base_attack_dmg: float = 25.0
var attacks_per_sec: float = 0.25
var dmg_types: Dictionary = {}
var current_attack_ani: String = "SPELL_1"
var default_idle: String = "IDLE"
var current_attack_type: ATTACK_TYPE = ATTACK_TYPE.PROJECTILE

# Etc
var gold_cost: int = 125
var refund_percentage: float = 1.0

# Stats
var twr_stats: TowerStatistics

# Internal Mgmt
var last_attack_msec: float = 0.0
var is_attacking: bool = false  # Whether we are attempting an attack or not
var is_firing: bool = false     # Whether the actual projectile/dmg has gone out

# Tower tile mgmt 
var twr_tile: TowerTileGridmap              # A reference to the tower tile map we're spawned on
var twr_tile_pos: Vector3 = Vector3.ZERO    # The position that will allow us to find our tile again

# Targeting
var avail_targets: Array = []
var current_target: EnemyBody


# Signals ... that I dont know if they should exist yet
signal enemy_killed

    
func required_keys() -> Dictionary:
    return {
        "targeting_strat": targeting_strat,
        "tower_range_radius": tower_range_radius,
        "tower_range_height": tower_range_height,
        "base_attack_dmg": base_attack_dmg,
        "attacks_per_sec": attacks_per_sec,
        "dmg_types": dmg_types,
        "current_attack_ani": current_attack_ani,
        "current_attack_type": current_attack_type,
        "gold_cost": gold_cost,
    }

func _init(init_dict: Dictionary) -> void:
    is_valid = verify_tower_init(init_dict)
    assert(is_valid, "Invalid tower setup. 97B296J0")
    
    targeting_strat         = init_dict["targeting_strat"]
    tower_range_radius      = init_dict["tower_range_radius"]
    tower_range_height      = init_dict["tower_range_height"]
    base_attack_dmg         = init_dict["base_attack_dmg"]
    attacks_per_sec         = init_dict["attacks_per_sec"]
    dmg_types               = init_dict["dmg_types"]
    current_attack_ani      = init_dict["current_attack_ani"]
    current_attack_type     = init_dict["current_attack_type"]
    gold_cost               = init_dict["gold_cost"]
    
    twr_stats = TowerStatistics.new()
    create_attack_range()

func _ready() -> void:
    """
    If this gets overwritten and 'initialize_animations' never gets called, we will never
        attack or do anything...
    """
    initialize_animations()

func _physics_process(_delta: float) -> void:
    """
    If this gets overwritten and 'initiate_attack' never gets called, we will never
        attack. 
    """
    initiate_attack()

func initialize_tower(_twr_tile: TowerTileGridmap, _twr_tile_pos: Vector3) -> void:
    twr_tile = _twr_tile
    twr_tile_pos = _twr_tile_pos
    mark_tile_taken()   # Mark this tile as having a tower on it
    
func get_gold_cost() -> int:
    return gold_cost

func get_gold_refund() -> int:
    return round(gold_cost*refund_percentage)

func dir_to_target(preserve_y: bool=false) -> Vector3:
    var rtrn_val: Vector3 = Vector3.ZERO
    if current_target:
        rtrn_val = (current_target.global_position - self.global_position).normalized()
        if not preserve_y:
            rtrn_val.y = 0
    return rtrn_val

func get_dir_string() -> String:
    var dir_str: String = ""
    match round(dir_to_target()):
        Vector3.FORWARD:
            # (0,0,-1), -Z
            dir_str = "-Z"
        Vector3.BACK:
            # (0,0,1), +Z
            dir_str = "+Z"
        Vector3.LEFT, Vector3(-1,0,1), Vector3(-1,0,-1):
            # (-1,0,0), -X
            dir_str = "-X"
        Vector3.RIGHT, Vector3(1,0,1), Vector3(1,0,-1):
            # (1,0,0), +X
            dir_str = "+X"
        _:
            print("Tower Default. PSHU7PRU")
            dir_str = "+X" # Always return something valid
    return dir_str

func verify_tower_init(init_dict: Dictionary) -> bool:
    var tmp_req_keys: Dictionary = required_keys()
    for _k in tmp_req_keys.keys():
        if _k not in init_dict.keys():
            printt(_k," IS A MISSING KEY")
            return false
        if not typeof(init_dict[_k]) == typeof(tmp_req_keys[_k]):
            printt(_k," IS THE WRONG TYPE")
            return false
    return true

func initialize_animations() -> bool:
    for _x in $SubViewport/Tower2DSpriteContainer.get_children(true):
        if _x is Sprite2D:
            if _x.get_child(0) is AnimationPlayer:
                current_equipment_ani_players.append(_x.get_child(0))
    if len(current_equipment_ani_players) <= 0:
        return false
    return true

func find_target() -> EnemyBody:
    if not current_target or not current_target.is_valid or (not current_target in avail_targets and len(avail_targets) > 0):
        match targeting_strat:
            TARGETING_STRATEGY.CLOSEST:
                var this_dist: float = 0.0
                for targ_ in avail_targets:
                    var t_dist: float = global_position.distance_squared_to(targ_.global_position)
                    if t_dist <= this_dist or this_dist == 0.0:
                        this_dist = t_dist
                        current_target = targ_
            TARGETING_STRATEGY.LIFO:
                if len(avail_targets):
                    current_target = avail_targets[len(avail_targets)-1]
            TARGETING_STRATEGY.RAND:
                current_target = avail_targets.pick_random()
            TARGETING_STRATEGY.LOWEST_HEALTH:
                var this_health: float = 0.0
                for targ_ in avail_targets:
                    var t_health: float = targ_.current_health()
                    if t_health <= this_health or this_health == 0.0:
                        this_health = t_health
                        current_target = targ_
            TARGETING_STRATEGY.HIGHEST_HEALTH:
                var this_health: float = 0.0
                for targ_ in avail_targets:
                    var t_health: float = targ_.current_health()
                    if t_health >= this_health or this_health == 0.0:
                        this_health = t_health
                        current_target = targ_
            _:
                pass
    # Recheck ... not sure if necessary, but seems useful as some of these loops could be not-instant
    if not current_target or current_target not in avail_targets or not current_target.is_valid:
        current_target = null
    return current_target

func create_attack_range() -> void:
    # Create area with out custom range/height
    var attack_area: Area3D = Area3D.new()
    var attack_area_coll_shape:= CollisionShape3D.new()
    var attack_area_shape:= CylinderShape3D.new()
    attack_area_shape.height = tower_range_height
    attack_area_shape.radius = tower_range_radius
    attack_area_coll_shape.shape = attack_area_shape
    add_child(attack_area)
    attack_area.add_child(attack_area_coll_shape)
    # Connect signals 
    attack_area.connect("body_entered",target_in_range)
    attack_area.connect("body_exited",target_out_of_range)

func target_in_range(body: Node3D) -> void:
    if body is EnemyBody and body not in avail_targets and body.is_valid and verify_target_distance(body):
        avail_targets.append(body)
        twr_stats.register_enemy_seen()  # Very well might get deleted if it is as useless as its kind of feeling
    
func target_out_of_range(body: Node3D) -> void:
    if body is EnemyBody and body in avail_targets:
        avail_targets.remove_at(avail_targets.find(body))

func verify_target_distance(targ_: EnemyBody) -> bool:
    """
    Verify the distance on the x doesnt exceed range in that direction, same for y    
    Not enitrely sure why I need it b/c I would think that the signals being fired for body
        entering/leaving range would be accurate. Im not sure exactly what is happening
    """
    var dist_to_targ: Vector3 = targ_.global_position - global_position
    if dist_to_targ.x > tower_range_radius:
        return false
    if dist_to_targ.y > tower_range_height:
        return false
    return true    

func place_tower(pos: Vector3) -> void:
    """
    Gets called when placing a tower. 
    Sets to desired position with correct y to display nicely
    """
    pos.y += tower_height/2
    global_position = pos

func _input(_event: InputEvent) -> void:
    if Input.is_action_just_pressed("one"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/BOW_-X")
    elif Input.is_action_just_pressed("two"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/IDLE_-X")
    elif Input.is_action_just_pressed("three"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/IDLE_+Z")
    elif Input.is_action_just_pressed("four"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/IDLE_-Z")
    elif Input.is_action_just_pressed("five"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/WALK_+X")
    elif Input.is_action_just_pressed("six"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/WALK_-X")
    elif Input.is_action_just_pressed("seven"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/WALK_+Z")
    elif Input.is_action_just_pressed("eight"):
        for x in current_equipment_ani_players:
            x.play("TowerMvmt/WALK_-Z")
    elif Input.is_action_just_pressed("nine"):
        pass
    elif Input.is_action_just_pressed("zero"):
        pass

func initiate_attack() -> void:
    """
    If it's time to attack again, 
        - try to find a target
        - Loop all our gear to play the correct attack animation
        - wait for the animations to call the functions that finish our attack
    """
    # We should not attack if it'd make us deal dmg faster than our attack speed
    var time_since_last_attack: float = (Time.get_ticks_msec() - last_attack_msec)
    if time_since_last_attack > (1/attacks_per_sec)*1000*1.5:       # This will allow us to recover from any sort of interuption
        is_attacking = false
    if is_attacking or time_since_last_attack < (1/attacks_per_sec)*1000 or not find_target():
        return
    is_attacking = true                         # Takes a hold on starting a new attack animation
    is_firing = false                           # Allows us to release another fireball
    last_attack_msec = Time.get_ticks_msec()    # Assuming getting here means we are successfull attacking
    var attack_ani_name: String = "TowerAttack/"+current_attack_ani+"_"+get_dir_string()
    for x in current_equipment_ani_players:
        # Math to make sure we can reach our 'promised' attack speed
        var ani_speed: float = x.speed_scale    # It's convienient to be able to just dump this var into our play call, so I populate it with w/e it's set to originally by default
        var ani_lenght: float = x.get_animation(attack_ani_name).length
        if  ani_lenght > 1/attacks_per_sec:
            ani_speed = ani_lenght*attacks_per_sec  # Force animation speed to be attack speed if it is less than attack time so players never 'lose' attack speed
        x.stop()        # So that we never fail to start up our attack process, even at great attack speeds
        x.play(attack_ani_name, -1, ani_speed, false)
        x.queue("TowerMvmt/"+default_idle+"_"+get_dir_string())

func execute_attack() -> void:
    """
    Spawns the projectile, does dmg, etc. 
    Called by our animations to deal the dmg, spawn projectiles, or w/e, so that 
        everything looks pretty
    """
    # Prevents us from being called multiple times by each animation player..
    if is_firing:
        return
    # We re-check find target, because it has been some amt of time in which an animation was playing, so lets verify we have a valid target, and if we don't, find one seamlessly w/o restarting any animations.
    if not find_target():
        # If we fail to find a new target, fail gracefully
        is_attacking = false
        last_attack_msec = 0.0
        return
    is_firing = true   # Required or else each animation will try to execute attack

    # We are assuming towers will define this method for themselves
    _tower_attack()
    
    is_attacking = false
    twr_stats.register_dmg_raw_out(current_target.get_fs_id(), base_attack_dmg)
    twr_stats.register_attack_out()   # Register when we successfully execute an attack

func _tower_attack() -> void:
    """
    This is what will be over wrirten by all other towers to perform their special
        attacks. All other things can be 
    """
    print("Please override _tower_attack() so that your tower can attack!!!")

func register_death(death_sheet: Dictionary) -> void:
    """
    A place for enemies to register their deaths officially
    """
    twr_stats.register_kill(death_sheet)
    enemy_killed.emit()
    
func register_dmg(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    A place for enemies to make register the dmg they have recieved 
    """
    twr_stats.register_dmg_actual_out(fact_sheet_id, dmg_amt)
    
func mark_tile_taken() -> void:
    """
    Marks the tower tile we were placed on as 'taken' so no other towers can be placed here
    Gets called in 'initialize_tower()'
    """
    # Change the tile to reflect our new status
    twr_tile.change_tile(twr_tile_pos, TowerTileGridmap.TILE_TYPE.TOWER_SPACE_TAKEN)

func mark_tile_free() -> void:
    """
    When someone dismisses a tower we must mark the tower space available again
    Gets called by 'dismissed()'
    """
    # Change the tile to reflect our new status
    twr_tile.change_tile(twr_tile_pos, TowerTileGridmap.TILE_TYPE.TOWER_SPACE_AVAIL)
    
func dismiss() -> void:
    mark_tile_free()
    queue_free()
