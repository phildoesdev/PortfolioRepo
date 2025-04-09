extends RigidBody3D

## This is a description for thrust because it begins with two hash marks
@export_range(100.0 ,3000.0) var thrust: float = 1000.0
@export var torque_thrust: float = 100.0

# Audio
@onready var explosion_audio: AudioStreamPlayer = $Audio/DeathExplosion
@onready var success_audio: AudioStreamPlayer = $Audio/SuccessAudio
@onready var rocket_audio: AudioStreamPlayer3D = $Audio/RocketAudio

# Particles
@onready var booster_particles: GPUParticles3D = $Body/BoosterParticles
@onready var booster_particles_right: GPUParticles3D = $Body/RightBooster/BoosterParticlesRight
@onready var booster_particles_left: GPUParticles3D = $Body/LeftBooster/BoosterParticlesLeft
@onready var success_particles: GPUParticles3D = $Particles/SuccessParticles
@onready var explosion_particles: GPUParticles3D = $Particles/ExplosionParticles


var is_transitioning: bool = false

func _process(delta: float) -> void:
    if Input.is_action_just_pressed("ui_cancel"):
        get_tree().quit()
    if Input.is_action_pressed("boost"):
        apply_central_force(basis.y * delta * thrust)
        booster_particles.emitting = true
        if not rocket_audio.playing:
            rocket_audio.play()
    else:
        booster_particles.emitting = false
        rocket_audio.stop()
    if Input.is_action_pressed("rotate_left"):
        apply_torque(Vector3(0.0, 0.0, torque_thrust)*delta) 
        booster_particles_left.emitting = true
    else:
        booster_particles_left.emitting = false
        
    if Input.is_action_pressed("rotate_right"):
        apply_torque(Vector3(0.0, 0.0, -1*torque_thrust)*delta)
        booster_particles_right.emitting = true
    else:
        booster_particles_right.emitting = false
        

func _on_body_entered(body: Node) -> void:
    if not is_transitioning:
        if "Goal" in body.get_groups(): 
            complete_level(body.file_path)
        if "Hazard" in body.get_groups():
            crash_sequence()

func crash_sequence() -> void:
    end_lvl_cleanup()
    explosion_particles.emitting = true
    explosion_audio.play()
    set_process(false)
    is_transitioning = true
    var tween = create_tween()
    tween.tween_interval(2.5)
    tween.tween_callback(get_tree().reload_current_scene)

func complete_level(next_level_file: String) -> void:
    end_lvl_cleanup()
    success_particles.emitting = true
    success_audio.play()
    set_process(false)
    is_transitioning = true
    var tween = create_tween()
    tween.tween_interval(2.4)
    tween.tween_callback(get_tree().change_scene_to_file.bind(next_level_file))

func end_lvl_cleanup():
    if rocket_audio.playing:
        rocket_audio.stop()
    booster_particles.emitting = false
    booster_particles_left.emitting = false
    booster_particles_right.emitting = false
    
