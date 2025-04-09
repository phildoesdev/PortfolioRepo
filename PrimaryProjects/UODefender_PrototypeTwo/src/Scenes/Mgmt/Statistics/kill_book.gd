class_name KillBook
extends Node
"""
Holds information on a towers kills and dmg distribution
Is also used to pass along statistics to our master 'Game Statistics' class
"""

# Book that holds information on what kind of dmg we've dealth to which kind of monsters
var kill_book: Dictionary = {}

func _init() -> void:
    initialize_killbook_dict()

func initialize_killbook_dict() -> void:
    """
    Initializes the killbook dictinoary for all enemies
    """
    for _enemy in EnemyDefinitions.ENEMIES.values():
        kill_book[_enemy] = {
            "kills": 0,
            "dmg_raw":0.0,
            "dmg_actual":0.0
        }

func register_kill(death_sheet: Dictionary) -> void:
    """
    We are notified by the enemies we kill that they are dead and we try to 
        put together w/e interesting stats that might be able to come from that
    """
    kill_book[death_sheet["id"]]["kills"] += 1

func register_dmg_raw_out(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    This is the dmg that the tower 'sends out', before any resists, dmg buffs, or debuffs
    """
    kill_book[fact_sheet_id]["dmg_raw"] += dmg_amt
    
func register_dmg_actual_out(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    This is the dmg done as reported by the enemies who took the dmg
    """
    kill_book[fact_sheet_id]["dmg_actual"] += dmg_amt
