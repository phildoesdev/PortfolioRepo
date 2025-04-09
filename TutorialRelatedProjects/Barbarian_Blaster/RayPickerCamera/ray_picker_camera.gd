extends Camera3D

var ray_distance: float = 75.0

@export var gridmap: GridMap
@export var turret_manager: Node3D
@export var turret_cost := 10

@onready var ray_cast_3d: RayCast3D = $RayCast3D
@onready var bank = get_tree().get_first_node_in_group("bank")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(__delta: float) -> void:
    flip_cell()

func flip_cell():
    var mouse_position: Vector2 = get_viewport().get_mouse_position()
    ray_cast_3d.target_position = project_local_ray_normal(mouse_position)*ray_distance
    ray_cast_3d.force_raycast_update()
    
    # printt(ray_cast_3d.get_collider(), ray_cast_3d.get_collision_point())
    if ray_cast_3d.is_colliding():
        if bank.current_gold >= turret_cost:        
            Input.set_default_cursor_shape(Input.CURSOR_POINTING_HAND)
            # could be anything, so lets grab it and find out what it is 
            var collider = ray_cast_3d.get_collider()
            if collider is GridMap and Input.is_action_pressed("click"):
                # Now lets figure out what cell we are hovering over
                var collision_point = ray_cast_3d.get_collision_point()
                # Then we are getting the position of the cell relative to the gridmap's center (returns a Vector3i)
                var cell = gridmap.local_to_map(collision_point)            
                # The if statement then says, hey gridmap, get me the cell from the GridMap's MeshLibrary at this relative location. 
                # (This is an item #, 0 for the first element in the mesh library, 1 for the second, etc. Returns -1 if the current selection is not in the meshlibrary).
                # We are specifically looking to see if the user clicked on the first element
                if gridmap.get_cell_item(cell) == 0:
                    # We are then saying set the cell at this position to the Item #1 in the mesh library ( set_cell_item(position: Vector3i, item: int, orientation: int = 0) )
                    gridmap.set_cell_item(cell, 1)
                    # We are then coming back to get the global position of this cell based on the map_to_local method call. 
                    var tile_position = gridmap.map_to_local(cell)
                    # We can then pass this global position into the connected turret manager, where we are asking it to build us a new instance of a turret scene
                    # I wish there was a better way to pass the scenes than with the gui. I am sure there is.
                    turret_manager.build_turret(tile_position)
                    bank.current_gold -= turret_cost
                    Input.set_default_cursor_shape(Input.CURSOR_ARROW)
    else:
        Input.set_default_cursor_shape(Input.CURSOR_ARROW)
            

    
    
    
