extends Node3D

var starting_health: int = 10
var current_health: int = 10

var current_score: int = 0

@onready var health_label: Label = %HealthLabel
@onready var score_label: Label = $Overlay/MarginContainer/Score/ScoreLabel

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    current_health = starting_health
    update_health_label()
    update_score_label()

func take_dmg(amt: int) -> void:
    var new_health: int = current_health - amt
    if new_health < 0:
        new_health = 0
    if new_health <= 0:
        print("YOU DEAD")
    current_health = new_health
    update_health_label()

func update_player_score(points: int=0) -> void:
    current_score += points
    update_score_label()

func update_health_label() -> void:
    health_label.text = "Health: " + str(current_health)

func update_score_label() -> void:
    score_label.text = "Score: " + str(current_score)
