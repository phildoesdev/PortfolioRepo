extends StaticBody3D

var walk_speed: float = 0.1
var ani_speed: float = 0.75
var dmg_out: float = 1.0
var point_value: float = 10.0

@onready var display_sprite_main_2d: Sprite2D = $"../2DScreen/SpriteContainer/DisplaySpriteMain2d"
@onready var skeleton: PathFollow3D = $"../.."
@onready var animation_player: AnimationPlayer = $"../2DScreen/SpriteContainer/DisplaySpriteMain2d/AnimationPlayer"


# Keep track of our direction so that we can choose the correct animation
var last_pos: Vector3 = Vector3.ZERO
var current_direction: Vector3 = Vector3.ZERO

func _physics_process(delta: float) -> void:
    last_pos = global_position
    skeleton.progress += walk_speed*delta
    if skeleton.progress_ratio >= .98:
        skeleton.progress = 0
        # skeleton.queue_free()
    current_direction = (global_position - last_pos).normalized()
    play_correct_mvmt_animation()
    
func play_correct_mvmt_animation() -> void:
    match current_direction:
        Vector3.FORWARD:
            # (0,0,-1), -Z
            animation_player.play("WALK_-Z", -1, ani_speed)
        Vector3.BACK:
            # (0,0,1), +Z
            animation_player.play("WALK_+Z", -1, ani_speed)
        Vector3.LEFT:
            # (-1,0,0), -X
            animation_player.play("WALK_-X", -1, ani_speed)
        Vector3.RIGHT:
            # (1,0,0), +X
            animation_player.play("WALK_+X", -1, ani_speed)
        _:
            pass
            


func _input(_event: InputEvent) -> void:    
    if Input.is_action_just_pressed("one"):
        display_sprite_main_2d.frame = 100
    elif Input.is_action_just_pressed("two"):
        display_sprite_main_2d.frame = 110
    elif Input.is_action_just_pressed("three"):
        display_sprite_main_2d.frame = 120
    elif Input.is_action_just_pressed("four"):
        display_sprite_main_2d.frame = 130
    elif Input.is_action_just_pressed("five"):
        display_sprite_main_2d.frame = 140
    elif Input.is_action_just_pressed("six"):
        pass
    elif Input.is_action_just_pressed("seven"):
        skeleton.progress += walk_speed
    elif Input.is_action_just_pressed("eight"):
        skeleton.progress -= walk_speed
    elif Input.is_action_just_pressed("nine"):
        set_physics_process(not(is_physics_processing()))
    elif Input.is_action_just_pressed("zero"):
        display_sprite_main_2d.flip_h = not display_sprite_main_2d.flip_h
        print("Flipped: ", display_sprite_main_2d.flip_h)
