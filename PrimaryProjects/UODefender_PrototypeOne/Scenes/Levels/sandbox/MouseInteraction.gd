class_name MouseInteraction
extends Node3D

## Camera Settings
var main_camera: Camera3D
@export var mouse_coll_detect_distance: float = 100.0
    
## Click
var last_click_2d: Vector2 = Vector2.ZERO
var last_click_3d: Vector3 = Vector3.ZERO
var space_state: PhysicsDirectSpaceState3D

## Signals 4 clix
signal clk_twr_space_grid(collision_dict: Dictionary)       # TowerSpaceGridMap
signal r_clk_twr(collision_dict: Dictionary)                # Right clicked a tower

func _ready() -> void:
    main_camera = get_viewport().get_camera_3d()
    space_state = get_world_3d().direct_space_state

func _physics_process(_delta: float) -> void:
    # I feel like I could/should put a timer on this but I don't have a clear idea on what timing might be nice. So I'm just going to let it ALWAYS run, and if it becomes a problem we'll come back to it. Thinking that this will generally be pretty useful, though
    
    # See where the mouse is currently hovering
    var coll_dict: Dictionary = get_current_collider_dict()
    if coll_dict:
        var collider = coll_dict.collider
        var coll_position: Vector3 = coll_dict.position
        # Universal mouse hover changer... *shrugs*
        change_mouse_cursor(collider, coll_position)
        
        if Input.is_action_just_pressed("action_button"):
            printt("Mouse Click: ", collider.position)
            if collider is TowerSpaceGridMap:
                clk_twr_space_grid.emit(coll_dict)
            elif collider is Tower:
                print("Pop menu")
                pass
        if Input.is_action_just_pressed("mouse_right"):
            if collider is Tower:
                # Connected to TowerManager
                r_clk_twr.emit(coll_dict)

func get_current_collider_dict() -> Dictionary:
    return get_collisions_from_cursor(main_camera, space_state, mouse_coll_detect_distance) 

func get_collisions_from_cursor(cam: Camera3D, space_st: PhysicsDirectSpaceState3D, ray_dist: float) -> Dictionary:
    """
    Given a Camera3D and PhysicsDirectSpaceState3D, project a ray 'ray_dist' from that camera to the current mouse postion projected into 3d space... Something like this
        :param cam: The world camera we're projecting our ray query from. Currently saved in the _ready func and set to get_viewport().get_camera_3d()
        :param space_st: Space state... Currently saved in the _ready func and set to get_world_3d().direct_space_state
        :param ray_dist: How far to look
    """
    last_click_2d = get_viewport().get_mouse_position()
    last_click_3d = cam.project_ray_normal(last_click_2d)
    
    # This is confusing because the 'last_click_2d' seems to not matter, I always just get the x,y,z transform of my camera. I get what it is, but not why I have to pass in my 2d click coords
    var ray_origin: Vector3 = cam.project_ray_origin(last_click_2d)
    # I do not fully understand why we add ray origin to the actual vector we care about - it doesnt seem like it makes sense to me
    var ray_end: Vector3 = ray_origin + last_click_3d * ray_dist
    var ray_query: PhysicsRayQueryParameters3D = PhysicsRayQueryParameters3D.create(ray_origin, ray_end)
    ray_query.collide_with_areas = true
    # ray_query.collision_mask = 0b0010
    var result: Dictionary = space_st.intersect_ray(ray_query)
    return result

func change_mouse_cursor(collider: Object, coll_position: Vector3) -> void:
    """
    Depending on the type of collider, try to predict the appropriate cursor
    Defaults to "mouse_default"
    """
    ## Mouse Icon Defaults
    var mouse_default = Input.CURSOR_ARROW
    var mouse_hover = Input.CURSOR_POINTING_HAND
    var mouse_bad = Input.CURSOR_HELP

    if collider is TowerSpaceGridMap:
        var tile_type = collider.get_tile_type(coll_position)
        match tile_type:
            TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_AVAIL, TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_TAKEN:
                Input.set_default_cursor_shape(mouse_hover)
            TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_INVALID, TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_DEBRIS:
                Input.set_default_cursor_shape(mouse_bad)
            _:
                Input.set_default_cursor_shape(mouse_default)                
    else:
        Input.set_default_cursor_shape(mouse_default)

"""
# Example: Setting mask value for enabling layers 1, 3 and 4

# Binary - set the bit corresponding to the layers you want to enable (1, 3, and 4) to 1, set all other bits to 0.
# Note: Layer 32 is the first bit, layer 1 is the last. The mask for layers 4,3 and 1 is therefore
0b00000000_00000000_00000000_00001101
# (This can be shortened to 0b1101)

# Hexadecimal equivalent (1101 binary converted to hexadecimal)
0x000d
# (This value can be shortened to 0xd)

# Decimal - Add the results of 2 to the power of (layer to be enabled - 1).
# (2^(1-1)) + (2^(3-1)) + (2^(4-1)) = 1 + 4 + 8 = 13
pow(2, 1-1) + pow(2, 3-1) + pow(2, 4-1)
           
"""
