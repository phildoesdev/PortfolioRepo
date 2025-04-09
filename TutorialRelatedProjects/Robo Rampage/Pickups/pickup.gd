extends Area3D

@export var ammo_type: AmmoHandler.ammo_type = AmmoHandler.ammo_type.SMALL_BULLET
@export var ammmo_amt: int = 25

func _on_body_entered(body: Node3D) -> void:
    if body.is_in_group("player"):
        body.ammo_handler.add_ammo(ammo_type, ammmo_amt)
        body.ammo_handler.update_ammo_label_if_active(ammo_type)
        queue_free()
