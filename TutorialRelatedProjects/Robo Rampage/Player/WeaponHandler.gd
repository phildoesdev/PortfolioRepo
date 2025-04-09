extends Node3D

@export var weapon_1: Node3D
@export var weapon_2: Node3D
@export var current_weapon_index: int = 0


var can_change_wep: bool = true

func _ready() -> void: 
    equip(weapon_1)
    current_weapon_index = 0
    

func equip(active_weapon: Node3D) -> void:
    # Loop alll children of this node (should all be weapons)
    for child in get_children():
        if child == active_weapon:
            child.visible = true
            child.set_process(true)
            child.ammo_handler.update_ammo_label(child.ammo_type)
        else:
            child.visible = false
            child.set_process(false)

func _unhandled_input(event: InputEvent) -> void:
    if event.is_action_pressed("weapon_1"):
        equip(weapon_1)
    if event.is_action_pressed("weapon_2"):
        equip(weapon_2)
    if event.is_action_pressed("next_weapon"):
        next_weapon(1)
    if event.is_action_pressed("last_weapon"):
        next_weapon(-1)

func next_weapon(direction: int) -> void:
    if not can_change_wep:
        return
    can_change_wep = false
    current_weapon_index = wrapi(current_weapon_index+direction, 0, get_child_count())
    equip(get_child(current_weapon_index))

func get_current_index() -> int:
    var return_val := 0
    for i in get_child_count():
        if get_child(i).visible == true:
            return_val = i
            break
    return return_val

func _on_weapon_change_timer_timeout() -> void:
    can_change_wep = true
