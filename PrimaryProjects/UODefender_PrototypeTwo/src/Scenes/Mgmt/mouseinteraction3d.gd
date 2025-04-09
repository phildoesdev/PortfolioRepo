class_name MouseInteraction3d
extends Node3D

""" 
Casts raycasts from camera and returns collisions and things like this
"""
# Max select distance
var ray_dist: int = 100
var debug_mode: bool = false
var last_collision: Dictionary 

var debug_draw: DebugDraw3D


func _init() -> void:
    debug_draw = DebugDraw3D.new()
    add_child.call_deferred(debug_draw)

func _physics_process(_delta: float) -> void:
    """
    We are hijacking the physics process method from anyone who inherits from us
        so we can do the debug print nicely. *shrug*
    """
    _mouse_interaction()
    if Input.is_action_pressed("action_button"):
        output_click_debug()

func _mouse_interaction() -> void:
    """
    Method intended to be overridden by different scenes who care about mouse interaction
    """
    print("Please override _mouse_interaction()")
        
func cast_raycast_to_cursor() -> Dictionary:
    """
    casts a raycast from the cursor to the gameworld and returns info about what the ray intersects 
        with, if anything
    """
    var cam: Camera3D = get_viewport().get_camera_3d()
    var space_state: PhysicsDirectSpaceState3D = get_world_3d().direct_space_state
    var last_click_2d: Vector2 = get_viewport().get_mouse_position()
    
    var ray_origin: Vector3 = cam.project_ray_origin(last_click_2d)
    var ray_normal: Vector3 = cam.project_ray_normal(last_click_2d)
    var ray_end: Vector3 =  ray_origin + ray_normal*ray_dist
    
    var query := PhysicsRayQueryParameters3D.create(ray_origin, ray_end)
    # query.collide_with_areas = false
    var result: Dictionary = space_state.intersect_ray(query)
    if result:
        # Append ray info
        result["ray_origin"] = ray_origin
        result["ray_normal"] = ray_normal
        result["ray_end"] = ray_end
    last_collision = result
    return result

func output_click_debug() -> void:
    """
    Given the results from a ray cast query, draw a line with connecting balls
    """
    if debug_mode and debug_draw and last_collision:
        debug_draw.clear_all()
        debug_draw.draw_sphere(last_collision["ray_origin"], 0.03, Color.RED)
        debug_draw.draw_line(last_collision.ray_origin, last_collision.position, Color.BLACK)
        debug_draw.draw_sphere(last_collision["position"], 0.03, Color.RED)
        print(last_collision)
