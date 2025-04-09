extends Node3D

@export var max_health: int = 10
@export var current_health: int:
    set(health_in):
        current_health = health_in
        update_health_label()
        if current_health <= 0:
            get_tree().reload_current_scene()            
    get:
        return current_health

@onready var health_label: Label3D = $HealthLabel

func update_health_label():
    health_label.text = str(current_health) + "/" + str(max_health)
    var curr_ratio = float(current_health)/float(max_health)
    # health_label.modulate = Color((1-curr_ratio), curr_ratio, curr_ratio)
    health_label.modulate = Color.RED.lerp(Color.WHITE, curr_ratio)
    
func _ready():
    current_health = max_health

func take_dmg(dmg: int) -> void:
    current_health -= dmg
