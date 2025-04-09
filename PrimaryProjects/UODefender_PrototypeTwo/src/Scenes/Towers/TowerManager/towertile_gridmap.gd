class_name TowerTileGridmap
extends GridMap

# TowerTileTypes - Mapping of our gridmap's tiles so that we can act on them
enum TILE_TYPE {TOWER_SPACE_AVAIL=0, TOWER_SPACE_DEBRIS=1, TOWER_SPACE_TAKEN=2, TOWER_SPACE_INVALID=3}
    
func get_tile_type(pos: Vector3) -> TILE_TYPE:
    var translated_pos: Vector3i = local_to_map(to_local(pos))
    var cell_item: int = get_cell_item(translated_pos)
    return cell_item as TILE_TYPE

func get_tile_center(pos: Vector3) -> Vector3:
    var translated_pos: Vector3i = local_to_map(to_local(pos))
    var cell_item_center_global: Vector3 = to_global(map_to_local(translated_pos))
    return cell_item_center_global
    
func change_tile(pos: Vector3, to_tile_type: TILE_TYPE) -> void:
    """ 
    Change tile at position pos to a valid tile_type
    Returns true if successfully flipped (it was what they thought and turned into what they thought)
    """    
    # Sanity check - did they enter valid tile types?
    assert(to_tile_type in TILE_TYPE.values(), "Invalid tile type. H1E907PT")
    
    # Get Cell Info & set it to our new tile type
    var translated_pos: Vector3i = local_to_map(to_local(pos))
    set_cell_item(translated_pos, to_tile_type)
