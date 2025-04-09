extends Node2D

@onready var sprite: Sprite2D = $Icon


#func _process(delta: float) -> void:
    #var TIME = Time.get_ticks_msec()/1000.0;
    #print("TIME: {"+str(TIME)+"}")
    #print("OSILATING VALUE: " + str((sin(TIME) +1.0)*0.5))

func _process(_delta: float) -> void:
    if Input.is_action_just_pressed("set_color"):
        sprite.material.set_shader_parameter("my_color", Vector4(randf(), randf(), randf(), randf()))
    
    if Input.is_action_just_pressed("set_speed"):
        print("New Speed " + str(sprite.material.get_shader_parameter("my_float")+10))
        sprite.material.set_shader_parameter("my_float", sprite.material.get_shader_parameter("my_float")+10);
