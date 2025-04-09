class_name TowerMouseInteraction3d
extends MouseInteraction3d
"""
Extends our basic raycast, point detection from our custom mouse interaction class
"""

# Signals tower manager cares about
signal twr_tile_gridmap_lclick(coll_dict: Dictionary)
signal twr_tile_gridmap_rclick(coll_dict: Dictionary)
signal twr_lclick(coll_dict: Dictionary)
signal twr_rclick(coll_dict: Dictionary)

func _ready() -> void:
    debug_mode = false

func _mouse_interaction() -> void:
    """
    The method intended to be overwrirtten. Gets called in MouseInteraction3d's 
        physics process
    """
    var coll_dict:Dictionary = cast_raycast_to_cursor()
    # Collided w/ something
    if coll_dict:
        var collider = coll_dict.collider
        change_mouse_cursor(collider, coll_dict.position)
        if Input.is_action_just_pressed("action_button"):
            # Pass click info along to the TowerManager
            if collider is TowerTileGridmap:
                twr_tile_gridmap_lclick.emit(coll_dict)
            if collider is Tower:
                twr_lclick.emit(coll_dict)
        if Input.is_action_just_pressed("unaction_button"):
            if collider is Tower:
                twr_rclick.emit(coll_dict)

func change_mouse_cursor(collider: Object, coll_position: Vector3) -> void:
    """
    Depending on the type of collider, try to predict the appropriate cursor
    Defaults to "mouse_default"
    """
    ## Mouse Icon Defaults
    var mouse_default = Input.CURSOR_ARROW
    var mouse_hover = Input.CURSOR_POINTING_HAND
    var mouse_bad = Input.CURSOR_HELP
    if collider is TowerTileGridmap:
        var tile_type = collider.get_tile_type(coll_position)
        match tile_type:
            TowerTileGridmap.TILE_TYPE.TOWER_SPACE_AVAIL, TowerTileGridmap.TILE_TYPE.TOWER_SPACE_TAKEN:
                Input.set_default_cursor_shape(mouse_hover)
            TowerTileGridmap.TILE_TYPE.TOWER_SPACE_INVALID, TowerTileGridmap.TILE_TYPE.TOWER_SPACE_DEBRIS:
                Input.set_default_cursor_shape(mouse_bad)
            _:
                Input.set_default_cursor_shape(mouse_default)
        return         
    if collider is Tower:
        Input.set_default_cursor_shape(mouse_hover)
        return
    Input.set_default_cursor_shape(mouse_default)
        
