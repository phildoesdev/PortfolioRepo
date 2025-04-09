class_name TowerManager
extends Node3D

"""
    Listen for signals from the mouse interaction class
"""
# Internal mgmt
var towers_container: Node3D    # A managable place to spawn towers
var mouse_interaction3d: TowerMouseInteraction3d
var twr_scn: PackedScene = preload("res://src/Scenes/Towers/mage_tower.tscn") ## res://src/Scenes/Towers/tower.tscn

# We need to know about the player so that we can charge for towers and such
var player: Player
var player_group_name: String = "Player"

var bottom_menu_bar: BottomMenuBarControl
var bottom_menu_bar_group_name = "BottomMenuBarControl"

func _ready() -> void:
    towers_container = Node3D.new()
    add_child(towers_container)
    mouse_interaction3d = TowerMouseInteraction3d.new()
    add_child(mouse_interaction3d)
    init_signals()
    find_nodes_by_groups()

func find_nodes_by_groups() -> void:
    player = get_tree().get_first_node_in_group(player_group_name)
    assert(player,"KOQ6JPMP. Unable to locate player node under group '"+player_group_name+"'")
    bottom_menu_bar = get_tree().get_first_node_in_group(bottom_menu_bar_group_name)
    assert(player,"KOQ6JPMP. Unable to locate bottom_menu_bar node under group '"+bottom_menu_bar_group_name+"'")
    
        
func init_signals() -> void:
    """
    Wrapper for initializing all the different signals we want to connect to,
        mainly mouse interaction currently
    """
    mouse_interaction3d.twr_tile_gridmap_lclick.connect(process_gridmap_lclick)
    # mouse_interaction3d.twr_tile_gridmap_rclick.connect(process_gridmap_rclick)
    mouse_interaction3d.twr_lclick.connect(process_twr_lclick)
    mouse_interaction3d.twr_rclick.connect(process_twr_rclick)
    
func process_gridmap_lclick(coll_dict: Dictionary) -> void:
    """
    This gets called when someone clicks a tower tile gridmap, this will place a tower,
        clear debris, or pass
    """
    if coll_dict and coll_dict.collider is TowerTileGridmap:
        var tile_type: TowerTileGridmap.TILE_TYPE = coll_dict.collider.get_tile_type(coll_dict.position)
        match tile_type:
            TowerTileGridmap.TILE_TYPE.TOWER_SPACE_AVAIL:
                """
                Check player's bank for enough cash, withdraw that cash
                Change the tile to 'Tower_space_taken'
                Instantiate tower, set it's global position
                """
                var twr := twr_scn.instantiate()
                if not player.withdraw_gp(twr.get_gold_cost()):    # Attempt to charge the player
                    print_rich("[bgcolor=white][b]YOU DO NOT HAVE TH FUNDS[/b][/bgcolor]")
                    return
                twr.initialize_tower(coll_dict.collider, coll_dict.position)
                twr.enemy_killed.connect(update_gui_enemies_killed) ## Stupid way we are keeping track of kills atm
                towers_container.add_child(twr)
                # Set the tower's position correctly
                twr.place_tower(coll_dict.collider.get_tile_center(coll_dict.position))
                GameStatisticsSingleton.update_total_towers_placed()
            TowerTileGridmap.TILE_TYPE.TOWER_SPACE_DEBRIS:
                pass
            TowerTileGridmap.TILE_TYPE.TOWER_SPACE_TAKEN:
                pass
            TowerTileGridmap.TILE_TYPE.TOWER_SPACE_INVALID:
                pass

func process_twr_lclick(coll_dict: Dictionary) -> void:
    """
    Gets called when a signal emits from TowerMouseInteraction3d.
    Pops a menu with info on this tower.
    """
    pass
    
func process_twr_rclick(coll_dict: Dictionary) -> void:
    """
    Gets called when a signal emits from TowerMouseInteraction3d.
    Prompts a user to dismiss a tower
    """
    player.deposit_gp(coll_dict.collider.get_gold_refund())
    coll_dict.collider.dismiss()
    GameStatisticsSingleton.update_total_towers_dismissed()
    
func update_gui_enemies_killed() -> void:
    pass
    # bottom_menu_bar.update_enemies_killed(1)
