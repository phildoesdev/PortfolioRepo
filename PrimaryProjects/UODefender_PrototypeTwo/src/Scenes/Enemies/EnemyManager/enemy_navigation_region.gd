class_name EnemyNavigationRegion
extends NavigationRegion3D

"""
Create several navigation regions for map, including 'all mesh', give them IDs for niceness later, 
These get created per level b/c they are specific to each lvl, added to the correct spot, and then the
    enemy manager chooses a place to spawn w/ w/e navigation layer....
"""

var active_nav_layers: Array = []

# A referenceable ID
@export var id: int = -1
var map_rid: RID

# Whether this is the mesh that covers all walkable areas. Generally here to test for 'any available path'
@export var is_mesh_all: bool = false 
@export var destination_override: Marker3D  # Need to make this a less manual thing. dislike exorting/assigning things... seems lkik,e a way to make a thing get forgot.
@export var description: String = "Default Navigation Region"

@onready var spawn_points: Node3D = $SpawnPoints
@onready var destination_points: Node3D = $DestinationPoints

func _ready() -> void:
    assert(id > 0, "Choose a non-zero ID for this nav mesh. IT5AM6Y2")
    # Create a unique navigation map so that we can assign individual 'monsters' to it
    map_rid = NavigationServer3D.map_create()
    set_navigation_map.call_deferred(map_rid)
    NavigationServer3D.map_set_active(map_rid, true)
    bake_navigation_mesh()

func get_destination_point() -> Vector3:
    # Destination override gets rid of our requirement for a destination point
    if destination_override:
        return destination_override.global_position
    var num_dest_markers: int = destination_points.get_child_count()
    assert(num_dest_markers > 0, "No destination point markers, universal or otherwise for this nav-mesh. 43DQPDK6")
    return destination_points.get_children()[randi_range(0, num_dest_markers-1)].global_position
    
func get_spawn_point() -> Vector3:
    var num_spawn_points: int = spawn_points.get_child_count()
    assert(num_spawn_points > 0, "No spawn point markers, universal or otherwise for this nav-mesh. 9YGCDBY6")
    return spawn_points.get_children()[randi_range(0, num_spawn_points-1)].global_position

func get_active_map_rid() -> RID:
    """
    Simple wrapper for getting this nav regions map rid so we can setup our nav agents correctly
    """
    return get_navigation_map()
