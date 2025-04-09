class_name PlayerBank
extends Node3D
"""
It feels quite possible that this will turn into a singleton and be referenced quite differnetly in ther future
    but as it stands, that should not be too much of a mess 
"""

enum transaction_types {WITHDRAWL, DEPOSIT, BALANCE_INQUERY}

var gold_pieces:int = 0     # Gold Current

signal balance_changed(gp)

func _init(starting_gold: int) -> void:
    gold_pieces = starting_gold
    balance_changed.emit(gold_pieces)   # If anyone is listening we should let them know

func deposit_gp(amt: int) -> bool:
    if amt <= 0:
        print_rich("[bgcolor=red][b]YOU SNEAKY BUGGER 1[/b][/bgcolor]")
        return false
    get_trans_msg(amt, transaction_types.DEPOSIT)
    return process_transaction(amt)

func withdraw_gp(amt: int) -> bool:
    if amt > 0:     # Withdrawls should be negative, lets force this
        amt *= -1
    get_trans_msg(amt, transaction_types.WITHDRAWL)
    return process_transaction(amt)

func balance_gp() -> int:
    get_trans_msg(0, transaction_types.BALANCE_INQUERY)
    return gold_pieces
    
func process_transaction(amt: int) -> bool:
    var new_balance: int = gold_pieces + amt
    if new_balance < 0:
        return false
    gold_pieces = new_balance
    balance_changed.emit(gold_pieces)
    return true
    
func get_trans_msg(amt: int, trans_type: transaction_types, print_msg=true) -> String:
    var return_value: String = "-undefined transaction- "
    if trans_type == transaction_types.WITHDRAWL:
        return_value = "Withdrawing: " + str(abs(amt)) + " gold from your bank"
    elif trans_type == transaction_types.DEPOSIT:
        return_value = "Depositing: " + str(amt) + " gold into your bank"
    elif trans_type == transaction_types.BALANCE_INQUERY:
        return_value = "Your current balance is: " + str(gold_pieces)
    if print_msg:
        print(return_value)
    return return_value
