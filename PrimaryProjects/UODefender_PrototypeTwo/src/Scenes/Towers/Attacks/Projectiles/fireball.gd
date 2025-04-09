class_name Fireball
extends Area3D

enum MODE {
    NONE=0,
    FOLLOW=1,
    TO_POSITION=2
    }


# Projectile Information including targeting
var projectile_src: Tower
var cur_target: Node3D
var dmg: float = 10.0
var dmg_types: Dictionary = {Tower.DMG_TYPES.FIRE: 75.0, Tower.DMG_TYPES.CHOAS: 25.0}
var projectile_velocity: float = 7.0

# internal mgmt
var cur_dir: Vector3 = Vector3.ZERO     # Keep track of which direction we are traveling for mvmt + 
var active: bool = true                 # Helps us continue to act after applying dmg
var lifetime: Timer
var max_lifetime_s: float = 3.0

# Our sprite, so that we can rotate it correctly
@onready var fireball_sprite: AnimatedSprite2D = $SubViewport/FireballSprite
func _ready() -> void:
    body_entered.connect(apply_dmg_to_target)
    create_max_lifespan_timer()
    
func create_max_lifespan_timer() -> void:
    """
    Creates a timer as a 'sanity check' to clear projectiles that may 
        find it difficult to reach their target for w/e reason.
    As of now, we just die, we assume they were never meant to reach their target
    I think there is probably a smarter/nicer/cleaner way to do this, but this is at least an absolute fail-safe...
    """
    lifetime = Timer.new()
    add_child(lifetime)
    lifetime.wait_time = max_lifetime_s
    lifetime.one_shot = true  # YOLO
    lifetime.start()
    lifetime.timeout.connect(die)

func _physics_process(delta: float) -> void:
    if not cur_target:
        die()
        return
    cur_dir = (cur_target.global_position - global_position).normalized()
    global_position += cur_dir*projectile_velocity*delta
    face_correct_dir()

func initialize_projectile(_target:Node3D, _dmg: float, _dmg_types: Dictionary, _src:Tower) -> void:
    cur_target=_target
    dmg = _dmg
    dmg_types = _dmg_types
    projectile_src = _src

func apply_dmg_to_target(body: Node3D) -> void:
    if active and body == cur_target:
        body_entered.disconnect(apply_dmg_to_target)
        active = false
        cur_target.take_dmg(dmg, dmg_types, projectile_src)
        die()

func die() -> void:
    set_physics_process(false)
    set_process(false)
    queue_free()

func face_correct_dir() -> void:
    match round(cur_dir):
        Vector3(0,0,-1):
            #VECTOR3.FORWARD, -Z
            fireball_sprite.rotation = deg_to_rad(0)
            fireball_sprite.offset = Vector2(0,0)
        Vector3(-1,0,-1):
            #BACK LEFT, -X, -Z
            fireball_sprite.rotation = deg_to_rad(315)
            fireball_sprite.offset = Vector2(-25,0)
        Vector3(1,0,-1):
            #UP LEFT, +X, -Z
            fireball_sprite.rotation = deg_to_rad(45)
            fireball_sprite.offset = Vector2(0,-25)
        Vector3(0,0,1):
            #VECTOR3.BACK, +Z
            fireball_sprite.rotation = deg_to_rad(180)
            fireball_sprite.offset = Vector2(-50,-50)
        Vector3(1,0,1):
            # UP RIGHT +X, +Z
            fireball_sprite.rotation = deg_to_rad(135)
            fireball_sprite.offset = Vector2(-25,-50)
        Vector3(-1,0,1):
            # BUTTOM RIGHT +Z, -X
            fireball_sprite.rotation = deg_to_rad(225)
            fireball_sprite.offset = Vector2(-50,-25)
        Vector3(-1,0,0):
            #VECTOR3.LEFT, -X
            fireball_sprite.rotation = deg_to_rad(270)
            fireball_sprite.offset = Vector2(-50,0)
        Vector3(1,0,0):
            #VECTOR3.RIGHT, +X
            fireball_sprite.rotation = deg_to_rad(90)
            fireball_sprite.offset = Vector2(0,-50)            
        _:
            # Preserve w/e was last set b/c how could we know?
            pass
