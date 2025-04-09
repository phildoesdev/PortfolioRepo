extends Node2D
class_name LevelParent

var rot_speed: float = 180

var laser_scene: PackedScene = preload('res://scenes/projectiles/laser.tscn')
var grenade_scene: PackedScene = preload('res://scenes/projectiles/grenade.tscn')
var item_scene: PackedScene = preload("res://scenes/items/item.tscn")
    
func _ready():
    for container in get_tree().get_nodes_in_group("container"):
        container.connect("spawn_items", _on_spawn_items)
    for enemy in get_tree().get_nodes_in_group("Enemies"):
        if "laser_fired" in enemy:
            enemy.connect("laser_fired", _on_laser_fired)
        
func _on_spawn_items(pos_out, _current_direction):
    var item = item_scene.instantiate()
    item.position = pos_out
    item.direction = _current_direction
    $Items.call_deferred('add_child', item)

func _process(__delta):
    if (Input.is_action_pressed("left")):
        pass
        
func create_laser(pos, dir):
    var laser_instance = laser_scene.instantiate() as Area2D
    laser_instance.position = pos
    laser_instance.rotation_degrees = rad_to_deg(dir.angle())
    laser_instance.direction = dir
    $Projectiles.add_child(laser_instance)

func _on_player_laser_fired(pos, dir):
    create_laser(pos, dir)
    
func _on_player_grenade_fired(pos, dir):
    var grenade_instance = grenade_scene.instantiate() as RigidBody2D
    grenade_instance.position = pos
    grenade_instance.linear_velocity = dir * grenade_instance.speed
    $Projectiles.add_child(grenade_instance)

func _on_laser_fired(pos, dir):
    create_laser(pos, dir)
