extends MarginContainer

@export var starting_gold := 175
@export var current_gold := 0:
    set(val):
        current_gold = max(0, val)
        gold_label.text = "Gold: " + str(current_gold)
    get:
        return current_gold

@onready var gold_label: Label = $GoldLabel

func _ready():
    current_gold = starting_gold
