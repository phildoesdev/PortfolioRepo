extends Node


signal health_change
signal laser_change
signal grenade_change

var player_hit_sound: AudioStreamPlayer2D


var health: int = 100: 
    get:
        return health
    set(val):
        if not can_take_dmg:
            return health
        if val < health:
            player_hit_sound.play()
            
        if val <= 0:
            can_take_dmg = false
            player_invul_timer()
        if val >= 100:
            health = 100
        elif val <= 0:
            health = 0
        else:
            health = val
        health_change.emit()
        print(health)

var laser_amount_max: int = 1200
var laser_amount_rate: int = 125
var laser_amount: int = 120:
    get:
        return laser_amount
    set(val):
        laser_amount = val
        laser_change.emit()
        

var grenade_amount_max: int = 100
var grenade_amount_rate: int = 10
var grenade_amount: int = 100: 
    get:
        return grenade_amount
    set(val):
        grenade_amount = val
        grenade_change.emit()

var player_pos: Vector2

var attack_power: float = 10


var can_take_dmg: bool = true

func player_invul_timer():
    await get_tree().create_timer(0.5).timeout
    can_take_dmg = true

# it is possible to create timers in script like this, with await get_tree().create_timer(0.5).timeout ... and then that func will hold on until that timeout finishes...

func _ready():
    player_hit_sound = AudioStreamPlayer2D.new()
    player_hit_sound.stream = load("res://resources/audio/moomph03.wav")
    add_child(player_hit_sound)
