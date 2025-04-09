class_name Projectile
extends Area3D

var direction: Vector3
var target: Node3D
var projectile_speed: float = 10.0
var dmg: int = 1

# how long before we automatically queue_free out projectile
@export var death_timer: Timer
var life_span_s: float = 99.0

func _ready() -> void:
    body_entered.connect(target_hit)
    create_timers()

func target_hit(body: Node3D) -> void:
    if body == target:
        body.take_dmg(dmg)
        die()
    
func create_timers() -> void:
    ## Death Timer -- How long we wait until we queue free our projectile
    if death_timer == null:
        death_timer = Timer.new()
        death_timer.wait_time = life_span_s
        add_child(death_timer)
        death_timer.start()
    death_timer.timeout.connect(death_timer_timeout)
    
func _physics_process(delta: float) -> void:
    if not target:
        # No target- clean up. It will be our parent's responsibility for re-targeting, if desired
        die()
        set_process(false)
        return

    direction = global_position.direction_to(target.global_position)
    direction.y = 0
    global_position += direction*projectile_speed*delta

func death_timer_timeout() -> void:
    die()
    
func die() -> void:
    """ Useful if I want to do stats, or w/e else... we will see"""
    queue_free()
