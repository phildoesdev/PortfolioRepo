extends Area2D

@export var speed: int = 1000
var direction: Vector2 = Vector2.RIGHT

# Called when the node enters the scene tree for the first time.
func _ready():
    $SelfDestruct.start()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
    if direction == Vector2.ZERO:
        direction = Vector2.RIGHT
    position += direction * speed * delta


func _on_body_entered(body):
    if "hit" in body:
        body.hit(1*Globals.attack_power)
    queue_free()


func _on_timer_timeout():
    queue_free()
