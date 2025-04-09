extends CharacterBody2D


signal laser_fired(position, dir)
signal grenade_fired(position, dir)

@export var max_speed: int = 700
@export var speed: int = 700

var surface_speed: float = speed
var direction: Vector2 = Vector2.ZERO
var last_direction: Vector2 = direction
var facing_direction: Vector2 = Vector2.RIGHT

var can_laser: bool = true
var can_grenade: bool = true

var pos: Vector2 = Vector2.ZERO

func move():
    direction = Input.get_vector("left", "right", "up", "down")
    if direction != Vector2.ZERO:
        last_direction = direction
    
    # Rotate
    look_at(get_global_mouse_position())
    
    velocity = direction * speed
    move_and_slide()
    Globals.player_pos = global_position

func get_rand_firing_pos() -> Vector2:
    var marker_list = $LaserStartPositions.get_children()
    var selected_marker = marker_list[randi() % marker_list.size()]
    return selected_marker.global_position
    

func hit(power=1):
    print("HIT WITH POWER: " + str(power))
    Globals.health -= power

func _ready():
    Globals.player_pos = global_position
    var ammo_restock_timer = $AmmoRestock as Timer
    ammo_restock_timer.start()
    

func _process(__delta):
    move()
    facing_direction = (get_global_mouse_position() - position).normalized()
    if (Input.is_action_pressed("primary action")) and can_laser and Globals.laser_amount > 0:
        $LaserNoise.play()
        Globals.laser_amount -= 1
        pos = get_rand_firing_pos()
        laser_fired.emit(pos, facing_direction)
        $LaserParticles.emitting = true
        can_laser = false
        $Timers/AttackPrimaryTimer.start()
        
    if (Input.is_action_pressed("secondary action")) and can_grenade and Globals.grenade_amount > 0:
        Globals.grenade_amount -= 1
        $GrenadeLaunch.play()
        pos = get_rand_firing_pos()
        grenade_fired.emit(pos, facing_direction)
        can_grenade = false
        $Timers/AttackSecondaryTimer.start()

func _on_attack_primary_timer_timeout():
    can_laser = true

func _on_attack_secondary_timer_timeout():
    can_grenade = true

func _on_ammo_restock_timeout():
    if (Globals.laser_amount + Globals.laser_amount_rate) < Globals.laser_amount_max:
        Globals.laser_amount += Globals.laser_amount_rate
    if (Globals.grenade_amount + Globals.grenade_amount_rate) < Globals.grenade_amount_max:
        Globals.grenade_amount += Globals.grenade_amount_rate
