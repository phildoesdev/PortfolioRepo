extends CharacterBody2D

signal laser_fired(position, dir)

var player_nearby: bool = false
var can_laser: bool = true
var laser_arm: bool = true

var health: int = 150
var can_be_hit: bool = true

func _ready():
    $Timers/LaserCooldown.start()
    $Timers/DmgCooldown.start()

func hit(power=1):
    if not can_be_hit:
        return
    health -= power
    print(" health: " + str(health))
    # var light_tween: Tween = get_tree().create_tween()
    
    $Sprite2D.material.set_shader_parameter("progress", .2)
    if health <= 0:
        queue_free()

func _process(_delta):
    if player_nearby:
        look_at(Globals.player_pos)
        if can_laser:
            laser_arm = not laser_arm
            var pos: Vector2 = $LaserSpawnPositions.get_children()[int(laser_arm)].global_position
            var direction: Vector2 = (Globals.player_pos - position).normalized()
            laser_fired.emit(pos, direction)
            can_laser = false

func _on_attack_area_body_entered(_body):
    player_nearby = true

func _on_attack_area_body_exited(_body):
    player_nearby = false

func _on_laser_cooldown_timeout():
    can_laser = true

func _on_dmg_cooldown_timeout():
    # This feels bad because I 'miss" random dmg that I am expecting to be there. Who gives a shit if WE can one-shot. Should only exist on PLAYER.
    can_be_hit = true
    $Sprite2D.material.set_shader_parameter("progress", 0)
