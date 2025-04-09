extends CharacterBody2D

var speed: int = 500
var direction: Vector2 = Vector2.RIGHT

var health: int = 15

var notice_player: bool = false
var attack_player: bool = false

var can_attack: bool = true
var can_pursue: bool = false
var can_take_dmg: bool = false


var is_firing: bool = false

signal laser_fired(pos, dir)

func fire():
    if not is_firing:
        is_firing = true
        $BugAnimation.stop()
        $BugAnimation.play("attack")
        await $BugAnimation.animation_finished

        direction = (Globals.player_pos - position).normalized()
        var pos: Vector2 = $FiringPos/FirePointMarker.global_position
        var dir: Vector2 = (Globals.player_pos - position).normalized()
        laser_fired.emit(pos, dir)
        is_firing = false

func pursue_player():
    direction = (Globals.player_pos - position).normalized()
    velocity = direction * speed
    move_and_slide()
    
func hold_ground():
    $BugAnimation.stop()
    
func move():
    if not is_firing:
        $BugAnimation.play("walk")
    look_at(Globals.player_pos)
    pursue_player()

func _on_notice_area_2d_body_entered(_body):
    notice_player = true
    $Timers/NoticeTimer.start()

func _on_notice_area_2d_body_exited(_body):
    notice_player = false
    can_pursue = false
    $Timers/NoticeTimer.stop()

func _on_attack_area_2d_body_entered(_body):
    attack_player = true
    $Timers/AttackTimer.start()

func _on_attack_area_2d_body_exited(_body):
    attack_player = false

func _ready():
    $Timers/AttackTimer.start()
    $Timers/DmgTimer.start()
    
func _process(__delta):	
    if notice_player and can_pursue:
        move()
    else:
        hold_ground()

    if attack_player and can_attack and can_pursue:
        fire()
        can_attack = false

func _on_attack_timer_timeout():
    can_attack = true

func _on_notice_timer_timeout():
    can_pursue = true

func _on_dmg_timer_timeout():
    $BugAnimation.material.set_shader_parameter("progress", 0)
    can_take_dmg = true

func hit(modifier=1):
    $AudioStreamPlayer2D.play()
    $Particles/HitParticles.emitting = true
    $BugAnimation.material.set_shader_parameter("progress", .2)
    can_take_dmg = false
    health -= modifier
    
    if health <= 0:
        can_attack = false
        can_pursue = false
        $BugAnimation.stop()
        await get_tree().create_timer(0.25).timeout
        queue_free()
    
