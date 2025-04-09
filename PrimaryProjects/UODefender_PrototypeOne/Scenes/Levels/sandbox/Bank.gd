extends Node3D

@export var starting_gold := 2500
var current_gold := 0

enum transaction_types {WITHDRAWL, DEPOSIT}

@onready var gold_label: Label = %GoldLabel

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    current_gold = starting_gold
    update_overlay()
    set_process(false)

func deposit(amt: int) -> bool:
    return process_request(amt, transaction_types.DEPOSIT)

func withdraw(amt: int) -> bool:
    return process_request(-amt, transaction_types.WITHDRAWL)
    
func process_request(amt: int, trans_type: transaction_types) -> bool:
    var new_balance: int = current_gold + amt
    if new_balance < 0:
        return false
    if trans_type == transaction_types.WITHDRAWL:
        prints("Withdrawing", abs(amt), "gold from bank")
    elif trans_type == transaction_types.DEPOSIT:
        prints("Depositing", amt, "gold into bank")
    current_gold = new_balance
    update_overlay()
    return true

func update_overlay() -> void:
    gold_label.text = "Gold: " + str(current_gold)
