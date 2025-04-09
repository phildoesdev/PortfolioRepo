extends CanvasLayer


func change_scene(target: String) -> void:
    $FadeToBlack.play("fade_to_black")
    await $FadeToBlack.animation_finished
    get_tree().change_scene_to_file(target)
    $FadeToBlack.play("reveal")

# Called when the node enters the scene tree for the first time.
func _ready():
    pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
    pass
