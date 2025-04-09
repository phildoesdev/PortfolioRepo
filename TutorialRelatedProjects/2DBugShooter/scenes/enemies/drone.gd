extends CharacterBody2D

var direction: Vector2 = Vector2.RIGHT
var speed: int = 300
var max_speed: int = 3000
var hit_radius: int = 500
var hit_dmg: int = 500
var health: int = 50

var can_move: bool = true
var active_seek: bool = false

var can_hit: bool = true
var can_take_dmg: bool = true



func _ready():
    $Node/DmgTimer.start()

func move(delta):
    if active_seek:
        look_at(Globals.player_pos)
        direction = (Globals.player_pos - position).normalized()
    # else:
        # direction =  Vector2.RIGHT
        
    velocity = direction * speed
    var collision = move_and_collide(velocity*delta)
    if collision:
        explode_self()

func find_hit():
    for target in get_tree().get_nodes_in_group("Entity"):
        if target != $".":
            if "hit" in target and position.distance_to(target.position) <= hit_radius:
                target.hit(hit_dmg)

func _process(delta):
    if can_move:
        move(delta)

func explode_self():
    can_move = false
    $AnimationPlayer.play("explosions")

func hit(modifier=1):
    $Drone.material.set_shader_parameter("progress", .2)
    activate_seek_go()
    if can_take_dmg:
        health -= modifier
        can_take_dmg = false
    
    if health <= 0:
        explode_self()

func _on_notice_area_body_entered(_body):
    if not active_seek:
        activate_seek_go()

func activate_seek_go():
    $AudioStreamPlayer2D.play()
    speed += 300
    active_seek = true
    var speed_tween = get_tree().create_tween() as Tween
    speed_tween.tween_property(self, 'speed', max_speed, 7)
    

func _on_dmg_timer_timeout():
    can_take_dmg = true
    $Drone.material.set_shader_parameter("progress", 0)
