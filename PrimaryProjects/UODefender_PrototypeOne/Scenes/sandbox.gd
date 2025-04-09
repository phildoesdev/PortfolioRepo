extends Node3D

@export var current_camera: Camera3D


## Was using this to auto-detect 
#func _ready() -> void:
    #for child in get_children():
        #if child is Camera3D and child == current_camera:
            #child.set_current(true)
        #el Vector3(7.7, 11.2, 5.15)if child is Camera3D:
            #child.set_current(false)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
    pass
