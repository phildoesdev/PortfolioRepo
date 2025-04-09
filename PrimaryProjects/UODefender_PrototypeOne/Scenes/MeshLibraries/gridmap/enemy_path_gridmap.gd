class_name EnemyPathGridMap
extends GridMap

enum TILE_TYPE {ENEMY_PATH_EXIT=0, ENEMY_PATH_INVALID=1, ENEMY_PATH_SPAWN=2, ENEMY_PATH_VALID=3}

# list of gridmap coords to all TILE_TYPE.ENEMY_PATH_SPAWN
var gridmap_spawn_cells: Array
# list of gridmap coords to all TILE_TYPE.ENEMY_PATH_EXIT
var gridmap_exit_cells: Array

func _ready() -> void:
    gridmap_spawn_cells = calculate_spawn_points()
    gridmap_exit_cells = calculate_exit_points()

func calculate_spawn_points() -> Array:
    return get_used_cells_by_item(TILE_TYPE.ENEMY_PATH_SPAWN)
    
func calculate_exit_points() -> Array:
    return get_used_cells_by_item(TILE_TYPE.ENEMY_PATH_EXIT)

func get_rand_spawn_point() -> Vector3:
    """Gets a global position for a valid spawn point
    """
    printt("Wtfm8:",gridmap_spawn_cells, gridmap_exit_cells)
    if not gridmap_spawn_cells:
        return Vector3.ZERO
    else:
        return map_to_local(gridmap_spawn_cells.pick_random())
    
func get_rand_exit_point() -> Vector3:
    if not gridmap_exit_cells:
        return Vector3.ZERO
    else:
        return map_to_local(gridmap_exit_cells.pick_random())
