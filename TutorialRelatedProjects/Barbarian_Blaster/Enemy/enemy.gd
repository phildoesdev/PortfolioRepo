extends PathFollow3D

# Stats
@export var gold_dropped := 5
@export var speed: float = 10.0
@export var dmg_out: int = 1
@export var max_health:= 10
@export var current_health := 0:
    set(val):
        if val < current_health:
            animation_player.play("TakeDamage")
            if val <= 0:
                await animation_player.animation_finished
                bank.current_gold += gold_dropped
                queue_free()
        current_health = val
    get:
        return current_health
        
# 
@onready var base = get_tree().get_first_node_in_group("base")
@onready var animation_player: AnimationPlayer = $AnimationPlayer

@onready var bank = get_tree().get_first_node_in_group("bank")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    # current_health = max_health
    pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
    progress += delta*speed
    if self.progress_ratio >= 1:
        base.take_dmg(dmg_out)
        set_process(false)
        queue_free()

func get_hit(dmg: int) -> void:
    current_health -= dmg
