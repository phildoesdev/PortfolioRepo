extends PathFollow2D

var player_near: bool = false
var speed: float = 0.05
var scout_speed: float = 0.08
var attack_speed: float = 0.01

var attack_power: int = 50

@onready var barrel1: Line2D = $Turret/Barrel1/Laser1
@onready var barrel2: Line2D = $Turret/Barrel2/Laser2

@onready var gunshot1: Sprite2D = $Turret/GunShot
@onready var gunshot2: Sprite2D = $Turret/GunShot2


func fire():
    Globals.health -= attack_power
    gunshot1.modulate.a = 1
    gunshot2.modulate.a = 1
    
    var tween = create_tween()
    tween.set_parallel(true)
    tween.tween_property(gunshot1, "modulate:a", 0, randf_range(0.1,0.5))
    tween.tween_property(gunshot2, "modulate:a", 0, randf_range(0.1,0.5))

func move(delta):
    progress_ratio += speed * delta
    
func _ready():
    # barrel2.add_point($Turret/Barrel2.target_position)
    pass

func _process(delta):
    move(delta)
    if player_near:
        $Turret.look_at(Globals.player_pos)

func _on_notice_area_body_entered(_body):
    player_near = true
    speed = attack_speed
    $LaserCharge.play("LaserLoad")

func _on_notice_area_body_exited(_body):
    player_near = false
    speed = scout_speed
    $LaserCharge.stop()
