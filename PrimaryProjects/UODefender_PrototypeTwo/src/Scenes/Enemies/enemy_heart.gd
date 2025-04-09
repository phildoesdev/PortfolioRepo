class_name EnemyHeart
extends Node
"""
Manages all matters of the heart, especially bleeding
"""
# Health
var health_b: float = 0.0
var health_c: float = 0.0

# Resists
var dmg_type_resists: Dictionary = {}
var dmg_type_vuln: Dictionary = {}
# etc
var health_regen: float = 0.0

# Signals
signal zero_health
signal dmg_taken(dmg: float)

func _init(fs:EnemyFactSheet) -> void:
    health_b = fs.health
    health_c = health_b
    health_regen = fs.health_regen
    dmg_type_vuln = fs.dmg_type_vuln
    dmg_type_resists = fs.dmg_type_resists

func take_dmg(amt: float, dmg_types:Dictionary) -> void:
    """
    Figures the current health, emits signals, and w/e else might need to happen
        when a monster takes dmg
    """
    var dmg_in: float = calculate_dmg(amt, dmg_types)    
    dmg_taken.emit(dmg_in)
    var tmp_health: float = health_c - dmg_in
    if tmp_health <= 0:
        tmp_health = 0
        zero_health.emit()
    health_c = tmp_health
    # print_rich("[b][color=black][bgcolor=red]HEALTH[/bgcolor][/color][/b]:",health_c,"/",health_b,)

func calculate_dmg(amt: float, dmg_types:Dictionary) -> float:
    """
    Does the dmg calculations including resists & vulnerabilities
    """
    var dmg: float = amt
    return dmg
