extends CharacterBody2D

var speed: int = 200
var direction: Vector2 = Vector2.RIGHT

var health: int = 250

var notice_player: bool = false
var attack_player: bool = false

func _ready():
    set_physics_process(false)
    call_deferred("enemies_setup")

func enemies_setup():
    # Wait for the first physics frame so the NavigationServer can sync.
    await get_tree().physics_frame
    set_physics_process(true)
    $NavigationAgent2D.target_position = Globals.player_pos
    $NavigationAgent2D.path_desired_distance = 4.0
    $NavigationAgent2D.target_desired_distance = 4.0
    
func attack():
    if attack_player:
        Globals.health -= 25

func move():
    var next_path_pos = $NavigationAgent2D.get_next_path_position()
    direction = (next_path_pos - global_position).normalized()
    velocity = direction * speed
    move_and_slide()
    var look_angle = direction.angle()
    rotation = look_angle + PI/2

func _physics_process(_delta):
    if notice_player:
        move()
    if attack_player:
        $AnimationPlayer.play("attack")

func _on_notice_area_body_entered(_body):
    $AnimationPlayer.play("walk")
    notice_player = true
    
func _on_attack_area_body_entered(_body):
    attack_player = true

func _on_notice_area_body_exited(_body):
    $AnimationPlayer.stop()
    notice_player = false

func _on_attack_area_body_exited(_body):
    attack_player = false

func _on_navigation_timer_timeout():
    if notice_player:
        $NavigationAgent2D.target_position = Globals.player_pos

func hit(modifier=1):
    health -= modifier
        
    if health <= 0:
        print("DEAD HUNTER!!")
        queue_free()
