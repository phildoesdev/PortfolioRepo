extends Node3D

@export var fire_rate := 14.0
@export var recoil := 0.1
@export var weapon_mesh: Node3D
@export var weapon_range := 100.0
@export var weapon_dmg := 15
@export var muzzle_flash: GPUParticles3D 
@export var sparks_scene: PackedScene       # I think we're doing a packed scene here insxtead of a node b/c we're bringing that whole animation with us. I don't think you'd really do this like we are but idk. Also, doesn't require us to create the node w/in 
@export var ammo_type: AmmoHandler.ammo_type
@export var ammo_handler: AmmoHandler

@export var automatic: bool


@onready var cooldown_timer: Timer = $CooldownTimer
@onready var weapon_position: Vector3 = weapon_mesh.position
@onready var target_ray_cast: RayCast3D = $TargetRayCast

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    target_ray_cast.target_position.z = -1 * weapon_range


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
    if automatic:
        if Input.is_action_pressed("fire"):
            if cooldown_timer.is_stopped():
                shoot()
    else:
        if Input.is_action_just_pressed("fire"):
            if cooldown_timer.is_stopped():
                shoot()
        
    weapon_mesh.position = weapon_mesh.position.lerp(weapon_position, 10*delta)
 
func shoot() -> void:
    if ammo_handler.has_ammo(ammo_type):
        muzzle_flash.restart()
        cooldown_timer.start(1.0 / fire_rate) # this gives us seconds per shot to get fire_rate shots per second
        weapon_mesh.position.z += recoil
        if target_ray_cast.is_colliding():
            var collider = target_ray_cast.get_collider()
            if collider is Enemy:
                collider.current_hitpoints -= weapon_dmg
            var spark = sparks_scene.instantiate()
            add_child(spark)
            spark.global_position = target_ray_cast.get_collision_point()
            ammo_handler.use_ammo(ammo_type)
    
