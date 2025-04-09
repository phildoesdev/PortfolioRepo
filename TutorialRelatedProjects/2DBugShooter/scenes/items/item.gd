extends Area2D

var rotation_speed: int = 1
var avail_types: Array[String] = ['laser', 'health', 'grenade']
var type: String = avail_types[randi()%len(avail_types)]

var direction: Vector2 = Vector2.ZERO
var distance: int = randi_range(150, 250)

# Called when the node enters the scene tree for the first time.
func _ready():
    if type == 'laser':
        $ItemIcon.modulate = Color(0.145, 0.271, 0.722)
    elif type == 'grenade':
        $ItemIcon.modulate = Color(0.3, 0.255, 0.141, 0.9)
    elif type == 'health':
        $ItemIcon.modulate = Color(0.643, 0, 0, 0.953)
        
    # Tween animation
    var target_pos = position + direction * distance
    var mvmt_tween: Tween = create_tween()
    mvmt_tween.set_parallel(true)
    mvmt_tween.tween_property(self, "position", target_pos, 0.5)
    mvmt_tween.tween_property(self, "scale", Vector2(1, 1), 0.4).from(Vector2(0,0))


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
    rotation += rotation_speed * delta


func _on_body_entered(_body):
    if _body is CharacterBody2D:
        if type == 'health':
            Globals.health += 75
        elif type == "laser":
            Globals.laser_amount += 1000
        elif type == "grenade":
            Globals.grenade_amount += 100
        $AudioStreamPlayer2D.play()
        $ItemIcon.hide()
        await $AudioStreamPlayer2D.finished
        queue_free()
