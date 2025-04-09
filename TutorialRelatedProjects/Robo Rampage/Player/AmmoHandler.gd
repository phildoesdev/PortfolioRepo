class_name AmmoHandler

extends Node

@export var ammo_label: Label

var currentlyActive: ammo_type

enum ammo_type {
    BULLET,
    SMALL_BULLET
}

var ammo_storage := {
    ammo_type.BULLET: 24,
    ammo_type.SMALL_BULLET: 125
}

func has_ammo(type: ammo_type) -> bool:
    return ammo_storage[type] > 0

func use_ammo(type: ammo_type) -> void:
    if has_ammo(type):
        ammo_storage[type] -= 1 
        update_ammo_label(type)
    
func add_ammo(type, amt):
    ammo_storage[type] += amt

func update_ammo_label(type: ammo_type) -> void:
    currentlyActive = type
    ammo_label.text = str(ammo_storage[type])

func update_ammo_label_if_active(type: ammo_type) -> void:
    if currentlyActive == type:
        update_ammo_label(type)
