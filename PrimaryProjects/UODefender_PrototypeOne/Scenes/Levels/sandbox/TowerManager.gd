class_name TowerManager
extends Node3D

@onready var mouse_interaction: MouseInteraction = $"../MouseInteraction"
@onready var bank: Node3D = $"../Bank"
@onready var tower_tile_gmap: TowerSpaceGridMap = $TowerTileGridMap

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    init_signals()
    
func init_signals() -> void:
    # Tower space grid "action button" click
    mouse_interaction.clk_twr_space_grid.connect(process_towerspace_click)
    # Tower right click
    mouse_interaction.r_clk_twr.connect(process_tower_r_click)

func process_towerspace_click(collision_dict: Dictionary) -> void:
    # {TowerSpaceGridMap.TOWER_SPACE_AVAIL, TowerSpaceGridMap.TOWER_SPACE_DEBRIS, TowerSpaceGridMap.TOWER_SPACE_TAKEN, TowerSpaceGridMap.TOWER_SPACE_INVALID}
    if collision_dict and collision_dict.collider is TowerSpaceGridMap:
        var tile_type: TowerSpaceGridMap.TILE_TYPE = collision_dict.collider.get_tile_type(collision_dict.position)
        match tile_type:
            TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_AVAIL:
                var tower_scene: PackedScene = load("res://Scenes/Tower/tower.tscn")
                var new_tower := tower_scene.instantiate()
                var gold_withdrawn: bool = bank.withdraw(new_tower.placement_cost)
                if gold_withdrawn:
                    # I have to spawn a tower- or call a menu - or w/e.. For now I will try to change the tile map to the correct type and then spawn a tower there                
                    var tile_changed: bool = collision_dict.collider.change_tile(collision_dict.position, TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_TAKEN)
                    if not tile_changed:
                        print("Unexpected error on a condition I'm not really catching. Expect weirdness. WD9VM096")
                    add_child(new_tower)
                    new_tower.global_position = collision_dict.collider.get_tile_center(collision_dict.position)
            TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_DEBRIS:
                pass
            TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_TAKEN:
                pass
            TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_INVALID:
                pass

func process_tower_r_click(collision_dict: Dictionary) -> void:
    # Sanity check
    if not (collision_dict and collision_dict.collider is Tower):
        print("Cannot process tower right click... This signal should never have been called. KMM0P19J")
        return
    var tower := collision_dict.collider as Tower
    var refund_amt: int = tower.get_refund_amt()
    bank.deposit(refund_amt)
    tower.queue_free()
    var placement_pos: Vector3 = collision_dict.position
    placement_pos.y = 0
    tower_tile_gmap.change_tile(placement_pos, TowerSpaceGridMap.TILE_TYPE.TOWER_SPACE_AVAIL)
    
    # NOW I NEED TO FLIP OUR TILE BACK TO 'OPEN SPACE' /// need to abstract out some of this?
