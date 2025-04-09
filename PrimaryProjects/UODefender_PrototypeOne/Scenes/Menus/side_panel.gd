extends Control

var menu_open: bool = false

@onready var menu_animations: AnimationPlayer = $MenuAnimations

func _input(event: InputEvent) -> void:
    if Input.is_action_just_pressed("special_action"):
        menu_slide_open()
    if Input.is_action_just_pressed("special_action_negative"):
        menu_slide_close()

func menu_slide_open() -> void:
    if not menu_open:
        menu_open = true
        menu_animations.play("show")
    
func menu_slide_close() -> void:
    if menu_open:
        menu_open = false
        menu_animations.play_backwards("show")
        await menu_animations.animation_finished
