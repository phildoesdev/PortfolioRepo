class_name EnemyManager
extends Node3D

# Base enmy
var enemy_scene: PackedScene = load("res://src/Scenes/Enemies/enemy.tscn")

# Internal tracking
var heartbeat_timer: Timer
var heartbeat_rate: float = 1.0

# Nav regions/meshes
@onready var enemy_spawn_meshes: Node3D = $EnemySpawnMeshes
var nav_mesh_all: EnemyNavigationRegion
var nav_mesh_container: Array

# Enemy mgmt
@onready var enemy_container: Node3D = $Enemies
var enemy_definitions: EnemyDefinitions     # Holds all info on enemy definitions... Allows us to spawn enemies efficiently

func _ready() -> void:
    enemy_definitions = EnemyDefinitions.new()
    find_nav_regions()
    init_spawn_timer()
    
func find_nav_regions() -> void:
    """
    We need to find and grab reference to all possible nav meshes for enemy spawns 
    """
    # Find 'all mesh', give me some valid ids for meshes to spawn from
    assert(enemy_spawn_meshes.get_child_count() > 0, "Create nav meshes for this level. KUWV4W6B")
    # See what we have for possible navigation regions/meshes
    for child_region in enemy_spawn_meshes.get_children():
        # Sanity
        if not child_region is EnemyNavigationRegion:
            continue
        # Give me a list of usable meshes and a clean reference to the master
        if child_region.is_mesh_all:
            nav_mesh_all = child_region
        else:
            nav_mesh_container.append(child_region)
    # If nav enemy spawn meshes only has the master 'all' nav mesh, we'll dump that into our spawn mesh array to make things flow nicely later
    if not len(nav_mesh_container):
        nav_mesh_container.append(nav_mesh_all)

func init_spawn_timer() -> void:
    """
    Right now this starts our clock... nothing could spawn at an interval faster than this beats
    Spawning enemy at heartbeat_rate monsters/seconds 
    """
    heartbeat_timer = Timer.new()
    add_child(heartbeat_timer)
    heartbeat_timer.wait_time = heartbeat_rate
    heartbeat_timer.start()
    heartbeat_timer.timeout.connect(heartbeat_timeout)
    
func heartbeat_timeout() -> void:
    spawn_enemy(EnemyDefinitions.ENEMIES.ZOMBIE)
    spawn_enemy(EnemyDefinitions.ENEMIES.SKELETON)

func spawn_enemy(enemy_type: EnemyDefinitions.ENEMIES) -> void:
    # Get nav mesh information
    var active_mesh: EnemyNavigationRegion = get_rand_spawn_mesh()
    var active_nav_map_rid: RID = active_mesh.get_active_map_rid()
    var spawn_pos: Vector3 = active_mesh.get_spawn_point()
    var dest_point: Vector3 = active_mesh.get_destination_point()
    
    # Build enemy
    var new_enemy: EnemyBody = enemy_scene.instantiate()
    enemy_container.add_child(new_enemy)
    new_enemy.initialize_enemy(enemy_definitions.get_factsheet(enemy_type))
    new_enemy.set_nav_map_rid(active_nav_map_rid)
    new_enemy.set_spawn_position(spawn_pos)
    new_enemy.set_destination(dest_point)
    GameStatisticsSingleton.update_total_enemies_spawned()
    
    # Eventually we're going to be switching to the universal map if this happens, and then, check destination again
    #   and if it is still not reachable we need to put them on a different nav layer so they'll just walk thru obstacles, or we can destroy random ones, or w/e...
    if not new_enemy.is_destination_reachable():
        print("Cannot reach destination on this mesh, we should be picking a new one. PMOYWDT9")
        # assert(1==0, "Cannot reach destination on this mesh, we should be picking a new one. PMOYWDT9")
    
func get_rand_spawn_mesh() -> EnemyNavigationRegion:
    var spawn_path_idx: int = randi_range(0, len(nav_mesh_container)-1)
    return nav_mesh_container[spawn_path_idx]




"""

# Eventually this could be loaded in from a file, or from a script on the lvl apge that defines the waves, what can be in them, how hard they should be, and things like this
var waves: Array = [
    {
        "sort_order": 1
        ,"time_between_groupings_s": 2
        ,"min_group_size": 1
        ,"max_group_size": 1
    }
]
var usable_enemies: Array = [
        {
            "name": "zombie"
            ,"path": 'res://src/Scenes/Enemies/zombie.tscn'
            ,"resist_type": ["RESISTS.FIRE", "RESISTS.BLAHBLAH"]
        }
    ]

"""
