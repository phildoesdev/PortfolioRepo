class_name MageTower
extends Tower

enum TOWER_SUB_TYPES {
    NONE=0,
    FIRE=1,    
}

# How I am pre-loading attacks
var fireball_atk: PackedScene = preload("res://src/Scenes/Towers/Attacks/Projectiles/fireball.tscn")

func _init() -> void:
    var mage_tower_init: Dictionary = {
        "targeting_strat": Tower.TARGETING_STRATEGY.LIFO,
        "tower_range_radius": 5.0,
        "tower_range_height": 2.0,
        "base_attack_dmg": 13.0,
        "attacks_per_sec": 0.5,
        "dmg_types": {},
        "current_attack_ani": "SPELL_1",
        "current_attack_type": Tower.ATTACK_TYPE.PROJECTILE,
        "gold_cost": 125
    }
    super(mage_tower_init)

func _tower_attack() -> void:
    """
    This is what will be over wrirten by all other towers to perform their special
        attacks. All other things can be 
    """    
    var fireball: Fireball = fireball_atk.instantiate()
    fireball.initialize_projectile(current_target, base_attack_dmg, dmg_types, self)
    add_child(fireball)
    fireball.global_position = global_position
    fireball.global_position.y = tower_height
